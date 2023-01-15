using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class AICapitalBoost
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "AICapitalBoost";
            if (Properties.Settings.Default.cbAICapitalBoost || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var f in World.PlayableFactions)
                    c.Append(Script.If($"I_IsFactionAIControlled {f.ID}", $"console_command create_building {f.Capital} monument_{f.ID}"));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tterminate_monitor");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
