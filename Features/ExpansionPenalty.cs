using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ExpansionPenalty
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "ExpansionPenalty";
            if (Properties.Settings.Default.cbExpansionPenalty || isAlwaysActive)
            {
                c.Clear();
                foreach (var f in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_event FactionTurnStart FactionType {f.ID}");
                    c.Append($"\n\tand I_NumberOfSettlements {f.ID} > {f.GetCountStartRegions()}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.TerminateIfAI(f.ID));
                    foreach (var (n, x) in f.Neighbours.Select((v, n) => (v, n)).ToList())
                    {
                        c.Append($"\n\t\tgenerate_random_counter x{x} 1 10");
                        foreach (var i in 1.To(10))
                            c.Append(Script.If($"I_EventCounter x{x} = {i}\nand I_CompareCounter isAlly{World.Factions.First(a => a.ID == n).Order} = 0", Script.SetFactionStanding(f.ID, n, i / 10.0 * -1.0)));
                    }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\tterminate_monitor");
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
