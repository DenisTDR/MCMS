using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace MCMS.Data
{
    public static class DtQueryHelper
    {
        public static (string queryStr, object parameters) BuildCondition(
            string propName, object compareValue,
            string format = null, bool exactMatch = false, string objectName = "x", string paramName = "param")
        {
            string queryStr;
            var leftHand = (!string.IsNullOrEmpty(objectName) ? objectName + "." : "") + propName;
            object parameters = null;
            if (string.IsNullOrEmpty(format))
            {
                if (!exactMatch)
                {
                    queryStr = $"EF.Functions.ILike({leftHand}, MDbFunctions.Concat('%', {paramName}, '%'))";
                    compareValue = compareValue.ToString();
                }
                else
                {
                    queryStr = $"{leftHand} == {paramName}";
                }

                parameters = new {param = compareValue};
            }
            else
            {
                var hasConditionPlaceholder = format.Contains("<condition>");
                var hasSelectorTag = format.Contains("<sel>"); // && format.Contains("</sel>");
                if (hasConditionPlaceholder != hasSelectorTag)
                {
                    throw new ArgumentException(null, nameof(format));
                }

                if (!hasConditionPlaceholder)
                {
                    format = "<condition><sel>" + format;
                }

                var selector = format.Split("<sel>")[1];

                var (condition, parametersL) =
                    BuildCondition(selector, compareValue, null, exactMatch, null, paramName);
                parameters = parametersL;
                format = format.Split("<sel>")[0].Replace("<condition>", condition);

                queryStr = string.Format(format, leftHand, paramName);
            }

            Console.WriteLine("queryStr= " + queryStr);
            Console.WriteLine("params= " + JsonConvert.SerializeObject(parameters));
            return (queryStr, parameters);
        }
    }
}