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
    static class Loans
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Loans";
            if (Properties.Settings.Default.cbLoans || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var l in World.Loans)
                {
                    c.Append($"\n\tif I_CompareCounter {l.ID} > 1");
                    c.Append($"\n\t\tinc_counter {l.ID} -1");
                    c.Append(Script.AddMoneyToPlayer(l.Payment * -1));
                    foreach (var r in (l.Rounds - 1).To(0))
                    {

                        c.Append(Script.If($"I_CompareCounter {l.ID} = {r}", $"historic_event {l.ID}_{r}"));
                        HEGenerator.Add($"{l.ID}_{r}", $"Loan Payment",
                            $"Our debt has been served in time to the Bank which previously gave us a loan. {r} turns remain to be paid in the future.", $"-{l.Payment}", "@52");
                    }
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < 0");
                c.Append($"\n\tand RandomPercent < {Tuner.LoanChance}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tgenerate_random_counter x 1 {World.Loans.Count}");
                foreach (var (l, cnt) in World.Loans.Select((v, i) => (v, i)).ToList())
                {
                    var bank = Rndm.Pick(World.Regions.Where(a => !(a.IsNewWorld || a.IsUnreachable)).Select(a => a.CityName).ToList());
                    HEGenerator.Add(l.ID, $"Loan Offer Bank of {bank}",
                        $"As our financial situation has become bad, the Bank of {bank} has offered us a loan with the following conditions.||" +
                        $"Loan amount: {l.Amount}|Payment per round: {l.Payment}|Interest rate: {l.InterestRate} %|Rounds to pay: {l.Rounds}||Will you sign the contract?", "@26");
                    c.Append($"\n\t\tif I_EventCounter x = {cnt + 1}");
                    c.Append($"\n\t\t\tand I_CompareCounter {l.ID} < 1");
                    c.Append(Script.YesNoQuestion(l.ID));
                    c.Append($"\n\t\t\t\tif I_EventCounter {l.ID}_accepted = 1");
                    c.Append(Script.AddMoneyToPlayer(l.Amount));
                    c.Append($"\n\t\t\t\t\tinc_counter {l.ID} {l.Rounds}");
                    c.Append($"\n\t\t\t\tend_if");
                    c.Append(Script.If($"I_EventCounter {l.ID}_declined = 1", $""));
                    c.Append($"\n\t\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
