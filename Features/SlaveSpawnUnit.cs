using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class SlaveSpawnUnit
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "SlaveSpawnUnit";
            if (Properties.Settings.Default.cbSlaveSpawnUnits || isAlwaysActive)
            {
                c.Clear();
                c.Append("\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\n\t\tif I_SettlementOwner {r.CID} = slave");
                    foreach (var unit in World.RebelFactions.First(a => a.ID == r.Rebels).Units)
                    {
                        c.Append($"\n\t\t\tif RandomPercent < {Tuner.SlaveSpawnChancePerRegionAndUnit}");
                        c.Append($"\n\t\t\t\tand I_CompareCounter ss{r.ID}Cnt < {Tuner.SlaveSpawnUnitMaxSpawns}");
                        c.Append(Script.SpawnUnit(r.CID, unit, 1, 0, 0, 0));
                        c.Append($"\n\t\t\t\t\tinc_counter ss{r.ID}Cnt 1");
                        c.Append($"\n\t\t\tend_if");
                    }
                    c.Append($"\n\t\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
