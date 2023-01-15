using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ironclad.Helper
{
    public static class FO
    {
        public static void CopyEntireDirectory(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);
            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                try
                {
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
                catch
                {
                    IO.Log($"Could not copy {source} to {target}");
                }
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        public static void WriteAllTextUCS2LEBOM(string file, string txt)
        {
            File.WriteAllText(file, "\xac" + txt, new UnicodeEncoding());
        }
        public static string GetFileName(string fullFilePath)
        {
            return fullFilePath.Split(@"\").Last();
        }
        public static string GetSizeKB(string fullFilePath)
        {
            if (!File.Exists(fullFilePath))
                return "?";
            return Convert.ToString(new FileInfo(fullFilePath).Length / 1024);
        }
        public static string GetSizeMB(string fullFilePath)
        {
            if (!File.Exists(fullFilePath))
                return "?";
            return Convert.ToString(new FileInfo(fullFilePath).Length / 1024 / 1024);
        }
        public static int CountWordInFile(string fullFilePath, string word)
        {
            if (!File.Exists(fullFilePath))
                return 0;
            return File.ReadLines(fullFilePath).Select(line => Regex.Matches(line, $@"(?i)\b{word}\b").Count).Sum();
        }
        public static int CountWordInString(string inString, string word)
        {
            return Regex.Matches(inString.ToLower(), String.Format("\b{0}\b", word)).Count;
        }
        public static void RemoveEmptyLines(string file)
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                using (var streamReader = new StreamReader(file))
                using (var streamWriter = new StreamWriter(tempFileName))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            streamWriter.WriteLine(line);
                    }
                }
                File.Copy(tempFileName, file, true);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }
    }
}
