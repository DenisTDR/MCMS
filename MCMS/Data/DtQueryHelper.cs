using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Helpers;
using MCMS.Models.Dt;

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

            return (queryStr, parameters);
        }

        public static bool BuildMultiTermQuery(DtColumn dtColumn, out (string quertStr, object parameters) result,
            bool isNumber = false)
        {
            var col = dtColumn.MatchedTableColumn.DbColumn;
            var terms = dtColumn.Search.Value.Split(" ",
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var queryStrL = new List<string>();

            if (terms.Length == 0)
            {
                result = (null, null);
                return false;
            }

            if (terms.Length > 7)
            {
                terms = new[] {dtColumn.Search.Value.Trim()};
            }

            for (var index = 0; index < terms.Length; index++)
            {
                var format = dtColumn.MatchedTableColumn.DbFuncFormat;
                var (qStr, qParameter) =
                    BuildCondition(col, terms[index], format, paramName: "param" + index,
                        objectName: string.IsNullOrEmpty(format) ? "(string)(object)x" : "x");
                queryStrL.Add(qStr);
            }

            var qStrFinal = string.Join(" && ", queryStrL);
            var qParam = DummyDynamicQueryParams.Create(terms.Cast<object>().ToList());
            result = (qStrFinal, qParam);
            return true;
        }
    }
}