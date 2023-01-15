using Ironclad.Entities;
using Ironclad.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class AlliedAssist
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()




        {
            var isAlwaysActive = false;
            var scriptGroup = "AlliedAssist";
            var order = 66;
            if (Properties.Settings.Default.cbAlliedAssist || isAlwaysActive)
            {
                c.Clear();
                var registeredPairings = new List<String>() { };
                foreach (var fAI1 in World.PlayableFactionsOldWorld)
                    foreach (var fAI2 in World.PlayableFactionsOldWorld.Where(a => fAI1.ID != a.ID))
                    {
                        var pairing = fAI1.Order > fAI2.Order ? $"{fAI1.Order}_{fAI2.Order}" : $"{fAI2.Order}_{fAI1.Order}";
                        if (!registeredPairings.Contains(pairing))
                        {
                            registeredPairings.Add(pairing);
                            c.Append($"\nmonitor_event FactionWarDeclared FactionType {fAI1.ID}");
                            c.Append($"\nand TargetFactionType {fAI2.ID}");
                            c.Append($"\nand I_CompareCounter aasr = 0"); // AlliedAssist Stop Recursion
                            c.Append($"\n\tset_counter aasr 1");
                            foreach (var fAI1Ally in World.PlayableFactionsOldWorld.Where(a => a.ID != fAI1.ID && a.ID != fAI2.ID))
                            {
                                c.Append($"\nif ! I_LocalFaction {fAI1Ally.ID}");
                                c.Append($"\nand I_CompareCounter {Script.GetIsAllyCounter(fAI1Ally, fAI1)} = 1");
                                c.Append($"\n\tif RandomPercent < {Tuner.AlliedAssistChanceEndsAllianceWithAlly}");
                                c.Append(Script.SetDiplomaticStance(fAI1Ally.ID, fAI1.ID, "neutral"));
                                c.Append($"\n\tend_if");
                                c.Append($"\n\tif RandomPercent < {Tuner.AlliedAssistChanceDeclaresWarOnAllysEnemy}");
                                c.Append($"\n\tand I_CompareCounter {Script.GetIsWarCounter(fAI1Ally, fAI2)} = 0");
                                c.Append($"\n\tand I_CompareCounter {Script.GetIsAllyCounter(fAI1Ally, fAI1)} = 1"); // still allied?
                                c.Append(Script.SetDiplomaticStance(fAI1Ally.ID, fAI2.ID, "war"));
                                c.Append(Script.SetFactionStanding(fAI1Ally.ID, fAI2.ID, -1.0));
                                c.Append(Script.SetFactionStanding(fAI1Ally.ID, fAI1.ID, 1.0));
                                c.Append($"\n\tend_if");
                                c.Append($"\nend_if");
                            }
                            c.Append($"\n\tset_counter aasr 0");
                            c.Append($"\nend_monitor");
                        }
                    }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}