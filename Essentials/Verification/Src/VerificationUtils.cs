using System;

namespace GameFramework.Verification
{
    internal static class VerificationUtils
    {
        public static bool NeedShowAlertDialog { get; private set; } = true;
        
        public static string GetVerificationMessage(string userMessage, UnityEngine.Object context, string sourceFilePath, int sourceLineNumber, string memberName)
        {
            string locationInfo = FormatLocationInfo(sourceFilePath, sourceLineNumber, memberName);
            return FormatMessage(userMessage, locationInfo, context);
        }
        
        public static void OnClickSkipAll()
        {
            NeedShowAlertDialog = false;
        }
        
        private static string FormatLocationInfo(string sourceFilePath, int sourceLineNumber, string memberName)
        {
            int assetsIndex = sourceFilePath.IndexOf("Assets", System.StringComparison.Ordinal);
            string relativeFilePath = sourceFilePath;
            if (assetsIndex != -1)
            {
                relativeFilePath = sourceFilePath.Substring(assetsIndex);
            }

            return "File Path: " + relativeFilePath + $" (Line {sourceLineNumber})\n" + "Member Name: " + memberName;
        }
        
        private static string FormatMessage(string userMessage, string locationInfo, UnityEngine.Object context)
        {
            var messageBuilder = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(userMessage))
            {
                messageBuilder.Append($"Message: {userMessage}");
            }

            if (context != null)
            {
                if (messageBuilder.Length > 0)
                {
                    messageBuilder.AppendLine();
                }
                messageBuilder.Append($"Context object name: {context.name}");
            }

            if (!string.IsNullOrEmpty(locationInfo))
            {
                if (messageBuilder.Length > 0)
                {
                    messageBuilder.AppendLine();
                    messageBuilder.AppendLine();
                }
                messageBuilder.Append(locationInfo);
            }

            return messageBuilder.ToString();
        }
    }
}