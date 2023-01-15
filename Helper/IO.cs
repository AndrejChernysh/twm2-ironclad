
using System;

namespace Ironclad.Helper
{
    public static class IO
    {
        private static DateTime last = DateTime.Now;
        private static string LogFile = Settings.P(@"ironclad\ironclad.log");
        public static void Log(string LogText)
        {
            var ellapsed = Convert.ToInt32(Math.Round((DateTime.Now - last).TotalSeconds));
            if (ellapsed > 5)
                LogText = $"[!] {LogText}";
            LogText = $"{DateTime.Now:HH:mm:ss}\tsec {ellapsed}\t{LogText}\n";
            last = DateTime.Now;
            System.IO.File.AppendAllText(LogFile, LogText);
        }
        public static void Val(bool Expression, string LogTextIfFalse, string LogTextIfTrue = "")
        {
            if (Expression)
            {
                if (LogTextIfTrue != "")
                {
                    var ellapsed = Convert.ToInt32(Math.Round((DateTime.Now - last).TotalSeconds));
                    if (ellapsed > 5)
                        LogTextIfTrue = $"[!] {LogTextIfTrue}";
                    LogTextIfTrue = $"{DateTime.Now:HH:mm:ss}\tsec {ellapsed}\t{LogTextIfTrue}\n";
                    last = DateTime.Now;
                    System.IO.File.AppendAllText(LogFile, LogTextIfTrue);
                }
            }
            else
            {
                var ellapsed = Convert.ToInt32(Math.Round((DateTime.Now - last).TotalSeconds));
                if (ellapsed > 5)
                    LogTextIfFalse = $"[!] {LogTextIfFalse}";
                LogTextIfFalse = $"{DateTime.Now:HH:mm:ss}\tsec {ellapsed}\t{LogTextIfFalse}\n";
                last = DateTime.Now;
                System.IO.File.AppendAllText(LogFile, LogTextIfFalse);
            }
        }
    }
}
