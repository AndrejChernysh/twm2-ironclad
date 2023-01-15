using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class DiplomacyCosts
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "DiplomacyCosts";
            if (Properties.Settings.Default.cbDiplomacyCosts || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionAllianceDeclared FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append(Script.AddMoneyToPlayer(-4000));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                foreach (var b in new List<string>() { "diplomacy_offer_button", "diplomacy_accept_offer_button", "diplomacy_counter_offer_button" })
                {
                    c.Append($"\nmonitor_event ButtonPressed ButtonPressed {b}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.AddMoneyToPlayer(-1000));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
