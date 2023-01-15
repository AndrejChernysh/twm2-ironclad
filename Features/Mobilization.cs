using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Mobilization
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Mobilization";
            if (Properties.Settings.Default.cbMobilization || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event ButtonPressed ButtonPressed recruitment_button");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions)
                {
                    HEGenerator.Add($"{r.CID}MOB", $"{r.CityName} mobilizing", $"Your call to arms is being sent to people all across {r.RegionName}. Hopefully, enough brave men can be rallied in {r.CityName} to reinforce our troops.", $"-{Tuner.MobilizationFixedCost}", "@54");
                    HEGenerator.Add($"{r.CID}NOMOB", $"{r.CityName} already mobilized", $"There is already a mobilization ongoing in {r.CityName}. Wait until your next turn to initiate a new mobilization if necessary.", "@54");
                    c.Append($"\n\tif I_SettlementSelected {r.CID}");
                    c.Append($"\n\t\tand I_CompareCounter {r.CID}MobilizingCooloff != 0");
                    c.Append($"\n\t\t\thistoric_event {r.CID}NOMOB");
                    c.Append($"\n\tend_if");
                    c.Append($"\n\tif I_SettlementSelected {r.CID}");
                    c.Append($"\n\t\tand I_CompareCounter {r.CID}MobilizingCooloff = 0");
                    c.Append($"\n\t\t\tlog always Mobilizing {r.CityName}");
                    c.Append(Script.AlterRecruitPoolUnits(r.RID, 1, true));
                    c.Append($"\n\t\t\tset_counter {r.CID}MobilizingCooloff 1");
                    c.Append(Script.IfChance(Tuner.MobilizationChanceAdditionalEmigration, $"set_counter {r.CID}PopLose 1"));
                    c.Append(Script.IfChance(Tuner.MobilizationChanceAdditionalTurmoil, $"add_settlement_turmoil {r.CID} {Tuner.MobilizationAdditionalTurmoil}"));
                    c.Append(Script.AddMoneyToPlayer(Tuner.MobilizationFixedCost * -1));
                    c.Append($"\n\t\t\tconsole_command add_population {r.CID} -{Tuner.MobilizationFixedPopulationDecrease}");
                    c.Append($"\n\t\t\tadd_settlement_turmoil {r.CID} {Tuner.MobilizationFixedTurmoil}");
                    c.Append($"\n\t\t\thistoric_event {r.CID}MOB");
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
