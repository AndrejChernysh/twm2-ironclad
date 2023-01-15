using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Razing
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Razing";
            if (Properties.Settings.Default.cbRazing || isAlwaysActive)
            {
                c.Clear();
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    HEGenerator.Add($"{r.CID}_DOWN", $"{r.CityName} destroyed!", $"{r.CityName} in {r.RegionName} has been completely destroyed and razed down to the ground. All of it's citizen were either killed or sold into slavery.", "@22");
                    c.Append($"\nmonitor_event ExterminatePopulation IsRegionOneOf {r.RID}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.xl() ? $"\nlog always Razed: {r.CID}" : "");
                    c.Append($"\n\tconsole_command add_population {r.CID} -50000");
                    c.Append(Script.DamageAllBuildings(r.CID));
                    c.Append(Script.AlterRecruitPoolUnits(r.CID, -5));
                    if (!r.IsNewWorld)
                        c.Append($"\n\thistoric_event {r.CID}_DOWN");
                    c.Append(Script.SpawnAgent("heretic", r, "slave"));
                    c.Append(Script.FireInRegionOptical(r.CID, 5));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    foreach (var f in World.PlayableFactionsOldWorld)
                    {
                        c.Append($"\nif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\nand ! I_IsFactionAIControlled {f.ID}");
                        foreach (var f2 in World.PlayableFactionsOldWorld.Where(a => a.ID != f.ID))
                            c.Append(Script.IfChance(Tuner.RazingDiplomacyPenaltyChance, Script.SetFactionStanding(f.ID, f2.ID, -1)));
                        c.Append($"\nend_if");
                    }
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
