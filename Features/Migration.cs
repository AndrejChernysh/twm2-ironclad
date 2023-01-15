using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Migration
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Migration";
            if (Properties.Settings.Default.cbMigration || isAlwaysActive)
            {
                c.Clear();
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\nmonitor_event SettlementTurnEnd SettlementName {r.CID}");
                    c.Append($"\n\tand SettlementTaxLevel = tax_low");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.IfChance(Tuner.MigrationChanceLowTaxes, $"set_counter {r.CID}PopGain 1"));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");

                    c.Append($"\nmonitor_event SettlementTurnEnd SettlementName {r.CID}");
                    c.Append($"\n\tand SettlementTaxLevel = tax_extortionate");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append(Script.IfChance(Tuner.MigrationChanceVeryHighTaxes, $"set_counter {r.CID}PopLose 1"));
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\nend_monitor");
                }
                c.Append($"\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var r in World.Regions.Where(a => a.IsDifficult))
                    c.Append(Script.IfChance(Tuner.MigrationChanceDifficultRegion, $"set_counter {r.CID}PopLose 1"));
                foreach (var r in World.Regions.Where(a => !a.IsUnreachable))
                {
                    c.Append($"\n\tif I_SettlementUnderSiege {r.CID}");
                    c.Append($"\n\tand RandomPercent < {Tuner.MigrationChanceSiege}");
                    c.Append($"\n\t\tset_counter {r.CID}PopLose 1");
                    c.Append($"\n\tend_if");

                    c.Append($"\n\tif I_CompareCounter {r.CID}PopGain = 1");
                    c.Append($"\n\t\tset_counter {r.CID}PopGain 0");
                    c.Append($"\n\t\tgenerate_random_counter x 1 {Tuner.MigrationAmounts.Count}");
                    foreach (var (a, i) in Tuner.MigrationAmounts.Select((v, i) => (v, i)).ToList())
                    {
                        c.Append($"\n\t\tif I_EventCounter x = {i + 1}");
                        c.Append($"\n\t\t\tconsole_command add_population {r.CID} {a}");
                        c.Append(Script.xl() ? $"\nlog always Migration {r.CityName} +{a}" : "");
                        c.Append(Script.If($"I_CompareCounter isPlayer{r.CID} = 1", $"historic_event {r.CID}_IMMIGRATION_{a}"));
                        HEGenerator.Add($"{r.CID}_IMMIGRATION_{a}", $"{r.RegionName} Welcomes Migrants",
                            $"{r.RegionName} and especially the regional capital {r.CityName} are currently experiencing a wave of immigration. {a} new citizens have been registered.", "@46");
                        c.Append($"\n\t\tend_if");
                    }
                    c.Append($"\n\t\tend_if");

                    c.Append($"\n\tif I_CompareCounter {r.CID}PopLose = 1");
                    c.Append($"\n\t\tset_counter {r.CID}PopLose 0");
                    c.Append($"\n\t\tgenerate_random_counter x 0 {Tuner.MigrationAmounts.Count}");
                    foreach (var (a, i) in Tuner.MigrationAmounts.Select((v, i) => (v, i)).ToList())
                    {
                        c.Append($"\n\t\tif I_EventCounter x = {i}");
                        c.Append($"\n\t\t\tconsole_command add_population {r.CID} -{a}");
                        c.Append(Script.xl() ? $"\nlog always Migration {r.CityName} -{a}" : "");
                        c.Append(Script.If($"I_CompareCounter isPlayer{r.CID} = 1", $"historic_event {r.CID}_EMIGRATION_{a}"));
                        HEGenerator.Add($"{r.CID}_EMIGRATION_{a}", $"Emigration in {r.RegionName}",
                            $"{r.RegionName} and especially the regional capital {r.CityName} are currently experiencing a wave of emigration - {a} citizen have left the region.", "@8");
                        c.Append($"\n\t\tend_if");
                    }
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
