using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Consumables
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Consumables";
            if (Properties.Settings.Default.cbConsumables || isAlwaysActive)
            {
                var counter = 0;
                c.Clear();
                // Start of section: Trigger
                c.Append($"\nmonitor_event SettlementSelected");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tif I_CompareCounter CactivateCooloff = 0");
                c.Append($"\n\t\tset_counter Cactivate 1");
                c.Append($"\n\tend_if");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event CharacterSelected");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tif I_CompareCounter CactivateCooloff = 0");
                c.Append($"\n\t\tset_counter Cactivate 1");
                c.Append($"\n\tend_if");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_conditions I_CompareCounter Cactivate = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tset_counter Cactivate 0");
                // Start of section: Consumption
                c.Append($"\n\tgenerate_random_counter x 0 {World.Regions.Count(a => !a.IsUnreachable) - 1}");
                foreach (var (r, i) in World.Regions.Where(a => !a.IsUnreachable).Select((v, i) => (v, i)).ToList())
                {
                    c.Append($"\n\t\tif I_EventCounter x = {i}");
                    c.Append($"\n\t\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                    c.Append($"\n\t\t\t\tgenerate_random_counter co 0 {World.Consumables.Count(a => a != "NULL") - 1}");
                    foreach (var (co, n) in World.Consumables.Where(a => a != "NULL").Select((v, i) => (v, i)).ToList())
                    {
                        c.Append($"\n\t\t\t\t\tif I_EventCounter co = {n}");
                        c.Append($"\n\t\t\t\t\t\tand I_CompareCounter s{co} = 0");
                        c.Append($"\n\t\t\t\t\t\t\tset_counter sp{co}{r.CID} 1");
                        c.Append($"\n\t\t\t\t\tend_if");
                        c.Append($"\n\t\t\t\t\tif I_EventCounter co = {n}");
                        c.Append($"\n\t\t\t\t\t\tand I_CompareCounter s{co} > 0");
                        c.Append($"\n\t\t\t\t\t\t\tinc_counter s{co} -1");
                        c.Append($"\n\t\tset_counter CactivateCooloff 1");
                        foreach (var a in 0.To(Tuner.ConsumablesStorageMax - 1))
                        {
                            c.Append(Script.IfCounter($"s{co}", a, $"historic_event {r.CID}{co}Minus{a}"));
                            HEGenerator.Add($"{r.CID}{co}Minus{a}", $"{Translator.Consumable(co)} consumed ({r.CityName})", $"{Translator.Consumable(co)} were consumed in {r.CityName} ({r.RegionName}) after being taken from our storages.||{Translator.Consumable(co)} stocked after consumption: {Translator.Storage(a)}", $"@{Translator.Consumable(co)}");
                        }
                        c.Append($"\n\t\t\t\t\tend_if");
                    }
                    c.Append($"\n\t\tend_if");
                }
                // Start of section: Production
                c.Append($"\n\tgenerate_random_counter x 0 {World.Regions.Count(a => !a.IsUnreachable) - 1}");
                foreach (var (r, i) in World.Regions.Where(a => !a.IsUnreachable).Select((v, i) => (v, i)).ToList())
                {
                    var consumablesInR = r.Resources.Select(a => a.Consumable).Distinct().Where(a => a != "NULL").ToList();
                    c.Append($"\n\t\tif I_EventCounter x = {i}");
                    c.Append($"\n\t\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                    c.Append($"\n\t\t\tand ! I_SettlementUnderSiege {r.CID}");
                    c.Append($"\n\t\t\tand I_CompareCounter state{r.CID} < 9");
                    if (!consumablesInR.IsEmpty())
                        if (consumablesInR.Count == 1)
                        {
                            c.Append($"\n\t\t\tif I_CompareCounter s{consumablesInR.First()} < {Tuner.ConsumablesStorageMax}");
                            c.Append(Script.xl() ? $"\n\t\t\tlog always {consumablesInR.First()} {r.CityName} +1" : "");
                            c.Append($"\n\t\t\tinc_counter s{consumablesInR.First()} 1");
                            c.Append($"\n\t\tset_counter CactivateCooloff 1");
                            foreach (var a in 1.To(Tuner.ConsumablesStorageMax))
                            {
                                c.Append(Script.IfCounter($"s{consumablesInR.First()}", a, $"historic_event {r.CID}{consumablesInR.First()}Plus{a}"));
                                HEGenerator.Add($"{r.CID}{consumablesInR.First()}Plus{a}", $"{Translator.Consumable(consumablesInR.First())} produced ({r.CityName})", $"{Translator.Consumable(consumablesInR.First())} were produced in {r.CityName} ({r.RegionName}) and stored in our storages.||{Translator.Consumable(consumablesInR.First())} stocked after production: {Translator.Storage(a)}", $"@{Translator.Consumable(consumablesInR.First())}");
                            }
                            c.Append($"\n\t\t\tend_if");
                        }
                        else
                        {
                            c.Append($"\n\t\t\tgenerate_random_counter rg 0 {consumablesInR.Count - 1}");
                            foreach (var (rg, n) in consumablesInR.Select((v, i) => (v, i)).ToList())
                            {
                                c.Append($"\n\t\t\tif I_EventCounter rg = {n}");
                                c.Append($"\n\t\t\t\tand I_CompareCounter s{rg} < {Tuner.ConsumablesStorageMax}");
                                c.Append(Script.xl() ? $"\n\t\t\tlog always {consumablesInR.First()} {r.CityName} +1" : "");
                                c.Append($"\n\t\t\tinc_counter s{consumablesInR.First()} 1");
                                c.Append($"\n\t\tset_counter CactivateCooloff 1");
                                foreach (var a in 1.To(Tuner.ConsumablesStorageMax))
                                {
                                    c.Append(Script.IfCounter($"s{consumablesInR.First()}", a, $"historic_event {r.CID}{consumablesInR.First()}Plus{a}"));
                                    HEGenerator.Add($"{r.CID}{consumablesInR.First()}Plus{a}", $"{Translator.Consumable(consumablesInR.First())} produced ({r.CityName})", $"{Translator.Consumable(consumablesInR.First())} were produced in {r.CityName} ({r.RegionName}) and stored in our storages.||{Translator.Consumable(consumablesInR.First())} stocked after production: {Translator.Storage(a)}", $"@{Translator.Consumable(consumablesInR.First())}");
                                }
                                c.Append($"\n\t\t\tend_if");
                            }
                        }
                    c.Append($"\n\t\tend_if");
                }
                // Start of section: Penalty
                foreach (var co in World.Consumables.OrderBy(a => a))
                    foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                    {
                        c.Append($"\n\t\t\tif I_CompareCounter sp{co}{r.CID} = 1");
                        c.Append($"\n\t\t\t\tset_counter sp{co}{r.CID} 0");
                        c.Append($"\n\tif RandomPercent < 25");
                        c.Append(Script.FireInCityOpticalAndEffect(r.RID));
                        c.Append(Script.AlterRecruitPoolUnits(r.RID, -1));
                        c.Append(Script.CityRevolt(r.CID));
                        c.Append(Script.ClickOn("prebattle_fight_button"));
                        c.Append($"\n\tend_if");
                        c.Append($"\n\tif RandomPercent < 25");
                        c.Append(Script.SpawnRebelArmy(r.ResourcePositions.First(), World.Factions.First(a => a.ID == r.HomeFaction), 10, 17, 201, r));
                        c.Append($"\n\tend_if");
                        c.Append($"\n\tif RandomPercent < 25");
                        c.Append($"\n\t\t\t\t\t\tset_counter {r.CID}PopLose 1");
                        c.Append($"\n\tend_if");
                        c.Append($"\n\tif RandomPercent < 25");
                        c.Append($"\n\t\tadd_settlement_turmoil {r.CID} 16");
                        c.Append($"\n\tend_if");
                        c.Append($"\n\t\tset_counter CactivateCooloff 1");
                        c.Append($"\n\t\t\t\t\thistoric_event sp{co}{r.CID}");
                        HEGenerator.Add($"sp{co}{r.CID}", $"{r.RegionName} lacks {Translator.Consumable(co)}", $"There is currently a shortage for demand of {Translator.Consumable(co)} in the region of {r.RegionName}, especially in the city of {r.CityName}.||The stockpile of {Translator.Consumable(co)} is currently empty. We have to acquire or capture a settlement which has one of these resources: {string.Join(", ", World.Resources.Where(a => a.Consumable == co).Select(a => a.Name))}.", $"@{Translator.Consumable(co)}");
                        c.Append($"\n\t\t\tend_if");
                    }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                // Start of section: Show Storage Counts
                c.Append($"\nmonitor_event ButtonPressed ButtonPressed radar_zoom_out_button");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                var cnt = 0;
                foreach (var i0 in 0.To(Tuner.ConsumablesStorageMax))
                    foreach (var i1 in 0.To(Tuner.ConsumablesStorageMax))
                        foreach (var i2 in 0.To(Tuner.ConsumablesStorageMax))
                            foreach (var i3 in 0.To(Tuner.ConsumablesStorageMax))
                                foreach (var i4 in 0.To(Tuner.ConsumablesStorageMax))
                                    foreach (var i5 in 0.To(Tuner.ConsumablesStorageMax))
                                    {
                                        c.Append($"\n\t\t\tif I_CompareCounter s{World.Consumables.ElementAt(0)} = {i0}");
                                        c.Append($"\n\t\t\t\tand I_CompareCounter s{World.Consumables.ElementAt(1)} = {i1}");
                                        c.Append($"\n\t\t\t\tand I_CompareCounter s{World.Consumables.ElementAt(2)} = {i2}");
                                        c.Append($"\n\t\t\t\tand I_CompareCounter s{World.Consumables.ElementAt(3)} = {i3}");
                                        c.Append($"\n\t\t\t\tand I_CompareCounter s{World.Consumables.ElementAt(4)} = {i4}");
                                        c.Append($"\n\t\t\t\tand I_CompareCounter s{World.Consumables.ElementAt(5)} = {i5}");
                                        cnt++;
                                        HEGenerator.Add($"sr{cnt}", "Stocked Consumables",
                                            $"{Translator.Storage(i0, true)}      {Translator.Storage(i1, true)}      {Translator.Storage(i2, true)}      {Translator.Storage(i3, true)}      {Translator.Storage(i4, true)}      {Translator.Storage(i5, true)}||" +
                                            $"{Translator.Consumable(World.Consumables.ElementAt(0))}: {Translator.Storage(i0)}|{string.Join(", ", World.Resources.Where(a => a.Consumable == World.Consumables.ElementAt(0)).Select(a => a.Name))}||" +
                                            $"{Translator.Consumable(World.Consumables.ElementAt(1))}: {Translator.Storage(i1)}|{string.Join(", ", World.Resources.Where(a => a.Consumable == World.Consumables.ElementAt(1)).Select(a => a.Name))}||" +
                                            $"{Translator.Consumable(World.Consumables.ElementAt(2))}: {Translator.Storage(i2)}|{string.Join(", ", World.Resources.Where(a => a.Consumable == World.Consumables.ElementAt(2)).Select(a => a.Name))}||" +
                                            $"{Translator.Consumable(World.Consumables.ElementAt(3))}: {Translator.Storage(i3)}|{string.Join(", ", World.Resources.Where(a => a.Consumable == World.Consumables.ElementAt(3)).Select(a => a.Name))}||" +
                                            $"{Translator.Consumable(World.Consumables.ElementAt(4))}: {Translator.Storage(i4)}|{string.Join(", ", World.Resources.Where(a => a.Consumable == World.Consumables.ElementAt(4)).Select(a => a.Name))}||" +
                                            $"{Translator.Consumable(World.Consumables.ElementAt(5))}: {Translator.Storage(i5)}|{string.Join(", ", World.Resources.Where(a => a.Consumable == World.Consumables.ElementAt(5)).Select(a => a.Name))}||", $"@storage");
                                        c.Append($"\n\t\t\t\t\thistoric_event sr{cnt}");
                                        c.Append($"\n\t\t\tend_if");
                                    }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Replace("[counter]", $"{counter}");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
