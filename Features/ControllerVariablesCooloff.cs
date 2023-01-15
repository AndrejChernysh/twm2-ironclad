using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ControllerVariablesCooloff
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = true;
            var scriptGroup = "ControllerVariablesCooloff";
            var order = 2;
            if (isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var counter in ScriptGenerator.Counters.Where(a => a.Key.EndsWithIgnoreCase("cooloff")))
                    c.Append(Script.DecreaseCounterIfGreaterZero(counter.Key));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);

            }
            return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
        }
    }
}
