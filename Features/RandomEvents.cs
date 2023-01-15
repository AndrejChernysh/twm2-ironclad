using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class RandomEvents
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "RNGE";
            if (Properties.Settings.Default.cbRNGE || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append($"\n\tand I_TurnNumber > 1");
                c.Append($"\n\tand RandomPercent < {Tuner.RNGEChance}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tgenerate_random_counter x 0 |xx|");
                var cnt = 0;
                foreach (var s in new Dictionary<string, int>() { { "Currency Value", 15 }, { "Employment", 20 }, { "Export Quota", 25 }, { "Production Output", 30 }, { "Tax Quota", 35 } })
                    foreach (var a in new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 })
                        foreach (var m in new Dictionary<string, int>() { { "rose", 1 }, { "fell", -1 } })
                        {
                            var value = s.Value * a * m.Value;
                            c.Append(Script.If($"I_EventCounter x = {cnt}", $"{Script.AddKingsPurseToPlayer(value)}\n\thistoric_event RNGE{cnt}"));
                            HEGenerator.Add($"RNGE{cnt}", $"{s.Key} +{a * m.Value}%".Replace("+-", "-"), $"{s.Key} recently {m.Key} by {a}%.||Effect on Fixed Income: +{value}".Replace("+-", "-"), "@44");
                            cnt++;
                        }
                foreach (var r in World.Regions.Where(a => !a.IsNewWorld && !a.IsUnreachable))
                {
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append($"\n\t\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                    c.Append(Script.AlterRecruitPoolUnits(r.RID, -2));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    HEGenerator.Add($"RNGE{cnt}", $"Recruits shortage in {r.CityName}",
                        $"{r.CityName} in {r.RegionName} is in the midst of a shortage regarding young men willing to fight and die for their king and country.", "@28");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append($"\n\t\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                    c.Append(Script.AlterRecruitPoolUnits(r.RID, 1));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    HEGenerator.Add($"RNGE{cnt}", $"Recruits surplus in {r.CityName}",
                        $"{r.CityName} in {r.RegionName} is in the midst of a surplus regarding young men willing to fight and die for their king and country.", "@61");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append(Script.FireInCityOpticalAndEffect(r.RID));
                    c.Append(Script.DamageAllBuildings(r.CID, false));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    HEGenerator.Add($"RNGE{cnt}", $"Great Fire of {r.CityName}",
                        $"{r.CityName} in {r.RegionName} has been struck by an unprecedented fire catastrophe which destroyed a big part of the city.", "@59");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append(Script.StormInCity(r.RID));
                    c.Append(Script.DamageAllBuildings(r.CID, false));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    HEGenerator.Add($"RNGE{cnt}", $"Heavy Storm in {r.RegionName}",
                        $"{r.RegionName} has been struck by an unprecedented storm catastrophe which destroyed a big part of the regions infrastucture and especially the settlement of {r.CityName}.", "@35");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append(Script.DamageAllBuildings(r.CID, false));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    c.Append(Script.FireInRegionOptical(r.RID, 5));
                    HEGenerator.Add($"RNGE{cnt}", $"Earthquake in {r.RegionName}",
                        $"{r.RegionName} has been struck by an unprecedented earthquake which destroyed a big part of the regions infrastucture and especially the settlement of {r.CityName}.", "@45");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append(Script.SpawnAgent("heretic", r, "slave", 2));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    HEGenerator.Add($"RNGE{cnt}", $"Heresy in {r.RegionName}",
                        $"{r.RegionName} has been struck by an unprecedented level of heresy - a group is trying to found a new faith in {r.CityName}.", "@37");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                    c.Append($"\n\t\t\tand ! I_SettlementOwner {r.CID} = slave");
                    c.Append(Script.CityRevolt(r.CID));
                    c.Append(Script.AttackCity(World.Factions.First(a => a.ID == "slave"), r.CID, 15, 18));
                    c.Append($"\n\t\t\thistoric_event RNGE{cnt}");
                    HEGenerator.Add($"RNGE{cnt}", $"{r.RegionName} declares Independence",
                        $"Radical bandits got the power over {r.CityName} and news have it, that the Region of {r.RegionName} is now an independent state.", "@65");
                    c.Append($"\n\t\tend_if");
                    cnt++;
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Replace("|xx|", $"{cnt}");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
