using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ControllerActions
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = true;
            var scriptGroup = "ControllerActions";
            if (isAlwaysActive)
            {
                c.Clear();

                //Auto zoom minimap radar

                c.Append($"\nmonitor_event PreFactionTurnStart FactionIsLocal");
                c.Append(Script.ClickOn("radar_zoom_in_button"));
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append(Script.ClickOn("radar_zoom_in_button"));
                c.Append($"\nend_monitor");

                //RCID - Rebellion in City 0.2 MB
                c.Append($"\nmonitor_conditions I_CompareCounter Ractivate = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tset_counter Ractivate 0");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\n\tif I_CompareCounter R{r.CID} = 1"); // full rebellion
                    c.Append(Script.xl() ? $"\n\t\tlog always {r.CityName} rebelling" : "");
                    c.Append($"\n\t\tset_counter R{r.CID} 0");
                    c.Append($"\n\t\tconsole_command add_population {r.CID} 5000");
                    c.Append(Script.DamageAllBuildings(r.CID));
                    c.Append($"\n\t\tconsole_command create_building {r.CID} rhq");
                    c.Append($"\n\t\tadd_settlement_turmoil {r.CID} 16");
                    c.Append($"\n\t\tprovoke_rebellion {r.CID}");
                    c.Append($"\n\t\tset_counter {r.CID}RiotCooloff 3");
                    c.Append(Script.SpawnAgent("heretic", World.Regions.First(a => a.CID == r.CID), "slave"));
                    c.Append(Script.FireInRegionOptical(r.CID));
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");

                //CS - City State 0.1 MB
                //1 High Export
                //2 Moderate Export
                //3 Import Workforce
                //4 Import Potables
                //5 Import Food
                //6 Import Clothing
                //7 Import Materials
                //8 Import Additives
                //9 No Work, No Import, No Expert

                c.Append($"\nmonitor_event ButtonPressed ButtonPressed games_policy_inc_gadget");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\nset_counter changed{r.CID} 0");
                    c.Append($"\nif I_SettlementSelected {r.CID}");
                    c.Append($"\n\tand I_CompareCounter state{r.CID} < 9");
                    c.Append($"\n\tand I_CompareCounter changed{r.CID} = 0");
                    c.Append($"\n\t\tinc_counter state{r.CID} 1");
                    c.Append($"\n\t\tset_counter changed{r.CID} 1");
                    c.Append($"\nend_if");
                    c.Append($"\nif I_SettlementSelected {r.CID}");
                    c.Append($"\n\tand I_CompareCounter state{r.CID} = 9");
                    c.Append($"\n\tand I_CompareCounter changed{r.CID} = 0");
                    c.Append($"\n\t\tset_counter state{r.CID} 1");
                    c.Append($"\n\t\tset_counter changed{r.CID} 1");
                    c.Append($"\nend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");

                c.Append($"\nmonitor_event ButtonPressed ButtonPressed games_policy_dec_gadget");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\nset_counter changed{r.CID} 0");
                    c.Append($"\nif I_SettlementSelected {r.CID}");
                    c.Append($"\n\tand I_CompareCounter state{r.CID} = 1");
                    c.Append($"\n\tand I_CompareCounter changed{r.CID} = 0");
                    c.Append($"\n\t\tset_counter state{r.CID} 9");
                    c.Append($"\n\t\tset_counter changed{r.CID} 1");
                    c.Append($"\nend_if");
                    c.Append($"\nif I_SettlementSelected {r.CID}");
                    c.Append($"\n\tand I_CompareCounter state{r.CID} > 1");
                    c.Append($"\n\tand I_CompareCounter changed{r.CID} = 0");
                    c.Append($"\n\t\tinc_counter state{r.CID} -1");
                    c.Append($"\n\t\tset_counter changed{r.CID} 1");
                    c.Append($"\nend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");

                //OED - opticalEffectOnDevastation 0.1 MB
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\nmonitor_event SabotageMission IsRegionOneOf { r.RID}");
                    c.Append($"\n\tand MissionSucceeded");
                    c.Append($"\n\tand RandomPercent < {Tuner.SabotageFireChance}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.FireInCityOpticalAndEffect(r.RID));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                //DAB - damageAllBuildings 11.9 MB
                c.Append($"\nmonitor_conditions I_CompareCounter DABactivate = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    // DAB CID = 1 = 100 % Damage 50 % Buildings
                    c.Append($"\n\tif I_CompareCounter DAB{r.CID} = 1");
                    foreach (var chain in World.BuildingChains.Where(a => !a.IsHidden))
                        c.Append(Script.IfChance(50, Script.DamageBuilding(r.CID, chain.ID, 100)));
                    c.Append($"\n\tset_counter DAB{r.CID} 0");
                    c.Append($"\n\tend_if");

                    // DAB CID = 2 = 100 % Damage 100 % Buildings
                    c.Append($"\n\tif I_CompareCounter DAB{r.CID} = 2");
                    foreach (var chain in World.BuildingChains.Where(a => !a.IsHidden))
                        c.Append(Script.DamageBuilding(r.CID, chain.ID, 100));
                    c.Append($"\n\tset_counter DAB{r.CID} 0");
                    c.Append($"\n\tend_if");
                }
                c.Append($"\n\tset_counter DABactivate 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                //ARPU - alterRecruitPoolUnits 14.9 MB
                c.Append($"\nmonitor_conditions I_CompareCounter ARPUactivate = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    // 100 % Chance per unit
                    c.Append($"\n\twhile I_CompareCounter ARPU{r.RID} > 0");
                    foreach (var u in World.Units.Where(a => !a.IsNaval && a.IsRecruitable))
                        c.Append($"\n\t\tinc_recruit_pool {r.RID} 1 {u.IntName}");
                    c.Append($"\n\t\tinc_counter ARPU{r.RID} -1");
                    c.Append($"\n\tend_while");
                    c.Append($"\n\twhile I_CompareCounter ARPU{r.RID} < 0");
                    foreach (var u in World.Units.Where(a => !a.IsNaval && a.IsRecruitable))
                        c.Append($"\n\t\tinc_recruit_pool {r.RID} -1 {u.IntName}");
                    c.Append($"\n\t\tinc_counter ARPU{r.RID} 1");
                    c.Append($"\n\tend_while");
                    // 50 % Chance per unit
                    c.Append($"\n\twhile I_CompareCounter ARPU{r.RID}50 > 0");
                    foreach (var u in World.Units.Where(a => !a.IsNaval && a.IsRecruitable))
                        c.Append(Script.IfChance(50, $"\n\t\t\tinc_recruit_pool {r.RID} 1 {u.IntName}"));
                    c.Append($"\n\t\tinc_counter ARPU{r.RID}50 -1");
                    c.Append($"\n\tend_while");
                    c.Append($"\n\twhile I_CompareCounter ARPU{r.RID}50 < 0");
                    foreach (var u in World.Units.Where(a => !a.IsNaval && a.IsRecruitable))
                        c.Append(Script.IfChance(50, $"\n\t\t\tinc_recruit_pool {r.RID} -1 {u.IntName}"));
                    c.Append($"\n\t\tinc_counter ARPU{r.RID}50 1");
                    c.Append($"\n\tend_while");
                }
                c.Append($"\n\tset_counter ARPUactivate 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                // AMP - addMoneyPlayer 0.1 MB
                foreach (var f in World.PlayableFactions)
                {
                    c.Append($"\nmonitor_conditions I_CompareCounter AMP != 0");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.TerminateIfAI(f.ID));
                    foreach (var i in new List<int>() { 5000, 1000, 100, 10, 5 })
                    {
                        c.Append($"\n\twhile I_CompareCounter AMP > {i}");
                        c.Append($"\n\t\tadd_money {f.ID} {i}");
                        c.Append($"\n\t\tinc_counter AMP -{i}");
                        c.Append($"\n\tend_while");
                    }
                    c.Append($"\n\twhile I_CompareCounter AMP > 0");
                    c.Append($"\n\t\tadd_money {f.ID} 1");
                    c.Append($"\n\t\tinc_counter AMP -1");
                    c.Append($"\n\tend_while");
                    foreach (var i in new List<int>() { 5000, 1000, 100, 10, 5 })
                    {
                        c.Append($"\n\twhile I_CompareCounter AMP < -{i}");
                        c.Append($"\n\t\tadd_money {f.ID} -{i}");
                        c.Append($"\n\t\tinc_counter AMP {i}");
                        c.Append($"\n\tend_while");
                    }
                    c.Append($"\n\twhile I_CompareCounter AMP < 0");
                    c.Append($"\n\t\tadd_money {f.ID} -1");
                    c.Append($"\n\t\tinc_counter AMP 1");
                    c.Append($"\n\tend_while");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
