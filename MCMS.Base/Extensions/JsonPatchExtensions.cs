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
    }
}