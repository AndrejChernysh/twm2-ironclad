using Ironclad.Entities;
using Ironclad.Helper;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Ironclad.Features
{
    static class Deals
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Deals";
            var order = 3;
            if (Properties.Settings.Default.cbDeals || isAlwaysActive)
            {
                var resources = World.Resources.Where(a => !a.IsNewWorld);
                var maxValue = resources.OrderByDescending(a => a.Value).First().Value * Tuner.DealMultiplier;
                c.Clear();
                // Player
                foreach (var m in new List<string>() { "AcquisitionMission", "SufferAcquisitionAttempt" })
                {
                    c.Append($"\nmonitor_event {m} FactionIsLocal");
                    c.Append($"\n\tand RandomPercent < {Tuner.DealChance}");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\t\tgenerate_random_counter f 1 {World.PlayableFactionsOldWorld.Count}");
                    foreach (var f in World.PlayableFactionsOldWorld)
                    {
                        c.Append($"\n\t\tif I_EventCounter f = {f.Order}");
                        c.Append($"\n\t\t\tand I_IsFactionAIControlled {f.ID}");
                        c.Append($"\n\t\t\tand I_NumberOfSettlements {f.ID} > 0");
                        c.Append($"\n\t\t\tand I_CompareCounter isEnemy{f.Order} = 0");
                        if (Properties.Settings.Default.cbConsumables)
                        {
                            var rg = Rndm.Pick(World.Resources.Where(a => !a.IsNewWorld && a.Consumable != "NULL")).Consumable;
                            c.Append($"\n\t\t\t\tgenerate_random_counter o 1 2");
                            c.Append($"\n\t\t\t\t\tif I_EventCounter o = 1");
                            c.Append($"\n\t\t\t\t\t\tand I_CompareCounter s{rg} > 1");
                            c.Append($"\n\t\t\t\t\t\t\tset_counter ots{rg} 1");
                            c.Append($"\n\t\t\t\t\tend_if");
                            c.Append($"\n\t\t\t\t\tif I_EventCounter o = 2");
                            c.Append($"\n\t\t\t\t\t\tand I_CompareCounter s{rg} < {Tuner.ConsumablesStorageMax - 1}");
                            c.Append($"\n\t\t\t\t\t\t\tset_counter otb{rg} 1");
                            c.Append($"\n\t\t\t\t\tend_if");
                        }
                        c.Append($"\n\t\t\t\tgenerate_random_counter r 1 {resources.Count()}");
                        foreach (var (r, i) in resources.Select((v, i) => (v, i)).ToList())
                        {
                            var v = r.Value * Tuner.DealMultiplier;
                            HEGenerator.Add($"bds{r.ID}{f.Order}_{v}", $"Sold {r.Name} to {f.NameShort}", $"Our merchant sold some {r.Name} to a competitor who was acting for The {f.Name}.", $"{v}", "@10");
                            HEGenerator.Add($"bdb{r.ID}{f.Order}_{v}", $"Bought {r.Name} from {f.NameShort}", $"Our merchant bought some {r.Name} from a competitor who was acting for The {f.Name}.", $"-{v}", "@10");
                            c.Append($"\n\t\t\t\t\tif I_EventCounter r = {i + 1}");
                            c.Append($"\n\t\t\t\t\t\tgenerate_random_counter a 1 2");
                            c.Append($"\n\t\t\t\t\t\tif I_EventCounter a = 1");
                            c.Append($"\n\t\t\t\t\t\t\thistoric_event bds{r.ID}{f.Order}_{v}");
                            c.Append($"\n\t\t\t\t\t\t\tadd_money {f.ID} {v * -1}");
                            c.Append(Script.AddMoneyToPlayer(v));
                            c.Append(Script.xl() ? $"\nlog always Deal {r.ID} Player +{v} {f.ID} -{v}" : "");
                            c.Append(Script.IfChance(v * 100 / maxValue / 2, Script.SetFactionStandingToPlayer(f.ID, -1)));
                            c.Append(Script.IfChance(v * 100 / maxValue / 4, Script.SetDiplomaticStanceToPlayer(f.ID, "war")));
                            c.Append($"\n\t\t\t\t\t\tend_if");
                            c.Append($"\n\t\t\t\t\t\tif I_EventCounter a = 2");
                            c.Append($"\n\t\t\t\t\t\t\thistoric_event bdb{r.ID}{f.Order}_{v}");
                            c.Append($"\n\t\t\t\t\t\t\tadd_money {f.ID} {v}");
                            c.Append(Script.AddMoneyToPlayer(v * -1));
                            c.Append(Script.xl() ? $"\nlog always Deal {r.ID} Player -{v} {f.ID} +{v}" : "");
                            c.Append($"\n\t\t\t\t\t\tend_if");
                            c.Append($"\n\t\t\t\t\tend_if");
                        }
                        c.Append($"\n\t\tend_if");
                    }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                // AI
                c.Append($"\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append($"\n\tand RandomPercent < 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tgenerate_random_counter fa 1 {World.PlayableFactionsOldWorld.Count}");
                foreach (var (f1, i1) in World.PlayableFactionsOldWorld.Select((v, i) => (v, i)).ToList())
                {
                    c.Append($"\n\t\tif I_EventCounter fa = {i1 + 1}");
                    c.Append($"\n\t\t\tgenerate_random_counter fb 1 {World.PlayableFactionsOldWorld.Count - 1}");
                    foreach (var (f2, i2) in World.PlayableFactionsOldWorld.Where(a => a.ID != f1.ID).Select((v, i) => (v, i)).ToList())
                    {
                        HEGenerator.Add($"{f1.Order}TW{f2.Order}", $"Trade War {f1.NameShort}-{f2.NameShort}", $"The {f1.Name} and The {f2.Name} have had some disputes regarding their trading activities together for quite some time until now, resulting in conflict.", "@1");
                        c.Append($"\n\t\t\tif I_EventCounter fa = {i1 + 1}");
                        c.Append($"\n\t\t\t\tand I_EventCounter fb = {i2 + 1}");
                        c.Append($"\n\t\t\t\tand I_NumberOfSettlements {f1.ID} > 0");
                        c.Append($"\n\t\t\t\tand I_IsFactionAIControlled {f1.ID}");
                        c.Append($"\n\t\t\t\tand I_NumberOfSettlements {f2.ID} > 0");
                        c.Append($"\n\t\t\t\tand I_IsFactionAIControlled {f2.ID}");
                        c.Append($"\n\t\t\t\t\thistoric_event {f1.Order}TW{f2.Order}");
                        c.Append(Script.SetDiplomaticStance(f1.ID, f2.ID, "war"));
                        c.Append(Script.SetFactionStanding(f1.ID, f2.ID, -1));
                        c.Append(Script.TransferMoneyFromTo(f1.ID, f2.ID, maxValue));
                        c.Append(Script.xl() ? $"\nlog always Trade War: {f1.ID} {f2.ID}" : "");
                        c.Append($"\n\t\t\tend_if");
                    }
                    c.Append($"\n\t\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
