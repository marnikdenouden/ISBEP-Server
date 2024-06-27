using System.Collections.Generic;
using UnityEngine;

namespace ISBEP.Utility
{
    public class Util
    {
        /// <summary>
        /// Specify if all debug messages should be displayed.
        /// </summary>
        public static bool DebugMessages = false;
        private static readonly List<string> debugContexts = new List<string>();

        /// <summary>
        /// Add a context to the debug context set, so debug logs will be displayed for the specified context.
        /// </summary>
        /// <param name="context">Context string to display debug logs for.</param>
        public static void AddDebugContext(string context)
        {
            debugContexts.Add(context);
        }

        /// <summary>
        /// Log a message for the specified context, if context is in debug context set.
        /// </summary>
        /// <param name="context">Context string to log the message for</param>
        /// <param name="message">Message string to log</param>
        /// <param name="shifted">Truth assignment, if label should be shifted right.</param>
        /// <param name="label">Label string to override the context to display before the message.</param>
        public static void DebugLog(string context, string message, bool shifted = false, string label = "")
        {
            if (DebugMessages || debugContexts.Contains(context))
            {
                if (!label.Equals(string.Empty) || shifted)
                {
                    Log(label, message, shifted);
                } else
                {
                    Log(context, message, shifted);
                }
            }
        }

        /// <summary>
        /// Log a message for the specified context.
        /// </summary>
        /// <param name="label">Label string to log the message for</param>
        /// <param name="message">Message string to log</param>
        /// <param name="shifted">Truth assignment, if context name should be shifted right.</param>
        public static void Log(string label, string message, bool shifted = false)
        {
            int LOG_CONTEXT_SPACE = 29;
            string spacing = "";
            for (int i = label.Length; i < LOG_CONTEXT_SPACE; i++)
            {
                spacing += " ";
            }
            if (shifted)
            {
                Debug.Log($"{spacing}{label} | {message}");
            }
            else
            {
                Debug.Log($"{label}{spacing} | {message}");
            }
        }
    }
}
