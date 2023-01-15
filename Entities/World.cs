using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Ironclad.Entities
{
    static class World
    {
        public static List<Faction> Factions = new List<Faction>();
        public static List<Faction> PlayableFactions = new List<Faction>();
        public static List<Faction> PlayableFactionsOldWorld = new List<Faction>();
        public static List<Resource> Resources = new List<Resource>();
        public static List<string> Consumables = new List<string>();
        public static List<ResourcePool> ResourcePools = new List<ResourcePool>();
        public static List<BuildingChain> BuildingChains = new List<BuildingChain>();
        public static List<Building> Buildings = new List<Building>();
        public static List<HiddenResource> HiddenResources = new List<HiddenResource>();
        public static List<MercPool> MercPools = new List<MercPool>();
        public static List<Region> Regions = new List<Region>();
        public static List<RebelFaction> RebelFactions = new List<RebelFaction>();
        public static List<Ancillary> Ancillaries = new List<Ancillary>();
        public static List<Trait> Traits = new List<Trait>();
        public static List<Unit> Units = new List<Unit>();
        public static List<Entities.Trigger> Triggers = new List<Entities.Trigger>();
        public static List<Name> Names = new List<Name>();
        public static List<SettlementLevel> SettlementLevels = new List<SettlementLevel>();
        public static List<Loan> Loans = new List<Loan>();
        public static List<Position> AllPositions = new List<Position>();
        public static List<Event> Events = new List<Event>();
        public static IDictionary<string, int> ResourceOnMapCounts = new Dictionary<string, int>();

        internal static void ValidateObjects()
        {
            foreach (var f in Factions)
            {
                Hardcoded.ValidEffects.Add($"Combat_V_Faction_{f.ID.Title()}");
                Hardcoded.ValidEffects.Add($"Combat_V_Religion_{f.Religion}");
                IO.Val(Hardcoded.ValidCultures.Contains(f.Culture), $"Invalid Culture for {f.ID}: {f.Culture}");
                foreach (var c in f.InvasionTargets)
                    IO.Val(Regions.Select(a => a.CID).ToList().Contains(c) || c.Equals("NULL"), $"Invasion target {c} for {f.ID} but does not exist");
                foreach (var n in f.Neighbours)
                    IO.Val(Factions.Select(a => a.ID).ToList().Contains(n) || n.Equals("NULL"), $"Neighbour {n} for {f.ID} but does not exist");
                IO.Val(Regions.Select(a => a.CID).ToList().Contains(f.Capital) && f.Capital != "NULL", $"Invalid capital defined for {f.ID}: {f.Capital}");
                IO.Val(Names.Select(a => a.NameSet).ToList().Contains(f.NameSet) && f.NameSet != "NULL", $"Invalid NameSet defined for {f.ID}: {f.NameSet}");
            }
            foreach (var r in Regions)
            {
                IO.Val(Factions.Select(a => a.ID).ToList().Contains(r.HomeFaction) && r.HomeFaction != "NULL", $"Invalid HomeFaction for {r.RegionName}: {r.HomeFaction}");
                IO.Val(RebelFactions.Select(a => a.ID).ToList().Contains(r.Rebels) && r.Rebels != "NULL", $"Invalid Rebels for {r.RegionName}: {r.Rebels}");
                foreach (var hr in r.HiddenResources)
                    IO.Val(HiddenResources.Select(a => a.IntName).ToList().Contains(hr) || hr == "none", $"Invalid HiddenResource for {r.RegionName}: {hr}");
                IO.Val(ResourcePools.Select(a => a.Name).ToList().Contains(r.ResourcePool), $"Invalid ResourcePool for {r.RegionName}: {r.ResourcePool}");
                IO.Val(Factions.Select(a => a.ID).ToList().Contains(r.Owner), $"Invalid Owner for {r.RegionName}: {r.Owner}");
                IO.Val(r.StartLvl > -1 && r.StartLvl < 7, $"Invalid StartLvl for {r.RegionName}: {r.StartLvl}");
                foreach (var bd in r.Buildings)
                {
                    var rt = r.IsCastle ? "castle" : "city";
                    var vb = from b in Buildings
                             join bc in BuildingChains on b.Chain equals bc.ID
                             where (bc.SettlementType == rt || bc.SettlementType == "both") && b.MinSettlementLevel <= r.StartLvl
                             select b.ID;
                    IO.Val(vb.Contains(bd) || bd == "NULL" || bd.StartsWith("random_"), $"Invalid Building for {r.RegionName}: {bd}");
                }
                IO.Val(MercPools.Select(a => a.Name).ToList().Contains(r.MercPool), $"Invalid MercPool for {r.RegionName}: {r.MercPool}");
            }
            foreach (var mp in MercPools)
                IO.Val(Units.Select(a => a.IntName).ToList().Contains(mp.Unit), $"Invalid Unit for MercPool {mp.Name}: {mp.Unit}");
            foreach (var RebelFaction in RebelFactions)
            {
                foreach (var u in RebelFaction.Units)
                    IO.Val(Units.Where(a => a.Factions.Contains("slave")).Select(a => a.IntName).ToList().Contains(u), $"Invalid Unit for {RebelFaction.ID}: {u}");
                IO.Val(Hardcoded.ValidRebelFactionCategories.Contains(RebelFaction.Category), $"Invalid Unit for {RebelFaction.ID}: {RebelFaction.Category} (does not exist/not owned by slave)");
            }
            foreach (var n in Names)
                IO.Val(Hardcoded.ValidNameTypes.Contains(n.Type), $"Invalid Type for Name {n.Txt}: {n.Type}");
        }

        public static void LoadObjects(string Config)
        {
            IO.Log("Loading Objects");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Faction").Rows)
            {
                var f = new Faction($"{x[0]}".ToInt(), $"{x[1]}", $"{x[2]}", $"{x[3]}", $"{x[4]}", $"{x[5]}", $"{x[6]}", $"{x[7]}",
                    $"{x[8]}", $"{x[9]}", $"{x[10]}".ToInt(), $"{x[11]}".ToInt(), $"{x[12]}".ToInt(), $"{x[13]}".ToInt(),
                    $"{x[14]}".Split(",").ToList(), $"{x[15]}".ToInt(), $"{x[16]}", $"{x[17]}", $"{x[18]}", $"{x[19]}", $"{x[20]}",
                    $"{x[21]}", $"{x[22]}", $"{x[23]}", $"{x[24]}", $"{x[25]}", $"{x[26]}", $"{x[27]}", $"{x[28]}", $"{x[29]}",
                    $"{x[30]}", $"{x[31]}", $"{x[32]}", $"{x[33]}", $"{x[34]}");
                Factions.Add(f);
                if (f.ID != "slave")
                    PlayableFactions.Add(f);
                if (f.ID != "slave" && f.Culture != "mesoamerican")
                    PlayableFactionsOldWorld.Add(f);
            }
            IO.Log($"Loaded {Factions.Count} Factions");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Resource").Rows)
                Resources.Add(new Resource($"{x[0]}", $"{x[1]}", Convert.ToInt32(x[2]), $"{x[3]}", $"{x[4]}", $"{x[5]}".ToBool(), $"{x[6]}".ToBool(), $"{x[7]}", $"{x[8]}".ToInt(), $"{x[9]}".ToInt()));
            Validator.ValidateResources();
            IO.Log($"Loaded {Resources.Count} Resources");
            Consumables = Resources.Where(a => a.Consumable != "NULL").Select(a => a.Consumable.Substring(0, 2)).Distinct().OrderBy(a => a).ToList();
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "ResourcePool").Rows)
                ResourcePools.Add(new ResourcePool($"{x[0]}", $"{x[1]}", $"{x[2]}", $"{x[3]}", $"{x[4]}", $"{x[5]}", $"{x[6]}", $"{x[7]}", $"{x[8]}", $"{x[9]}", $"{x[10]}", $"{x[11]}", $"{x[12]}", $"{x[13]}", $"{x[14]}", $"{x[15]}", $"{x[16]}", $"{x[17]}", $"{x[18]}", $"{x[19]}", $"{x[20]}", $"{x[21]}", $"{x[22]}", $"{x[23]}", $"{x[24]}", $"{x[25]}", $"{x[26]}", $"{x[27]}"));
            IO.Log($"Loaded {ResourcePools.Count} ResourcePools");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "BuildingChain").Rows)
            {
                var c = new BuildingChain($"{x[0]}", $"{x[1]}", $"{x[2]}", $"{x[3]}", $"{x[4]}", $"{x[5]}".ToInt(), $"{x[6]}".ToInt(), $"{x[7]}", $"{x[8]}".ToBool(), $"{x[9]}".ToBool(), $"{x[10]}", $"{x[11]}".ToBool());
                BuildingChains.Add(c);
            }
            IO.Log($"Loaded {BuildingChains.Count} BuildingChains");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Building").Rows)
            {
                if (x[2] != "")
                {
                    var b = new Building($"{x[0]}", $"{x[1]}".ToInt(), $"{x[2]}", $"{x[3]}", $"{x[4]}", $"{x[5]}", $"{x[6]}".ToBool(), $"{x[7]}", $"{x[8]}", $"{x[9]}".ToInt(), $"{x[10]}", $"{x[11]}", $"{x[12]}", $"{x[13]}", $"{x[14]}", $"{x[15]}");
                    Buildings.Add(b);
                }
            }
            IO.Log($"Loaded {Buildings.Count} Buildings");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "HiddenResource").Rows)
                HiddenResources.Add(new HiddenResource(x[0].ToString()));
            IO.Log($"Loaded {HiddenResources.Count} HiddenResources");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "MercPool").Rows)
                MercPools.Add(new MercPool($"{x[0]}", $"{x[1]}", $"{x[2]}".ToInt(), $"{x[3]}".ToDec(), $"{x[4]}".ToDec(), $"{x[5]}".ToInt(), $"{x[6]}".ToInt()));
            IO.Log($"Loaded {MercPools.Count} MercPools");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Region").Rows)
            {
                var r = new Region($"{x[0]}".ToInt(), $"{x[1]}", $"{x[2]}", $"{x[3]}", $"{x[4]}", $"{x[5]}".ToInt(), $"{x[6]}".ToInt(), $"{x[7]}".ToInt(),
                    $"{x[8]}".Split(",").ToList(), $"{x[9]}", $"{x[10]}".ToInt(), $"{x[11]}".ToInt(), $"{x[12]}", $"{x[13]}".ToBool(), $"{x[14]}".ToInt(),
                    $"{x[15]}".ToInt(), $"{x[16]}".ToInt(), $"{x[17]}".ToBool(), $"{x[18]}".Rem(" ").Split(",").ToList(), $"{x[19]}", $"{x[20]}",
                    Rndm.Int($"{x[21]}".ToInt(), $"{x[22]}".ToInt()), $"{x[23]}".ToBool(), $"{x[24]}".ToBool(), $"{x[25]}", $"{x[26]}".ToBool(), $"{x[27]}", $"{x[28]}");
                Regions.Add(r);
            }
            foreach (var crusadeTarget in Rndm.Pick(Regions.Where(a => Factions.Where(a => a.Religion == "islam").Select(a => a.ID).Contains(a.Owner)).ToList(), 10))
                Regions.First(a => a.ID == crusadeTarget.ID).HiddenResources.Add("crusade");
            foreach (var jihadTarget in Rndm.Pick(Regions.Where(a => Factions.Where(a => a.Religion == "catholic").Select(a => a.ID).Contains(a.Owner)).ToList(), 10))
                Regions.First(a => a.ID == jihadTarget.ID).HiddenResources.Add("jihad");
            foreach (var region in Regions.Where(a => a.HiddenResources.Count > 1 && a.HiddenResources.First() == "none"))
                Regions.First(a => a.ID == region.ID).HiddenResources.RemoveAt(0);
            IO.Log("Assigned 10 crusade targets and 10 jihad targets.");
            IO.Val(Regions.Select(a => a.RID).Distinct().Count() == Regions.Select(a => a.RID).Count(), "There are duplicate region IDs! Check config.xlsx");
            foreach (var f in Factions)
            {
                try
                {
                    f.Capital = Regions.First(a => a.CityName.Equals(f.Capital)).CID;
                }
                catch
                {
                    IO.Log($"ERROR: Could not find settlement {f.Capital} as capital of {f.ID}");
                }
                IO.Val(Regions.Any(a => a.CID == f.Capital && a.Owner == f.ID), $"{f.ID} defined capital {f.Capital} which it does not own!");
            }
            foreach (var r in Resources)
                ResourceOnMapCounts.Add(r.ID, 0);
            foreach (var r in Resources.Where(a => a.Occurances > 0).OrderBy(a => a.Occurances)) // Limited resources
            {
                var retries = 0;
                while (ResourceOnMapCounts[r.ID] < r.Occurances && retries < 9)
                {
                    retries++;
                    var pr = Regions.Where(a => a.PossibleResources.Contains(r.ID) && !a.Resources.Select(a => a.ID).Contains(r.ID));
                    if (pr.Any())
                    {
                        var fpr = pr.Where(a => a.ResourceSlots > a.Resources.Count);
                        if (fpr.Any())
                        {
                            var reg = Rndm.Pick(fpr).RID;
                            Regions.First(a => a.RID == reg).Resources.Add(r);
                            ResourceOnMapCounts[r.ID]++;
                        }
                    }
                }
            }
            foreach (var r in Regions.Where(a => a.ResourceSlots > a.Resources.Count)) // Unlimited resources
            {
                var retries = 0;
                while (r.ResourceSlots > r.Resources.Count && retries < 9)
                {
                    retries++;
                    var res = Rndm.Pick(Resources.Where(a => a.Occurances == 0 && r.PossibleResources.Contains(a.ID)));
                    if (res != null)
                    {
                        Regions.First(a => a.RID == r.RID).Resources.Add(res);
                        ResourceOnMapCounts[res.ID]++;
                    }
                }
            }
            IO.Log($"Loaded {Regions.Count} Regions");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "RebelFaction").Rows)
                RebelFactions.Add(new RebelFaction($"{x[0]}", $"{x[1]}", $"{x[2]}", $"{x[3]}".ToInt(), $"{x[4]}".Split(",").ToList()));
            IO.Log($"Loaded {RebelFactions.Count} RebelFactions");
            if (Validator.isCampaignMapModified() || !File.Exists(Settings.cachePositionsAll) || !File.Exists(Settings.cachePositionsSettlements))
            {
                foreach (var x in 1.To(Map.Regions.Width - 1))
                    foreach (var y in 1.To(Map.Regions.Height - 1))
                    {
                        var p = new Position(x, y);
                        if (p.IsSea && !p.IsPort)
                        {
                            if (Rndm.Chance(50)) // Reduce cache size by reducing Sea positions
                                AllPositions.Add(new Position(x, y));
                        }
                        else
                        {
                            AllPositions.Add(new Position(x, y));
                        }
                    }
                File.Delete(Settings.cachePositionsAll);
                File.Delete(Settings.cachePositionsSettlements);
                using StreamWriter sw = File.AppendText(Settings.cachePositionsAll);
                foreach (var p in AllPositions)
                {
                    if (p.RegionID != 0 && Regions.First(a => a.ID == p.RegionID).IsUnreachable)
                    {
                        if (p.IsCity)
                            sw.WriteLine(p.SaveToFile());
                        if (!p.IsCity && Rndm.Chance(10))
                            sw.WriteLine(p.SaveToFile());
                    }
                    else
                    {
                        sw.WriteLine(p.SaveToFile());
                    }
                }
                IO.Log($"Loaded and cached {AllPositions.Count} Positions");
                foreach (var region in Regions)
                {
                    region.GetAutoPosition();
                    IO.Val(region.Position != null, $"Region without Positions: {region.RID}");
                }
                IO.Log($"Loaded and cached {Regions.Count} Settlement Positions");
            }
            else
            {
                var list = new List<string>();
                using (var sr = new StreamReader(Settings.cachePositionsAll))
                    while (!sr.EndOfStream)
                        AllPositions.Add(Position.LoadFromLine(sr.ReadLine()));
                IO.Log($"Loaded {AllPositions.Count} cached Positions");
                using (var sr = new StreamReader(Settings.cachePositionsSettlements))
                    while (!sr.EndOfStream)
                        list.Add(sr.ReadLine());
                foreach (var l in list)
                    Regions.First(a => a.CID == l.Split(";")[0]).Position = new Position(l.Split(";")[1].ToInt(), l.Split(";")[2].ToInt());
                IO.Log($"Loaded {Regions.Count} cached Settlement Positions");
            }
            if (Validator.isCampaignMapModified() || !File.Exists(Settings.cacheRegionNeighbours))
            {
                File.Delete(Settings.cacheRegionNeighbours);
                using StreamWriter sw = File.AppendText(Settings.cacheRegionNeighbours);
                {
                    var cnt = 0;
                    var ncnt = 0;
                    foreach (var r in Regions.Where(a => !a.IsUnreachable))
                    {
                        var nRegionIds = new List<int>();
                        foreach (var p in AllPositions.Where(a => a.RegionID == r.ID && a.X.IsBetween(2, Map.Regions.Width - 2) && a.Y.IsBetween(2, Map.Regions.Height - 2) && a.X % 2 == 0))
                            foreach (var offsX in new List<int>() { 0, 1, -1 })
                                foreach (var offsY in new List<int>() { 0, 1, -1 })
                                    if (!$"{offsX}{offsY}".Equals("00"))
                                    {
                                        try
                                        {
                                            var newId = GetPosition(p.X + offsX, p.Y + offsY).RegionID;
                                            if (newId != p.RegionID && newId != 0 && !nRegionIds.Contains(newId) && !Regions.First(a => a.ID == newId).IsUnreachable)
                                            {
                                                nRegionIds.Add(newId);
                                                r.NeighbourRegions.Add(Regions.First(a => a.ID == newId));
                                            }
                                        }
                                        catch
                                        {
                                            // do nothing
                                        }
                                    }
                        if (nRegionIds.Any())
                        {
                            sw.WriteLine($"{r.ID};{string.Join(",", nRegionIds)}");
                            IO.Log($"Loaded and cached Neighbours for Region: {r.RegionName}");
                            cnt++;
                            ncnt = ncnt + nRegionIds.Count;
                        }
                    }
                    IO.Log($"Loaded and cached {ncnt} Neighbours for {cnt} Regions");
                }
            }
            else
            {
                var cnt = 0;
                using (var sr = new StreamReader(Settings.cacheRegionNeighbours))
                    while (!sr.EndOfStream)
                    {
                        cnt++;
                        var line = sr.ReadLine();
                        var focusRegionId = line.Split(";")[0].ToInt();
                        var neighboursString = line.Split(";")[1];
                        var neighbourRegionIds = neighboursString.Contains(",") ? neighboursString.Split(",").ToList() : new List<string>() { neighboursString };
                        var r = Regions.First(a => a.ID == focusRegionId);
                        foreach (var neighbourRegionId in neighbourRegionIds)
                            r.NeighbourRegions.Add(Regions.First(a => a.ID == neighbourRegionId.ToInt()));
                    }
                IO.Log($"Loaded cached Neighbours for {cnt} Regions");
            }
            foreach (DataRow dr in Excel.GetDataTableFromExcel(Config, "Ancillary").Rows)
            {
                var effects = new List<Effect>();
                foreach (int i in Enumerable.Range(3, 10)) { effects.Add(new Effect(dr[i].ToString())); }
                var NewObject = new Ancillary(dr[0].ToString(), dr[1].ToString(), Convert.ToBoolean(dr[2]), effects, dr[13].ToString(), dr[14].ToString(), dr[15].ToString(), dr[16].ToString(), Convert.ToBoolean(dr[17]));
                Ancillaries.Add(NewObject);
            }
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Loan").Rows)
                if (Rndm.Chance(33)) // Reducing excessive amount of event strings
                    Loans.Add(new Loan(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]), Convert.ToInt32(x[2]), Convert.ToInt32(x[3]), Loans.Count()));
            IO.Log($"Loaded {Loans.Count} Loans");
            foreach (DataRow dr in Excel.GetDataTableFromExcel(Config, "Trait").Rows)
            {
                var effects = new List<Effect>();
                foreach (int i in Enumerable.Range(6, 10))
                { effects.Add(new Effect(dr[i].ToString())); }
                var NewObject = new Trait(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString().Split(",").ToList(), Convert.ToInt32(dr[5]), effects, Convert.ToInt32(dr[16]), dr[17].ToString(), dr[18].ToString(), dr[19].ToString(), dr[20].ToString());
                Traits.Add(NewObject);
            }
            IO.Log($"Loaded {Traits.Count} Traits");
            foreach (DataRow dr in Excel.GetDataTableFromExcel(Config, "Trigger").Rows)
            {
                var conditions = new List<String>() { dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString(), dr[8].ToString(), dr[9].ToString(), dr[10].ToString(), dr[11].ToString(), dr[12].ToString(), dr[13].ToString(), dr[14].ToString(), dr[15].ToString(), dr[16].ToString() };
                var effects = new List<Effect>() {new Effect(dr[17].ToString(), Convert.ToInt32(dr[18])), new Effect(dr[19].ToString(), Convert.ToInt32(dr[20])) , new Effect(dr[21].ToString(), Convert.ToInt32(dr[22])) , new Effect(dr[23].ToString(), Convert.ToInt32(dr[24])) , new Effect(dr[25].ToString(), Convert.ToInt32(dr[26])) ,
                    new Effect(dr[27].ToString(), Convert.ToInt32(dr[28])) , new Effect(dr[29].ToString(), Convert.ToInt32(dr[30])) , new Effect(dr[31].ToString(), Convert.ToInt32(dr[32])) , new Effect(dr[33].ToString(), Convert.ToInt32(dr[34])) , new Effect(dr[35].ToString(), Convert.ToInt32(dr[36]))};
                var NewObject = new Entities.Trigger(dr[0].ToString(), dr[1].ToString(), conditions, effects);
                Triggers.Add(NewObject);
            }
            IO.Log($"Loaded {Triggers.Count} Triggers");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Name").Rows)
                Names.Add(new Name($"{x[0]}", $"{x[1]}", $"{x[2]}", $"{x[3]}".ToInt()));
            IO.Log($"Loaded {Names.Count} Names");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "SettlementLevel").Rows)
                SettlementLevels.Add(new SettlementLevel($"{x[0]}".ToInt(), $"{x[1]}", $"{x[2]}", $"{x[3]}", $"{x[4]}".ToInt(), $"{x[5]}".ToInt()));
            IO.Log($"Loaded {SettlementLevels.Count} SettlementLevels");
            var unit = "";
            var dictName = "";
            var unitModelsString = "";
            var unitClass = "";
            var category = "";
            var isGeneral = false;
            var isNaval = false;
            var isMercenary = false;
            var isPeasant = false;
            var isCatapult = false;
            var ownership = new List<String>();
            var costMoney = 0;
            foreach (var line in File.ReadLines(Hardcoded.EXPORT_DESCR_UNIT))
            {
                if (!line.Trim().StartsWith(";"))
                {
                    string actualLine = line.Contains(";") ? line.Split(";")[0] : line;
                    if (actualLine.StartsWith("type"))
                    {
                        if (unit != "" && ownership.Count > 0)
                            Units.Add(new Unit(unit, dictName, Translator.TranslateCulturesAndFactionsToFactions(ownership, Factions), isGeneral, costMoney, isNaval, unitModelsString, isMercenary, isCatapult, unitClass, isPeasant, category));
                        unit = actualLine.Rem("type").Trim();
                    }
                    else if (actualLine.StartsWith("dictionary"))
                        dictName = actualLine.Rem("dictionary").Trim();
                    else if (actualLine.StartsWith("engine"))
                        isCatapult = actualLine.Rem("engine").Trim() == "catapult";
                    else if (actualLine.StartsWith("category"))
                        category = actualLine.Rem("category").Trim();
                    else if (actualLine.StartsWith("class"))
                        unitClass = actualLine.Rem("class").Trim();
                    else if (actualLine.StartsWith("attributes"))
                    {
                        isGeneral = actualLine.Contains("general_unit");
                        isMercenary = actualLine.Contains("mercenary_unit");
                        isPeasant = actualLine.Contains("is_peasant");
                    }
                    else if (actualLine.StartsWith("armour_ug_models"))
                        unitModelsString = actualLine.Rem("armour_ug_models").Trim();
                    else if (actualLine.StartsWith("ownership"))
                        ownership = actualLine.Rem("ownership").Trim().Split(",").Select(a => a.Trim()).ToList();
                    else if (actualLine.StartsWith("stat_cost"))
                        costMoney = Convert.ToInt32(actualLine.Rem("stat_cost").Trim().Split(",")[1].Trim());
                    isNaval = category == "ship";
                }
            }
            IO.Log($"Loaded {Units.Count} Units");
            foreach (DataRow x in Excel.GetDataTableFromExcel(Config, "Event").Rows)
                Events.Add(new Event($"{x[0]}", $"{x[1]}", $"{x[2]}", $"{x[3]}", $"{x[4]}".ToInt(), $"{x[5]}".ToBool(), $"{x[6]}", $"{x[7]}".ToBool(),
                    $"{x[8]}".ToBool(), $"{x[9]}", $"{x[10]}", $"{x[11]}", $"{x[12]}", $"{x[13]}".ToBool(), $"{x[14]}", $"{x[15]}", $"{x[16]}", $"{x[17]}", $"{x[18]}", $"{x[19]}"));
            IO.Log($"Loaded {Events.Count} Events");
            var tcnt = 0;
            foreach (var f in PlayableFactionsOldWorld)
            {
                var ts = new List<string>();
                foreach (var n in Factions.Where(a => f.Neighbours.Contains(a.ID)).ToList())
                    foreach (var target in Regions.Where(a => a.Owner == n.ID && !a.IsUnreachable).Select(a => a.CID).ToList())
                    {
                        if (f.InvasionTargets.Count < 2 || Rndm.Chance(33))
                        {
                            ts.Add(target);
                            tcnt++;
                        }
                    }
                f.InvasionTargets = ts;
                Factions.First(a => a.ID == f.ID).InvasionTargets = ts;
                PlayableFactions.First(a => a.ID == f.ID).InvasionTargets = ts;
            }
            IO.Log($"Assigned {tcnt} invasion targets for {PlayableFactionsOldWorld.Count} factions");
            //ROOM FOR TESTING
        }

        internal static Position GetNearCityPosition(string CID)
        {
            return Regions.First(a => a.CID == CID).ResourcePositions.First();
        }
        internal static string GetGeneralUnit(Faction faction)
        {
            return Units.First(a => a.Factions.Contains(faction.ID) && a.IsGeneral).IntName;
        }
        internal static Position GetPosition(int x, int y)
        {
            return AllPositions.First(a => a.X == x && a.Y == y);
        }
    }
}
