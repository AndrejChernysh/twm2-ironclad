using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Documents;

namespace Ironclad.Features
{
    static class CivilWars
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "CivilWars";
            if (Properties.Settings.Default.cbCivilWars || isAlwaysActive)
            {
                c.Clear();
                HEGenerator.Add($"cw", $"Civil War?",
                    $"Local politicians from our core regions gathered together to declare your reign as obsolete. In the last years they gained much support from all sides and now demand the following:||" +
                    $" - Pay us 20000 florins|| - Disband all your land and naval military|| - Dismiss all your agents||If you accept, no further damage will be done to your empire.||" +
                    $"If you decline their demands, civil war will break out all throughout your empire and its consequences cannot be foreseen.", $"@42");
                HEGenerator.Add("cwa", "Civil War Averted", "Mighty local politicians have been consent with our approval of their terms. Civil war has been averted for now and all our local authorities remain under our control.", "@40");
                HEGenerator.Add("cws", "Civil War Begins", "As negotiations with powerful local authorities and the central government ended without an agreement, it seems like the dispute over the control of our empire will be decided by force. Civil war lies now ahead - may god be with us!", "@57");
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions)
                {
                    c.Append($"\n\tif I_CompareCounter {r.CID}RiotCooloff = 1");
                    c.Append($"\n\t\tconsole_command add_population {r.CID} -5000");
                    foreach (var f in World.PlayableFactions)
                        c.Append(Script.If($"I_SettlementOwner {r.CID} = {f.ID}", $"destroy_buildings {f.ID} hinterland_r false"));
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                foreach (var f in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_conditions I_CompareCounter cww{f.Order} = 1");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    foreach (var r in World.Regions)
                    {
                        c.Append($"\n\tif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\t\tand RandomPercent < {Tuner.CivilWarRebellionChancePerCity}");
                        c.Append(Script.CityRevolt(r.CID));
                        c.Append($"\n\tend_if");
                    }
                    c.Append($"\n\tset_counter cww{f.Order} 0");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                foreach (var e in new List<string>() { "CeasedFactionLeader", "FactionExcommunicated" })
                    foreach (var f in World.PlayableFactions)
                        foreach (var p in Tuner.CivilWarChances)
                        {
                            HEGenerator.Add($"cwai{f.Order}", $"{f.Adjective} Civil War",
                                $"These people have raised their weapons against their own rulers. The {f.Name} is now in the middle of a Civil War - maybe we should help one of the conflict parties?", $"@{f.ID}");
                            c.Append($"\nmonitor_event {e} FactionType {f.ID}");
                            c.Append($"\n\tand I_NumberOfSettlements {f.ID} >= {p.Key.Split("-")[0]}");
                            c.Append($"\n\tand I_NumberOfSettlements {f.ID} <= {p.Key.Split("-")[1]}");
                            c.Append($"\n\tand RandomPercent < {p.Value}");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\n\t\tif I_IsFactionAIControlled {f.ID}");
                            if (f.Culture != "mesoamerican")
                                c.Append($"\n\t\t\thistoric_event cwai{f.Order}");
                            c.Append($"\n\t\t\tset_counter cww{f.Order} 1");
                            c.Append($"\n\t\tend_if");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f.ID}");
                            c.Append(Script.YesNoQuestion("cw"));
                            c.Append($"\n\t\t\tif I_EventCounter cw_accepted = 1");
                            c.Append($"\n\t\t\t\tset_counter cww{f.Order} 0");
                            c.Append($"\n\t\t\t\tadd_money {f.ID} -20000");
                            c.Append($"\n\t\t\t\tdestroy_units {f.ID} free_upkeep_unit");
                            c.Append($"\n\t\t\t\tdestroy_units {f.ID} is_peasant");
                            foreach (var agent in Hardcoded.AgentsRecruitable)
                                c.Append($"\n\t\t\t\tretire_characters {f.ID} {agent}");
                            c.Append($"\n\t\t\t\thistoric_event cwa");
                            c.Append($"\n\t\t\tend_if");
                            c.Append($"\n\t\t\tif I_EventCounter cw_declined = 1");
                            c.Append($"\n\t\t\t\thistoric_event cws");
                            c.Append($"\n\t\t\t\tset_counter cww{f.Order} 1");
                            c.Append($"\n\t\t\tend_if");
                            c.Append($"\n\t\tend_if");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\nend_monitor");
                        }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
