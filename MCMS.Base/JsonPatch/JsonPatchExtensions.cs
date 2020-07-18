using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MCMS.Base.JsonPatch
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

        public static Operation<TOut> CloneFor<TOut>(this Operation source) where TOut : class
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

        public static void ApplyTo<T>(
            this JsonPatchDocument<T> patchDoc,
            T objectToApplyTo,
            IAdapterFactory adapterFactory,
            ModelStateDictionary modelState,
            string prefix = "") where T : class
        {
            if (patchDoc == null)
            {
                throw new ArgumentNullException(nameof(patchDoc));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            var objectAdapter = new ObjectAdapter(patchDoc.ContractResolver, jsonPatchError =>
            {
                var affectedObjectName = jsonPatchError.AffectedObject.GetType().Name;
                var key = string.IsNullOrEmpty(prefix) ? affectedObjectName : prefix + "." + affectedObjectName;
                modelState.TryAddModelError(key, jsonPatchError.ErrorMessage);
            }, adapterFactory);
            patchDoc.ApplyTo(objectToApplyTo, objectAdapter);
        }

        public static void ApplyTo<T>(
            this JsonPatchDocument<T> patchDoc,
            T objectToApplyTo,
            IAdapterFactory adapterFactory) where T : class
        {
            if (patchDoc == null)
            {
                throw new ArgumentNullException(nameof(patchDoc));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (adapterFactory == null)
            {
                throw new ArgumentNullException(nameof(adapterFactory));
            }

            var objectAdapter = new ObjectAdapter(patchDoc.ContractResolver, null, adapterFactory);
            patchDoc.ApplyTo(objectToApplyTo, objectAdapter);
        }
    }
}