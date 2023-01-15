using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Empire
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Empire";
            if (Properties.Settings.Default.cbEmpire || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var f in World.PlayableFactions)
                {
                    HEGenerator.Add($"{f.Order}ISEMPIRE", $"{f.Adjective} Empire proclaimed", $"As these people have grown more and more power over the past decades, their governors have now proclaimed that their people are living in a golden age of progress. Other factions are now becoming well aware of the threat that is The {f.Adjective} Empire.", $"@{f.ID}");
                    c.Append($"\n\tif I_NumberOfSettlements {f.ID} > {Tuner.EmpireCityLimit}");
                    c.Append($"\n\t\tand I_CompareCounter {f.Order}isEmpire = 0");
                    c.Append($"\n\t\tlog always Empire {f.ID}");
                    c.Append($"\n\t\thistoric_event {f.Order}ISEMPIRE");
                    c.Append($"\n\t\tset_kings_purse {f.ID} 0");
                    foreach (var agent in Hardcoded.AgentsRecruitable)
                        c.Append($"\n\t\tretire_characters {f.ID} {agent}");
                    c.Append($"\n\t\tadd_money {f.ID} -20000");
                    foreach (var f2 in World.PlayableFactionsOldWorld.Where(a => a.ID != f.ID).ToList())
                    {
                        c.Append(Script.IfChance(50, Script.SetFactionStanding(f.ID, f2.ID, -1.0)));
                        c.Append(Script.IfChance(50, Script.SetDiplomaticStance(f.ID, f2.ID, "war")));
                    }
                    c.Append($"\n\t\tset_counter {f.Order}isEmpire 1");
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
