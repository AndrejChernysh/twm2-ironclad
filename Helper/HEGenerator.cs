using Ironclad.Extensions;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ironclad.Helper
{
    static class HEGenerator
    {
        public static StringBuilder code = new StringBuilder();
        public static List<string> alreadyIn = new List<string>();

        public static void GenerateHistoricEventsText()
        {
            File.Delete(Hardcoded.HISTORIC_EVENTS);
            FO.WriteAllTextUCS2LEBOM(Hardcoded.HISTORIC_EVENTS, code.ToString().Replace("|", "\\n") + "\r\n");
        }
        public static void Add(string ID, string Title, string Body)
        {
            if (!alreadyIn.Contains(ID))
            {
                code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}");
                code.Append($"\n{{{ID.ToUpper()}_TITLE}}{Title.ToTitleCase()}");
                alreadyIn.Add(ID);
            }
        } // WARNING: TEXT ENTRY IN BRACKETS MUST BE UPPERCASE OTHERWISE CTD!
        public static void Add(string ID, string Title, string Body, string addendum)
        {
            if (!alreadyIn.Contains(ID))
            {
                if (addendum.StartsWith("/"))
                {
                    ProcessRandomPic(ID, addendum);
                    code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}");
                }
                if (addendum.StartsWith("@"))
                {
                    ProcessPic(ID, addendum);
                    code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}");
                }
                else
                {
                    if (!addendum.Contains("-"))
                        code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}||Effect on your Treasury: +{addendum} florins");
                    else
                        code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}||Effect on your Treasury: {addendum} florins");
                }
                code.Append($"\n{{{ID.ToUpper()}_TITLE}}{Title.ToTitleCase()}");
                alreadyIn.Add(ID);
            }
        } // WARNING: TEXT ENTRY IN BRACKETS MUST BE UPPERCASE OTHERWISE CTD!
        public static void Add(string ID, string Title, string Body, string EffectOnTreasury, string PictureFolder)
        {
            if (!alreadyIn.Contains(ID))
            {
                if (!EffectOnTreasury.Contains("-"))
                    code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}||Effect on your Treasury: +{EffectOnTreasury} florins");
                else
                    code.Append($"\n{{{ID.ToUpper()}_BODY}}{Body}||Effect on your Treasury: {EffectOnTreasury} florins");
                code.Append($"\n{{{ID.ToUpper()}_TITLE}}{Title.ToTitleCase()}");
                alreadyIn.Add(ID);
                if (PictureFolder.StartsWith("/"))
                    ProcessRandomPic(ID, PictureFolder);
                if (PictureFolder.StartsWith("@"))
                    ProcessPic(ID, PictureFolder);
                else
                    IO.Log($"ERROR: PictureFolder parameter in Add() must start with / (folder for random picture) or @ (picture file) but is: {PictureFolder}");
            }
        } // WARNING: TEXT ENTRY IN BRACKETS MUST BE UPPERCASE OTHERWISE CTD!

        private static void ProcessPic(string id, string nr)
        {
            nr = nr.Rem("@");
            if (!File.Exists(Settings.P(@$"data\ui\southern_european\eventpics\{id}.tga")))
            {
                try
                {
                    File.Copy(Settings.P(@$"ironclad\picpool\{nr}.tga"), Settings.P(@$"data\ui\southern_european\eventpics\{id}.tga"));
                } catch
                {
                    IO.Log($"Could not process pic for historic event. Id: {id} Nr: {nr}");
                }
            }
        }

        private static void ProcessRandomPic(string id, string pictureFolder)
        {
            pictureFolder = pictureFolder.Rem("/");
            var t = Settings.P(@$"data\ui\southern_european\eventpics\{id}.tga");
            var s = Settings.P(@$"ironclad\picpool\{pictureFolder}");
            if (Directory.Exists(s))
            {
                if (File.Exists(t))
                    File.Delete(t);
                var files = Directory.EnumerateFiles(s, "*.tga");
                if (files.Any())
                    File.Copy(Rndm.Pick(files), t);
            } else
                IO.Log($"ERROR: Picture Source Folder for event {id} does not exist: {s}");
        }
    }
}
