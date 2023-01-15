using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Autonomies
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Autonomies";
            if (Properties.Settings.Default.cbAutonomies || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event ButtonPressed ButtonPressed show_construction_advice_button");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    HEGenerator.Add($"tribute_{r.CID}_ends", $"{r.CityName} ends Autonomy",
                        $"The once autonomous and tribute-paying vassals in {r.RegionName} have lost power in {r.CityName} and were replaced by people who no longer feel obliged to pay us.");
                    foreach (var f in World.PlayableFactions)
                        c.Append(Script.If($"! I_IsFactionAIControlled {f.ID}\nand I_NumberOfSettlements {f.ID} = 1", $"inc_counter {r.CID}AutonomyCooloff 1"));
                    c.Append($"\n\tif I_SettlementSelected {r.CID}");
                    c.Append($"\n\t\tand ! I_SettlementUnderSiege {r.CID}");
                    c.Append($"\n\t\tand ! I_CompareCounter {r.CID}AutonomyCooloff = 0");
                    c.Append($"\n\t\tgenerate_random_counter x 1 {Tuner.AutonomiesOffersMoneyPerRound.Count * Tuner.AutonomiesOffersRounds.Count}");
                    var cnt = 0;
                    foreach (var (m, i) in Tuner.AutonomiesOffersMoneyPerRound.Select((v, i) => (v, i)).ToList())
                        foreach (var (t, i2) in Tuner.AutonomiesOffersRounds.Select((v, i) => (v, i)).ToList())
                        {
                            cnt++;
                            c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                            var e = $"aut{r.CID}{m}_{t}";
                            HEGenerator.Add(e, $"Autonomy for {r.RegionName}?",
                                $"You are about to release {r.RegionName} and its capital {r.CityName} into full Autonomy from your faction.||" +
                                $"A group of locals who are willing to take control of the region after your troops are gone agree to pay you {m} florins for {t} turns if you leave all your troops in the city before you accept.||" +
                                $"Do you want to give them control of the region? You cannot get more than one autonomy offer per turn per region.||Before you agree, make sure there is no garrison in {r.CityName}!");
                            c.Append(Script.YesNoQuestion(e));
                            c.Append(Script.If($"I_EventCounter {e}_accepted = 1", $"{Script.AttackCity(World.Factions.First(a => a.ID == "slave"), r.CID, 12, 17)}" +
                                $"\nset_counter tribute_{r.CID}_{m} 0 \ninc_counter tribute_{r.CID}_{m} {t}\n{Script.SpawnAgent("heretic", r, "slave")}"));
                            3.Times(() => c.Append($"{Script.ClickOn("faction_button")}\ncampaign_wait 0.5"));
                            c.Append(Script.ClickOn("faction_button"));
                            c.Append(Script.If($"I_EventCounter {e}_declined = 1", $"set_counter {r.CID}AutonomyCooloff 1"));
                            c.Append($"\n\t\tend_if");
                        }
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var m in Tuner.AutonomiesOffersMoneyPerRound)
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        var cntr = $"tribute_{r.CID}_{m}";
                        c.Append(Script.If($"I_CompareCounter {cntr} > 0\nand I_SettlementOwner {r.CID} = slave",
                            $"inc_counter {cntr} -1\n{Script.AddMoneyToPlayer(m)}\nhistoric_event {cntr}"));
                        HEGenerator.Add($"{cntr}", $"{r.CityName} paid tribute",
                            $"The autonomous region of {r.RegionName} has paid their outstanding tribute of {m} florins just in time.||" +
                            $"As long as {r.CityName} stays under its control, we should receive the rest of their tributes each turn.");
                        c.Append(Script.If($"I_CompareCounter {cntr} > 0\nand ! I_SettlementOwner {r.CID} = slave",
                            $"set_counter {cntr} 0\nhistoric_event tribute_{r.CID}_ends"));
                    }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
