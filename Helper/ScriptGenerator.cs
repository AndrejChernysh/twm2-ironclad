using Ironclad.Entities;
using Ironclad.Features;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Helper
{
    static class ScriptGenerator
    {
        public static List<Script> Scripts = new List<Script>() { };
        public static Dictionary<String, int> Counters = new Dictionary<String, int>() { };
        public static void AddCounter(string counter, int value)
        {
            if (!Counters.ContainsKey(counter))
                Counters.Add(counter, value);
        }
        internal static void GenerateAll()
        {
            Counters.Clear();
            Scripts.Clear();
            Scripts.Add(ControllerActions.Get());
            Scripts.Add(ControllerPlayerOwnership.Get());
            Scripts.Add(ControllerDiplomaticStance.Get());
            Scripts.Add(Unification.Get());
            Scripts.Add(SlaveSpawnUnit.Get());
            Scripts.Add(Council.Get());
            Scripts.Add(BattleAI.Get());
            Scripts.Add(AIExpansion.Get());
            Scripts.Add(AICapitalBoost.Get());
            Scripts.Add(Blockades.Get());
            Scripts.Add(Migration.Get());
            Scripts.Add(Razing.Get());
            Scripts.Add(FirstContact.Get());
            Scripts.Add(RandomEvents.Get());
            Scripts.Add(Empire.Get());
            Scripts.Add(Loans.Get());
            Scripts.Add(ForcedMarch.Get());
            Scripts.Add(Mobilization.Get());
            Scripts.Add(Raids.Get());
            Scripts.Add(Loot.Get());
            Scripts.Add(SeaLoot.Get());
            Scripts.Add(Deals.Get());
            Scripts.Add(CivilWars.Get());
            Scripts.Add(Regulation.Get());
            Scripts.Add(Colonies.Get());
            Scripts.Add(ReEmergence.Get());
            Scripts.Add(PopeMechanics.Get());
            Scripts.Add(AlliedAssist.Get());
            Scripts.Add(Peaceseeking.Get());
            Scripts.Add(Invasions.Get());
            Scripts.Add(DebtCrises.Get());
            Scripts.Add(CapitalLost.Get());
            Scripts.Add(CapitalChange.Get());
            Scripts.Add(EmergencyTaxes.Get());
            Scripts.Add(Consumables.Get());
            Scripts.Add(ConsumablesTrading.Get());
            Scripts.Add(SiegeRestriction.Get());
            Scripts.Add(Informants.Get());
            Scripts.Add(Garrisons.Get());
            Scripts.Add(DiplomacyCosts.Get());
            Scripts.Add(SelectHeir.Get());
            Scripts.Add(ExpansionPenalty.Get());
            Scripts.Add(SiegeCosts.Get());
            Scripts.Add(FirstTimeBuild.Get());
            Scripts.Add(Autonomies.Get());
            Scripts.Add(ReligiousUnrest.Get());
            Scripts.Add(AIRecruitmentBoost.Get());
            Scripts.Add(EventSpawns.Get());
            Scripts.Add(ControllerVariables.Get()); // Must be 3rd last!
            Scripts.Add(ControllerVariablesCooloff.Get()); // Must be 2nd last!
            Scripts.Add(ControllerScript.Get()); // Must be last!
            File.Delete(Hardcoded.CAMPAIGN);
            foreach (var script in Scripts.Where(a => a.Code != "\n\n").OrderBy(a => a.Order).ToList())
                File.AppendAllText(Hardcoded.CAMPAIGN, script.Code.Replace("\n", "\r\n").Replace("\t", "").Replace("#", "\t"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.CAMPAIGN)} ({FO.GetSizeKB(Hardcoded.CAMPAIGN)} KB)");
            FO.RemoveEmptyLines(Hardcoded.CAMPAIGN);
            IO.Log($"Removed empty lines from {FO.GetFileName(Hardcoded.CAMPAIGN)} ({FO.GetSizeKB(Hardcoded.CAMPAIGN)} KB)");
            if (Properties.Settings.Default.cbShowStatsScripts)
            {
                var sMonitors = Convert.ToDecimal(Scripts.Where(a => a.Code != "\n\n").Sum(a => a.MonitorsCount));
                var sSize = Convert.ToDecimal(Scripts.Where(a => a.Code != "\n\n").Sum(a => a.SizeMB));
                IO.Log($"--- BY MONITORS ---");
                IO.Log($"ExtName\tDescription\tMonitors");
                foreach (var s in Scripts.Where(a => a.Code != "\n\n").OrderByDescending(a => a.MonitorsCount))
                    IO.Log($"{s.ID}\t{s.MonitorsCount}\t{Math.Round(s.MonitorsCount / sMonitors * Convert.ToDecimal(100))}%");
                IO.Log($"--- BY SIZE ---");
                IO.Log($"ExtName\tDescription\tSize in MB");
                foreach (var s in Scripts.Where(a => a.Code != "\n\n").OrderByDescending(a => a.SizeMB))
                    IO.Log($"{s.ID}\t{s.SizeMB}\t{Math.Round((s.SizeMB / sSize * Convert.ToDecimal(100)))}%");
            }
            if (Properties.Settings.Default.cbShowStatsNamesets)
            {
                foreach (var nameset in World.Names.Select(a => a.NameSet).Distinct())
                {
                    var male = World.Names.Count(a => a.NameSet == nameset && a.Type == "characters");
                    var surname = World.Names.Count(a => a.NameSet == nameset && a.Type == "surnames");
                    IO.Log($"{nameset} Male: {male} Surname: {surname} Total: {male + surname} ");
                }
            }
        }

    }
}