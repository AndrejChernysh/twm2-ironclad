using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using OfficeOpenXml.ConditionalFormatting;
using ServiceStack;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ForcedMarch
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "ForcedMarch";
            if (Properties.Settings.Default.cbForcedMarch || isAlwaysActive)
            {
                c.Clear();
                foreach (var f in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_event ButtonPressed ButtonPressed show_bodygaurd_unit_button");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.TerminateIfAI(f.ID));
                    foreach (var i in 1.To(2))
                    {
                        c.Append($"\n\tif I_CompareCounter fmCooloff = {i}");
                        c.Append($"\n\t\t\thistoric_event fomaco{i}");
                        HEGenerator.Add($"fomaco{i}", $"Forced March not possible", $"Wait {i} more turns to be able to activate Forced March again.".Replace("1 more turns", "1 more turn"), "@56");
                        c.Append($"\n\tend_if");
                    }
                    c.Append($"\n\tif I_CompareCounter fmCooloff = 0");
                    var cnt = 0;
                    foreach (var n in World.Names.Where(a => a.Type == "characters" && a.NameSet == f.NameSet).Select(a => a.NID).Distinct())
                    {
                        c.Append($"\n\t\tif I_CharacterSelected {n}");
                        c.Append($"\n\t\t\tconsole_command character_reset {n}");
                        c.Append($"\n\t\tend_if");
                        foreach (var s in World.Names.Where(a => a.Type == "surnames" && a.NameSet == f.NameSet).Select(a => a.NID).Distinct())
                        {
                            c.Append($"\n\t\tif I_CharacterSelected {n} {s}");
                            c.Append($"\n\t\t\tconsole_command character_reset \"{n} {s}\"");
                            c.Append($"\n\t\tend_if");
                            cnt++;
                        }
                    }
                    //IO.Log($"{f.NameSet}: {cnt}");
                    c.Append($"\n\t\tset_counter fmCooloff 2");
                    c.Append($"\n\t\tgenerate_random_counter x 1 9");
                    foreach (var i in 1.To(9))
                    {
                        c.Append($"\n\t\tif I_EventCounter x = {i}");
                        c.Append($"\n\t\t\tadd_money {f.ID} -{i * 500 + 500}");
                        c.Append($"\n\t\t\tconsole_command give_trait this FM{i} 1");
                        c.Append($"\n\t\t\thistoric_event foma{i}");
                        HEGenerator.Add($"foma{i}", $"Forced March activated", $"The selected character ordered his troops to go into forced march, ordering additional resources to achieve this feat and taking his toll due to exhaustion and discontent among his men.", $"-{i * 500 + 500}", "@56");
                        c.Append($"\n\t\tend_if");
                    }
                    c.Append($"\n\tend_if");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
