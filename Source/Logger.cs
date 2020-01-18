namespace ToolkitPatchLib
{
    public static class ToolkitPatchLogger
    {
        public static string LOGGERNAME = "ToolkitPatch";

        public static void Log(string logger, string msg)
        {
            Verse.Log.Message($"<color=#5034CC>[{LOGGERNAME}::{logger}]</color> DEBUG: {msg}");
        }

        public static void ErrorLog(string logger, string msg)
        {
            Verse.Log.Message($"<color=#990000>[{LOGGERNAME}::{logger}]</color> ERROR: {msg}");
        }
    }
}
