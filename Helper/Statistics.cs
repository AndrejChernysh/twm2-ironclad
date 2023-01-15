using Ironclad.Entities;
using Ironclad.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ironclad.Helper
{
    public static class Statistics
    {
        public static void ShowResources()
        {
            IO.Log("----- Factions -----");
            IO.Log("Faction\tSumCityStartLvls\tBuildings\tPopulation");
            foreach (var f in World.PlayableFactions)
                IO.Log($"{f.ID}\t{World.Regions.Where(a => a.Owner == f.ID).Sum(a => a.StartLvl)}" +
                    $"\t{World.Regions.Where(a => a.Owner == f.ID && a.Buildings.First().StartsWith("random")).Select(a => Convert.ToInt32(a.Buildings.First().Rem("random_"))).Sum()}" +
                    $"\t{World.Regions.Where(a => a.Owner == f.ID).Sum(a => ((a.StartPopMin + a.StartPopMax) / 2))}");
            IO.Log("----- Resources -----");
            var valSum = Convert.ToDouble(World.ResourceOnMapCounts.Sum(a => a.Value));
            foreach (var r in World.ResourceOnMapCounts.OrderByDescending(a => a.Value))
                IO.Log($"Count {World.Resources.First(a => a.ID == r.Key).Name}: {r.Value} ({Math.Round(r.Value / valSum * 100.0, 1)}%)");
            IO.Log("----- Resource Pools -----");
            Dictionary<string, int> d2 = new Dictionary<string, int>();
            var sum2 = 0.0;
            foreach (var pool in World.ResourcePools)
            {
                var result = File.ReadLines(Hardcoded.DESCR_STRAT).Select(line => Regex.Matches(line, $@"(?i)\bPool: {pool.Name}\b").Count).Sum();
                d2.Add(pool.Name, result);
                sum2 = sum2 + result;
            }
            foreach (var r in d2.OrderByDescending(a => a.Value))
                IO.Log($"Count {r.Key}: {r.Value} ({Math.Round(r.Value / sum2 * 100.0, 1)}%)");
        }

    }
}
