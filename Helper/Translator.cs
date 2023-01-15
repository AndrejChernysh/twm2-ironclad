using Ironclad.Entities;
using Ironclad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ironclad.Helper
{
    public static class Translator
    {
        public static string TranslateCultureToFaction(string FactionOrCulture, List<Faction> Factions)
        {
            var cultures = Factions.Select(a => a.Culture).ToList();
            if (cultures.Contains(FactionOrCulture))
            {
                return String.Join(",", Factions.Where(a => a.Culture == FactionOrCulture).Select(a => a.ID).ToList()).Trim();
            }
            else
            {
                return FactionOrCulture.Trim();
            }
        }

        internal static string NToA(string Input)
        {
            return Input.Replace("0", "A").Replace("1", "B").Replace("2", "C").Replace("3", "D").Replace("4", "E").Replace("5", "F").Replace("6", "G")
                .Replace("7", "H").Replace("8", "I").Replace("9", "J");
        }

        internal static Region ToRegion(string RidCidOrCityName)
        {
            if (RidCidOrCityName[0].Equals("R") && RidCidOrCityName.Length == 4)
                return World.Regions.First(a => a.RID == RidCidOrCityName);
            if (RidCidOrCityName[0].Equals("C") && RidCidOrCityName.Length == 4)
                return World.Regions.First(a => a.CID == RidCidOrCityName);
            else
                return World.Regions.First(a => a.CityName == RidCidOrCityName);
        }

        internal static string NIDToName(string Input)
        {
            var names = new List<string>();
            if (Input.Contains(" "))
                names = Input.Split(" ").ToList();
            else
            {
                names.Add(Input);
            }
            var result = new List<string>();
            foreach (var name in names)
                result.Add(World.Names.First(a => a.NID == name).Txt); 
            return String.Join(" ", result);
        }

        internal static string Consumable(string consumableShort)
        {
            return World.Resources.First(a => a.Consumable.Substring(0, 2) == consumableShort).ConsumableText;
        }

        internal static string ReplaceCulturesWithFactions(string input)
        {
            foreach (var culture in World.Factions.Select(a => a.Culture).Distinct())
            {
                if (input.Contains(culture)) {
                    var factions = World.Factions.Where(a => a.Culture.Equals(culture)).Select(a => a.ID).ToList();
                    input = input.Replace(culture, String.Join(", ", factions));
                }
            }
            return input;
        }

        public static List<string> TranslateCulturesAndFactionsToFactions(List<string> CulturesAndFactions, List<Faction> Factions)
        {
            List<string> newList = new List<string>();
            foreach (string element in CulturesAndFactions.Select(a => TranslateCultureToFaction(a, Factions)).ToList())
            {
                foreach (string faction in element.Split(","))
                {
                    newList.Add(faction);
                }
            }
            return newList;
        }
        internal static String BuildingEffectsToEffectDescs(String buildingEffect)
        {
            var r = new StringBuilder();
            while (buildingEffect.Contains("\n\n") && buildingEffect.Contains("  "))
                buildingEffect = buildingEffect.Replace("\n\n", "\n").Replace("  ", " ");
            buildingEffect = $"{buildingEffect}\n";
            foreach (var effect in buildingEffect.Split("\n"))
            {
                if (!effect.Trim().StartsWith(";") && BuildingEffectToEffectDesc(effect) != "")
                    r.Append($"\\n{BuildingEffectToEffectDesc(effect)}");
            }
            if (r.ToString().StartsWith("\\n"))
                return r.ToString().Remove(0, 2);
            return r.ToString();
        }

        private static String BuildingEffectToEffectDesc(String e)
        {
            if (e.StartsWith(" "))
                e = e.Remove(0, 1);
            var p = "";
            if (e.Contains("requires"))
                p = "*";
            if (e.StartsWith("pope_approval"))
                return $"Improves relations with the Pope";
            if (e.StartsWith("weapon_melee_blade"))
                return $"Improves melee weapons: +{(Convert.ToInt32(e.Split(" ")[1])) * 33 + 1}%{p}";
            if (e.StartsWith("construction_time_bonus_other"))
                return $"Speeds up construction: +{Convert.ToInt32(e.Split(" ")[1])}%{p}";
            if (e.StartsWith("mine_resource"))
                return $"Effect on income: +{Convert.ToInt32(e.Split(" ")[1]) * 5}%{p}";
            if (e.StartsWith("stage_games"))
                return $"Enables dog races{p}";
            if (e.StartsWith("stage_races"))
                return $"Enables horse races{p}";
            if (e.StartsWith("trade_fleet"))
                return $"Provides additional trade ships: +{e.Split(" ")[1]}{p}";
            if (e.StartsWith("free_upkeep bonus"))
                return $"Provides additional free upkeep for units recruitable here: +{e.Split(" ")[2]}{p}";
            if (e.StartsWith("recruits_exp_bonus"))
                return $"Increases experience for units: +{e.Split(" ")[1]}{p}";
            if (e.StartsWith("gun_bonus"))
                return $"Increases experience for gunpowder units: +{e.Split(" ")[1]}{p}";
            if (e.StartsWith("cavalry_bonus bonus"))
                return $"Increases experience for non-heavy cavalry units: +{e.Split(" ")[2]}{p}";
            if (e.StartsWith("heavy_cavalry_bonus bonus"))
                return $"Increases experience for heavy cavalry units: +{e.Split(" ")[2]}{p}";
            if (e.StartsWith("road_level"))
                return $"Provides roads: {RoadLevelToTxt(e.Split(" ")[1])}{p}";
            if (e.StartsWith("armour"))
                return $"Provides armour: {ArmourLevelToTxt(e.Split(" ")[1])}{p}";
            if (e.StartsWith("gate_strength"))
                return $"Gate Strength: +{(Convert.ToInt32(e.Split(" ")[1])) * 50}%{p}";
            if (e.StartsWith("wall_level"))
                return $"Wall Strength: +{(Convert.ToInt32(e.Split(" ")[1]) + 1) * 20}%{p}";
            if (e.StartsWith("tower_level"))
                return $"Tower Strength: +{(Convert.ToInt32(e.Split(" ")[1])) * 33 + 1}%{p}";
            if (e.StartsWith("recruitment_slots"))
                return $"Provides additional recruitment slots: +{Convert.ToInt32(e.Split(" ")[1]) - 1}{p}";
            if (e.StartsWith("farming_level"))
                return $"Population growth: +{Convert.ToDouble(e.Split(" ")[1]) / 2}%\\nEffect on income: +{Convert.ToInt32(e.Split(" ")[1])}%{p}";
            if (e.StartsWith("income_bonus bonus"))
            {
                var v = Convert.ToInt32(e.Split(" ")[2]);
                if (v > 0)
                    return $"Fixed income per round: +{v} florins{p}";
                return $"Fixed cost per round: {v} florins{p}";
            }
            if (e.StartsWith("law_bonus bonus") || e.StartsWith("happiness_bonus bonus"))
                return $"Effect on civil order: +{Convert.ToInt32(e.Split(" ")[2]) * 5}%{p}".Replace("+-", "-");
            if (e.StartsWith("population_health_bonus bonus"))
                return $"Effect on civil health: +{Convert.ToInt32(e.Split(" ")[2]) * 5}%{p}".Replace("+-", "-");
            if (e.StartsWith("population_growth_bonus bonus"))
                return $"Population growth: +{Convert.ToDouble(e.Split(" ")[2]) / 2}%{p}".Replace("+-", "-");
            if (e.StartsWith("trade_base_income_bonus bonus"))
                return $"Effect on income: +{Convert.ToInt32(e.Split(" ")[2]) * 5}%{p}".Replace("+-", "-");
            if (e.StartsWith("religion_level bonus"))
                return $"Improves religious conversion: +{Convert.ToInt32(e.Split(" ")[2]) * 5}%{p}".Replace("+-", "-");
            if (e.StartsWith("amplify_religion_level"))
                return $"Amplifies religious conversion: +{Convert.ToDouble(e.Split(" ")[1]) * 30}%{p}".Replace("+-", "-");
            if (e.StartsWith("retrain_cost_bonus bonus"))
                return $"Reduces cost of retraining units: -{Convert.ToInt32(e.Split(" ")[2]) * 10}%{p}";
            if (e.StartsWith("recruitment_cost_bonus_naval bonus"))
                return $"Reduces cost of naval unit recruitment: -{Convert.ToInt32(e.Split(" ")[2]) * 10}%{p}";
            if (e.StartsWith("agent_limit"))
                return $"Increases {e.Split(" ")[1]} agent limit by: +{e.Split(" ")[2]}{p}";
            if (e.StartsWith("construction_cost_bonus_stone bonus"))
                return $"Decreases cost of stone structures: -{Convert.ToInt32(e.Split(" ")[2])}%{p}";
            if (e.StartsWith("construction_cost_bonus_wooden bonus"))
                return $"Decreases cost of wooden structures: -{Convert.ToInt32(e.Split(" ")[2])}%{p}";
            return "";
        }

        private static String RoadLevelToTxt(string v)
        {
            if (v == "0")
                return "Dirt Roads (+25% movement)";
            if (v == "1")
                return "Paved Roads (+50% movement)";
            if (v == "2")
                return "Advanced Paved Roads (+75% movement)";
            return v;
        }
        private static String ArmourLevelToTxt(string v)
        {
            if (v == "1")
                return "Padded, Leather (+16% protection)";
            if (v == "2")
                return "Mail, Lamellar (+32% protection)";
            if (v == "3")
                return "Heavy Mail, Medium Lamellar (+48% protection)";
            if (v == "4")
                return "Partial Plate, Heavy Lamellar, Splint (+64% protection)";
            if (v == "5")
                return "Full Plate, Kataphract, Heavy Splint (+80% protection)";
            if (v == "6")
                return "Advanced Plate (+96% protection)";
            return v;
        }

        internal static object IntToExReligion(string religion)
        {
            return religion.Title();
        }

        internal static object IntToExCulture(string culture)
        {
            return culture.DropBlank("_").Title();
        }

        public static string TranslateCityID(int ID)
        {
            return "C" + ID.ToString("D3");
        }

        internal static string CityLevelToCityLevelName(int cityLevel, List<SettlementLevel> sl)
        {
            var q = from s in sl where s.Level == cityLevel select s.Name;
            return q.First();
        }

        internal static string Storage(int i, bool align=false)
        {
            if (!align && i == 0)
                return "0%";
            if (!align && i == 1)
                return "20%";
            if (!align && i == 2)
                return "40%";
            if (!align && i == 3)
                return "60%";
            if (!align && i == 4)
                return "80%";
            if (align && i == 0)
                return "   0%";
            if (align && i == 1)
                return "  20%";
            if (align && i == 2)
                return "  40%";
            if (align && i == 3)
                return "  60%";
            if (align && i == 4)
                return "  80%";
            if (i == 5)
                return "100%";
            return "?";
        }
    }
}
