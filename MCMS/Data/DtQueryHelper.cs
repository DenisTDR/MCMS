using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Helpers;
using MCMS.Models.Dt;

namespace MCMS.Data
{
    public static class DtQueryHelper
    {
        public static QueryCondition BuildCondition(
            string propName, object compareValue, string format = null, bool exactMatch = false,
            string objectName = "x", string paramName = "param")
        {
            string queryStr;
            var leftHand = (!string.IsNullOrEmpty(objectName) ? objectName + "." : "") + propName;
            object parameters;
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

                parameters = new { param = compareValue };
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

                var qCond =
                    BuildCondition(selector, compareValue, null, exactMatch, null, paramName);
                parameters = qCond.Params;
                format = format.Split("<sel>")[0].Replace("<condition>", qCond.Query);

                queryStr = string.Format(format, leftHand, paramName);
            }

            return new QueryCondition(queryStr, parameters);
        }

        public static bool BuildMultiTermQuery(DtColumn dtColumn, out QueryCondition result,
            DbColumnMetadata dbColumn)
        {
            var terms = dtColumn.Search.Value.Split(" ",
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var queryStrL = new List<string>();

            if (terms.Length == 0)
            {
                // result = null;
                result = new QueryCondition();
                return false;
            }

            if (terms.Length > 7)
            {
                terms = new[] { dtColumn.Search.Value.Trim() };
            }

            for (var index = 0; index < terms.Length; index++)
            {
                var format = dbColumn.DbFuncFormat;
                var qCond =
                    BuildCondition(dbColumn.DbColumn, terms[index], format, paramName: "param" + index,
                        objectName: string.IsNullOrEmpty(format) ? "(string)(object)x" : "x");
                queryStrL.Add(qCond.Query);
            }

            var qStrFinal = string.Join(" && ", queryStrL);
            var qParam = DummyDynamicQueryParams.Create(terms.Cast<object>().ToList());
            result = new QueryCondition(qStrFinal, qParam);
            return true;
        }
    }

    public struct QueryCondition
    {
        public string Query { get; set; }
        public object Params { get; set; }
        public bool Valid => !string.IsNullOrEmpty(Query);

        public QueryCondition(string query, object @params)
        {
            Query = query;
            Params = @params;
        }
    }
}