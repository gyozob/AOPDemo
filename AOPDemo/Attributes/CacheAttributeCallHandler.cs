using AOPDemo.Caching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Interception.PolicyInjection.Pipeline;

namespace AOPDemo.Attributes
{
    public class CacheAttributeCallHandler : ICacheAttributeCallHandler
    {
        private readonly ICacheProvider _cache;
        public CacheAttributeCallHandler(ICacheProvider cache)
        {
            _cache = cache;
        }
        private string _cacheKeyPattern;
        public int Order
        {
            get
            {
                return 1;
            }

            set
            {
            }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var key = CacheHelper.BuildCacheKey(new TargetMethodArgsMapToKeyPattern()
            {
                Method = input,
                KeyPattern = _cacheKeyPattern
            });
            ValidateCacheKeyAndThrow(key);
            return input.CreateMethodReturn("valami");
        }

        private static void ValidateCacheKeyAndThrow(string cacheKey)
        {
            var unparsedKeyPatternPlacehoders = CacheHelper.GetMappedKeysFromPattern(cacheKey);
            if (unparsedKeyPatternPlacehoders.Count > 0)
            {

                throw new Exception("Unparsed placeholders in key pattern");
            }
        }

        public ICacheAttributeCallHandler SetCacheKeyPattern(string cacheKeyPattern)
        {
            _cacheKeyPattern = cacheKeyPattern;
            return this;
        }
    }

    public interface ICacheAttributeCallHandler: ICallHandler
    {
        ICacheAttributeCallHandler SetCacheKeyPattern(string cacheKey);
    }

    static class CacheHelper
    {
        public static string BuildCacheKey(TargetMethodArgsMapToKeyPattern model)
        {
            var cacheKey = model.KeyPattern;
            var cacheKeyDict = GetMappedKeysFromPattern(model.KeyPattern);
            AssociateArgumentValuesToKeyPatternMapping(model.Method, cacheKeyDict);
            foreach (var item in cacheKeyDict)
            {
                cacheKey = cacheKey.Replace("{"+item.Key+"}", item.Value);
            }

            return cacheKey;
        }

        public static Dictionary<string, string> GetMappedKeysFromPattern(string pattern)
        {
            var cacheKeyDict = new Dictionary<string, string>();
            var matches = Regex.Matches(pattern, @"{(.+?)}");
            foreach (Match match in matches)
            {
                cacheKeyDict.Add(match.Groups[1].Value, null);
            }

            return cacheKeyDict;
        }

        private static void AssociateArgumentValuesToKeyPatternMapping(
            IMethodInvocation method, 
            Dictionary<string, string> keyPatternMapping)
        {
            var methodParameters = method.MethodBase.GetParameters();
            for (int parameterIndex = 0; parameterIndex < methodParameters.Length; parameterIndex++)
            {
                if (methodParameters[parameterIndex].ParameterType.IsPrimitive)
                {
                    SetParameterValue(methodParameters[parameterIndex], keyPatternMapping, method.Arguments[parameterIndex]);
                }
                else
                {
                    var argumentParameters = methodParameters[parameterIndex].ParameterType.GetProperties();
                    foreach (var parameter in argumentParameters)
                    {
                        SetPropertyValue(parameter, keyPatternMapping, method.Arguments[parameterIndex]);
                    }
                }
            }
        }

        private static void SetPropertyValue(PropertyInfo property, Dictionary<string, string> keyPatternMapping, object parameterValue)
        {
            if (property.PropertyType.IsPrimitive)
            {
                if (keyPatternMapping.ContainsKey(property.Name))
                {
                    keyPatternMapping[property.Name] = property.GetValue(parameterValue, null).ToString();
                }
            }
        }
        private static void SetParameterValue(ParameterInfo parameter, Dictionary<string, string> keyPatternMapping, object argumentValue)
        {
            if (parameter.ParameterType.IsPrimitive)
            {
                if (keyPatternMapping.ContainsKey(parameter.Name))
                {
                    keyPatternMapping[parameter.Name] = argumentValue.ToString();
                }
            }
        }
    }

    class TargetMethodArgsMapToKeyPattern
    {
        public string KeyPattern { get; set; }
        public IMethodInvocation Method { get; set; }
    }
}