using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ConsumablesTrading
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Consumables";
            if (Properties.Settings.Default.cbConsumables || isAlwaysActive)
            {
                c.Clear();

                //Trigger Trades
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\n\tif I_CompareCounter isPlayer{r.CID} = 1");
                    c.Append($"\n\t\tand ! I_SettlementUnderSiege {r.CID}");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 1");
                    c.Append($"\n\t\t\t\tand I_CompareCounter ostpt = 0");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesExportHighChance}");
                    c.Append($"\n\t\t\t\t\tgenerate_random_counter x 0 {World.Consumables.Count - 1}");
                    foreach (var (rg, i) in World.Consumables.Select((v, i) => (v, i)).OrderBy(a => a.v).ToList())
                    {
                        c.Append($"\n\t\t\t\t\tif I_EventCounter x = {i}");
                        c.Append($"\n\t\t\t\t\t\tand I_CompareCounter s{rg} > 0");
                        c.Append($"\n\t\t\t\t\t\t\tset_counter ots{rg} 1");
                        c.Append($"\n\t\t\t\t\t\t\tset_counter ostpt 1");
                        c.Append($"\n\t\t\t\t\tend_if");
                    }
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 2");
                    c.Append($"\n\t\t\t\tand I_CompareCounter ostpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesExportModerateChance}");
                    c.Append($"\n\t\t\t\t\tgenerate_random_counter x 0 {World.Consumables.Count - 1}");
                    foreach (var (rg, i) in World.Consumables.Select((v, i) => (v, i)).OrderBy(a => a.v).ToList())
                    {
                        c.Append($"\n\t\t\t\t\tif I_EventCounter x = {i}");
                        c.Append($"\n\t\t\t\t\t\tand I_CompareCounter s{rg} > 0");
                        c.Append($"\n\t\t\t\t\t\t\tset_counter ots{rg} 1");
                        c.Append($"\n\t\t\t\t\t\t\tset_counter ostpt 1");
                        c.Append($"\n\t\t\t\t\tend_if");
                    }
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 3");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesImportWorkforceChance}");
                    c.Append($"\n\t\t\t\tand I_CompareCounter sWo < {Tuner.ConsumablesStorageMax}");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter otbWo 1");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter obtpt 1");
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 4");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesImportPotablesChance}");
                    c.Append($"\n\t\t\t\tand I_CompareCounter sPo < {Tuner.ConsumablesStorageMax}");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter otbPo 1");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter obtpt 1");
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 5");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesImportFoodChance}");
                    c.Append($"\n\t\t\t\tand I_CompareCounter sFo < {Tuner.ConsumablesStorageMax}");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter otbFo 1");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter obtpt 1");
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 6");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesImportClothingChance}");
                    c.Append($"\n\t\t\t\tand I_CompareCounter sCl < {Tuner.ConsumablesStorageMax}");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter otbCl 1");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter obtpt 1");
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 7");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesImportMaterialsChance}");
                    c.Append($"\n\t\t\t\tand I_CompareCounter sMa < {Tuner.ConsumablesStorageMax}");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter otbMa 1");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter obtpt 1");
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\t\t\tif I_CompareCounter state{r.CID} = 8");
                    c.Append($"\n\t\t\t\tand I_CompareCounter obtpt = 0");
                    c.Append($"\n\t\t\t\tand RandomPercent < {Tuner.ConsumablesImportAdditivesChance}");
                    c.Append($"\n\t\t\t\tand I_CompareCounter sAd < {Tuner.ConsumablesStorageMax}");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter otbAd 1");
                    c.Append($"\n\t\t\t\t\t\t\tset_counter obtpt 1");
                    c.Append($"\n\t\t\tend_if");
                    c.Append($"\n\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                //Reset One Trade Per Turn Counter
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\tset_counter ostpt 0");
                c.Append($"\n\tset_counter obtpt 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                //Execute Trades Buy
                var counter = 0;
                foreach (var co in World.Consumables.OrderBy(a => a))
                {
                    c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                    c.Append($"\n\tand I_CompareCounter otb{co} = 1");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\tgenerate_random_counter x 1 [counter]");
                    foreach (var f in World.PlayableFactionsOldWorld)
                        foreach (var o in Tuner.ConsumablesTradingOffers)
                        {
                            counter++;
                            c.Append($"\n\t\tif I_EventCounter x = {counter}");
                            c.Append($"\n\t\t\tand I_IsFactionAIControlled {f.ID}");
                            c.Append($"\n\t\t\tand I_NumberOfSettlements {f.ID} > 0");
                            c.Append($"\n\t\t\tand I_CompareCounter isEnemy{f.Order} = 0");
                            if (o <= Convert.ToInt32(Convert.ToDouble(Tuner.ConsumablesTradingOffers.Max()) * 0.5))
                                c.Append($"\n\t\t\tand I_CompareCounter isAlly{f.Order} = 1");
                            if (Properties.Settings.Default.cbFirstContact)
                                c.Append($"\n\t\t\tand I_CompareCounter isContacted{f.Order} = 1");
                            c.Append(Script.xl() ? $"\nlog always otb{co} {f.ID} {o}" : "");
                            foreach (var i in 0.To(Tuner.ConsumablesStorageMax - 1))
                            {
                                var t = $"otb{co.First()}{counter}_{i}";
                                HEGenerator.Add(t, $"Buy {Translator.Consumable(co)}?", $"A merchant from {f.NameShort} is offering us to sell {Translator.Consumable(co)} for a price of {o} florins. Do you agree?||Current {Translator.Consumable(co)} stock: {Translator.Storage(i)}", $"@{Translator.Consumable(co)}");
                                c.Append($"\n\tif I_CompareCounter s{co} = {i}");
                                c.Append($"\n\t\tand I_CompareCounter otb{co} = 1");
                                c.Append(Script.YesNoQuestion(t));
                                c.Append($"\n\t\t\tif I_EventCounter {t}_accepted = 1");
                                c.Append(Script.AddMoneyToPlayer(o * -1));
                                c.Append($"\n\t\t\t\tadd_money {f.ID} {o}");
                                c.Append($"\n\t\t\t\tinc_counter s{co} 1");
                                c.Append($"\n\t\t\t\tset_counter otb{co} 0");
                                c.Append($"\n\t\t\tend_if");
                                c.Append(Script.If($"I_EventCounter {t}_declined = 1", $""));
                                c.Append($"\n\t\tset_counter otb{co} 0");
                                c.Append($"\n\tend_if");
                            }
                            c.Append($"\n\t\tend_if");
                        }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                    c.Replace("[counter]", $"{counter}");
                    //Execute Trades Sell
                    c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                    c.Append($"\n\tand I_CompareCounter ots{co} = 1");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\tgenerate_random_counter x 1 [counter]");
                    counter = 0;
                    foreach (var f in World.PlayableFactionsOldWorld)
                        foreach (var o in Tuner.ConsumablesTradingOffers)
                        {
                            counter++;
                            c.Append($"\n\t\tif I_EventCounter x = {counter}");
                            c.Append($"\n\t\t\tand I_IsFactionAIControlled {f.ID}");
                            c.Append($"\n\t\t\tand I_NumberOfSettlements {f.ID} > 0");
                            c.Append($"\n\t\t\tand I_CompareCounter isEnemy{f.Order} = 0");
                            if (Properties.Settings.Default.cbFirstContact)
                                c.Append($"\n\t\t\tand I_CompareCounter isContacted{f.Order} = 1");
                            if (o >= Convert.ToInt32(Convert.ToDouble(Tuner.ConsumablesTradingOffers.Max()) * 0.5))
                                c.Append($"\n\t\t\tand I_CompareCounter isAlly{f.Order} = 1");
                            c.Append(Script.xl() ? $"\nlog always ots{co} {f.ID} {o}" : "");
                            foreach (var i in 1.To(Tuner.ConsumablesStorageMax))
                            {
                                var t = $"ots{co.First()}{counter}_{i}";
                                HEGenerator.Add(t, $"Sell {Translator.Consumable(co)}?", $"A merchant from {f.NameShort} is offering us to buy {Translator.Consumable(co)} for a price of {o} florins. Do you agree?||Current {Translator.Consumable(co)} stock: {Translator.Storage(i)}", $"@{Translator.Consumable(co)}");
                                c.Append($"\n\tif I_CompareCounter s{co} = {i}");
                                c.Append($"\n\t\tand I_CompareCounter ots{co} = 1");
                                c.Append(Script.YesNoQuestion(t));
                                c.Append($"\n\t\t\tif I_EventCounter {t}_accepted = 1");
                                c.Append(Script.AddMoneyToPlayer(o));
                                c.Append($"\n\t\t\tadd_money {f.ID} {o * -1}");
                                c.Append($"\n\t\t\t\tinc_counter s{co} -1");
                                c.Append($"\n\t\t\t\tset_counter ots{co} 0");
                                c.Append($"\n\t\t\tend_if");
                                c.Append(Script.If($"I_EventCounter {t}_declined = 1", $""));
                                c.Append($"\n\t\t\tset_counter ots{co} 0");
                                c.Append($"\n\tend_if");
                            }
                            c.Append($"\n\t\tend_if");
                        }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                    c.Replace("[counter]", $"{counter}");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
