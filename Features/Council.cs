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
    static class Council
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Council";
            if (Properties.Settings.Default.cbCouncil || isAlwaysActive)
            {
                c.Clear();
                // Count administrators with parties
                foreach (var p in Settings.Parties)
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        c.Append($"\nmonitor_event CharacterTurnStart IsRegionOneOf {r.RID}");
                        c.Append($"\n\tand EndedInSettlement");
                        c.Append($"\n\tand GovernorTaxLevel >= tax_low"); // Is Character a Governor?
                        c.Append($"\n\tand Trait pp{p.Key} >= 1");
                        c.Append($"\n\tand CharacterIsLocal");
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append($"\n\t\tinc_counter {r.CID}{p.Key} 1");
                        c.Append($"\n\t\tinc_counter p{p.Key} 1");
                        c.Append($"\n\t\tchange_settlement_name {r.CID} {r.CID}{p.Key}");
                        if (r.CityNameColonized != null)
                            c.Append(Script.If($"! I_SettlementOwner {r.CID} = slave\nand ! I_SettlementOwner {r.CID} = apachean\nand ! I_SettlementOwner {r.CID} = aztecs", $"\n\t\tchange_settlement_name {r.CID} {r.CID}{p.Key}C"));
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        c.Append($"\nend_monitor");
                    }
                // Reset all settlement names and party counts
                c.Append($"\nmonitor_event PreFactionTurnStart FactionIsLocal");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tset_counter rp 0");
                c.Append(Script.xl() ? $"\nlog always reset rp to 0" : "");
                foreach (var p in Settings.Parties)
                {
                    c.Append($"\n\tset_counter p{p.Key} 0");
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        c.Append($"\n\tset_counter {r.CID}{p.Key} 0\n\tchange_settlement_name {r.CID} {r.CID}");
                        if (r.CityNameColonized != null)
                            c.Append(Script.If($"! I_SettlementOwner {r.CID} = slave\nand ! I_SettlementOwner {r.CID} = apachean\nand ! I_SettlementOwner {r.CID} = aztecs", $"\n\t\tchange_settlement_name {r.CID} {r.CID}C"));
                    }
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                // Show council composition
                c.Append($"\nmonitor_event ButtonPressed ButtonPressed radar_zoom_in_button");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tset_counter updc 2");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                // Trigger Update council
                foreach (var b in new List<string>() { "FactionTurnStart", "FactionTurnEnd" })
                {
                    c.Append($"\nmonitor_event {b} FactionIsLocal");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\tset_counter updc 1");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                // Update council: updc 1 = refresh Council, updc 2 = refresh and show Council
                c.Append($"\nmonitor_conditions I_CompareCounter updc > 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                var info = $"Each governor general gets a place in the Council. If one party becomes too powerful, the others might revolt.";
                foreach (var p1 in 0.To(30))
                    foreach (var p2 in 0.To(30))
                        foreach (var p3 in 0.To(30))
                        {
                            c.Append($"\n\tif I_CompareCounter p1 = {p1}");
                            c.Append($"\n\t\tand I_CompareCounter p2 = {p2}");
                            c.Append($"\n\t\tand I_CompareCounter p3 = {p3}");
                            var seats = new List<int>() { p1, p2, p3 };
                            var noMajority = seats.Count(a => a == seats.Max()) > 1;
                            var ruler = noMajority ? "Balanced||Effect in all regions: None" : $"{Settings.Parties.ElementAt(seats.IndexOf(seats.Max())).Value}||Effect in all regions: {Settings.PartiesEffects[Settings.Parties.ElementAt(seats.IndexOf(seats.Max())).Value]}";
                            if (seats.Sum() == 0)
                                HEGenerator.Add($"p{p1}_{p2}_{p3}", $"Council",
                                    $"{Settings.Parties.ElementAt(0).Value}: {p1} (0%)|" +
                                    $"{Settings.Parties.ElementAt(1).Value}: {p2} (0%)|" +
                                    $"{Settings.Parties.ElementAt(2).Value}: {p3} (0%)||" +
                                    $"In Power: {ruler}||{info}");
                            else
                                HEGenerator.Add($"p{p1}_{p2}_{p3}", $"Council",
                                    $"{Settings.Parties.ElementAt(0).Value}: {p1} ({p1.PercentageOf(seats.Sum())}%)|" +
                                    $"{Settings.Parties.ElementAt(1).Value}: {p2} ({p2.PercentageOf(seats.Sum())}%)|" +
                                    $"{Settings.Parties.ElementAt(2).Value}: {p3} ({p3.PercentageOf(seats.Sum())}%)||" +
                                    $"In Power: {ruler}||{info}");
                            c.Append(Script.IfCounter("updc", 2, $"historic_event p{p1}_{p2}_{p3}"));
                            var rp = noMajority ? 0 : Settings.Parties.ElementAt(seats.IndexOf(seats.Max())).Key;
                            c.Append($"\n\t\t\tset_counter rp {rp}");
                            c.Append(Script.xl() ? $"\nlog always set rp to {rp}" : "");
                            c.Append($"\n\t\t\tif I_CompareCounter lrp != {rp}");
                            c.Append($"\n\t\t\t\tand I_CompareCounter updc = 1");
                            if (rp == 0)
                                HEGenerator.Add("nrp0", "Stalemate in Council", $"No party is currently in power in the council, thus no party faction bonus takes effect.||" +
                                    $"Rebalance the council seats by reappointing settlement governors, but make sure that no party gets too powerful.", "@42");
                            else
                                HEGenerator.Add($"nrp{rp}", $"Council Majority for {Settings.Parties[rp]}", $"The {Settings.Parties[rp]} have got the majority in your factions council.||" +
                                    $"Faction-wide effect: {Settings.PartiesEffects[Settings.Parties[rp]]}", Settings.PartiesPic[Settings.Parties[rp]]);
                            c.Append($"\n\t\t\t\t\thistoric_event nrp{rp}");
                            c.Append($"\n\t\t\tset_counter lrp {rp}");
                            c.Append($"\n\t\t\tend_if");
                            foreach (var (e, i) in seats.Select((v, i) => (v, i)).ToList())
                                c.Append(seats.Sum() > 0 && e.PercentageOf(seats.Sum()) > Tuner.CouncilConflictPercentageSeatsThreshold ?
                                    "\n\t\tif I_CompareCounter updc = 1\t\tset_counter cptp 1\n\t\tend_if" : "");
                            c.Append($"\n\tend_if");
                        }
                foreach (var f in World.Factions)
                    c.Append($"\n\tdestroy_buildings {f.ID} hinterland_rpb false");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    foreach (var p in Settings.Parties)
                        c.Append(Script.If($"I_CompareCounter isPlayer{r.CID} = 1\nand I_CompareCounter rp = {p.Key}", $"console_command create_building {r.CID} rpb_{p.Key}"));
                c.Append($"\n\tset_counter updc 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                // Council party too powerful
                c.Append($"\nmonitor_conditions I_CompareCounter cptp > 0");
                c.Append($"\n\tand I_TurnNumber > {Tuner.CouncilCivilWarsFromTurn}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tset_counter cptp 0");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    foreach (var p in Settings.Parties)
                    {
                        c.Append($"\n\tif I_CompareCounter isPlayer{r.CID} = 1");
                        c.Append($"\n\t\tand I_CompareCounter rp = {p.Key}");
                        c.Append($"\n\t\tand I_CompareCounter {r.CID}{p.Key} = 0");
                        c.Append($"\n\t\tand RandomPercent < {Tuner.CouncilConflictProbabilityPerTurnPerSettlement}");
                        c.Append($"\n\t\t\tgenerate_random_counter x 1 9");
                        c.Append($"\n\t\t\tif I_EventCounter x = 1");
                        c.Append(Script.CityRevolt(r.CID));
                        c.Append($"\n\t\t\t\thistoric_event cwcpe1{r.CID}");
                        HEGenerator.Add($"cwcpe1{r.CID}", $"Governor of {r.CityName} resigns", $"The governor of {r.RegionName} makes it clear that he is no longer willing to hold control of his region. He is known for protesting against the current inbalance in the factions council, as he is not part of the party which is currently in great power.", $"@52");
                        c.Append($"\n\t\t\tend_if");
                        foreach (var x in new Dictionary<int, int>() { { 2, -250 }, { 3, -500 }, { 4, -750 }, { 5, -1000 } })
                        {
                            c.Append($"\n\t\t\tif I_EventCounter x = {x.Key}");
                            c.Append(Script.AddMoneyToPlayer(x.Value));
                            c.Append($"\n\t\t\t\thistoric_event cwcpe{x.Key}{r.CID}");
                            HEGenerator.Add($"cwcpe{x.Key}{r.CID}", $"No taxes from {r.CityName}", $"The governor of {r.RegionName} seems to have withheld some of the income from taxation. He is known for protesting against the current inbalance in the factions council, as he is not part of the party which is currently in great power.", $"{x.Value}", $"@52");
                            c.Append($"\n\t\t\tend_if");
                        }
                        foreach (var x in new Dictionary<int, int>() { { 6, -1 }, { 7, -2 }, { 8, -3 } })
                        {
                            c.Append($"\n\t\t\tif I_EventCounter x = {x.Key}");
                            c.Append(Script.AlterRecruitPoolUnits(r.RID, x.Value, true));
                            c.Append($"\n\t\t\t\thistoric_event cwcpe{x.Key}{r.CID}");
                            HEGenerator.Add($"cwcpe{x.Key}{r.CID}", $"Recruitment halted in {r.RegionName}", $"The governor of {r.RegionName} seems to have halted some of the recruitment activity within his region. He is known for protesting against the current inbalance in the factions council, as he is not part of the party which is currently in great power.", $"@53");
                            c.Append($"\n\t\t\tend_if");
                        }
                        c.Append($"\n\t\t\tif I_EventCounter x = 9");
                        c.Append(Script.FireInCityOpticalAndEffect(r.CID));
                        c.Append($"\n\t\t\t\thistoric_event cwcpe8{r.CID}");
                        HEGenerator.Add($"cwcpe8{r.CID}", $"Repressions in {r.RegionName}", $"The governor of {r.RegionName} seems to have initiated repressions in his region. He is known for protesting against the current inbalance in the factions council, as he is not part of the party which is currently in great power.", $"@64");
                        c.Append($"\n\t\t\tend_if");
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
