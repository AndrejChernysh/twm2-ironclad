using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class FirstTimeBuild
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "FirstTimeBuild";
            if (Properties.Settings.Default.cbFirstTimeBuild || isAlwaysActive)
            {
                c.Clear();
                if (World.Buildings.Any(a => a.MsgCompletionTitle != "NULL"))
                {
                    foreach (var f in World.PlayableFactionsOldWorld)
                        foreach (var b in World.Buildings.Where(a => a.MsgCompletionTitle != "NULL"))
                        {
                            var evt = $"FTB{b.MsgCompletionTitle.Rem(" ")}{f.Order}";
                            var counter = $"FTB{b.MsgCompletionTitle}".Rem(" ", "?", "!", "-");
                            c.Append($"\nmonitor_event FactionTurnEnd FactionType {f.ID}");
                            c.Append($"\n\tand FactionBuildingExists = {b.ID}");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append(Script.If($"I_CompareCounter {counter} = 1", "terminate_monitor"));
                            c.Append($"\n\t\thistoric_event {evt} {b.VideoCompletion}");
                            c.Append($"\n\t\tset_counter {counter} 1");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\n\t\tterminate_monitor");
                            if (b.PicCompletion == "NULL")
                                HEGenerator.Add($"{evt}", $"{b.MsgCompletionTitle.Replace("FACTION", f.NameShort)}", $"{b.MsgCompletionBody.Replace("FACTION", f.NameShort)}");
                            else
                                HEGenerator.Add($"{evt}", $"{b.MsgCompletionTitle.Replace("FACTION", f.NameShort)}", $"{b.MsgCompletionBody.Replace("FACTION", f.NameShort)}", $"@{b.PicCompletion}");
                            c.Append("\nend_monitor");
                        }
                    return new Script(scriptGroup, c.ToString(), isAlwaysActive);
                }
                else
                {
                    return new Script(scriptGroup, "\n;No buildings have been configured in config.xlsx for notification", isAlwaysActive);
                }
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
