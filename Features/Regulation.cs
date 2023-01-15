using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Regulation
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Regulation";
            if (Properties.Settings.Default.cbRegulation || isAlwaysActive)
            {
                c.Clear();
                foreach (var fAI in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_event FactionTurnStart FactionType {fAI.ID}");
                    c.Append($"\n\tand Treasury < {Tuner.AITreasuryMinLimit}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.TerminateIfPlayer(fAI.ID));
                    c.Append(Script.xl() ? $"\nlog always AITreasuryRegulation {fAI.ID} MinLimit to {Tuner.AITreasuryMinLimit}" : "");
                    c.Append(Script.SetTreasury(fAI.ID, Tuner.AITreasuryMinLimit));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                    foreach (var lim in Tuner.AITreasuryMaxLimits)
                    {
                        c.Append($"\nmonitor_event FactionTurnStart FactionType {fAI.ID}");
                        c.Append($"\n\tand I_NumberOfSettlements {fAI.ID} >= {lim.Key.Split("-")[0]}");
                        c.Append($"\n\tand I_NumberOfSettlements {fAI.ID} <= {lim.Key.Split("-")[1]}");
                        c.Append($"\n\tand Treasury > {lim.Value}");
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append(Script.TerminateIfPlayer(fAI.ID));
                        var min = Tuner.AITreasuryMaxLimitPenaltyMinPercentage / 10;
                        var max = Tuner.AITreasuryMaxLimitPenaltyMaxPercentage / 10;
                        c.Append($"\n\t\tgenerate_random_counter x {min} {max}");
                        foreach (var i in min.To(max))
                        {
                            c.Append($"\n\t\tif I_EventCounter x = {i}");
                            var deduction = Convert.ToInt32(Convert.ToDouble(lim.Value) * (Convert.ToDouble(i) * 0.1));
                            c.Append($"\n\t\t\tadd_money {fAI.ID} -{deduction}");
                            c.Append(Script.xl() ? $"\nlog always AITreasuryRegulation {fAI.ID} MaxLimit {lim.Value} to {lim.Value - deduction}" : "");
                            c.Append($"\n\t\tend_if");
                        }
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append($"\nend_monitor");
                    }
                }
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var fAI in World.PlayableFactions)
                {
                    foreach (var l in Tuner.AITreasuryFixedIncomePerRoundMultiplier)
                    {
                        c.Append($"\n\tif I_IsFactionAIControlled {fAI.ID}");
                        c.Append($"\n\t\tand I_NumberOfSettlements {fAI.ID} >= {l.Key.Split("-")[0]}");
                        c.Append($"\n\t\tand I_NumberOfSettlements {fAI.ID} <= {l.Key.Split("-")[1]}");
                        c.Append($"\n\t\tand I_TurnNumber > {Tuner.AITreasuryFixedIncomePerRoundActivateFromTurn}");
                        c.Append($"\n\t\t\tset_kings_purse {fAI.ID} {Convert.ToInt32(Rndm.Int(fAI.FixedIncomeMin, fAI.FixedIncomeMax) * l.Value)}");
                        c.Append($"\n\tend_if");
                    }
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
