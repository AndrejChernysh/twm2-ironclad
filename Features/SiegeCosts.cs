using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class SiegeCosts
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "SiegeCosts";
            if (Properties.Settings.Default.cbSiegeCosts || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event CharacterTurnEnd CharacterIsLocal");
                c.Append($"\n\tand IsBesieging");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append(Script.AddMoneyToPlayer(-1000));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
