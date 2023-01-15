using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ReEmergence
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "ReEmergence";
            if (Properties.Settings.Default.cbReEmergence || isAlwaysActive)
            {
                c.Clear();
                foreach (var f in World.PlayableFactions.Where(a => a.ID != Hardcoded.PapalFaction))
                    foreach (var r in World.Regions.Where(a => a.Owner == f.ID).ToList())
                    {
                        // Rioting from faction
                        HEGenerator.Add($"{f.ID.ToUpper()}_RISES", $"{f.NameShort} rises", $"A rebellion is raging in the former territories of The {f.Name} around {r.CityName} - the natives are using their weapons against foreign occupiers and were successful in re-establishing their country as a sovereign, even though currently weak, state.", $"@{f.ID}");
                        c.Append($"\nmonitor_event SettlementTurnStart SettlementName {r.CID}");
                        c.Append($"\n\tand SettlementLoyaltyLevel < loyalty_disillusioned");
                        c.Append($"\n\tand I_CompareCounter {f.Order}risingCooloff = 0");
                        c.Append($"\n\tand I_NumberOfSettlements {f.ID} < 1");
                        c.Append($"\n\tand ! I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\tand ! I_LocalFaction {f.ID}");
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append(Script.AttackCity(f, r.CID, 17, 18));
                        foreach(var f2 in World.PlayableFactions.Where(a => a.ID != f.ID))
                            c.Append(Script.IfOwner(r.CID, f2.ID, $"faction_emerge {f.ID} {f2.ID} 2 400.0 0.0 1.2 town true"));
                        c.Append($"\n\t\thistoric_event {f.ID.ToUpper()}_RISES");
                        c.Append($"\n\t\tset_counter {f.Order}risingCooloff {Tuner.FactionReemergenceCooloffRounds}");
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append($"\nend_monitor");
                        // Rioting from slave
                        c.Append($"\nmonitor_event SettlementTurnStart SettlementName {r.CID}");
                        c.Append($"\n\tand I_SettlementOwner {r.CID} = slave");
                        c.Append($"\n\tand I_CompareCounter {f.Order}risingCooloff = 0");
                        c.Append($"\n\tand I_NumberOfSettlements {f.ID} < 1");
                        c.Append($"\n\tand ! I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\tand ! I_LocalFaction {f.ID}");
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append(Script.AttackCity(f, r.CID, 17, 18));
                        c.Append($"\n\t\tfaction_emerge {f.ID} slave 2 400.0 0.0 1.2 town true");
                        c.Append($"\n\t\thistoric_event {f.ID.ToUpper()}_RISES");
                        c.Append($"\n\t\tset_counter {f.Order}risingCooloff {Tuner.FactionReemergenceCooloffRounds}");
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append($"\nend_monitor");
                    }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
