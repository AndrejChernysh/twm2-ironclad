using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class SiegeRestriction
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "SiegeRestriction";
            if (Properties.Settings.Default.cbSiegeRestriction || isAlwaysActive)
            {
                c.Clear();
                HEGenerator.Add("siege_only_for_generals", "Siege Not Possible",
                    "Our army does not have the expertise and leadership to perform a siege. While the captain in charge of that army sure has his set of skills, he is not suitable for attacking a settlement or fort.||" +
                    "Sieges can only be started by an army led by family member generals.", "@54");
                c.Append("\nmonitor_event ScrollOpened ScrollOpened siege_scroll");
                c.Append("\n\tand I_AgentSelected general");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\thistoric_event siege_only_for_generals");
                c.Append("\n\t\tcampaign_wait 0.1");
                c.Append("\n\t\tdisable_cursor");
                c.Append("\n\t\tcampaign_wait 0.1");
                c.Append(Script.ClickOn("siege_end_button"));
                c.Append("\n\t\tenable_cursor");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
