using System.IO;
using MCMS.Base.Helpers;

namespace MCMS.Files
{
    public static class MFiles
    {
        public static string ContentPath => Env.GetOrThrow("CONTENT_PATH");
        public static string PublicPath => Path.Combine(ContentPath, "public");
        public static string PrivatePath => Path.Combine(ContentPath, "private");
        public static string PublicVirtualPath => "/content/public";
    }
}