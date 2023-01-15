using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class SeaLoot
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "SeaLoot";
            if (Properties.Settings.Default.cbSeaLoot || isAlwaysActive)
            {
                var resourcesOldWorld = World.Resources.Where(a => !a.IsNewWorld);
                var maxValue = resourcesOldWorld.OrderByDescending(a => a.Value).First().Value * Tuner.SeabattleLootValueMultiplier;
                c.Clear();
                //Player won
                c.Append($"\nmonitor_event PostBattle FactionIsLocal");
                c.Append($"\n\tand WonBattle");
                c.Append($"\n\tand I_ConflictType Naval");
                c.Append($"\n\tand RandomPercent < {Tuner.SeabattleLootChance}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\n\t\tgenerate_random_counter x 0 {World.PlayableFactionsOldWorld.Count - 1}");
                foreach (var (f, i) in World.PlayableFactionsOldWorld.Select((v, i) => (v, i)).ToList())
                {
                    c.Append($"\n\t\tif I_EventCounter x = {i}");
                    c.Append($"\n\t\t\tand I_IsFactionAIControlled {f.ID}");
                    c.Append($"\n\t\t\t\tgenerate_random_counter y 0 {resourcesOldWorld.Count()}");
                    foreach (var (r, cnt) in resourcesOldWorld.Select((v, i) => (v, i)).ToList())
                    {
                        var v = r.Value * Tuner.SeabattleLootValueMultiplier;
                        var e = $"{f.Order}sbl{r.ID}";
                        HEGenerator.Add(e, $"Seabattle Loot", $"Turns out that the defeated fleet was protecting or holding hostage a trade ship from {f.Name}.||" +
                            $"It is transporting {r.Name}, valued at {v} florins - do you let them proceed unharmed?||" +
                            $"If you accept, the goods and their value will go to {f.Name}, otherwise you can add the goods to your storage or sell them.", "@55");
                        c.Append($"\n\tif I_EventCounter y = {cnt}");
                        c.Append(Script.YesNoQuestion(e));
                        c.Append(Script.If($"I_EventCounter {e}_accepted = 1", $"add_money {f.ID} {v}"));
                        c.Append($"\n\t\tif I_EventCounter {e}_declined = 1");
                        c.Append(Script.IfChance(v * 100 / maxValue / 2, Script.SetDiplomaticStanceToPlayer(f.ID, "war")));
                        c.Append(Script.IfChance(v * 100 / maxValue, Script.SetFactionStandingToPlayer(f.ID, -1.0)));
                        if (r.Consumable != "NULL")
                        {
                            c.Append($"\n\t\t\tif I_CompareCounter s{r.Consumable} < {Tuner.ConsumablesStorageMax}");
                            foreach (var a in (Tuner.ConsumablesStorageMax - 1).To(0))
                            {
                                HEGenerator.Add($"sblAsk{r.Consumable}{a}", $"Sell looted {r.ConsumableText}?", $"You looted {r.Name} and your {r.ConsumableText} storage is at {Translator.Storage(a)}.||" +
                                    $"Do you want to immediately sell the looted resources for {v} florins? If you decline, they will instead be added to your storage.", $"@{r.ConsumableText}");
                                c.Append($"\n\t\t\t\tif I_CompareCounter s{r.Consumable} = {a}");
                                c.Append(Script.YesNoQuestion($"sblAsk{r.Consumable}{a}"));
                                c.Append(Script.If($"I_EventCounter sblAsk{r.Consumable}{a}_accepted = 1", Script.AddMoneyToPlayer(v)));
                                c.Append(Script.If($"I_EventCounter sblAsk{r.Consumable}{a}_declined = 1", $"inc_counter s{r.Consumable} 1"));
                                c.Append($"\n\t\t\t\tend_if");
                            }
                            c.Append($"\n\t\t\tend_if");
                            c.Append(Script.IfCounter($"s{r.Consumable}", Tuner.ConsumablesStorageMax, Script.AddMoneyToPlayer(v)));
                        }
                        else
                            c.Append(Script.AddMoneyToPlayer(v));
                        c.Append($"\n\t\tend_if");
                        c.Append($"\n\tend_if");
                    }
                    c.Append($"\n\t\tend_if");
                }
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
