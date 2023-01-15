using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class DebtCrises
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "DebtCrises";
            if (Properties.Settings.Default.cbDebtCrises || isAlwaysActive)
            {
                c.Clear();
                HEGenerator.Add("DEBTCRISIS_FIRST", "Agents Dismissed",
                    "Due to a lack of government funding, all our agents except merchants have not been paid and were dismissed. Once our financial situation gets better, new agents still can be recruited.", "@53");
                HEGenerator.Add("DEBTCRISIS_SECOND", "Expensive Assets Sold",
                    $"Due to a lack of government funding, we had to sell several institution and their assets to private businessmen, some of them even from abroad.", "@53");
                HEGenerator.Add("DEBTCRISIS_THIRD", "Heavy Cavalry Dismissed",
                    "Due to a severe lack of government funding, all our heavy cavalry were dissolved as they did no longer receive any payment nor food and water from the officials.", "@53");
                HEGenerator.Add("DEBTCRISIS_FOURTH", "Armies Dissolved",
                    "Due to a severe lack of government funding, all our land armies were dissolved as they did no longer receive any payment nor food and water from the officials.", "@53");
                HEGenerator.Add("DEBTCRISIS_FIFTH", "Financial Crisis",
                    "As government funding are still in deep debt, civil unrest starts to spread in many locations. If these problems cannot be solved in the near future, there might be no tomorrow for the current ruling class.", "@53");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury > 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tset_event_counter debtcrisis_first 0");
                c.Append($"\n\t\tset_event_counter debtcrisis_second 0");
                c.Append($"\n\t\tset_event_counter debtcrisis_third 0");
                c.Append($"\n\t\tset_event_counter debtcrisis_fourth 0");
                c.Append($"\n\t\tset_event_counter debtcrisis_fifth 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < {Tuner.DebtCrisisLimit1}");
                c.Append($"\n\tand I_EventCounter debtcrisis_first == 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tset_event_counter debtcrisis_first 1");
                c.Append($"\n\t\thistoric_event DEBTCRISIS_FIRST");
                foreach (var agent in Hardcoded.AgentsRecruitable.Where(a => a != "merchant").ToList())
                    c.Append(Script.RemoveAgentsFromPlayer(agent));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < {Tuner.DebtCrisisLimit2}");
                c.Append($"\n\tand I_EventCounter debtcrisis_second == 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tset_event_counter debtcrisis_second 1");
                c.Append($"\n\t\thistoric_event DEBTCRISIS_SECOND");
                foreach (var chain in World.Buildings.Where(a => !a.Chain.Contains("hinterland") && a.Capabilities.Contains("income_bonus bonus -")).Select(a => a.Chain).Distinct())
                    c.Append(Script.DestroyChainsFromPlayer(chain));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < {Tuner.DebtCrisisLimit3}");
                c.Append($"\n\tand I_EventCounter debtcrisis_third == 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tset_event_counter debtcrisis_third 1");
                c.Append($"\n\t\thistoric_event DEBTCRISIS_THIRD");
                c.Append(Script.DestroyUnitsFromPlayer("knight"));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < {Tuner.DebtCrisisLimit4}");
                c.Append($"\n\tand I_EventCounter debtcrisis_fourth == 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tset_event_counter debtcrisis_fourth 1");
                c.Append($"\n\t\thistoric_event DEBTCRISIS_FOURTH");
                c.Append(Script.DestroyUnitsFromPlayer("free_upkeep_unit"));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < {Tuner.DebtCrisisLimit5}");
                c.Append($"\n\tand I_EventCounter debtcrisis_fifth == 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tset_event_counter debtcrisis_fifth 1");
                c.Append($"\n\t\thistoric_event DEBTCRISIS_FIFTH");
                if (Properties.Settings.Default.cbCivilWars)
                    foreach (var f in World.PlayableFactions)
                        c.Append(Script.If($"not I_IsFactionAIControlled {f.ID}", $"set_counter cww{f.Order} 1"));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
