using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Invasions
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Invasions";
            if (Properties.Settings.Default.cbInvasions || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var fAI in World.PlayableFactionsOldWorld)
                {
                    c.Append($"\n\tif I_CompareCounter invasion{fAI.Order} = 1");
                    c.Append($"\n\t\tand ! I_NumberOfSettlements {fAI.ID} = 0");
                    c.Append($"\n\t\t\tset_counter invasion{fAI.Order} 0");
                    c.Append($"\n\t\t\tgenerate_random_counter x_{fAI.ID} 1 {fAI.InvasionTargets.Count}");
                    foreach (var (t, n) in fAI.InvasionTargets.Select((v, i) => (v, i)).ToList())
                    {
                        c.Append($"\n\t\t\tif I_EventCounter x_{fAI.ID} = {n + 1}");
                        c.Append($"\n\t\t\t\tand ! I_SettlementUnderSiege {t}");
                        c.Append($"\n\t\t\t\tand ! I_SettlementOwner {t} = {fAI.ID}");
                        foreach (var of in World.PlayableFactions.Where(a => a.ID != fAI.ID).ToList())
                        {
                            c.Append($"\n\t\t\t\tif I_SettlementOwner {t} = {of.ID}");
                            c.Append(Script.SetFactionStanding(fAI.ID, of.ID, -1.0));
                            c.Append(Script.SetDiplomaticStance(fAI.ID, of.ID, "war"));
                            c.Append($"\n\t\t\t\tend_if");
                        }
                        c.Append(Script.AttackCity(fAI, t, 5, 10));
                        var r = World.Regions.First(a => a.CID == t);
                        HEGenerator.Add($"{fAI.Order}invades{t}", $"{fAI.NameShort} invades {r.CityName}",
                            $"We got reports that The {fAI.Name} has secretly invaded {r.RegionName} and is now probably heading towards {r.CityName}.", $"@15");
                        c.Append($"\n\t\t\t\thistoric_event {fAI.Order}invades{t}");
                        c.Append($"\n\t\t\tend_if");
                    }
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append($"\n\tand I_TurnNumber > {Tuner.InvasionAIMinRound}");
                c.Append($"\n\tand RandomPercent < {Tuner.InvasionAIChance}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tgenerate_random_counter x 0 {World.PlayableFactionsOldWorld.Count - 1}");
                foreach (var (f, i) in World.PlayableFactionsOldWorld.Select((v, i) => (v, i)).ToList())
                {
                    c.Append($"\n\t\tif I_EventCounter x = {i}");
                    c.Append($"\n\t\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\t\tand ! I_NumberOfSettlements {f.ID} = 0");
                    c.Append($"\n\t\t\t\tset_counter invasion{f.Order} 1");
                    c.Append($"\n\t\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
