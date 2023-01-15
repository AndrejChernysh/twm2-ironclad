using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class ControllerVariables
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = true;
            var scriptGroup = "ControllerVariables";
            var order = 1;
            if (isAlwaysActive)
            {
                c.Clear();

                // ALL SCRIPT VARIABLES ARE DECLARED HERE --------------------

                ScriptGenerator.AddCounter("00donotremovethis", 0); // Unused dummy variable must be on top

                ScriptGenerator.AddCounter("playerregioncount", 0);
                ScriptGenerator.AddCounter("active_heir_bucle", 0);
                ScriptGenerator.AddCounter("candidate", 0);
                ScriptGenerator.AddCounter("checking", 0);
                ScriptGenerator.AddCounter("decline_sh", 0);
                ScriptGenerator.AddCounter("first_candidate", 0);
                ScriptGenerator.AddCounter("iok", 0);
                ScriptGenerator.AddCounter($"EmergencyTaxesCooloff", 0);
                ScriptGenerator.AddCounter("debtcrisis_first", 0);
                ScriptGenerator.AddCounter("debtcrisis_second", 0);
                ScriptGenerator.AddCounter("debtcrisis_third", 0);
                ScriptGenerator.AddCounter("debtcrisis_fourth", 0);
                ScriptGenerator.AddCounter("debtcrisis_fifth", 0);
                ScriptGenerator.AddCounter($"fmCooloff", 0);
                ScriptGenerator.AddCounter($"rp", 0);
                ScriptGenerator.AddCounter($"aasr", 0);
                ScriptGenerator.AddCounter($"lrp", 0);
                ScriptGenerator.AddCounter($"updc", 0);
                ScriptGenerator.AddCounter($"cptp", 0);
                ScriptGenerator.AddCounter("AMP", 0);
                ScriptGenerator.AddCounter($"ARPUactivate", 0);
                ScriptGenerator.AddCounter($"DABactivate", 0);
                ScriptGenerator.AddCounter($"Ractivate", 0);
                ScriptGenerator.AddCounter("ostpt", 0);
                ScriptGenerator.AddCounter("obtpt", 0);
                ScriptGenerator.AddCounter($"Cactivate", 0);
                ScriptGenerator.AddCounter($"CactivateCooloff", 0);
                ScriptGenerator.AddCounter($"colCount", 0);
                ScriptGenerator.AddCounter($"colSnd", 0);
                ScriptGenerator.AddCounter($"ocpt", 0);
                ScriptGenerator.AddCounter($"oaiept", 0);
                foreach (var r in World.Regions)
                {
                    ScriptGenerator.AddCounter($"{r.CID}ursCooloff", 0);
                    ScriptGenerator.AddCounter($"ru{r.CID}CoolOff", 0);
                    ScriptGenerator.AddCounter($"orpt{r.CID}Cooloff", 0);
                    ScriptGenerator.AddCounter($"{r.CID}MobilizingCooloff", 0);
                    ScriptGenerator.AddCounter($"{r.CID}AutonomyCooloff", 0);
                    ScriptGenerator.AddCounter($"{r.CID}RiotCooloff", 0);
                    ScriptGenerator.AddCounter($"ss{r.ID}Cnt", 0);
                    ScriptGenerator.AddCounter($"{r.CID}PopGain", 0);
                    ScriptGenerator.AddCounter($"{r.CID}PopLose", 0);
                    ScriptGenerator.AddCounter($"askedg{r.CID}", 0);
                    ScriptGenerator.AddCounter($"mpg{r.CID}", 0);
                    ScriptGenerator.AddCounter($"isPlayer{r.CID}", 0);
                    ScriptGenerator.AddCounter($"ARPU{r.RID}50", 0);
                    ScriptGenerator.AddCounter($"ARPU{r.RID}", 0);
                    ScriptGenerator.AddCounter($"DAB{r.CID}", 0);
                    ScriptGenerator.AddCounter($"state{r.CID}", 1);
                    ScriptGenerator.AddCounter($"changed{r.CID}", 0);
                    ScriptGenerator.AddCounter($"R{r.CID}", 0);
                    ScriptGenerator.AddCounter($"pb{r.RID}", 0);
                    ScriptGenerator.AddCounter($"isEnemy{r.CID}", 0);
                    foreach (var f in World.Factions)
                        ScriptGenerator.AddCounter($"aie{f.Order}{r.CID}", 0);
                    foreach (var m in Tuner.AutonomiesOffersMoneyPerRound)
                        ScriptGenerator.AddCounter($"tribute_{r.CID}_{m}", 0);
                    foreach (var p in Settings.Parties)
                    {
                        ScriptGenerator.AddCounter($"{r.CID}{p.Key}", 0);
                        ScriptGenerator.AddCounter($"p{p.Key}", 0);
                        ScriptGenerator.AddCounter($"cwrp{p.Key}", 0);
                    }
                }
                foreach (var f in World.Factions)
                {
                    ScriptGenerator.AddCounter($"isPope{f.Order}", 0);
                    ScriptGenerator.AddCounter($"{f.Order}risingCooloff", 0);
                    ScriptGenerator.AddCounter($"{f.Order}PSCooloff", 0);
                    ScriptGenerator.AddCounter($"caga{f.Order}Cooloff", 0);
                    ScriptGenerator.AddCounter($"invasion{f.Order}", 0);
                    ScriptGenerator.AddCounter(Script.GetIsWarCounter(f, World.Factions.First(a => a.ID == "slave")), 1);
                    ScriptGenerator.AddCounter($"aie{f.Order}", 0);
                    ScriptGenerator.AddCounter($"aie{f.Order}Cooloff", Tuner.AIExpansionTurnIntervalCheck);
                    ScriptGenerator.AddCounter($"hmc{f.Order}", 1);
                    ScriptGenerator.AddCounter($"cww{f.Order}", 0);
                    ScriptGenerator.AddCounter($"col{f.Order}CoolOff", 0);
                    ScriptGenerator.AddCounter($"isAlly{f.Order}", 0);
                    if (f.ID == "slave")
                        ScriptGenerator.AddCounter($"isEnemy{f.Order}", 1);
                    else
                        ScriptGenerator.AddCounter($"isEnemy{f.Order}", 0);
                    ScriptGenerator.AddCounter($"isContacted{f.Order}", 0);
                    ScriptGenerator.AddCounter($"{f.Order}isEmpire", 0);
                    foreach (var b in World.Buildings.Where(a => a.MsgCompletionTitle != "NULL"))
                        ScriptGenerator.AddCounter($"FTB{b.MsgCompletionTitle}".Rem(" ", "?", "!", "-"), 0);
                    foreach (var f2 in World.Factions)
                    {
                        ScriptGenerator.AddCounter(Script.GetIsWarCounter(f, f2), 0);
                        ScriptGenerator.AddCounter(Script.GetIsAllyCounter(f, f2), 0);
                    }
                    foreach (var unification in World.Regions.Where(a => a.Unification != null).Select(a => a.Unification).Distinct())
                        ScriptGenerator.AddCounter($"u{unification.Replace(" ", "")}{f.Order}", 0);
                }
                foreach (var co in World.Consumables)
                {
                    ScriptGenerator.AddCounter($"s{co}", Rndm.Int(Tuner.ConsumablesStartAmountMin, Tuner.ConsumablesStartAmountMax));
                    ScriptGenerator.AddCounter($"ots{co}", 0);
                    ScriptGenerator.AddCounter($"otb{co}", 0);
                    foreach (var r in World.Regions)
                        ScriptGenerator.AddCounter($"sp{co}{r.CID}", 0);
                }
                foreach(var l in World.Loans)
                    ScriptGenerator.AddCounter(l.ID, 0);

                // ------------------------------------------------------

                foreach (var d in new List<string> { "declare", "set" })
                    foreach (var v in ScriptGenerator.Counters.OrderBy(a => a.Key))
                        if (!(d == "set" && v.Key == "00donotremovethis"))
                            c.Append($"\n{d}_counter {v.Key} {v.Value}");
                c.Append($"\nset_event_counter no_advice 1");
                return new Script(scriptGroup, c.ToString().Replace("declare_event_counter", "declare_counter"), isAlwaysActive, order);

            }
            return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
        }
    }
}
