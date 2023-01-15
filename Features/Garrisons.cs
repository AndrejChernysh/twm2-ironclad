using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Garrisons
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            {
                var isAlwaysActive = false;
                var scriptGroup = "Garrisons";
                if (Properties.Settings.Default.cbGarrisons || isAlwaysActive)
                {
                    c.Clear();
                    //AI Capital is attacked
                    c.Append("\nmonitor_event ButtonPressed ButtonPressed siege_assault_button");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    foreach (var f in World.PlayableFactions)
                    {
                        c.Append($"\nif I_SettlementUnderSiege {f.Capital}");
                        c.Append($"\n\tand I_SettlementOwner {f.Capital} = {f.ID}");
                        c.Append($"\n\tand I_CompareCounter caga{f.Order}Cooloff < 1");
                        c.Append($"\n\t\tlog always Capital Garrison {f.Capital}");
                        c.Append($"\n\t\tconsole_command add_population {f.Capital} {5 * -100}");
                        c.Append(Script.SpawnUnit(f.Capital, f.GetWeakestRangedUnit(), 5));
                        c.Append(Script.FireInCityOptical(f.Capital));
                        c.Append($"\n\t\tset_counter caga{f.Order}Cooloff 5");
                        c.Append($"\nend_if");
                    }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append("\nend_monitor");
                    c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    foreach (var r in World.Regions)
                    {
                        HEGenerator.Add($"mpg{r.CID}", $"Arm {r.CityName}s peasants?", $"{r.CityName} in {r.RegionName} is under siege!||" +
                            $"Do you mobilize the settlements peasants? If so, you will get an extra peasant unit each turn during the siege.||" +
                            $"However, this will reduce the number of working civilians there. Also, this will cost you 100 florins each turn.", "@27");
                        //AI Add unit each round
                        c.Append($"\n\tif I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\t\tand I_CompareCounter isPlayer{r.CID} = 0");
                        c.Append(Script.xl() ? $"\nlog always Garrison {r.CityName} +1" : "");
                        c.Append(Script.FireInCityOptical(r.CID));
                        c.Append($"\n\t\tconsole_command add_population {r.CID} -100");
                        foreach (var f in World.PlayableFactions)
                            c.Append(Script.IfOwner(r.CID, f.ID, Script.SpawnUnit(r.CID, f.GetWeakestRangedUnit())));
                        c.Append($"\n\tend_if");
                        //Player Add peasant unit each round if decided so
                        /*c.Append($"\n\tif I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                        c.Append($"\n\t\tand I_CompareCounter askedg{r.CID} = 0");
                        c.Append(Script.FireInCityOptical(r.CID));
                        c.Append(Script.YesNoQuestion($"mpg{r.CID}"));
                        c.Append(Script.If($"I_EventCounter mpg{r.CID}_accepted = 1", $"set_counter mpg{r.CID} 1\nset_counter askedg{r.CID} 1"));
                        c.Append(Script.If($"I_EventCounter mpg{r.CID}_declined = 1", $"set_counter askedg{r.CID} 1"));
                        c.Append($"\n\tend_if");
                        c.Append($"\n\tif I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                        c.Append($"\n\t\tand I_CompareCounter mpg{r.CID} = 1");
                        foreach (var f in World.PlayableFactions)
                            c.Append(Script.IfOwner(r.CID, f.ID, Script.SpawnUnit(r.CID, f.GetPeasant())));
                        c.Append(Script.xl() ? $"\nlog always Garrison {r.CityName} +1 Player" : "");
                        c.Append($"\n\t\tconsole_command add_population {r.CID} -200");
                        c.Append(Script.AddMoneyToPlayer(-100));
                        c.Append(Script.FireInCityOptical(r.CID));
                        c.Append($"\n\tend_if");
                        c.Append($"\n\tif ! I_SettlementUnderSiege {r.CID}");
                        c.Append($"\n\t\tand I_CompareCounter isPlayer{r.CID} = 1");
                        c.Append($"\n\t\t\tset_counter mpg{r.CID} 0");
                        c.Append($"\n\t\t\tset_counter askedg{r.CID} 0");
                        c.Append($"\n\tend_if");*/
                    }
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append("\nend_monitor");
                    return new Script(scriptGroup, c.ToString(), isAlwaysActive);
                }
                return new Script(scriptGroup, "", isAlwaysActive);
            }
        }
    }
}
