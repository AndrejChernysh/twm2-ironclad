using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Raids
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Raids";
            if (Properties.Settings.Default.cbRaids || isAlwaysActive)
            {
                c.Clear();
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable && a.Resources.Any()))
                {
                    c.Append($"\nmonitor_event CharacterTurnEnd FactionIsLocal");
                    c.Append($"\nand Trait GeneralPush = 1");
                    c.Append($"\nand ! IsBesieging");
                    c.Append($"\nand IsRegionOneOf {r.RID}");
                    c.Append($"\nand I_CompareCounter orpt{r.CID}Cooloff = 0");
                    c.Append($"\nand I_CompareCounter isEnemy{r.CID} = 1");
                    c.Append($"\nand RemainingMPPercentage > 85");
                    c.Append($"\nand RandomPercent < {Tuner.RaidsChanceMoney}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\tset_counter orpt{r.CID}Cooloff 1");
                    c.Append($"\n\tgenerate_random_counter x {Tuner.RaidsMultiplierMin} {Tuner.RaidsMultiplierMax}");
                    foreach (var m in Tuner.RaidsMultiplierMin.To(Tuner.RaidsMultiplierMax))
                    {
                        var loot = r.Resources.Select(a => a.Value).Sum() * m;
                        c.Append($"\n\t\tif I_EventCounter x = {m}");
                        c.Append(Script.AddMoneyToPlayer(loot));
                        c.Append($"\n\t\thistoric_event hll{loot}{r.CID}");
                        HEGenerator.Add($"hll{loot}{r.CID}", $"Plundered {loot} florins", $"Our armies have pillaged the hinterlands of {r.RegionName} and were able to loot a total of {loot} florins.||The following resources seem to be found in these lands: {string.Join(", ", r.Resources.Select(a => a.Name))}.", "@17");
                        c.Append($"\n\t\tend_if");
                    }
                    if (Properties.Settings.Default.cbConsumables && r.Resources.Any(a => a.Consumable != "NULL"))
                    {
                        foreach(var p in r.ResourcePositions)
                            foreach(var f in World.PlayableFactions)
                            {
                                c.Append($"\n\t\tif I_CharacterTypeNearTile {f.ID} family, 0 {p.X}, {p.Y}");
                                c.Append($"\n\t\tand I_LocalFaction {f.ID}");
                                c.Append($"\n\t\tand RandomPercent < {Tuner.RaidsChanceConsumables}");
                                c.Append($"\n\t\tand I_CompareCounter s{r.Resources.First().Consumable} < {Tuner.ConsumablesStorageMax}");
                                c.Append(Script.FireInPositionOptical(p));
                                c.Append($"\n\t\t\tinc_counter s{r.Resources.First().Consumable} 1");
                                c.Append($"\n\t\t\thistoric_event rr{r.Resources.First().Consumable}{r.RID}");
                                HEGenerator.Add($"rr{r.Resources.First().Consumable}{r.RID}", $"+1 {r.Resources.First().ConsumableText} raided", $"Our troops raided {r.Resources.First().ConsumableText} in the hostile region of {r.RegionName}.", $"@{r.Resources.First().ConsumableText}");
                                c.Append($"\n\t\tend_if");
                            }
                    }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
