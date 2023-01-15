using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ControllerPlayerOwnership
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = true;
            var scriptGroup = "ControllerPlayerOwnership";
            if (isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions)
                    c.Append($"\n\t\tset_counter isPlayer{r.CID} 0");
                foreach (var f in World.PlayableFactions)
                    foreach (var r in World.Regions)
                    {
                        c.Append($"\n\t\tif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\t\t\tand ! I_IsFactionAIControlled {f.ID}");
                        c.Append($"\n\t\t\t\tset_counter isPlayer{r.CID} 1");
                        c.Append(Script.xl() ? $"\nlog always Player: {r.CID}" : "");
                        c.Append($"\n\t\tend_if");
                    }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions)
                    c.Append($"\n\t\tset_counter isPlayer{r.CID} 0");
                foreach (var f in World.PlayableFactions)
                    foreach (var r in World.Regions)
                    {
                        c.Append($"\n\t\tif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\t\t\tand ! I_IsFactionAIControlled {f.ID}");
                        c.Append($"\n\t\t\t\tset_counter isPlayer{r.CID} 1");
                        c.Append(Script.xl() ? $"\nlog always Player: {r.CID}" : "");
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
