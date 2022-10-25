using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MCMS.Base.Helpers
{
    public static class DummyDynamicQueryParams
    {
        public static object Create([NotNull] List<object> values)
        {
            if (values.Count is 0 or > 7)
            {
                throw new ArgumentException("Only lists with length between 1 and 7 are accepted.", nameof(values));
            }

            var length = values.Count;
            var method = typeof(DummyDynamicQueryParams).GetMethod("Create" + length);
            return method?.Invoke(null, new object[] {values});
        }

        public static object Create1(List<object> values)
        {
            return new {param0 = values[0]};
        }

        public static object Create2(List<object> values)
        {
            return new
            {
                param0 = values[0],
                param1 = values[1]
            };
        }

        public static object Create3(List<object> values)
        {
            return new
            {
                param0 = values[0],
                param1 = values[1],
                param2 = values[2],
            };
        }

        public static object Create4(List<object> values)
        {
            return new
            {
                param0 = values[0],
                param1 = values[1],
                param2 = values[2],
                param3 = values[3],
            };
        }

        public static object Create5(List<object> values)
        {
            return new
            {
                param0 = values[0],
                param1 = values[1],
                param2 = values[2],
                param3 = values[3],
                param4 = values[4],
            };
        }

        public static object Create6(List<object> values)
        {
            return new
            {
                param0 = values[0],
                param1 = values[1],
                param2 = values[2],
                param3 = values[3],
                param4 = values[4],
                param5 = values[5],
            };
        }

        public static object Create7(List<object> values)
        {
            return new
            {
                param0 = values[0],
                param1 = values[1],
                param2 = values[2],
                param3 = values[3],
                param4 = values[4],
                param5 = values[5],
                param6 = values[6],
            };
        }
    }
}