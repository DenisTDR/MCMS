using System;

namespace MCMS.Base.Display.ModelDisplay
{
    [Flags]
    public enum ServerClient
    {
        None = 0,
        Client = 1,
        Server = 2,
        Both = 3
    }

    public static class ServerClientEnumExtensions
    {
        public static bool IsServer(this ServerClient val)
        {
            return (val & ServerClient.Server) != ServerClient.None;
        }

        public static bool IsClient(this ServerClient val, bool toggle = false)
        {
            if (toggle) return IsServer(val);
            return (val & ServerClient.Client) != ServerClient.None;
        }
    }
}