using Ironclad.Entities;
using Ironclad.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class Colonies
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "Colonies";
            if (Properties.Settings.Default.cbColonies || isAlwaysActive)
            {
                c.Clear();
                c.Append($"\nmonitor_event FactionTurnStart FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append(Script.DestroyChainsFromPlayer("hinterland_colony"));
                c.Append($"\n\tset_counter ocpt 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append($"\nend_monitor");
                foreach (var b in new List<string>() { "c_port_6", "port_6" })
                    foreach (var fAI in World.PlayableFactionsOldWorld)
                    {
                        c.Append($"\nmonitor_event FactionTurnStart FactionType {fAI.ID}");
                        c.Append($"\n\tand FactionBuildingExists >= {b}");
                        c.Append($"\n\tand I_CompareCounter col{fAI.Order}CoolOff = 0");
                        c.Append($"\n\tand I_CompareCounter ocpt = 0");
                        c.Append(Script.TerminateIfPlayer(fAI.ID));
                        c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                        var colonies = World.Regions.Where(a => a.IsNewWorld && !a.IsUnreachable && a.HasPort);
                        c.Append($"\n\tgenerate_random_counter x 1 {colonies.Count()}");
                        foreach (var (r, i) in colonies.Select((v, i) => (v, i)).ToList())
                        {
                            c.Append($"\n\tif I_EventCounter x = {i + 1}");
                            c.Append($"\n\t\tand I_SettlementOwner {r.CID} = slave");
                            c.Append($"\n\t\tand ! I_SettlementUnderSiege {r.CID}");
                            c.Append($"\n\t\tset_counter ocpt 1");
                            c.Append($"\n\t\tset_counter col{fAI.Order}CoolOff 20");
                            c.Append($"\n\t\tconsole_command create_building {r.CID} colony_fort");
                            c.Append(Script.SpawnFleet(Rndm.GetFleetPositionNearRegion(r), fAI, 2, 4, true));
                            c.Append(Script.AttackCity(fAI, r.CID, 15, 19));
                            HEGenerator.Add($"new_world_1_{fAI.ID}", "New World Discovered", $"Exploration is a very dangerous business. Superstitions persisted about what lay beyond Africas Cape Bojador, as no European had even seen the west coast of Africa beyond the Sahara. There were no maps or charts and very little knowledge of winds or currents.||Testing the theory that one can head east by sailing west carried the risk of a remote, watery grave. Yet this act of courage has been rewarded to The {fAI.Name} with the discovery of a strange new land... perchance a whole new world. Who knows what riches await those with the courage to explore.");
                            HEGenerator.Add($"new_world_2_{fAI.ID}", "Natives Contacted", $"News from The {fAI.Name} in the New World - not only many primitive tribes but also whole Empires of Natives have been discovered.||For a seemlingly simple people, their cities are vast and complicated. Their primitive outlook belies their ability to raise vast stone structures that rival anything the old world has to offer. And if gold is the measure of an Empires might, then these Natives are formidable indeed.");
                            HEGenerator.Add($"new_world_3_{fAI.ID}", "Colonisation continues", $"The old world is afire with tales from the New World. Rumours of cities made of gold and people covered in jewels, draws others to this strange new land. Even now, foreign fleets make their way across the sea.||The disovery and colonisation efforts are being led by The {fAI.Name}.");
                            HEGenerator.Add($"new_world_4_{fAI.ID}", "Native resistance", $"The elders of many Native Tribes of the New World feel that the time have declared a Warpath, calling all Natives to resist colonisation efforts, especially by The {fAI.Name}.");
                            HEGenerator.Add($"new_world_5_{fAI.ID}", "Colonisation becomes Conquest", $"Ever hungry for more gold and fresh conquests, Colonisers from the Old World send more expeditions to the New World. Wherever the cities of gold lie, these men, God willing, will find them.||Many years have passed since the New World has been discovered - as of today, The {fAI.Name} are the latest faction to have added more lands of the New World into their dominions.");
                            c.Append(Script.IfCounter("colCount", 4, $"historic_event new_world_5_{fAI.ID} event/new_world_5.bik\n\t\tset_counter colCount 5"));
                            c.Append(Script.IfCounter("colCount", 3, $"historic_event new_world_4_{fAI.ID} event/new_world_4.bik\n\t\tset_counter colCount 4"));
                            c.Append(Script.IfCounter("colCount", 2, $"historic_event new_world_3_{fAI.ID} event/new_world_3.bik\n\t\tset_counter colCount 3"));
                            c.Append(Script.IfCounter("colCount", 1, $"historic_event new_world_2_{fAI.ID} event/new_world_2.bik\n\t\tset_counter colCount 2"));
                            c.Append(Script.IfCounter("colCount", 0, $"historic_event new_world_1_{fAI.ID} event/new_world_1.bik\n\t\tset_counter colCount 1"));
                            c.Append($"\n\tend_if");
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
