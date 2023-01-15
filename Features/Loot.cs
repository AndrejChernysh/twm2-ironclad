using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Loot
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Loot";
            if (Properties.Settings.Default.cbLoot || isAlwaysActive)
            {
                c.Clear();
                Dictionary<string, int> types = new Dictionary<string, int>() { { "not", -1 }, { "", 1 } };
                foreach (var t in types.Keys)
                    foreach (var t2 in types.Keys)
                        foreach (var k in Tuner.BattleLootMinAmount.Keys)
                        {
                            c.Append($"\nmonitor_event PostBattle FactionIsLocal");
                            c.Append($"\n\tand {t} WonBattle");
                            c.Append($"\n\tand BattleSuccess = {k}");
                            c.Append($"\n\tand ! I_ConflictType Naval");
                            c.Append($"\n\tand RandomPercent < {Tuner.BattleLootChance}");
                            c.Append($"\n\tand {t2} GeneralFoughtFaction slave");
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            if (Properties.Settings.Default.cbConsumables)
                            {
                                var cnt = 0;
                                c.Append($"\n\t\tgenerate_random_counter x 1 |cnt|");
                                foreach (var m in Tuner.BattleLootMultipliers)
                                {
                                    cnt++;
                                    var lDouble = Convert.ToDouble((Tuner.BattleLootMinAmount[k] + m * 100) * types[t]);
                                    lDouble = t2 == "not" ? lDouble * 1 : lDouble * Tuner.BattleLootAgainstSlaveMultiplier;
                                    var lInt = Convert.ToInt32(lDouble);
                                    var title = $"BL{lInt}".Replace("-", "M");
                                    var titleExt = lInt < 0 ? "Lost" : "Looted";
                                    var pic = lInt < 0 ? "@66" : "@24";
                                    var body = lInt < 0 ? $"Our troops have not only been defeated but unfortunately also have lost the florins they have been transporting with them to the enemy." :
                                        $"Our troops have not only defeated the enemy in battle but also captured a florins reserve from the foe.";
                                    HEGenerator.Add(title, $"{titleExt} {lInt} florins in Battle".Rem("-"), $"{body}", $"{lInt}", pic);
                                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                                    c.Append(Script.AddMoneyToPlayer(lInt));
                                    c.Append($"\n\t\t\thistoric_event {title}");
                                    c.Append($"\n\t\t\tlog always Battle Loot +{lInt}".Replace("+-", "-"));
                                    c.Append($"\n\t\tend_if");
                                }
                                foreach (var co in World.Consumables)
                                {
                                    cnt++;
                                    var title = t == "not" ? "Lost" : "Looted";
                                    var pic = t == "not" ? 66 : 24;
                                    var txt = t == "not" ? $"defeat on the battlefield, our enemy looted {Translator.Consumable(co)} from the remains of our" :
                                        $"victory on the battlefield, we looted {Translator.Consumable(co)} from the remains of the enemy";
                                    var cond = t == "not" ? "> 0" : $"< {Tuner.ConsumablesStorageMax}";
                                    c.Append($"\n\t\tif I_EventCounter x = {cnt}");
                                    c.Append($"\n\t\t\tand I_CompareCounter s{co} {cond}");
                                    c.Append($"\n\t\t\tinc_counter s{co} {types[t]}");
                                    foreach (var i in 0.To(Tuner.ConsumablesStorageMax))
                                    {
                                        var titleId = t == "not" ? $"BL{co}M{i}" : $"BL{co}P{i}";
                                        HEGenerator.Add(titleId, $"{title} {Translator.Consumable(co)}", $"In the aftermath of the recent {txt} troops.||" +
                                            $"Current {Translator.Consumable(co)} stocked: {Translator.Storage(i)}", $"@{pic}");
                                        c.Append(Script.IfCounter($"s{co}", i, $"historic_event {titleId}"));
                                    }
                                    c.Append($"\n\t\t\tlog always Battle Loot {co} +{types[t]}".Replace("+-", "-"));
                                    c.Append($"\n\t\tend_if");
                                }
                                c.Replace("|cnt|", $"{cnt}");
                            }
                            else
                            {
                                c.Append($"\n\t\tgenerate_random_counter x 1 {Tuner.BattleLootMultipliers.Count}");
                                foreach (var m in Tuner.BattleLootMultipliers)
                                {
                                    var lDouble = Convert.ToDouble((Tuner.BattleLootMinAmount[k] + m * 100) * types[t]);
                                    lDouble = t2 == "not" ? lDouble * 1 : lDouble * Tuner.BattleLootAgainstSlaveMultiplier;
                                    var lInt = Convert.ToInt32(lDouble);
                                    var title = $"BL{lInt}".Replace("-", "M");
                                    var titleExt = lInt < 0 ? "Lost" : "Looted";
                                    var pic = lInt < 0 ? "@66" : "@24";
                                    var body = lInt < 0 ? $"been defeated but unfortunately also have lost the florins reserve they have been transporting with them to the enemy." :
                                        $"defeated the enemy in battle but also captured a florins reserve from the foe.";
                                    HEGenerator.Add(title, $"{titleExt} {lInt} florins in Battle".Rem("-"), $"Our troops have not only {body}", $"{lInt}", pic);
                                    c.Append($"\n\t\tif I_EventCounter x = {m}");
                                    c.Append(Script.AddMoneyToPlayer(lInt));
                                    c.Append($"\n\t\t\thistoric_event {title}");
                                    c.Append($"\n\t\t\tlog always Battle Loot +{lInt}".Replace("+-", "-"));
                                    c.Append($"\n\t\tend_if");
                                }
                            }
                            c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                            c.Append($"\nend_monitor");
                        }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
