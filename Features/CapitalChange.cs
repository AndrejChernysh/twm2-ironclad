using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class CapitalChange
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "CapitalChange";
            if (Properties.Settings.Default.cbCapitalChange || isAlwaysActive)
            {
                c.Clear();
                HEGenerator.Add("PLAYER_NEW_CAPITAL", "A New Capital",
                    $"We changed our capital in order to improve our ability to govern the lands and people under our rule more efficiently.||" +
                    $"Settlements further away from the capital are harder to manage, merchants further away from the capital gain experience faster.", $"-{Tuner.CapitalChangeCost}", "@21");
                c.Append($"\nmonitor_event FactionNewCapital FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append(Script.AddMoneyToPlayer(Tuner.CapitalChangeCost * -1));
                c.Append($"\n\thistoric_event PLAYER_NEW_CAPITAL");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
