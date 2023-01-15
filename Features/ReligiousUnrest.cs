using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ReligiousUnrest
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "ReligiousUnrest";
            if (Properties.Settings.Default.cbReligiousUnrest || isAlwaysActive)
            {
                c.Clear();
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\nmonitor_event CharacterTurnEnd IsRegionOneOf {r.RID}");
                    c.Append($"\n\tand ! InEnemyLands");
                    c.Append($"\n\tand ! AtSea");
                    c.Append($"\n\tand PopulationOwnReligion < {Tuner.ReligiousUnrestThreshold}");
                    c.Append($"\n\tand ! I_SettlementOwner {r.CID} = slave");
                    c.Append($"\n\tand I_CompareCounter ru{r.CID}CoolOff = 0");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\tset_counter ru{r.CID}CoolOff {Tuner.ReligiousUnrestCooloffTurns}");
                    c.Append($"\n\t\tgenerate_random_counter x 1 10");
                    c.Append(Script.If("I_EventCounter x = 1", Script.CityRevolt(r.CID)));
                    c.Append(Script.If("I_EventCounter x = 2", Script.DamageAllBuildings(r.CID, false)));
                    c.Append(Script.If("I_EventCounter x = 3", $"add_settlement_turmoil {r.CID} 16"));
                    c.Append(Script.If("I_EventCounter x = 4", Script.FireInCityOpticalAndEffect(r.CID)));
                    c.Append(Script.If("I_EventCounter x = 5", Script.AlterRecruitPoolUnits(r.RID, -1, true)));
                    c.Append(Script.If("I_EventCounter x = 6", $"set_counter {r.CID}PopLose 1"));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
