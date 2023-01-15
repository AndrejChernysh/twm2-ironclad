using Ironclad.Entities;
using Ironclad.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ironclad.Helper
{
    static class Validator
    {
        public static void ValidateWorld()
        {
            IO.Log("Hint: Crash after editing map, no map.rwm generated? " +
                "Max. Limit of land-connected regiongroups is 20 " +
                "(e.g. Lisbon and Moscow are land-connected, thus in 1 group). " +
                "Connect islands to main land or other islands with min. 1 pixel in map_regions.tga!");
            ValidateHistoricEvents();
            ValidateUnitsEDBtoEDU();
            ValidateUnitUI();
            ValidateResources();
            ValidateSettlementBuildingExists();
        }

        private static void ValidateSettlementBuildingExists()
        {
            foreach (var file in new List<string>() {Hardcoded.EXPORT_DESCR_ANCILLARIES, Hardcoded.EXPORT_DESCR_CHARACTER_TRAITS, Hardcoded.EXPORT_DESCR_GUILDS})
                foreach (var line in File.ReadLines(file))
                    if (line.Trim().Contains("SettlementBuildingFinished") || line.Trim().Contains("FactionBuildingExists"))
                        IO.Val(World.Buildings.Any(a => a.ID == line.Split("=")[1].Trim()), $"Building {line.Split("=")[1].Trim()} in {file} does not exist");
            IO.Log("Validated SettlementBuildingExists");
        }

        public static void ValidateResources()
        {
            foreach (var resource in World.Resources)
            {
                IO.Val(File.Exists(Settings.P(@$"{resource.Model.ToLower()}")), $"Missing file: {resource.Model.ToLower()}");
                IO.Val(File.Exists(Settings.P(@$"{resource.Icon.ToLower()}")), $"Missing file: {resource.Icon.ToLower()}");
                var content = File.ReadAllText(Settings.P(@$"{resource.Model.ToLower()}"));
                foreach (var line in content.Split("\n"))
                {
                    if (line.Contains("texture") && line.Contains(".tga"))
                    {
                        var fileToSeek = @"data\models_strat\textures\" + line.Split("textures\\")[1].Split(".tga")[0] + ".tga";
                        IO.Val(File.Exists(Settings.P(fileToSeek)), $"Missing file: {fileToSeek}");
                    }
                }
            }
            IO.Log("Validated Resources");
        }
        public static void ValidateUnitUI()
        {
            foreach (var unit in World.Units)
            {
                foreach (var ownership in unit.Factions)
                {
                    var unit_card_path = @$"data\ui\units\{ownership}\#{unit.DictName}.tga";
                    var unit_info_pic_path = @$"data\ui\unit_info\{ownership}\{unit.DictName}_info.tga";
                    IO.Val(File.Exists(Settings.P(unit_card_path)), $"Missing file: {unit_card_path}");
                    IO.Val(File.Exists(Settings.P(unit_info_pic_path)), $"Missing file: {unit_info_pic_path}");
                }
            }
            IO.Log("Validated Unit UI");
        }
        public static void ValidateUnitsEDBtoEDU()
        {
            foreach (var building in World.Buildings)
            {
                var capabilities = Array.Empty<string>();
                if (building.Capabilities.Contains("\n"))
                    capabilities = building.Capabilities.Split("\n");
                else
                    _ = capabilities.Append(building.Capabilities);
                foreach(var capability in capabilities)
                {
                    if (capability.Contains("recruit_pool") && !capability.Trim().StartsWith(";"))
                    {
                        var unitToCheck = capability.Split("\"")[1];
                        IO.Val(World.Units.Count(a => a.IntName == unitToCheck) != 0, $"Unit found in EDB but not in EDU: \"{unitToCheck}\"");
                    }
                }
            }
            IO.Log("Validated Units EDB to EDU");
        }

        internal static void IsUnit(string v, string e)
        {
            IO.Val(World.Units.Any(a => a.IntName == v), $"Invalid unit found in {e}: {v}");
        }

        public static void ValidateHistoricEvents()
        {
            var he = GetAllHistoricEvents();
            var het = GetAllHistoricEventsTxt();
            var missing = he.ConvertAll(d => d.ToUpper()).Except(het.ConvertAll(d => d.ToUpper())).ToList();
            var unused = het.ConvertAll(d => d.ToUpper()).Except(he.ConvertAll(d => d.ToUpper())).ToList();
            if (missing.Count > 0)
            {
                IO.Log($"Missing in {Hardcoded.HISTORIC_EVENTS}:");
                foreach (var entry in missing)
                    IO.Log(entry);
            } else
            {
                IO.Log($"All {he.Count} Historic Events have a text entry.");
                if (het.Count > he.Count)
                {
                    IO.Log($"However, there are text entries which seem to be unused:");
                    foreach(var entry in unused)
                        IO.Log(entry);
                }

            }
            IO.Log("Validated Historic Events");
        }

        internal static void IsFaction(string factionIntName)
        {
            IO.Val(World.Factions.Any(a => a.ID == factionIntName), $"Could not recognize faction: {factionIntName}");
        }
        internal static void IsRID(string rid)
        {
            IO.Val(World.Regions.Any(a => a.RID == rid), $"Could not recognize RID: {rid}");
        }

        internal static int FactionMaxCityLevel(string faction)
        {
            try
            {
                return World.Buildings.Where(a => a.Chain == "core_building" && a.Requirements.Contains(faction)).OrderByDescending(a => a.MinSettlementLevel).First().MinSettlementLevel;
            }
            catch
            {
                return 0;
            }
        }

        public static List<string> GetAllHistoricEventsTxt()
        {
            List<string> AllHistoricEvents = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(File.ReadAllText(Hardcoded.HISTORIC_EVENTS));
            var ContentCampaignScript = sb.ToString().DropBlank("\n", "\r", "}").Split(" ").Where(a => a.Length > 0).ToList();
            ContentCampaignScript.Each((word, n) =>
            {
                if (word.StartsWith("{"))
                    AllHistoricEvents.Add(word.Rem("{", "_BODY", "_TITLE"));
            });
            return AllHistoricEvents.Distinct().ToList();
        }
        public static List<string> GetAllHistoricEvents()
        {
            List<string> AllHistoricEvents = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(File.ReadAllText(Hardcoded.CAMPAIGN));
            while (sb.ToString().Contains("  "))
                sb = sb.Replace("  ", " ");
            var ContentCampaignScript = sb.ToString().DropBlank("\n", "\r", "\t").Split(" ").Where(a => a.Length > 0).ToList();
            ContentCampaignScript.Each((word, n) =>
            {
                if (word == "historic_event")
                    AllHistoricEvents.Add(ContentCampaignScript.ElementAt(n+1));
            });
            sb = new StringBuilder();
            sb.Append(File.ReadAllText(Hardcoded.DESCR_EVENTS));
            while (sb.ToString().Contains("  "))
                sb = sb.Replace("  ", " ");
            ContentCampaignScript = sb.ToString().DropBlank("\n", "\r", "\t").Split(" ").Where(a => a.Length > 0).ToList();
            ContentCampaignScript.Each((word, n) =>
            {
            if (word == "plague" || word == "earthquake")
                    AllHistoricEvents.Add(ContentCampaignScript.ElementAt(n + 1));
            });
            foreach (var hevent in World.Events)
                AllHistoricEvents.Add(hevent.ID);
            return AllHistoricEvents.Distinct().ToList();
        }

        internal static bool isCampaignMapModified()
        {
            var modifiedCounter = 0;
            foreach(var f in new List<string>() { Hardcoded.MAP_FEATURES, Hardcoded.MAP_GROUND_TYPES, Hardcoded.MAP_HEIGHTS, Hardcoded.MAP_REGIONS, Hardcoded.CONFIG })
            {
                if (!File.Exists($"{f}.ts"))
                    File.Create($"{f}.ts");
                if (File.GetLastWriteTimeUtc(f) > File.GetLastWriteTimeUtc($"{f}.ts"))
                {
                    IO.Log($"Recognized as modified: {f}");
                    File.SetLastWriteTimeUtc($"{f}.ts", File.GetLastWriteTimeUtc(f));
                    modifiedCounter++;
                }
            }
            if (modifiedCounter > 0)
                return true;
            return false;
        }
    }
}
