using System.Collections.Generic;
using UnityEngine;

namespace ISBEP.Utility
{
    public class Util
    {
        public static bool DebugMessages = false;
        public static List<string> debugContexts = new List<string>();

        public static void AddDebugContext(string context)
        {
            debugContexts.Add(context);
        }

        public static void DebugLog(string context, string message, bool shifted = false)
        {
            if (DebugMessages || debugContexts.Contains(context))
            {
                Log(context, message, shifted);
            }
        }

        public static void Log(string context, string message, bool shifted = false)
        {
            int LOG_CONTEXT_SPACE = 29;
            string spacing = "";
            for (int i = context.Length; i < LOG_CONTEXT_SPACE; i++)
            {
                spacing += " ";
            }
            if (shifted)
            {
                Debug.Log($"{spacing}{context} | {message}");
            }
            else
            {
                Debug.Log($"{context}{spacing} | {message}");
            }
        }
    }
}
