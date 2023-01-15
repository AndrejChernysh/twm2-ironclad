using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class FirstContact
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "FirstContact";
            if (Properties.Settings.Default.cbFirstContact || isAlwaysActive)
            {
                c.Clear();
                foreach (var f in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_event ObjSeen TargetFactionType {f.ID}");
                    c.Append($"\n\tand FactionIsLocal");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\tgenerate_random_counter x 1 10");
                    foreach (var f2 in World.PlayableFactions.Where(a => a.ID != f.ID).ToList())
                    {
                        c.Append($"\n\t\tif I_LocalFaction {f2.ID}");
                        foreach (var i in 1.To(10))
                        {
                            c.Append($"\n\t\t\tif I_EventCounter x = {i}");
                            c.Append($"\n\t\t\t\tincrement_kings_purse {f.ID} {i * 5}");
                            c.Append($"\n\t\t\t\tincrement_kings_purse {f2.ID} {i * 5}");
                            c.Append($"\n\t\t\t\thistoric_event fc{f.Order}_{i}");
                            c.Append($"\n\t\t\t\tset_counter isContacted{f.Order} 1");
                            HEGenerator.Add($"fc{f.Order}_{i}", $"Contacted {f.NameShort}", $"We established contact with The {f.Name}. As a matter of mutual respect, we exchanged small amounts of goods with these people and agreed on establishing business relations which can become more profitable for both sides if a trade agreement will be signed later on.||To negotiate a trade agreement, send a diplomat or princess.||Effect on your fixed tax income: +{i * 5} florins", $"@{f.ID}");
                            c.Append($"\n\t\t\tend_if");
                        }
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append($"\n\t\t\tterminate_monitor");
                        c.Append($"\n\t\tend_if");
                    }
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
