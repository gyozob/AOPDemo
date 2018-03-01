using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Unity.Interception.PolicyInjection.Pipeline;

namespace AOPDemo.Attributes
{
    public static class CacheHelper
    {
        public static string BuildCacheKey(TargetMethodArgsMapToKeyPattern model)
        {
            var keyPrefix = model.Method.MethodBase;
            var cacheKey = model.KeyPattern;
            var cacheKeyDict = GetMappedKeysFromPattern(model.KeyPattern);
            AssociateArgumentValuesToKeyPatternMapping(model.Method, cacheKeyDict);
            foreach (var item in cacheKeyDict)
            {
                cacheKey = cacheKey.Replace("{" + item.Key + "}", item.Value);
            }
            ValidateCacheKeyAndThrow(cacheKey);
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

        private static void ValidateCacheKeyAndThrow(string cacheKey)
        {
            var unparsedKeyPatternPlacehoders = CacheHelper.GetMappedKeysFromPattern(cacheKey);
            if (unparsedKeyPatternPlacehoders.Count > 0)
            {

                throw new Exception("Unparsed placeholders in key pattern");
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

    public class TargetMethodArgsMapToKeyPattern
    {
        public string KeyPattern { get; set; }
        public IMethodInvocation Method { get; set; }
    }
}