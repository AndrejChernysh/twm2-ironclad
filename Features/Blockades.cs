using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Blockades
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Blockades";
            var order = 11;
            if (Properties.Settings.Default.cbBlockades || isAlwaysActive)
            {
                c.Clear();
                //Reset counters to 0
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => a.HasPort && !a.IsUnreachable))
                    c.Append($"\n\t\tset_counter pb{r.RID} 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                //Set counter pbRID to 1 if blockaded
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var fAttacker in World.Factions)
                    foreach (var r in World.Regions.Where(a => a.HasPort && !a.IsUnreachable))
                    {
                        var l = 33 * r.Resources.First().Value;
                        var p = System.Convert.ToInt32(System.Convert.ToDecimal(l) * System.Convert.ToDecimal(0.25));
                        if (World.AllPositions.Count(a => a.RegionID == r.ID && a.IsPort) == 0)
                            IO.Log($"ControllerBlockades: Region {r.RegionName} HasPort but did not find any position at map");
                        else
                        {
                            var portpos = World.AllPositions.First(a => a.RegionID == r.ID && a.IsPort);
                            //Blockades with Ships 75,89 MB
                            foreach (var d in 2.To(2)) // 2 to 3 is best coverage but costs more RAM
                            {
                                c.Append($"\n\tif I_CharacterTypeNearTile {fAttacker.ID} admiral, {d} {portpos.X}, {portpos.Y}");
                                c.Append($"\n\t\tand I_CompareCounter pb{r.RID} = 0");
                                foreach (var fOwner in World.Factions.Where(a => a.ID != fAttacker.ID && a.ID != "slave"))
                                {
                                    c.Append($"\n\t\tif I_SettlementOwner {r.CID} = {fOwner.ID}");
                                    c.Append($"\n\t\t\tand I_CompareCounter {Script.GetIsWarCounter(fAttacker, fOwner)} = 1");
                                    c.Append($"\n\t\t\tand I_CompareCounter pb{r.RID} = 0");
                                    c.Append($"\n\t\t\tand RandomPercent < {Tuner.BlockadesChancePerTurn}");
                                    c.Append($"\n\t\t\tset_counter pb{r.RID} 1");
                                    c.Append($"\n\t\t\tadd_money {fOwner.ID} -{l + p}");
                                    c.Append($"\n\t\t\tadd_money {fAttacker.ID} {l}");
                                    HEGenerator.Add($"port{r.CID}b{fAttacker.Order}",
                                        $"{r.RegionName} blockaded (-{l + p} florins)", $"The {fAttacker.Name} is blockading our port of {r.RegionName}. They took from us the following incoming resource: {r.Resources.First().Name}.||{l} florins were looted by {fAttacker.NameShort}.||{p} florins were lost due to destroyed trade goods.", $"-{l + p}", "@2");
                                    HEGenerator.Add($"port{r.CID}o",
                                        $"{r.RegionName} blockaded (+{l} florins)", $"Our fleet is blockading the port of {r.RegionName}. We took and liquidated the following resource: {r.Resources.First().Name}.", $"{l}", "@55");
                                    c.Append(Script.IfCounter($"isPlayer{r.CID}", 1, $"historic_event port{r.CID}b{fAttacker.Order}"));
                                    c.Append(Script.If($"! I_IsFactionAIControlled {fAttacker.ID}", $"historic_event port{r.CID}o"));
                                    c.Append(Script.xl() ? $"\nlog always pb {r.RID} {fOwner.ID} -{l + p} {fAttacker.ID} +{l}" : "");
                                    c.Append($"\n\t\tend_if");
                                }
                                c.Append($"\n\t\tif I_SettlementOwner {r.CID} = slave");
                                c.Append($"\n\t\t\tand I_CompareCounter pb{r.RID} = 0");
                                c.Append($"\n\t\t\tand RandomPercent < {Tuner.BlockadesChancePerTurnSlave}");
                                c.Append($"\n\t\t\tset_counter pb{r.RID} 1");
                                c.Append($"\n\t\t\tadd_money {fAttacker.ID} {l}");
                                c.Append(Script.If($"! I_IsFactionAIControlled {fAttacker.ID}", $"historic_event port{r.CID}o"));
                                c.Append(Script.xl() ? $"\nlog always pb {r.RID} slave -{l + p} {fAttacker.ID} +{l}" : "");
                                c.Append($"\n\t\tend_if");
                                c.Append($"\n\tend_if");
                            }
                        }
                    }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                // Graphical effect
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => a.HasPort && !a.IsUnreachable))
                    if (World.AllPositions.Count(a => a.RegionID == r.ID && a.IsPort) != 0)
                        c.Append(Script.IfCounter($"pb{r.RID}", 1, Script.FireInPositionOptical(World.AllPositions.First(a => a.RegionID == r.ID && a.IsPort), 1)));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
            }
            return new Script(scriptGroup, "", isAlwaysActive, order);
        }
    }
}
