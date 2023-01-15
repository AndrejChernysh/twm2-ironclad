using Ironclad.Entities;
using Ironclad.Extensions;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ironclad.Helper
{
    public static class Rndm
    {
        private static readonly Random random = new Random();
        public static double Dbl(double minValue, double maxValue)
        {
            var next = random.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }
        public static int Int(int minValue, int maxValue)
        {
            var next = random.Next(minValue - 1, maxValue + 1);
            while (next < minValue || next > maxValue)
                next = random.Next(minValue - 1, maxValue + 1);
            return next;
        }

        internal static void RandomizeMusic()
        {
            File.Delete(Hardcoded.DESCR_SOUNDS_MUSIC);
            CreateDescrSoundsMusic();
        }

        internal static void RandomizeLoadingScreens()
        {
            var files = Directory.GetFiles(Hardcoded.LOADING_SCREEN, "loading_screen_*.tga", SearchOption.AllDirectories).ToList();
            foreach (var file in files)
                File.Move(file, $@"{Hardcoded.LOADING_SCREEN}\loading_screen_{Rndm.Int(0, 999999999)}.tga");
            files = Directory.GetFiles(Hardcoded.LOADING_SCREEN, "loading_screen_*.tga", SearchOption.AllDirectories).ToList();
            var i = 0;
            foreach (var file in files)
            {
                File.Move(file, $@"{Hardcoded.LOADING_SCREEN}\loading_screen_{i}.tga");
                i++;
            }
            IO.Log("Randomized loading screens");
        }


        private static void CreateDescrSoundsMusic()
        {
            StringBuilder c = new StringBuilder();
            c.Append(String.Concat(Enumerable.Repeat(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;\r\n", 9)));
            c.Append("\r\nDEFAULT: priority 9999 randomdelay 0 pan .5 volume 0");
            c.Append("\r\nDEFAULT: dry_level 1.0 wet_level 0.0 fadeout 1");
            c.Append("\r\nDEFAULT: pref MUSIC STREAMED");
            c.Append("\r\nrequired_samples_cutoff 3");
            c.Append("\r\nmusic_timeout   600");
            c.Append("\r\nmusic_retrigger 5"); // Pause between music samples
            c.Append("\r\nmusic_fade_in 0");
            c.Append("\r\nmusic_fade_out 3");
            c.Append("\r\nmusic_fade_out_timeout 10");
            c.Append("\r\nDEFAULT: streamed");
            c.Append("\r\nBANK: music_bank");
            c.Append("\r\n\tmusic_type northern_european");
            c.Append("\r\n\t\tstate MUSIC_FRONTEND");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("menu_*"));
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_TENSION");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("tension_euro*", Settings.MusicCampaignVariations));
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_MOBILIZE");
            c.Append("\r\n\t\t\tevent");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music ");
            c.Append(ListMp3Files("deployment_euro*", Settings.MusicCampaignVariations));
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_BATTLE");
            c.Append("\r\n\t\t\tevent");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("battle_euro*", Settings.MusicCampaignVariations));
            c.Append("\r\n            end     ");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_DRAW");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_STRATMAP_SUMMER");
            c.Append("\r\n\t\t\tevent delay 5 ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append(ListMp3Files("campaign_euro*", Settings.MusicCampaignVariations));
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\t\tstate MUSIC_STRATMAP_WINTER");
            c.Append("\r\n\t\t\tevent delay 5 ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append(ListMp3Files("campaign_euro*", Settings.MusicCampaignVariations));
            c.Append("\r\n\t\t\tend\t\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_LOADING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\t\tstate MUSIC_PREBATTLE_SCROLL");
            c.Append("\r\n\t\t\tevent  ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_DRAW");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_VICTORY");
            c.Append("\r\n\t\t\tevent looped ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_DEFEAT");
            c.Append("\r\n\t\t\tevent looped ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\tstate MUSIC_CREDITS1 delay 3");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(RandomMp3File("campaign_euro*"));
            c.Append("\r\n            \t\t\tend\t");
            c.Append("\r\n\tstate MUSIC_CREDITS2\t");
            c.Append("\r\n\t\t\tevent \t");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music\t");
            c.Append(RandomMp3File("campaign_euro*"));
            c.Append("\r\n            \t\t\tend");
            c.Append("\r\n\tmusic_type middle_eastern");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_TENSION");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("tension_muslim*", Settings.MusicCampaignVariations));
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_MOBILIZE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("deployment_muslim*", Settings.MusicCampaignVariations));
            c.Append("\r\n            \t\t\tend             ");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_BATTLE       ");
            c.Append("\r\n\t\t\tevent                       ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("battle_muslim*", Settings.MusicCampaignVariations));
            c.Append("\r\n            \t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_DRAW");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_STRATMAP_SUMMER");
            c.Append("\r\n\t\t\tevent delay 5 ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("campaign_muslim*", Settings.MusicCampaignVariations));
            c.Append("\r\n            \t\t\tend\t\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_LOADING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t\t");
            c.Append("\r\n\t\tstate MUSIC_PREBATTLE_SCROLL");
            c.Append("\r\n\t\t\tevent  ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_DRAW");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t\t\t\t\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_VICTORY");
            c.Append("\r\n\t\t\tevent  looped");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_DEFEAT");
            c.Append("\r\n\t\t\tevent  looped");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\tmusic_type mesoamerican");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_TENSION");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("tension_america*", Settings.MusicCampaignVariations));
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_MOBILIZE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("deployment_america*", Settings.MusicCampaignVariations));
            c.Append("\r\n            \t\t\tend             ");
            c.Append("\r\n\t\tstate MUSIC_BATTLE_BATTLE       ");
            c.Append("\r\n\t\t\tevent                       ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("battle_america*", Settings.MusicCampaignVariations));
            c.Append("\r\n            \t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_WIN_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_LOSE_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_DRAW");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_STRATMAP_SUMMER");
            c.Append("\r\n\t\t\tevent delay 5 ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append(ListMp3Files("campaign_america*", Settings.MusicCampaignVariations));
            c.Append("\r\n            \t\t\tend\t\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_LOADING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t\t");
            c.Append("\r\n\t\tstate MUSIC_PREBATTLE_SCROLL");
            c.Append("\r\n\t\t\tevent  ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_WIN_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_CLOSE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_AVERAGE");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_LOSE_CRUSHING");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend");
            c.Append("\r\n\t\tstate MUSIC_RESULT_STRAT_DRAW");
            c.Append("\r\n\t\t\tevent ");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t\t\t\t\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_VICTORY");
            c.Append("\r\n\t\t\tevent  looped");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            c.Append("\r\n\t\tstate MUSIC_CAMPAIGN_DEFEAT");
            c.Append("\r\n\t\t\tevent  looped");
            c.Append("\r\n\t\t\t\tfolder data/sounds/music");
            c.Append("\r\n\t\t\tend\t");
            File.WriteAllText(Hardcoded.DESCR_SOUNDS_MUSIC, c.ToString());
        }

        internal static bool Chance(int percentage)
        {
            return Int(0, 99) < percentage;
        }

        private static string ListMp3Files(string pattern, int amount=0)
        {
            var files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(Settings.P("data/sounds/music"), $"{pattern}.mp3"))
            {
                files.Add(Path.GetFileName(file));
            }
            IO.Val(files.Any(), $"No MP3-Files found for pattern {pattern} in data/sounds/music");
            var shuffledFiles = files.Select(x => new { value = x, order = new Random().Next() }).OrderBy(x => x.order).Select(x => x.value).ToList();
            if (amount > 0)
                return $"\r\n\t\t\t\t{string.Join("\r\n\t\t\t\t", shuffledFiles.Take(amount))}";
            return $"\r\n\t\t\t\t{string.Join("\r\n\t\t\t\t", shuffledFiles)}"; 
        }

        private static string RandomMp3File(string pattern)
        {
            var files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(Settings.P("data/sounds/music"), $"{pattern}.mp3"))
            {
                files.Add(Path.GetFileName(file));
            }
            IO.Val(files.Any(), $"No MP3-Files found for pattern {pattern} in data/sounds/music");
            return $"\r\n\t\t\t\t{Pick(files)}";
        }

        internal static object RandomWord(string v1, string v2, string v3="", string v4="", string v5="")
        {
            var words = new List<string> { v1, v2, v3, v4, v5 };
            var actualWords = words.Where(a => a != "").ToList();
            var count = actualWords.Count;
            return actualWords.ElementAt(Int(0, count - 1));
        }
        internal static List<Region> Pick(List<Region> list, int amount)
        {
            var result = new List<Region>();
            var picked = new List<string>();
            var i = 0;
            while (i < amount)
            {
                var newElement = list[Int(0, list.Count - 1)];
                if (!picked.Contains(newElement.RID))
                {
                    result.Add(newElement);
                    picked.Add(newElement.RID);
                    i++;
                }
            }
            return result;
        }
        internal static List<Position> Pick(List<Position> list, int amount)
        {
            var result = new List<Position>();
            var picked = new List<string>();
            var i = 0;
            while (i < amount)
            {
                var newElement = list[Int(0, list.Count - 1)];
                if (!picked.Contains($"{newElement.X}|{newElement.Y}"))
                {
                    result.Add(newElement);
                    picked.Add($"{newElement.X}|{newElement.Y}");
                    i++;
                }
            }
            return result;
        }
        internal static string Pick(List<string> list)
        {
            return list[Int(0, list.Count - 1)];
        }
        internal static bool Pick(bool b1, bool b2)
        {
            if (Chance(50))
                return b1;
            return b2;
        }
        internal static Position Pick(List<Position> list)
        {
            return list[Int(0, list.Count - 1)];
        }

        internal static Region Pick(List<Region> list)
        {
            return list[Int(0, list.Count - 1)];
        }
        internal static Region Pick(IEnumerable<Region> list)
        {
            return list.ElementAt(Int(0, list.Count() - 1));
        }
        internal static string Pick(IEnumerable<string> list)
        {
            return list.ElementAt(Int(0, list.Count() - 1));
        }

        internal static Position Pick(IEnumerable<Position> list)
        {
            return list.ElementAt(Int(0, list.Count() - 1));
        }

        internal static Position ArmyPositionInRegion(string rid)
        {
            var region = World.Regions.First(a => a.RID == rid);
            var position = Rndm.Pick(World.AllPositions.Where(a => a.RegionID == region.ID && a.IsSuitableForResource));
            return position;
        }
        internal static Resource Pick(IEnumerable<Resource> list)
        {
            return list.ElementAt(Int(0, list.Count() - 1));
        }

        internal static object Pick(Dictionary<int, string>.KeyCollection keys)
        {
            return keys.ElementAt(Int(0, keys.Count - 1));
        }

        internal static Position GetFleetPositionNearRegion(Region r)
        {
            var positions = World.AllPositions.Where(a => a.IsSea && !a.IsHighSea && a.X.IsBetween(r.Position.X - 10, r.Position.X + 10) && a.Y.IsBetween(r.Position.Y - 10, r.Position.Y + 10)).ToList();
            if (positions.IsEmpty())
                positions = World.AllPositions.Where(a => a.IsSea && !a.IsHighSea && a.X.IsBetween(r.Position.X - 15, r.Position.X + 15) && a.Y.IsBetween(r.Position.Y - 15, r.Position.Y + 15)).ToList();
            if (positions.IsEmpty())
                positions = World.AllPositions.Where(a => a.IsSea && !a.IsHighSea && a.X.IsBetween(r.Position.X - 30, r.Position.X + 30) && a.Y.IsBetween(r.Position.Y - 30, r.Position.Y + 30)).ToList();
            if (positions.IsEmpty())
                positions = World.AllPositions.Where(a => a.IsSea && !a.IsHighSea && a.X.IsBetween(r.Position.X - 100, r.Position.X + 100) && a.Y.IsBetween(r.Position.Y - 100, r.Position.Y + 100)).ToList();
            return Pick(positions);
        }
    }
}
