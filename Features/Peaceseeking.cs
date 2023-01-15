using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Peaceseeking
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Peaceseeking";
            if (Properties.Settings.Default.cbPeaceseeking || isAlwaysActive)
            {
                c.Clear();
                Dictionary<int, string> types = new Dictionary<int, string>() { { 2, "neutral" }, { 1, "allied" } };
                foreach (var fAI in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_event FactionTurnStart FactionType {fAI.ID}");
                    c.Append($"\n\tand ! I_FactionBesieging {fAI.ID}");
                    c.Append($"\n\tand Treasury < 1000");
                    c.Append($"\n\tand RandomPercent < {Tuner.PeaceseekingChance}");
                    c.Append($"\n\tand I_CompareCounter {fAI.Order}PSCooloff = 0");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.TerminateIfPlayer(fAI.ID));
                    foreach (var type in types.Keys)
                    {
                        c.Append($"\n\t\tif I_NumberOfSettlements {fAI.ID} = {type}");
                        c.Append($"\n\t\t\tgenerate_random_counter x 1 {Tuner.PeaceseekingOffers.Count}");
                        foreach (var (o, i) in Tuner.PeaceseekingOffers.Select((v, i) => (v, i)).ToList())
                        {
                            var title = $"{fAI.Order}w{types[type]}{i}";
                            var body = title.Contains("allied") ? $"Officials from The {fAI.Name} have arrived in our capital and are asking for an alliance, offering {o} florins for the deal - Do you accept?" :
                                $"Officials from The {fAI.Name} have arrived in our capital and are asking for peace, offering {o} florins for the deal - Do you accept?";
                            HEGenerator.Add(title, $"{fAI.NameShort} wants Peace", body, "@3");
                            c.Append($"\n\t\t\tif I_EventCounter x = {i + 1}");
                            c.Append($"\n\t\t\t\tif I_CompareCounter isEnemy{fAI.Order} = 1");
                            c.Append(Script.xl() ? $"\nlog always {fAI.ID} asks for {types[type]} offering {o} florins" : "");
                            c.Append(Script.YesNoQuestion(title));
                            c.Append($"\n\t\t\t\t\t\tif I_EventCounter {title}_accepted = 1");
                            c.Append(Script.SetFactionStandingToPlayer(fAI.ID, 1.0));
                            c.Append(Script.SetDiplomaticStanceToPlayer(fAI.ID, types[type]));
                            c.Append(Script.AddMoneyToPlayer(o));
                            c.Append($"\n\t\t\t\t\t\t\tset_counter {fAI.Order}PSCooloff {Tuner.PeaceseekingRoundsCooloff}");
                            c.Append($"\n\t\t\t\t\t\t\tset_counter isEnemy{fAI.Order} 0");
                            c.Append($"\n\t\t\t\t\t\tend_if");
                            c.Append($"\n\t\t\t\t\t\tif I_EventCounter {title}_declined = 1");
                            c.Append($"\n\t\t\t\t\t\tend_if");
                            c.Append($"\n\t\t\t\tend_if");
                            c.Append($"\n\t\t\tend_if");
                        }
                        c.Append($"\n\t\tend_if");
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
