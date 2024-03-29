using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace MCMS.Base.Helpers
{
    public static class MDbFunctions
    {
        public static string Concat(string arg1, string arg2)
            => throw new InvalidOperationException($"{nameof(Concat)} cannot be called client side");

        public static string Concat(string arg1, string arg2, string arg3)
            => Concat(arg1, arg2);

        public static string Concat(string arg1, string arg2, string arg3, string arg4)
            => Concat(arg1, arg2);

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5)
            => Concat(arg1, arg2);

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
            => Concat(arg1, arg2);

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6,
            string arg7)
            => Concat(arg1, arg2);

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6,
            string arg7, string arg8)
            => Concat(arg1, arg2);

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6,
            string arg7, string arg8, string arg9)
            => Concat(arg1, arg2);

        public static string Unaccent(string value)
            => throw new InvalidOperationException($"{nameof(Unaccent)} cannot be called client side");

        public static void Register(ModelBuilder builder)
        {
            var methodInfos = typeof(MDbFunctions).GetMethods().Where(mi => mi.Name == nameof(Concat));
            foreach (var methodInfo in methodInfos)
            {
                builder
                    .HasDbFunction(methodInfo)
                    .HasTranslation(args =>
                        new SqlFunctionExpression("CONCAT", args, false, args.Select(x => false), typeof(string), null)
                    );
            }

            var unaccentMethodInfo = typeof(MDbFunctions).GetMethod(nameof(Unaccent));
            builder
                .HasDbFunction(unaccentMethodInfo)
                .HasTranslation(args =>
                    new SqlFunctionExpression("unaccent", args, false, args.Select(x => false), typeof(string), null)
                );
        }
    }
}