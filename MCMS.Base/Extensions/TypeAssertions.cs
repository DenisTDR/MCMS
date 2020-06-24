using System;

namespace MCMS.Base.Extensions
{
    public static class TypeAssertions
    {
        public static void AssertInheritedGenericTypeWithTypeArguments(Type subject, Type genericType,
            params Type[] typeArguments)
        {
            if (!subject.TryGetGenericTypeOfImplementedGenericType(genericType,
                out var buildGenericType))
            {
                throw new Exception(
                    $"Type `{subject.FullName}` does not implement `{genericType.FullName}`.");
            }

            for (var i = 0; i < typeArguments.Length; i++)
            {
                AssertTypesEqualsOrNull(buildGenericType.GenericTypeArguments[i], typeArguments[i]);
            }
        }

        public static void AssertInherited(Type child, Type parent)
        {
            if (!parent.IsAssignableFrom(child))
            {
                throw new Exception($"Type '{child.FullName}' does not inherit '{parent.FullName}'.");
            }
        }

        public static void AssertInheritedOrNull(Type child, Type parent)
        {
            if (child != null)
            {
                AssertInherited(child, parent);
            }
        }

        public static void AssertTypesEqualsOrNull(Type a, Type b)
        {
            if (a == null || b == null)
            {
                return;
            }

            if (a != b)
            {
                throw new Exception($"Type '{a.FullName}' is not equal with '{b.FullName}'.");
            }
        }
    }
}