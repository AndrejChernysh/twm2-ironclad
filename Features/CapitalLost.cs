using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class CapitalLost
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "CapitalLost";
            if (Properties.Settings.Default.cbCapitalLost || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var f in World.PlayableFactions)
                {
                    var r = World.Regions.First(a => a.CID == f.Capital);
                    HEGenerator.Add($"{f.Order}LMC", $"{f.NameShort} loses {r.CityName}", $"The {f.Name} lost {r.CityName}, their most important city.", "@45");
                    HEGenerator.Add($"{f.Order}RMC", $"{f.NameShort} regains {r.CityName}", $"The {f.Name} regained {r.CityName}, their most important city.", "@45");
                    c.Append($"\n\tif ! I_SettlementOwner {f.Capital} = {f.ID}");
                    c.Append($"\n\t\tand I_CompareCounter hmc{f.Order} = 1");
                    c.Append($"\n\t\tand I_NumberOfSettlements {f.ID} > 0");
                    c.Append($"\n\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\t\thistoric_event {f.Order}LMC");
                    c.Append($"\n\t\t\tadd_settlement_turmoil {f.Capital} 10");
                    c.Append($"\n\t\t\tset_counter {f.Capital}PopLose 1");
                    c.Append($"\n\t\t\tset_counter hmc{f.Order} 0");
                    c.Append(Script.AlterRecruitPoolUnits(f.Capital, -5));
                    c.Append($"\n\tend_if");
                    c.Append($"\n\tif I_SettlementOwner {f.Capital} = {f.ID}");
                    c.Append($"\n\t\tand I_CompareCounter hmc{f.Order} = 0");
                    c.Append($"\n\t\tand I_NumberOfSettlements {f.ID} > 0");
                    c.Append($"\n\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\t\thistoric_event {f.Order}RMC");
                    c.Append($"\n\t\t\tset_counter {f.Capital}PopGain 1");
                    c.Append($"\n\t\t\tset_counter hmc{f.Order} 1");
                    c.Append(Script.AlterRecruitPoolUnits(f.Capital, 2));
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
