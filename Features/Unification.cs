using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Unification
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Unification";
            if (Properties.Settings.Default.cbUnification || isAlwaysActive)
            {
                c.Clear();
                var unifications = World.Regions.Where(a => a.Unification != null).Select(a => a.Unification).Distinct();
                c.Append($"\nmonitor_event PreFactionTurnStart FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var unification in unifications)
                    foreach (var f in World.PlayableFactionsOldWorld)
                    {
                        var regions = World.Regions.Where(a => a.Unification == unification);
                        if (regions.Select(a => a.RID).Except(World.Regions.Where(a => a.Owner == f.ID).Select(a => a.RID)).Any()) // Only if not all regions owned by faction from start
                        {
                            var unifiedCounter = $"u{unification.Replace(" ", "")}{f.Order}";
                            HEGenerator.Add($"{unifiedCounter}ai", $"{unification} unified", $"{f.NameShort} conquered {unification}, completing a long journey for glory and riches.");
                            HEGenerator.Add($"{unifiedCounter}pl", $"{unification} unified", $"We conquered {unification}, a great achievement which will go down in history.", $"{regions.Count() * 1000}");
                            c.Append($"\nif I_CompareCounter {unifiedCounter} = 0");
                            foreach (var r in regions)
                                c.Append($"\nand I_SettlementOwner {r.CID} = {f.ID}");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name} {f.ID} unified {unification}" : "");
                            c.Append($"\n\tset_counter {unifiedCounter} 1");
                            c.Append($"\n\tif I_IsFactionAIControlled {f.ID}");
                            c.Append($"\n\t\thistoric_event {unifiedCounter}ai");
                            c.Append($"\n\tend_if");
                            c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                            c.Append($"\n\t\thistoric_event {unifiedCounter}pl");
                            c.Append($"\n\tend_if");
                            c.Append($"\n\tadd_money {f.ID} {regions.Count() * 1000}");
                            c.Append($"\nend_if");
                        }
                    }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
