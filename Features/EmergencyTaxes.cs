using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class EmergencyTaxes
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "EmergencyTaxes";
            if (Properties.Settings.Default.cbEmergencyTaxes || isAlwaysActive)
            {
                c.Clear();
                HEGenerator.Add("emta", "Emergency Taxes",
                    $"We are in debt and lack the funds to pay them. Do you want to introduce emergency taxes for your people, risking their reaction?||" +
                    $"The revenues estimated to be gained by introducing the emergency taxes are between {1 * 250} and {10 * 250} per settlement you control.", "@47");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\n\tand Treasury < {Tuner.EmergencyTaxesActivateTreasuryThreshold}");
                c.Append($"\n\tand RandomPercent < {Tuner.EmergencyTaxesActivateChance}");
                c.Append($"\n\tand I_CompareCounter EmergencyTaxesCooloff = 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append(Script.YesNoQuestion("emta"));
                c.Append($"\n\t\t\tif I_EventCounter emta_accepted = 1");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\n\t\t\tif I_CompareCounter isPlayer{r.CID} = 1");
                    c.Append($"\n\t\t\t\tinc_counter playerregioncount 1");
                    c.Append(Script.IfChance(Tuner.EmergencyTaxesPenaltyChancePerCity, $"\nset_counter {r.CID}PopLose 1"));
                    c.Append(Script.IfChance(Tuner.EmergencyTaxesPenaltyChancePerCity, Script.SpawnAgent("heretic", r, "slave")));
                    c.Append(Script.IfChance(Tuner.EmergencyTaxesPenaltyChancePerCity, $"\nadd_settlement_turmoil {r.CID} 16"));
                    c.Append(Script.IfChance(Tuner.EmergencyTaxesPenaltyChancePerCity, Script.SpawnRebelArmy(r.ResourcePositions.First(), World.Factions.First(a => a.ID == r.HomeFaction), 3, 6, 201, r)));
                    c.Append($"\n\t\t\tend_if");
                }
                foreach (var m in 1.To(50))
                {
                    c.Append($"\n\t\t\tif I_CompareCounter playerregioncount = {m}");
                    c.Append($"\n\t\t\tgenerate_random_counter x 1 10");
                    foreach (var n in 1.To(10))
                    {
                        c.Append($"\n\t\t\tif I_EventCounter x = {n}");
                        c.Append(Script.AddMoneyToPlayer(m * n * 250));
                        c.Append($"\n\t\t\thistoric_event {m}emta{n}");
                        HEGenerator.Add($"{m}emta{n}", $"Emergency tax yields {m * n * 250} florins", $"Our emergency taxes were collected - the total income was counted as {m * n * 250} florins, however it seems that a lot of them was lost due to corruption and theft. In average, we collected {n * 250} per city.", "@44");
                        c.Append($"\n\t\t\tend_if");
                    }
                    c.Append($"\n\t\t\tend_if");
                }
                c.Append($"\n\t\t\tend_if");
                c.Append(Script.If($"I_EventCounter emta_declined = 1", $""));
                c.Append($"\n\tset_counter playerregioncount 0");
                c.Append($"\n\tset_counter EmergencyTaxesCooloff {Tuner.EmergencyTaxesCoolOff}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
