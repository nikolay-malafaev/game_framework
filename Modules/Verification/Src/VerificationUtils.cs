using System;

namespace GameFramework.Verification
{
    internal static class VerificationUtils
    {
        public static string FormatLogMessage(string message, 
            string sourceFilePath,
            int sourceLineNumber, 
            string memberName)
        {
            return string.Concat("Verification failed", string.IsNullOrWhiteSpace(message) ? " " : ": ",  
                message, "\n  at ", memberName, " () in ", TrimFilePath(sourceFilePath), ":", sourceLineNumber);
        }

        public static string FormatWindowMessage(string message, 
            string sourceFilePath,
            int sourceLineNumber, 
            string memberName)
        {
            if (string.IsNullOrEmpty(message))
            {
                return string.Concat(memberName, " ()\n", TrimFilePath(sourceFilePath), ":", sourceLineNumber);
            }
            return string.Concat(message, "\n \n", memberName, " ()\n", TrimFilePath(sourceFilePath), ":", sourceLineNumber);
        }

        private static string TrimFilePath(string sourceFilePath)
        {
            int assetsIndex = sourceFilePath.IndexOf("Assets", StringComparison.Ordinal);
            string relativeFilePath = sourceFilePath;
            if (assetsIndex != -1)
            {
                relativeFilePath = sourceFilePath.Substring(assetsIndex);
            }
            return relativeFilePath;
        }
    }
}