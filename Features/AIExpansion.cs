using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class AIExpansion
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "AIExpansion";
            if (Properties.Settings.Default.cbAIExpansion || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\t\tset_counter oaiept 1");
                // Is faction eligible for expansion help?
                foreach (var f in World.PlayableFactions.Where(a => !a.ID.Equals(Hardcoded.PapalFaction)))
                {
                    c.Append($"\n\tif I_NumberOfSettlements {f.ID} = 1");
                    c.Append($"\n\t\tand I_CompareCounter oaiept = 0");
                    c.Append($"\n\t\tand I_CompareCounter aie{f.Order}Cooloff = 0");
                    c.Append($"\n\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\tand RandomPercent < {Tuner.AIExpansionChancePerTurn1Settlement}");
                    c.Append($"\n\t\t\tset_counter aie{f.Order} 1");
                    c.Append(Script.xl() ? $"\nlog always {f.ID} will expand" : "");
                    c.Append($"\n\t\t\tset_counter aie{f.Order}Cooloff {Tuner.AIExpansionTurnIntervalCheck}");
                    c.Append($"\n\t\t\tset_counter oaiept 1");
                    c.Append($"\n\tend_if");
                    c.Append($"\n\tif I_NumberOfSettlements {f.ID} = 2");
                    c.Append($"\n\t\tand I_CompareCounter oaiept = 0");
                    c.Append($"\n\t\tand I_CompareCounter aie{f.Order}Cooloff = 0");
                    c.Append($"\n\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\tand RandomPercent < {Tuner.AIExpansionChancePerTurn2Settlements}");
                    c.Append($"\n\t\t\tset_counter aie{f.Order} 1");
                    c.Append(Script.xl() ? $"\nlog always {f.ID} will expand" : "");
                    c.Append($"\n\t\t\tset_counter aie{f.Order}Cooloff {Tuner.AIExpansionTurnIntervalCheck}");
                    c.Append($"\n\t\t\tset_counter oaiept 1");
                    c.Append($"\n\tend_if");
                    c.Append($"\n\tif I_NumberOfSettlements {f.ID} > 2");
                    c.Append($"\n\t\tand I_CompareCounter oaiept = 0");
                    c.Append($"\n\t\tand I_CompareCounter aie{f.Order}Cooloff = 0");
                    c.Append($"\n\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\t\tset_counter aie{f.Order}Cooloff {Tuner.AIExpansionTurnIntervalCheck}");
                    c.Append($"\n\t\t\tset_counter oaiept 1");
                    c.Append($"\n\tend_if");
                }
                // If faction eligible for expansion, get target from slave
                foreach (var r in World.Regions.Where(a => a.NeighbourRegions.Any(b => !b.IsUnreachable) && !a.IsUnreachable))
                    foreach (var f in World.PlayableFactions.Where(a => !a.ID.Equals(Hardcoded.PapalFaction)))
                    {
                        c.Append($"\n\tif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\t\tand I_CompareCounter aie{f.Order} = 1");
                        foreach (var n in r.NeighbourRegions)
                        {
                            c.Append($"\n\tif I_SettlementOwner {n.CID} = slave");
                            c.Append($"\n\t\tand ! I_SettlementUnderSiege {n.CID}");
                            c.Append($"\n\t\tand I_CompareCounter aie{f.Order} = 1");
                            c.Append($"\n\t\t\tset_counter aie{f.Order}{n.CID} 1");
                            c.Append($"\n\t\t\tset_counter aie{f.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always {f.ID} will expand to {n.CID}" : "");
                            c.Append($"\n\tend_if");
                        }
                        c.Append($"\n\tend_if");
                    }
                // If no target found from slave get regardless of owner
                foreach (var r in World.Regions.Where(a => a.NeighbourRegions.Any(b => !b.IsUnreachable) && !a.IsUnreachable))
                    foreach (var f in World.PlayableFactions.Where(a => !a.ID.Equals(Hardcoded.PapalFaction)))
                    {
                        c.Append($"\n\tif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\t\tand I_CompareCounter aie{f.Order} = 1");
                        foreach (var n in r.NeighbourRegions)
                        {
                            c.Append($"\n\tif ! I_SettlementOwner {n.CID} = {Hardcoded.PapalFaction}");
                            c.Append($"\n\t\tand ! I_SettlementUnderSiege {n.CID}");
                            c.Append($"\n\t\tand I_CompareCounter aie{f.Order} = 1");
                            c.Append($"\n\t\t\tset_counter aie{f.Order}{n.CID} 1");
                            c.Append($"\n\t\t\tset_counter aie{f.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always {f.ID} will expand to {n.CID}" : "");
                            c.Append($"\n\tend_if");
                        }
                        c.Append($"\n\tend_if");
                    }
                // If no target found, invade remote land
                foreach (var f in World.PlayableFactions.Where(a => !a.ID.Equals(Hardcoded.PapalFaction)))
                {
                    c.Append($"\n\tif I_CompareCounter aie{f.Order} = 1");
                    c.Append($"\n\t\tset_counter invasion{f.Order} 1");
                    c.Append(Script.xl() ? $"\nlog always Could not find attack target for {f.ID}, will invade" : "");
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                //Execution
                foreach (var f in World.PlayableFactions.Where(a => !a.ID.Equals(Hardcoded.PapalFaction)))
                {
                    c.Append($"\nmonitor_event FactionTurnStart FactionType {f.ID}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.TerminateIfPlayer(f.ID));
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        c.Append($"\n\tif I_CompareCounter aie{f.Order}{r.CID} = 1");
                        c.Append($"\n\t\tand ! I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\t\tand ! I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append(Script.AttackCity(f, r.CID, 10, 15));
                        c.Append($"\n\t\t\tset_counter aie{f.Order}{r.CID} 0");
                        c.Append($"\n\tend_if");
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
