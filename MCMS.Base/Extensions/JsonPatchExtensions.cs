using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace MCMS.Base.Extensions
{
    public static class JsonPatchExtensions
    {
        public static JsonPatchDocument<TOut> CloneFor<TIn, TOut>(this JsonPatchDocument<TIn> source)
            where TIn : class where TOut : class
        {
            var doc = new JsonPatchDocument<TOut>();
            doc.Operations.AddRange(source.Operations.Select(op => op.CloneFor<TIn, TOut>()));
            return doc;
        }

        public static bool IsEmpty<T>(this JsonPatchDocument<T> doc) where T : class
        {
            return !doc.Operations.Any();
        }

        public static Operation<TOut> CloneFor<TIn, TOut>(this Operation<TIn> source)
            where TIn : class where TOut : class
        {
            return new Operation<TOut>
            {
                from = source.from,
                path = source.path,
                op = source.op,
                value = source.value
            };
        }

        public static List<string> GetSplitPath<T>(this Operation<T> val) where T : class
        {
            return val.path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}