using System;
using System.Text;

namespace CDK {
    public static class CTimeSpanExtensions {
        
        public static string CGetTimeSpanFormattedVerbose(this TimeSpan timeSpan) {
            var sb = new StringBuilder();
            if ((int)timeSpan.TotalDays > 0) {
                sb.Append(timeSpan.Days)
                  .Append("d");
            }
            if (sb.Length > 0) sb.Append(" ");
            if ((int)timeSpan.TotalHours > 0) {
                sb.Append(timeSpan.Hours)
                .Append("h");
            }
            if (sb.Length > 0) sb.Append(" ");
            if ((int)timeSpan.TotalMinutes > 0) {
                sb.Append(timeSpan.Minutes)
                .Append("m");
            }
            if (sb.Length > 0) sb.Append(" ");
            if ((int)timeSpan.TotalSeconds > 0) {
                sb.Append(timeSpan.Seconds)
                .Append("s");
            }
            return sb.ToString();
        }
        
    }
}