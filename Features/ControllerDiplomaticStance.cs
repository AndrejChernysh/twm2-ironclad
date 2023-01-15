using Ironclad.Entities;
using Ironclad.Helper;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Ironclad.Features
{
    static class ControllerDiplomaticStance
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = true;
            var scriptGroup = "ControllerDiplomaticStance";
            var order = 2;
            if (isAlwaysActive)
            {
                c.Clear();
                var isWarCounters = new List<string>() { };
                foreach (var f1 in World.PlayableFactions)
                    foreach (var f2 in World.PlayableFactions.Where(a => a.ID != f1.ID))
                    {
                        if (!isWarCounters.Contains(Script.GetIsWarCounter(f1, f2)))
                        {
                            c.Append($"\nmonitor_event FactionTurnEnd FactionType {f1.ID}");
                            c.Append($"\n\tand DiplomaticStanceFromFaction {f2.ID} = AtWar");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\n\t\tset_counter {Script.GetIsWarCounter(f1, f2)} 1");
                            c.Append($"\n\t\tset_counter {Script.GetIsAllyCounter(f1, f2)} 0");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f1.ID}");
                            c.Append($"\n\t\t\tset_counter isEnemy{f2.Order} 1");
                            c.Append($"\n\t\t\tset_counter isAlly{f2.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always isEnemy 1 {f2.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f2.ID}");
                            c.Append($"\n\t\t\tset_counter isEnemy{f1.Order} 1");
                            c.Append($"\n\t\t\tset_counter isAlly{f1.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always isEnemy 1 {f1.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\nend_monitor");

                            c.Append($"\nmonitor_event FactionTurnEnd FactionType {f1.ID}");
                            c.Append($"\n\tand ! DiplomaticStanceFromFaction {f2.ID} = AtWar");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\n\t\tset_counter {Script.GetIsWarCounter(f1, f2)} 0");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f1.ID}");
                            c.Append($"\n\t\t\tset_counter isEnemy{f2.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always isEnemy 0 {f2.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f2.ID}");
                            c.Append($"\n\t\t\tset_counter isEnemy{f1.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always isEnemy 0 {f1.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\nend_monitor");

                            isWarCounters.Add(Script.GetIsWarCounter(f1, f2));
                        }
                    }
                // 870 Monitors!
                var isAllyCounters = new List<string>() { };
                foreach (var f1 in World.PlayableFactions)
                    foreach (var f2 in World.PlayableFactions.Where(a => a.ID != f1.ID))
                    {
                        if (!isAllyCounters.Contains(Script.GetIsAllyCounter(f1, f2)))
                        {
                            c.Append($"\nmonitor_event FactionTurnEnd FactionType {f1.ID}");
                            c.Append($"\n\tand DiplomaticStanceFromFaction {f2.ID} = Allied");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\n\t\tset_counter {Script.GetIsAllyCounter(f1, f2)} 1");
                            c.Append($"\n\t\tset_counter {Script.GetIsWarCounter(f1, f2)} 0");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f1.ID}");
                            c.Append($"\n\t\t\tset_counter isEnemy{f2.Order} 0");
                            c.Append($"\n\t\t\tset_counter isAlly{f2.Order} 1");
                            c.Append(Script.xl() ? $"\nlog always isAlly 1 {f2.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f2.ID}");
                            c.Append($"\n\t\t\tset_counter isEnemy{f1.Order} 0");
                            c.Append($"\n\t\t\tset_counter isAlly{f1.Order} 1");
                            c.Append(Script.xl() ? $"\nlog always isAlly 1 {f1.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\nend_monitor");

                            c.Append($"\nmonitor_event FactionTurnEnd FactionType {f1.ID}");
                            c.Append($"\n\tand ! DiplomaticStanceFromFaction {f2.ID} = Allied");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\n\t\tset_counter {Script.GetIsAllyCounter(f1, f2)} 0");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f1.ID}");
                            c.Append($"\n\t\t\tset_counter isAlly{f2.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always isAlly 0 {f2.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append($"\n\t\tif ! I_IsFactionAIControlled {f2.ID}");
                            c.Append($"\n\t\t\tset_counter isAlly{f1.Order} 0");
                            c.Append(Script.xl() ? $"\nlog always isAlly 0 {f1.ID}" : "");
                            c.Append($"\n\t\tend_if");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\nend_monitor");

                            isAllyCounters.Add(Script.GetIsAllyCounter(f1, f2));
                        }
                    }
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                foreach (var f in World.Factions)
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        c.Append($"\nif I_CompareCounter isEnemy{f.Order} = 1");
                        c.Append($"\nand I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\tset_counter isEnemy{r.CID} 1");
                        c.Append($"\nend_if");
                        c.Append($"\nif I_CompareCounter isEnemy{f.Order} = 0");
                        c.Append($"\nand I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\tset_counter isEnemy{r.CID} 0");
                        c.Append($"\nend_if");
                    }
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                foreach (var f in World.Factions)
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        c.Append($"\nif I_CompareCounter isEnemy{f.Order} = 1");
                        c.Append($"\nand I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\tset_counter isEnemy{r.CID} 1");
                        c.Append($"\nend_if");
                        c.Append($"\nif I_CompareCounter isEnemy{f.Order} = 0");
                        c.Append($"\nand I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\tset_counter isEnemy{r.CID} 0");
                        c.Append($"\nend_if");
                    }
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
