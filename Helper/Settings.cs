using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ironclad.Helper
{
    static class Settings
    {
        public static string VERSION = File.ReadAllLines(Hardcoded.VERSIONLOG).Last(a => a.StartsWith("VERSION")).Split(" ")[1];
        public static String cachePositionsAll = P(@"ironclad\cache\positions_all.csv");
        public static String cachePositionsSettlements = P(@"ironclad\cache\positions_settlements.csv");
        public static String cacheRegionNeighbours = P(@"ironclad\cache\region_neighbours.csv");
        public static int MusicCampaignVariations = 128; // uses RAM!
        internal static decimal warnLimitSubscriptSizeMB = 80;
        internal static int warnLimitMonitorsCount = 1000;
        internal static bool enableOpticalEvents = true;
        public static double WaitTimeEndTurnSecondsPerMonitor = 0.003;
        public static Dictionary<int, string> Parties = new Dictionary<int, string>() { { 1, "Monarchists" }, { 2, "Feudalists" }, { 3, "Republicans" } };
        public static Dictionary<string, string> PartiesEffects = new Dictionary<string, string>() { { "Monarchists", "+0.5% population growth" }, { "Feudalists", "+1 free upkeep unit in each settlement" }, { "Republicans", "+5% public order" } };
        public static Dictionary<string, string> PartiesPic = new Dictionary<string, string>() { { "Monarchists", "@62" }, { "Feudalists", "@24" }, { "Republicans", "@61" } };

        public static String P(string path) {
            return Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.GameExeFullpath), @"mods\retrofit", path);
        }
        public static String P()
        {
            return Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.GameExeFullpath), @"mods\retrofit");
        }
        public static String PG(string path) {
            return Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.GameExeFullpath), path);
        }
        public static String PG()
        {
            return Path.GetDirectoryName(Properties.Settings.Default.GameExeFullpath);
        }
    }
}
