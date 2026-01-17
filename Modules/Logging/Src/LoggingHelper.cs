using System.Text;

namespace GameFramework.Logging
{
    public class LoggingHelper
    {
        private StringBuilder _stringBuilder = new(256);
        
        public void SendLog(UnityEngine.LogType logType, string classTag = null, string[] tags = null, object message = null, UnityEngine.Object context = null)
        {
            _stringBuilder.Clear();
            if(classTag != null) AppendTag(classTag);
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    AppendTag(tag);
                }
            }
            string finallyTags = _stringBuilder.ToString();
            UnityEngine.Debug.unityLogger.Log(logType, finallyTags, message, context);
        }
        
        private void AppendTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            _stringBuilder.Append('[').Append(tag).Append(']').Append(' ');
        }
    }
}