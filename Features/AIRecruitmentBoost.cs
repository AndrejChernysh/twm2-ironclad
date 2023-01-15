using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class AIRecruitmentBoost
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "AIRecruitmentBoost";
            if (Properties.Settings.Default.cbAIRecruitmentBoost || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    var f = World.Factions.First(a => a.ID == r.HomeFaction);
                    var units = new List<string>() {f.GetPeasant(), f.GetWeakestRangedUnit(), f.GetWeakestInfantryUnit()};
                    if (f.GetWeakestSiegeUnit() != null)
                        units.Add(f.GetWeakestSiegeUnit());
                    if (f.GetWeakestCavalryUnit() != null && !r.IsNewWorld)
                        units.Add(f.GetWeakestCavalryUnit());
                    foreach (var u in units)
                    {
                        c.Append($"\n\tif I_SettlementOwner {r.CID} = {f.ID}");
                        c.Append($"\n\tand ! I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\tand I_IsFactionAIControlled {f.ID}");
                        c.Append($"\n\tand RandomPercent < {Tuner.AIRecruitmentBoostPerUnitPerHomeCityPerRound}");
                        c.Append(Script.xl() ? $"\nlog always AI recruitment boost {r.CID} {f.ID} {u}" : "");
                        c.Append(Script.SpawnUnit(r.CID, u));
                        c.Append($"\n\tend_if");
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
