using System;
using GameFramework.Types;
using UnityEngine;

// todo
namespace GameFramework.Utils
{
    public static class DataTimeUtils
    { 
        public static int GetPercentageBetweenDates(DateTime dateTime1, DateTime dateTime2)
        {
            var totalDuration = dateTime2 - dateTime1;
            var elapsed = DateTime.UtcNow - dateTime1;
            
            if (totalDuration.TotalSeconds <= 0)
            {
                return -1;
            }
            
            var percentage = (elapsed.TotalSeconds / totalDuration.TotalSeconds) * 100.0;
            percentage = Mathf.Clamp((float) percentage, 0, 100);
            return (int) percentage;
        }
        
        public static DateTime GetDateTimeByPercentage(DateTime dateTime1, DateTime dateTime2, pct_int percentage)
        {
            var totalDuration = dateTime2 - dateTime1;
            var elapsedSeconds = (totalDuration.TotalSeconds * percentage) / 100.0;
            return dateTime1.AddSeconds(elapsedSeconds);
        }
        
        public static DateTime GetDateTimeByPercentage(DateTime dateTime1, DateTime dateTime2, n_float percentage)
        {
            pct_int newPercentage = (int) Math.Range.Convert(0.0f, 1.0f, 0.0f, 100.0f, percentage);
            return GetDateTimeByPercentage(dateTime1, dateTime2, newPercentage);
        }
    }
}