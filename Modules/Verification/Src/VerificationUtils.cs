using System;
using Object = UnityEngine.Object;

namespace GameFramework.Verification
{
    internal static class VerificationUtils
    {
        public static string FormatMessage(string message, Object context, 
            string sourceFilePath,
            int sourceLineNumber, 
            string memberName)
        {
            string locationInfo = FormatLocationInfo(sourceFilePath, sourceLineNumber, memberName);
            return BuildMessage(message, locationInfo, context);
        }
        
        private static string FormatLocationInfo(string sourceFilePath, int sourceLineNumber, string memberName)
        {
            int assetsIndex = sourceFilePath.IndexOf("Assets", StringComparison.Ordinal);
            string relativeFilePath = sourceFilePath;
            if (assetsIndex != -1)
            {
                relativeFilePath = sourceFilePath.Substring(assetsIndex);
            }

            return "File Path: " + relativeFilePath + $" (Line {sourceLineNumber})\n" + "Member Name: " + memberName;
        }
        
        private static string BuildMessage(string userMessage, string locationInfo, Object context)
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