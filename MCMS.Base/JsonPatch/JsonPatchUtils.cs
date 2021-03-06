using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Base.JsonPatch
{
    public class JsonPatchUtils
    {
        public static JsonPatchDocument<T> CreatePatch<T>(T original, T modified) where T : class
        {
            var untyped = CreatePatchFromObjects(original, modified);

            var typed = new JsonPatchDocument<T>();
            typed.Operations.AddRange(untyped.Operations.Select(untypedOp => untypedOp.CloneFor<T>()));
            return typed;
        }

        public static JsonPatchDocument CreatePatchFromObjects(object originalObject, object modifiedObject)
        {
            var serializer = JsonSerializer.CreateDefault();
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
            var original = JObject.FromObject(originalObject, serializer);
            var modified = JObject.FromObject(modifiedObject, serializer);

            var patch = new JsonPatchDocument();
            FillPatchForObject(original, modified, patch, "/");

            return patch;
        }

        static void FillPatchForObject(JObject orig, JObject mod, JsonPatchDocument patch, string path)
        {
            var origNames = orig.Properties().Select(x => x.Name).ToArray();
            var modNames = mod.Properties().Select(x => x.Name).ToArray();

            // Names removed in modified
            foreach (var k in origNames.Except(modNames))
            {
                var prop = orig.Property(k);
                patch.Remove(path + prop.Name);
            }

            // Names added in modified
            foreach (var k in modNames.Except(origNames))
            {
                var prop = mod.Property(k);
                patch.Add(path + prop.Name, prop.Value);
            }

            // Present in both
            foreach (var k in origNames.Intersect(modNames))
            {
                var origProp = orig.Property(k);
                var modProp = mod.Property(k);

                if (origProp.Value.Type != modProp.Value.Type)
                {
                    patch.Replace(path + modProp.Name, modProp.Value);
                }
                else if (!string.Equals(
                    origProp.Value.ToString(Newtonsoft.Json.Formatting.None),
                    modProp.Value.ToString(Newtonsoft.Json.Formatting.None)))
                {
                    if (origProp.Value.Type == JTokenType.Object)
                    {
                        // Recurse into objects
                        FillPatchForObject(origProp.Value as JObject, modProp.Value as JObject, patch,
                            path + modProp.Name + "/");
                    }
                    else
                    {
                        // Replace values directly
                        patch.Replace(path + modProp.Name, modProp.Value);
                    }
                }
            }
        }
    }
}