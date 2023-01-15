using Ironclad.Entities;
using Ironclad.Helper;
using ServiceStack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Informants
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Informants";
            if (Properties.Settings.Default.cbInformants || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event PreFactionTurnStart FactionIsLocal");
                c.Append($"\n\nand I_CompareCounter iok = 0");
                c.Append($"\n\nand RandomPercent < {Tuner.InformantsChancePerRound}");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                foreach (var f in World.PlayableFactions)
                    foreach (var pos in World.AllPositions.Where(a => a.IsPort))
                        foreach (var a in new List<string>() { "spy", "assassin", "diplomat", "merchant", "princess" })
                            c.Append(Script.IfOnPosition(f, a, pos, $"{Script.ZoomToPosition(pos, true)}\nset_counter iok 1", true));
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Append($"\nmonitor_event FactionTurnStart FactionIsLocal");
                c.Append($"\nand I_CompareCounter iok = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                var cnt = 0;
                c.Append($"\n\tgenerate_random_counter x 1 |cnt|");
                foreach (var r in World.Regions.Where(a => !a.IsNewWorld && !a.IsUnreachable && !a.ResourcePositions.IsEmpty()))
                {
                    cnt++;
                    c.Append($"\n\tif I_EventCounter x = {cnt}");
                    c.Append($"\n\t\tand I_CompareCounter isPlayer{r.CID} = 0");
                    c.Append($"\n\t\t\tgenerate_random_counter y 1 {Tuner.InformantsOffers.Count}");
                    foreach (var x in Tuner.InformantsOffers)
                    {
                        c.Append($"\n\t\t\tif I_EventCounter y = {x / 1000}");
                        HEGenerator.Add($"inf{cnt}{x}", "Pay for resource map?", $"Our agent contacted sailors in a foreign port willing to sell us a map leading to one of the following resource(s): {string.Join(", ", r.Resources.Select(a => a.Name))}.||Do you agree to pay them {x} florins?", "@2");
                        c.Append(Script.YesNoQuestion($"inf{cnt}{x}"));
                        c.Append($"\n\t\t\t\tif I_EventCounter inf{cnt}{x}_accepted = 1");
                        c.Append(Script.AddMoneyToPlayer(x * -1));
                        c.Append(Script.ZoomToPosition(Rndm.Pick(r.ResourcePositions)));
                        c.Append($"\n\t\t\t\tend_if");
                        c.Append($"\n\t\t\t\tif I_EventCounter inf{cnt}{x}_declined = 1");
                        c.Append($"\n\t\t\t\tend_if");
                        c.Append($"\n\t\t\tend_if");
                    }
                    c.Append($"\n\tend_if");
                }
                foreach (var r in World.Regions.Where(a => !a.IsNewWorld && !a.IsUnreachable))
                {
                    cnt++;
                    c.Append($"\n\tif I_EventCounter x = {cnt}");
                    c.Append($"\n\t\tand I_CompareCounter isPlayer{r.CID} = 0");
                    c.Append($"\n\t\t\tgenerate_random_counter y 1 {Tuner.InformantsOffers.Count}");
                    foreach (var x in Tuner.InformantsOffers)
                    {
                        c.Append($"\n\t\t\tif I_EventCounter y = {x / 1000}");
                        HEGenerator.Add($"inf{cnt}{x}", $"Pay for map of {r.RegionName}?", $"Our agent contacted sailors in a foreign port willing to sell us a map of {r.RegionName}.||Do you agree to pay them {x} florins?", "@2");
                        c.Append(Script.YesNoQuestion($"inf{cnt}{x}"));
                        c.Append($"\n\t\t\t\tif I_EventCounter inf{cnt}{x}_accepted = 1");
                        c.Append(Script.AddMoneyToPlayer(x * -1));
                        c.Append(Script.ZoomToPosition(r.Position));
                        foreach (var p in World.AllPositions.Where(a => a.RegionID == r.ID))
                            if (Rndm.Chance(33))
                                c.Append($"{Script.RevealPosition(p)}\n\tcampaign_wait 0.1");
                        c.Append($"\n\t\t\t\tend_if");
                        c.Append($"\n\t\t\t\tif I_EventCounter inf{cnt}{x}_declined = 1");
                        c.Append($"\n\t\t\t\tend_if");
                        c.Append($"\n\t\t\tend_if");
                    }
                    c.Append($"\n\tend_if");
                }
                c.Append($"\n\tset_counter iok 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                c.Replace("|cnt|", $"{cnt}");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
