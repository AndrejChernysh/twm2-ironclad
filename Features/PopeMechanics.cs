using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class PopeMechanics
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "PopeMechanics";
            if (Properties.Settings.Default.cbPopeMechanics || isAlwaysActive)
            {
                c.Clear();
                var catholicFactionsExPope = World.Factions.Where(a => a.Religion == "catholic" && a.ID != Hardcoded.PapalFaction);
                var papalFaction = World.Factions.First(a => a.ID == Hardcoded.PapalFaction);
                foreach (var f in catholicFactionsExPope)
                {
                    c.Append($"\nmonitor_event FactionExcommunicated FactionType { f.ID}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    foreach (var f2 in catholicFactionsExPope.Where(a => a.ID != f.ID))
                        c.Append(Script.IfCounter(Script.GetIsWarCounter(f2, papalFaction), 0, Script.SetFactionStanding(f2.ID, f.ID, -1.0)));
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable && World.Factions.First(x => x.ID == a.HomeFaction).Religion == "catholic"))
                    {
                        c.Append($"\nif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append(Script.IfChance(Tuner.ExcommunicationTurmoilChancePerSettlement, $"add_settlement_turmoil {r.CID} 16"));
                        c.Append(Script.IfChance(Tuner.ExcommunicationHeresyChancePerSettlement, Script.SpawnAgent("heretic", r, f.ID)));
                        c.Append($"\nend_if");
                    }
                    c.Append($"\nretire_characters {f.ID} priest");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");

                    c.Append($"\nmonitor_event PopeElected TargetFactionType {f.ID}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    foreach (var f2 in catholicFactionsExPope.Where(a => a.ID != f.ID))
                    {
                        c.Append($"\nif I_CompareCounter isPope{f2.Order} = 1");
                        c.Append($"\n\tset_counter isPope{f2.Order} 0");
                        c.Append(Script.SetDiplomaticStance(f2.ID, Hardcoded.PapalFaction, "neutral"));
                        c.Append($"\nend_if");
                    }
                    c.Append(Script.SetDiplomaticStance(f.ID, Hardcoded.PapalFaction, "allied"));
                    c.Append(Script.SetFactionStanding(Hardcoded.PapalFaction, f.ID, 1.0));
                    c.Append($"\n\tset_counter isPope{f.Order} 1");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                    // Papal faction can only be allied with home faction of current pope
                    c.Append($"\nmonitor_event FactionTurnEnd FactionType {f.ID}");
                    c.Append($"\n\tand I_CompareCounter isPope{f.Order} = 0");
                    c.Append($"\n\tand I_CompareCounter {Script.GetIsAllyCounter(f, papalFaction)} = 1");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\t{Script.SetDiplomaticStance(f.ID, papalFaction.ID, "neutral")}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }

                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
