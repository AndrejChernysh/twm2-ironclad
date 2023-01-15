using Ironclad.Extensions;
using Ironclad.Helper;
using Microsoft.CodeAnalysis.CSharp;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ServiceStack;
using ServiceStack.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ironclad.Entities
{
    class Script
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public bool IsAlwaysActive { get; set; }
        public int Order { get; set; }
        public decimal SizeMB { get; set; }
        public int MonitorsCount { get; set; }

        public Script(string extName, string code, bool isActive, int order = 50)
        {
            ID = extName;
            Code = $"\n{code}\n";
            IsAlwaysActive = isActive;
            Order = order;
            SizeMB = decimal.Round(((decimal)Encoding.Unicode.GetByteCount(code) / 1048576), 2);
            if (SizeMB > Settings.warnLimitSubscriptSizeMB)
                IO.Log($"WARNING: Large subscript: {extName} {SizeMB} MB");
            MonitorsCount = Regex.Matches(code, "end_monitor").Count;
            if (MonitorsCount > Settings.warnLimitMonitorsCount)
                IO.Log($"WARNING: Many monitors: {extName} {MonitorsCount} monitors");
            IO.Log($"Initialized Feature: {ID}");
        }

        internal static string SpawnFleet(Position p, Faction f, int shipsMin, int shipsMax, bool isOceanFaring)
        {
            var u = isOceanFaring ? "carrack" : "cog";
            u = f.Culture == "mesoamerican" ? "canoe" : u;
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\tspawn_army");
            c.Append($"\n\t\tfaction {f.ID}");
            c.Append($"\n\t\tcharacter#random_name, admiral, age 30, x {p.X}, y {p.Y}");
            foreach (var i in 1.To(Rndm.Int(shipsMin, shipsMax)))
                c.Append($"\n\t\tunit#{u}#exp 0 armour 0 weapon_lvl 0");
            c.Append($"\n\t\tend");
            return c.ToString();
        }
        public static string DecreaseCounterIfGreaterZero(string counter)
        {
            return $"\n\tif I_CompareCounter {counter} > 0\n\t\tinc_counter {counter} -1\n\tend_if";
        }
        public static string TerminateIfAI(string factionIntNameVariable)
        {
            return $"\n\t\tif I_IsFactionAIControlled {factionIntNameVariable}\n\t\t\tterminate_monitor\n\t\tend_if";
        }
        public static string TerminateIfPlayer(string factionIntNameVariable)
        {
            return $"\n\t\tif ! I_IsFactionAIControlled {factionIntNameVariable}\n\t\t\tterminate_monitor\n\t\tend_if";
        }
        public static string YesNoQuestion(string ev)
        {
            return $"\n\t\t\thistoric_event {ev} true\n\t\t\twhile I_EventCounter {ev}_accepted = 0\n\t\t\t\tand I_EventCounter {ev}_declined = 0\n\t\t\tend_while";
        }
        public static string SetTreasury(string factionIntName, int money)
        {
            return $"\n\t\tadd_money {factionIntName} -2147483647\n\t\tadd_money {factionIntName} 2147483647\n\t\tadd_money {factionIntName} {money}";
        }
        public static string SetFactionStanding(string factionIntName1, string factionIntName2, double standing)
        {
            return $"\n\t\t\tset_faction_standing {factionIntName1} {factionIntName2} {standing}\n\t\t\tset_faction_standing {factionIntName2} {factionIntName1} {standing}";
        }
        public static string SetDiplomaticStance(string factionID1, string factionID2, string stance)
        {
            StringBuilder c = new StringBuilder();
            var f1 = World.Factions.First(a => a.ID == factionID1);
            var f2 = World.Factions.First(a => a.ID == factionID2);
            c.Append($"\n\tconsole_command diplomatic_stance {factionID1} {factionID2} {stance}");
            c.Append($"\n\tconsole_command diplomatic_stance {factionID2} {factionID1} {stance}");
            if (stance == "war")
            {
                c.Append($"\n\tset_counter {GetIsWarCounter(f1, f2)} 1");
                c.Append($"\n\tset_counter {GetIsAllyCounter(f1, f2)} 0");
            }
            if (stance == "neutral")
            {
                c.Append($"\n\tset_counter {GetIsWarCounter(f1, f2)} 0");
                c.Append($"\n\tset_counter {GetIsAllyCounter(f1, f2)} 0");
            }
            if (stance == "allied")
            {
                c.Append($"\n\tset_counter {GetIsWarCounter(f1, f2)} 0");
                c.Append($"\n\tset_counter {GetIsAllyCounter(f1, f2)} 1");
            }
            return c.ToString();
        }
        public static string GetIsWarCounter(Faction faction1, Faction faction2)
        {
            return faction1.Order < faction2.Order ? $"{faction1.Order}iw{faction2.Order}" : $"{faction2.Order}iw{faction1.Order}";
        }
        public static string GetIsAllyCounter(Faction faction1, Faction faction2)
        {
            return faction1.Order < faction2.Order ? $"{faction1.Order}ia{faction2.Order}" : $"{faction2.Order}ia{faction1.Order}";
        }
        public static string SetFactionStandingToPlayer(string factionID, double standing)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.PlayableFactions)
            {
                c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                c.Append($"\n\t\tset_faction_standing {f.ID} {factionID} {standing}");
                c.Append($"\n\t\tset_faction_standing {factionID} {f.ID} {standing}");
                c.Append($"\n\tend_if");
            }
            return c.ToString();
        }
        public static bool xl() //is extended logging active?
        {
            return Properties.Settings.Default.cbLogAll;
        }
        public static string SetDiplomaticStanceToPlayer(string factionID, string stance)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.PlayableFactions)
            {
                c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                c.Append($"\n\t\tconsole_command diplomatic_stance {f.ID} {factionID} {stance}");
                c.Append($"\n\t\tconsole_command diplomatic_stance {factionID} {f.ID} {stance}");
                c.Append($"\n\tend_if");
            }
            return c.ToString();
        }
        public static string ClickOn(string UIElementName)
        {
            return $"\n\tselect_ui_element {UIElementName}\n\tsimulate_mouse_click lclick_down\n\tsimulate_mouse_click lclick_up";
        }
        public static string DamageBuilding(string CID, string chain, int damage, int chance = 100)
        {
            if (chance == 100)
                return $"\n\tconsole_command set_building_health {CID} {chain} {100 - damage}";
            return IfChance(chance, $"\n\tconsole_command set_building_health {CID} {chain} {100 - damage}");
        }
        public static string AddMoneyToPlayer(int money)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\tset_counter AMP {money}");
            return c.ToString();
        }
        public static string DestroyUnitsFromPlayer(string unit)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.Factions)
            {
                c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                c.Append($"\n\t\tdestroy_units {f.ID} {unit}");
                c.Append($"\n\tend_if");
            }
            return c.ToString();
        }
        public static string DestroyChainsFromPlayer(string chain)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.PlayableFactions)
            {
                c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                c.Append($"\n\t\tdestroy_buildings {f.ID} {chain} false");
                c.Append($"\n\tend_if");
            }
            return c.ToString();
        }
        public static string RemoveAgentsFromPlayer(string agent)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.Factions)
            {
                c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                c.Append($"\n\t\tretire_characters {f.ID} {agent}");
                c.Append($"\n\tend_if");
            }
            return c.ToString();
        }
        public static string AddKingsPurseToPlayer(int money)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.Factions)
            {
                c.Append($"\n\tif ! I_IsFactionAIControlled {f.ID}");
                c.Append($"\n\t\tincrement_kings_purse {f.ID} {money}");
                c.Append($"\n\tend_if");
            }
            return c.ToString();
        }

        public static string AddMoneyToSettlementOwner(int money, string CID)
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.Factions)
            {
                c.Append($"\n\t\t\t\tif I_SettlementOwner {CID} = {f.ID}");
                c.Append($"\n\t\t\t\t\tadd_money {f.ID} {money}");
                c.Append($"\n\t\t\t\tend_if");
            }
            return c.ToString();
        }

        internal static string IsGeneral()
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\tand IsGeneral");
            c.Append($"\n\tand Trait GeneralPush = 1");
            return c.ToString();
        }
        internal static string AlterRecruitPoolUnits(string RID, int change, bool is50PercentChance = false)
        {
            if (is50PercentChance)
                return $"\n\t\t\tset_counter ARPU{RID.Replace("C", "R")}50 {change}\n\t\t\tset_counter ARPUactivate 1";
            return $"\n\t\t\tset_counter ARPU{RID.Replace("C", "R")} {change}\n\t\t\tset_counter ARPUactivate 1";
        }
        internal static string SpawnAgent(string agentType, Region region, string factionID, int amount = 1)
        {
            StringBuilder c = new StringBuilder();
            foreach (var p in region.GetAccessiblePositions(amount))
                c.Append($"\n\tspawn_character {factionID} random_name, {agentType}, age 30, x {p.X}, y {p.Y}");
            return c.ToString();
        }
        internal static string CityRevolt(string CID)
        {
            return $"\n\t\t\tset_counter R{CID} 1\n\t\t\tset_counter Ractivate 1";
        }
        internal static string DamageAllBuildings(string CID, bool damageAllCompletely = true)
        {
            if (damageAllCompletely)
                return $"\n\t\tset_counter DAB{CID} 2\n\t\tset_counter DABactivate 1";
            return $"\n\t\tset_counter DAB{CID} 1\n\t\tset_counter DABactivate 1";
        }
        internal static string IfChance(int chance, string command)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\t\t\tif RandomPercent < {chance}");
            c.Append($"\n\t\t\t\t\t{command}");
            c.Append($"\n\t\t\t\tend_if");
            return c.ToString();
        }
        internal static string RevealPosition(Position pos)
        {
            return $"\n\treveal_tile {pos.X}, {pos.Y}\n\thide_all_revealed_tiles";
        }
        internal static string ZoomToPosition(Position pos, bool quick = false)
        {
            StringBuilder c = new StringBuilder();
            c.Append(quick ? $"\n\tsnap_strat_camera {pos.X}, {pos.Y}" : $"\n\tmove_strat_camera {pos.X}, {pos.Y}");
            c.Append($"\n\tzoom_strat_camera 0.1");
            c.Append($"\n\treveal_tile {pos.X}, {pos.Y}");
            c.Append($"\n\thide_all_revealed_tiles");
            return c.ToString();
        }
        internal static string CameraToPosition(Position pos, bool quick = false)
        {
            StringBuilder c = new StringBuilder();
            c.Append(quick ? $"\n\tsnap_strat_camera {pos.X}, {pos.Y}" : $"\n\tmove_strat_camera {pos.X}, {pos.Y}");
            c.Append($"\n\treveal_tile {pos.X}, {pos.Y}");
            c.Append($"\n\thide_all_revealed_tiles");
            return c.ToString();
        }
        internal static string ZoomToCity(Region r)
        {
            return ZoomToPosition(World.Regions.First(a => a.CID == r.CID).Position);
        }
        internal static string ZoomToCID(string c)
        {
            return ZoomToPosition(World.Regions.First(a => a.CID == c.Replace("R", "C")).Position);
        }
        internal static string ZoomToPort(Region r)
        {
            if (r.HasPort && World.AllPositions.Any(a => a.RegionID == r.ID && a.IsPort))
                return ZoomToPosition(World.AllPositions.First(a => a.RegionID == r.ID && a.IsPort));
            return "";
        }
        internal static string TransferMoneyFromTo(string factionFrom, string factionTo, int money)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\t\tadd_money {factionFrom} {money * -1}");
            c.Append($"\n\t\t\tadd_money {factionTo} {money}");
            return c.ToString();
        }
        internal static string If(string condition, string command)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\tif {condition}");
            c.Append($"\n\t\t\t{command}");
            c.Append($"\n\t\tend_if");
            return c.ToString();
        }
        internal static string IfOnPosition(Faction f, string characterType, Position pos, string command, bool playerOnly = false)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\tif I_CharacterTypeNearTile {f.ID} {characterType}, 0 {pos.X}, {pos.Y}");
            c.Append(playerOnly ? $"\n\t\tand ! I_IsFactionAIControlled {f.ID}" : "");
            c.Append($"\n\t\t\t{command}");
            c.Append($"\n\tend_if");
            return c.ToString();
        }
        internal static string IfOnPosition(string factionID, string characterType, Position pos, string command, bool playerOnly = false)
        {
            return IfOnPosition(World.Factions.First(a => a.ID == factionID), characterType, pos, command, playerOnly);
        }
        internal static string IfCounter(string counter, int value, string command)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\tif I_CompareCounter {counter} = {value}");
            c.Append($"\n\t\t\t{command}");
            c.Append($"\n\t\tend_if");
            return c.ToString();
        }
        internal static string IfOwner(string CID, string factionID, string command)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\tif I_SettlementOwner {CID} = {factionID}");
            c.Append($"\n\t\t\t{command}");
            c.Append($"\n\t\tend_if");
            return c.ToString();
        }
        internal static string SpawnUnit(string CID, string unitID, int amount = 1, int exp = 0, int armour = 0, int weapon = 0)
        {
            StringBuilder c = new StringBuilder();
            foreach (var i in 1.To(amount))
                c.Append($"\n\tcreate_unit {CID}, {unitID}, num 1, exp {exp}, arm {armour}, wep {weapon}");
            return c.ToString();
        }
        private static string AddEvent(string eventType, Position pos, int delayYears = 0, bool posDeviation = false)
        {
            if (!Settings.enableOpticalEvents && eventType == "fire")
                return "";
            StringBuilder c = new StringBuilder();
            //if (eventType == "fire")
            //c.Append($"\n\tcampaign_wait {Rndm.Int(1, 9)}.5");
            c.Append($"\n\tadd_events");
            c.Append($"\n\t\tevent {eventType}");
            c.Append($"\n\t\tdate {delayYears}");
            if (posDeviation && pos.X.IsBetween(3, Map.Width - 3) && pos.Y.IsBetween(3, Map.Height - 3))
                c.Append($"\n\t\tposition {Rndm.Int(pos.X - 1, pos.X + 1)}, {Rndm.Int(pos.Y - 1, pos.Y + 1)}");
            else
                c.Append($"\n\t\tposition {pos.X}, {pos.Y}");
            c.Append($"\n\tend_add_events");
            return c.ToString();
        }
        internal static string StormInPosition(Position position)
        {
            return AddEvent("storm", position);
        }
        internal static string StormInRegion(string RID, int intensity = 4)
        {
            RID = RID.Replace("C", "R");
            var region = World.Regions.First(a => a.RID == RID);
            var positions = region.GetAccessiblePositions(intensity);
            var c = new StringBuilder();
            foreach (var p in positions)
                c.Append(StormInPosition(p));
            return c.ToString();
        }
        internal static string StormInCity(string CID)
        {
            CID = CID.Replace("R", "C");
            return AddEvent("storm", World.Regions.First(a => a.CID == CID).Position);
        }
        internal static string PlagueInCity(string CID, bool withMessage = false)
        {
            var r = World.Regions.First(a => a.CID == CID);
            CID = CID.Replace("R", "C");
            if (withMessage)
            {
                var e = $"p{CID}{Rndm.Int(0, 99999)}";
                HEGenerator.Add(e, $"Plague in {r.CityName}", $"A plague has hit {r.CityName} in {r.RegionName}, leading to many deaths and suffering.");
                return $"{AddEvent("plague", World.Regions.First(a => a.CID == CID).Position)}\n\thistoric_event {e}";
            }
            return AddEvent("plague", World.Regions.First(a => a.CID == CID).Position);
        }
        internal static string FireInCityOptical(string CID)
        {
            CID = CID.Replace("R", "C");
            return AddEvent("fire", World.Regions.First(a => a.CID == CID).Position);
        }
        internal static string FireInCityOpticalAndEffect(string CID)
        {
            CID = CID.Replace("R", "C");
            var r = World.Regions.First(a => a.CID == CID);
            return $"{FireInCityOptical(CID)}\n\t{AddEvent("earthquake", r.Position)}";
        }
        internal static string FireInPositionOptical(Position position, int intensity = 4)
        {
            var c = new StringBuilder();
            foreach (var i in 1.To(intensity))
                c.Append(AddEvent("fire", position, 0, true));
            return c.ToString();
        }
        internal static string FireInRegionOptical(string RID, int intensity = 4)
        {
            RID = RID.Replace("C", "R");
            var region = World.Regions.First(a => a.RID == RID);
            var positions = region.GetAccessiblePositions(intensity);
            var c = new StringBuilder();
            foreach (var p in positions)
                c.Append(FireInPositionOptical(p));
            return c.ToString();
        }
        internal static string AttackCity(Faction f, string CID, int minUnits, int maxUnits)
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n\t\t\t\tspawn_army");
            var sf = f.ID != "slave" ? World.Factions.First(a => a.ID == f.ID) : World.Factions.First(a => a.ID == World.Regions.First(a => a.CID == CID).HomeFaction);
            var subfactionTxt = f.ID == "slave" ? $", sub_faction {sf.ID}" : "";
            c.Append($"\n\t\t\t\tfaction {f.ID}{subfactionTxt}");
            if (f.ID != "slave")
                c.Append($"\n\t\t\t\tcharacter random_name, named character, age 30, x {World.GetNearCityPosition(CID).X}, y {World.GetNearCityPosition(CID).Y}, family, label ic{f.Order}{CID}");
            else
                c.Append($"\n\t\t\t\tcharacter random_name, named character, age 30, x {World.GetNearCityPosition(CID).X}, y {World.GetNearCityPosition(CID).Y}, label ic{f.Order}{CID}");
            c.Append($"\n\t\t\t\ttraits ReligionStarter 1, LoyaltyStarter 1, GoodCommander 3, Loyal 2, Just 2, NightBattleCapable 1");
            if (f.ID != "slave")
            {
                c.Append($"\n\t\t\t\tunit#{World.GetGeneralUnit(f)}#exp {Rndm.Int(3, 6)} armour {Rndm.Int(0, 1)} weapon_lvl {Rndm.Int(0, 1)}");
                c.Append($"\n\t\t\t\tunit#{f.GetCatapult()}#exp {Rndm.Int(3, 6)} armour 0 weapon_lvl 0");
                c.Append($"\n\t\t\t\tunit#{f.GetCatapult()}#exp {Rndm.Int(3, 6)} armour 0 weapon_lvl 0");
            }
            else
            {
                c.Append($"\n\t\t\t\tunit#{World.GetGeneralUnit(sf)}#exp {Rndm.Int(3, 6)} armour {Rndm.Int(0, 1)} weapon_lvl {Rndm.Int(0, 1)}");
                c.Append($"\n\t\t\t\tunit#{sf.GetCatapult()}#exp {Rndm.Int(3, 6)} armour 0 weapon_lvl 0");
                c.Append($"\n\t\t\t\tunit#{sf.GetCatapult()}#exp {Rndm.Int(3, 6)} armour 0 weapon_lvl 0");
            }
            if (maxUnits > 17)
                maxUnits = 17;
            if (f.ID == "slave")
            {
                foreach (var i in 1.To(Rndm.Int(minUnits, maxUnits)))
                    c.Append($"\n\t\t\t\tunit#{f.GetSlaveUnitMaxCost(399, sf)}#exp {Rndm.Int(3, 6)} armour {Rndm.Int(0, 1)} weapon_lvl {Rndm.Int(0, 1)}");
            }
            else
            {
                foreach (var i in 1.To(Rndm.Int(minUnits, maxUnits)))
                    c.Append($"\n\t\t\t\tunit#{f.GetUnitMaxCost(399)}#exp {Rndm.Int(3, 6)} armour {Rndm.Int(0, 1)} weapon_lvl {Rndm.Int(0, 1)}");
            }
            c.Append($"\n\t\t\t\tend");
            c.Append($"\n\t\t\t\tsiege_settlement ic{f.Order}{CID}, {CID}, attack");
            c.Append(Script.FireInCityOptical(CID));
            c.Append(Script.ClickOn("prebattle_fight_button"));
            c.Append(Script.xl() ? $"\n\nlog always Gave {CID} to {f.ID}" : "");
            return c.ToString();
        }
        internal static string SpawnRebelArmy(Position position, Faction subfaction, int minUnits, int maxUnits, int maxUnitCost, Region cityToAttack=null)
        {
            StringBuilder c = new StringBuilder();
            var slaveFaction = World.Factions.First(a => a.ID == "slave");
            c.Append($"\n\t\t\t\tspawn_army");
            c.Append($"\n\t\t\t\tfaction slave, sub_faction {subfaction.ID}");
            c.Append($"\n\t\t\t\tcharacter random_name, named character, age 30, x {position.X}, y {position.Y}, family, label sra{subfaction.Order}x{position.X}y{position.Y}");
            c.Append($"\n\t\t\t\ttraits ReligionStarter 1, LoyaltyStarter 1, GoodCommander {Rndm.Int(1, 3)}, NightBattleCapable 1");
            c.Append($"\n\t\t\t\tunit#{World.GetGeneralUnit(slaveFaction)}#exp 0 armour 0 weapon_lvl 0");
            c.Append($"\n\t\t\t\tunit#NE Ballista#exp 0 armour 0 weapon_lvl 0");
            c.Append($"\n\t\t\t\tunit#NE Catapult#exp 0 armour 0 weapon_lvl 0");
            if (maxUnits > 17)
                maxUnits = 17;
            if (minUnits >= maxUnits)
                minUnits = maxUnits - 1;
            foreach (var i in 1.To(Rndm.Int(minUnits, maxUnits)))
                c.Append($"\n\t\t\t\tunit#{slaveFaction.GetSlaveUnitMaxCost(maxUnitCost, subfaction)}#exp 0 armour 0 weapon_lvl 0");
            c.Append($"\n\t\t\t\tend");
            if (cityToAttack != null)
                c.Append(Script.IfChance(50, $"\nsiege_settlement sra{subfaction.Order}x{position.X}y{position.Y}, {cityToAttack.CID}, attack"));
            c.Append(Script.FireInPositionOptical(position));
            return c.ToString();
        }
    }
}
