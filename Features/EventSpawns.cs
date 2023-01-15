using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class EventSpawns
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var order = 75;
            var isAlwaysActive = false;
            var scriptGroup = "EventSpawns";
            if (Properties.Settings.Default.cbEventSpawns || isAlwaysActive)
            {
                c.Clear();
                foreach (var se in World.Events.Where(a => a.SpawnFaction != "NULL"))
                {
                    var f = World.Factions.First(a => a.ID == se.SpawnFaction);
                    c.Append($"\nmonitor_event EventCounter EventCounterType {se.ID}");
                    c.Append($"\n\tand EventCounter > 0");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\tspawn_army");
                    var subfaction = f.ID != "slave" ? World.Factions.First(a => a.ID == f.ID) : World.Factions.First(a => a.NameSet == World.Names.First(a => a.Type == "characters" && a.ID == Convert.ToInt32(se.SpawnGeneralNameID)).NameSet);
                    var subfactionTxt = f.ID == "slave" ? $", sub_faction {subfaction.ID}" : "";
                    c.Append($"\n\t\tfaction {se.SpawnFaction}{subfactionTxt}");
                    var sl = Rndm.ArmyPositionInRegion(Rndm.Pick(se.SpawnRIDs));
                    var nid = se.SpawnGeneralNameID == "NULL" ? f.GetRandomFullName() : World.Names.First(a => a.ID == Convert.ToInt32(se.SpawnGeneralNameID) && (a.NameSet == subfaction.NameSet)).NID;
                    c.Append($"\n\t\tcharacter#{nid}, named character, age {Rndm.Int(20, 50)}, x {sl.X}, y {sl.Y}, family");
                    c.Append($"\n\t\ttraits GoodCommander {Rndm.Int(1, 3)}, NightBattleCapable 1, ReligionStarter 1, LoyaltyStarter 1, Loyal 2");
                    f = f.ID == "slave" ? subfaction : f;
                    c.Append($"\n\t\tunit#{f.GetGeneralsBodyguardUnit()}#exp {Rndm.Int(se.SpawnUnitsExpMin, se.SpawnUnitsExpMax)} armour {Rndm.Int(se.SpawnUnitsArmMin, se.SpawnUnitsArmMax)} weapon_lvl {Rndm.Int(se.SpawnUnitsArmMin, se.SpawnUnitsArmMax)}");
                    foreach (var i in 1.To(Rndm.Int(17, 19)))
                        c.Append($"\n\t\tunit#{Rndm.Pick(se.SpawnUnits)}#exp {Rndm.Int(se.SpawnUnitsExpMin, se.SpawnUnitsExpMax)} armour {Rndm.Int(se.SpawnUnitsArmMin, se.SpawnUnitsArmMax)} weapon_lvl {Rndm.Int(se.SpawnUnitsArmMin, se.SpawnUnitsArmMax)}");
                    c.Append($"\n\tend");
                    c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                    c.Append($"\n\tterminate_monitor");
                    c.Append($"\nend_monitor\n");
                }
                return new Script(scriptGroup, c.ToString(), isAlwaysActive, order);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
