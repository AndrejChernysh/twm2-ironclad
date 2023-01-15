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
    static class ControllerScript
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = true;
            var scriptGroup = "ControllerScript";
            var order = 100;
            if (isAlwaysActive)
            {
                c.Clear();
                c.Append("\nrestrict_strat_radar false\nwait_monitors ;NEVER REMOVE THIS. MUST ALWAYS FINISH THE CAMPAIGN SCRIPT OTHERWISE NOTHING WILL WORK");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
            }
            return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
        }
    }
}

