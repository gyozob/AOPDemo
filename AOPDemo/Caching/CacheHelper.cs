using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Unity.Interception.PolicyInjection.Pipeline;

namespace AOPDemo.Caching
{
    public static class CacheHelper
    {
        /// <summary>
        /// Builds the cache key based on the provided arguments - key format: "{callerClassName}-{callerMethodReturnTypeName}-{parsed cacheKey pattern}
        /// </summary>
        /// <param name="methodInvocation"></param>
        /// <param name="keyPattern"></param>
        /// <returns>string cache key</returns>
        public static string BuildCacheKey(IMethodInvocation methodInvocation, string keyPattern)
        {
            var callerMethodInfo = (MethodInfo)methodInvocation.MethodBase;
            var callerClassName = methodInvocation.Target.ToString();
            var callerMethodReturnTypeName = callerMethodInfo.ReturnType.Name;
            var cacheKeySuffix = GetSubstitutedKeyPatternUsingMethodArguments(keyPattern, methodInvocation);
            ValidateCacheKeyAndThrow(cacheKeySuffix, callerMethodInfo);
            return $"{callerClassName}-{callerMethodReturnTypeName}-{cacheKeySuffix}";
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

        private static string GetSubstitutedKeyPatternUsingMethodArguments(
            string keyPattern,
            IMethodInvocation methodInvocation)
        {
            var keyPatternMapping = GetMappedKeysFromPattern(keyPattern);
            var mappedPatternPlaceholders = new List<string>();
            var methodParameters = methodInvocation.MethodBase.GetParameters();
            for (int parameterIndex = 0; parameterIndex < methodParameters.Length; parameterIndex++)
            {
                if (methodParameters[parameterIndex].ParameterType.IsPrimitive)
                {
                    SetParameterValue(methodParameters[parameterIndex], keyPatternMapping, methodInvocation.Arguments[parameterIndex], mappedPatternPlaceholders);
                }
                else
                {
                    var argumentParameters = methodParameters[parameterIndex].ParameterType.GetProperties();
                    foreach (var parameter in argumentParameters)
                    {
                        SetPropertyValue(parameter, keyPatternMapping, methodInvocation.Arguments[parameterIndex], mappedPatternPlaceholders);
                    }
                }
            }
            var cacheKey = keyPattern;
            foreach (var item in keyPatternMapping.Where(e => mappedPatternPlaceholders.Contains(e.Key)))
            {
                cacheKey = cacheKey.Replace("{" + item.Key + "}", item.Value);
            }

            return cacheKey;
        }

        private static void ValidateCacheKeyAndThrow(string cacheKey, MethodInfo callerMethod)
        {
            var unparsedKeyPatternPlacehoders = GetMappedKeysFromPattern(cacheKey);
            if (unparsedKeyPatternPlacehoders.Count > 0)
            {

                throw new Exception($"Unparsed placeholders in key pattern '{cacheKey}' at '{callerMethod.DeclaringType.Name}'method '{callerMethod.Name}'");
            }
        }
        private static void SetPropertyValue(
            PropertyInfo property, 
            Dictionary<string, string> keyPatternMapping, 
            object parameterValue,
            List<string> mappedPatternPlaceholders)
        {
            if (property.PropertyType.IsPrimitive)
            {
                if (keyPatternMapping.ContainsKey(property.Name))
                {
                    keyPatternMapping[property.Name] = property.GetValue(parameterValue, null).ToString();
                    mappedPatternPlaceholders.Add(property.Name);
                }
            }
        }

        private static void SetParameterValue(
            ParameterInfo parameter, 
            Dictionary<string, string> keyPatternMapping, 
            object argumentValue,
            List<string> mappedPatternPlaceholders)
        {
            if (parameter.ParameterType.IsPrimitive)
            {
                if (keyPatternMapping.ContainsKey(parameter.Name))
                {
                    keyPatternMapping[parameter.Name] = argumentValue.ToString();
                    mappedPatternPlaceholders.Add(parameter.Name);
                }
            }
        }
    }
}