namespace GameFramework.Verification
{
    internal static class VerificationUtils
    {
        public static bool NeedShowAlertDialog { get; private set; } = true;
        
        public static string GetVerificationMessage(string reason, string userMessage, UnityEngine.Object context, 
            string sourceFilePath, 
            int sourceLineNumber, 
            string memberName)
        {
            string locationInfo = FormatLocationInfo(sourceFilePath, sourceLineNumber, memberName);
            return FormatMessage(reason, userMessage, locationInfo, context);
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

            return "FilePath: " + relativeFilePath + $" (Line {sourceLineNumber})\n" + "MemberName: " + memberName;
        }
        
        private static string FormatMessage(string reason, string userMessage, string locationInfo, UnityEngine.Object context)
        {
            string message = $"Reason: {reason}";

            if (!string.IsNullOrEmpty(userMessage))
            {
                message += $"\nMessage: {userMessage}";
            }

            if(context != null)
            {
                message += $"\nObject context name: {context.name}";
            }
            
            message += $"\n\n{locationInfo}";
            return message;
        }
    }
}