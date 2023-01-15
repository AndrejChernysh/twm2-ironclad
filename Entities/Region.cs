using Ironclad.Helper;
using Microsoft.VisualBasic;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ironclad.Entities
{
    class Region
    {
        public int ID { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string HomeFaction { get; set; }
        public string Rebels { get; set; }
        public int RGB_R { get; set; }
        public int RGB_G { get; set; }
        public int RGB_B { get; set; }
        public List<string> HiddenResources { get; set; }
        public string ResourcePool { get; set; }
        public Position Position { get; set; }
        public List<Position> ResourcePositions { get; set; }
        public int FertilityMin { get; set; }
        public int FertilityMax { get; set; }
        public string Owner { get; set; }
        public bool HasPort { get; set; }
        public int StartLvl { get; set; }
        public int StartPopMin { get; set; }
        public int StartPopMax { get; set; }
        public bool IsCastle { get; set; }
        public List<string> Buildings { get; set; }
        public string MercPool { get; set; }
        public string RID { get; set; }
        public string CID { get; set; }
        public string MusicType { get; set; }
        public int ResourceSlots { get; set; }
        public List<Resource> Resources { get; set; }
        public List<string> PossibleResources { get; set; }
        public List<Region> NeighbourRegions { get; set; }
        public bool IsNewWorld { get; set; }
        public bool IsUnreachable { get; set; }
        public string Text { get; set; }
        public bool IsDifficult { get; set; }
        public string Unification { get; set; }
        public string CityNameColonized { get; set; }


        public Region(int iD, string cityName, string regionName, string homeFaction, string rebels, int rGB_R, int rGB_G, int rGB_B, List<string> hiddenResources, string resourcePool, int fertilityMin, int fertilityMax, string owner, bool hasPort, int startLvl, int startPopMin, int startPopMax, bool isCastle, List<string> buildings, string mercPool, string musicType, int resourcesCount, bool isNewWorld, bool isUnreachable, string text, bool isDifficult, string unification, string cityNameColonized)
        {
            ID = iD;
            CityName = cityName;
            RegionName = regionName;
            CityNameColonized = cityNameColonized.EqualsIgnoreCase("NULL") ? null : cityNameColonized;
            HomeFaction = homeFaction;
            Rebels = rebels;
            RGB_R = rGB_R;
            RGB_G = rGB_G;
            RGB_B = rGB_B;
            HiddenResources = hiddenResources.Select(a => a.Trim()).ToList();
            if (isNewWorld || IsUnreachable)
            {
                HiddenResources.Add("america");
                HiddenResources.Add("no_pirates");
                HiddenResources.Add("no_brigands");
            }
            ResourcePool = resourcePool;
            FertilityMin = fertilityMin;
            FertilityMax = fertilityMax;
            Owner = owner;
            HasPort = hasPort;
            StartLvl = startLvl;
            if (Validator.FactionMaxCityLevel(homeFaction) < startLvl)
                StartLvl = Validator.FactionMaxCityLevel(homeFaction);
            StartPopMin = startPopMin;
            StartPopMax = startPopMax;
            IsCastle = isCastle;
            if (homeFaction == "aztecs" || homeFaction == "apachean")
                IsCastle = false;
            Buildings = buildings;
            MercPool = mercPool;
            RID = Translator.TranslateCityID(iD).Replace("C", "R");
            CID = Translator.TranslateCityID(iD);
            MusicType = musicType;
            ResourceSlots = resourcesCount;
            Resources = new List<Resource>() { };
            PossibleResources = World.ResourcePools.First(a => a.Name == ResourcePool).getResourceIDs();
            NeighbourRegions = new List<Region>() { };
            IsNewWorld = isNewWorld;
            IsUnreachable = isUnreachable;
            Text = text == "NULL" ? "" : text.Replace("CITY", CityName).Replace("REGION", RegionName);
            IsDifficult = isDifficult;
            Unification = unification == "NULL" ? null : unification;
        }
        internal void GetAutoPosition()
        {
            var offsets = new List<int> {1, 0, -1};
            foreach (var p in World.AllPositions.Where(a => a.RegionID == ID && !a.IsCity && a.IsAccessible).ToList())
            {
                foreach (var offX in offsets)
                    foreach (var offY in offsets)
                    {
                        try
                        {
                            if (World.GetPosition(p.X + offX, p.Y + offY).IsCity)
                            {
                                Position = new Position(p.X + offX, p.Y + offY);
                                using StreamWriter sw = File.AppendText(Settings.cachePositionsSettlements);
                                    sw.WriteLine($"{CID};{Position.X};{Position.Y}");
                                return;
                            }
                        }
                        catch
                        {
                            // Nothing should be done
                        }
                    }
            }
        }

        public List<Position> GetAccessiblePositions(int count)
        {
            var positions = World.AllPositions.Where(a => a.RegionID == ID && a.IsSuitableForResource && !a.IsCity && !a.IsPort).ToList();
            if (!positions.Any())
                count = 0;
            if (positions.Count < count)
                count = positions.Count;
            var result = new List<Position>();
            while (result.Count < count)
            {
                var myPick = Rndm.Pick(positions);
                if (!result.Contains(myPick))
                    result.Add(myPick);
            }
            return result;
        }

        internal string GetDescrStratEntry()
        {
            var txt = new StringBuilder();
            if (IsCastle)
                txt.Append($"settlement castle\n{{\n\tlevel {Translator.CityLevelToCityLevelName(StartLvl, World.SettlementLevels)}");
            else
                txt.Append($"settlement\n{{\n\tlevel {Translator.CityLevelToCityLevelName(StartLvl, World.SettlementLevels)}");
            var pop = Rndm.Int(StartPopMin, StartPopMax);
            var ctype = IsCastle ? "castle" : "city";
            var sTypeMinPop = (from s in World.SettlementLevels where s.Type == ctype && s.Level == StartLvl select s.PopMin).First();
            var sTypeMaxPop = (from s in World.SettlementLevels where s.Type == ctype && s.Level == StartLvl select s.PopMax).First();
            pop = pop < sTypeMinPop ? sTypeMinPop : pop;
            pop = pop > sTypeMaxPop ? sTypeMaxPop : pop;
            txt.Append($"\n\tregion {RID}\n\tyear_founded 0\n\tpopulation {pop}");
            txt.Append($"\n\tplan_set default_set\n\tfaction_creator {HomeFaction}\n\t");
            String bldng = null;
            String core = "";
            if (IsCastle)
            {
                core = "core_castle_building";
                bldng = World.SettlementLevels.First(a => a.Level == StartLvl && a.Type == "castle").Building;
            } else
            {
                core = "core_building";
                bldng = World.SettlementLevels.First(a => a.Level == StartLvl && a.Type == "city").Building;
            }
            IO.Val(bldng != null, $"Could not get {core} for {CityName}");
            if (bldng != "NULL")
                txt.Append($"building\n\t{{\n\t\ttype {core} {bldng}\n\t}}");
            if (Buildings.First().StartsWith("random_"))
            {
                var count = Convert.ToInt32(Buildings.First().Split("_")[1]);
                var retries = 0;
                Buildings.RemoveAt(0);
                while (count != 0 && retries < 50)
                {
                    var rb = getRandomBuildingForThisRegion();
                    if ((!Buildings.Contains(rb) && rb != "") && (!rb.StartsWith("temple_") || (rb.StartsWith("temple_") && !Buildings.Any(a => a.StartsWith("temple_")))))
                    {
                        Buildings.Add(rb);
                        if (rb == "port" || rb == "c_port")
                            World.Factions.First(a => a.ID == Owner).HasPort = true;
                        count--;
                    }
                    retries++;
                }
            }
            if (!IsCastle && Properties.Settings.Default.cbRoadTest && !(Buildings.Contains("roads_1") || Buildings.Contains("roads_2") || Buildings.Contains("roads_3")))
                Buildings.Add("roads_3");
            if (IsCastle && Properties.Settings.Default.cbRoadTest && !(Buildings.Contains("c_roads_1") || Buildings.Contains("c_roads_2") || Buildings.Contains("c_roads_3")))
                Buildings.Add("c_roads_3");
            foreach (var building in Buildings.Where(a => !a.Equals("NULL")))
            {
                try
                {
                    var chain = from b in World.Buildings where b.ID == building select b.Chain;
                    if (Properties.Settings.Default.cbRoadTest)
                        txt.Append($"\n\tbuilding\n\t{{\n\t\ttype {chain.First()} {building.Replace("roads_1", "roads_3").Replace("roads_2", "roads_3")}\n\t}}");
                    else
                        txt.Append($"\n\tbuilding\n\t{{\n\t\ttype {chain.First()} {building}\n\t}}");
                }
                catch
                {
                    MessageBox.Show($"ERROR: Could not find chain for building {building}!");
                }
            }
            return txt.ToString() + "\n}";
        }

        private string getRandomBuildingForThisRegion()
        {
            List<string> buildings = World.Buildings.Where(a => a.MinSettlementLevel <= StartLvl && a.Level == 1 && a.ForRandom && a.canBeBuiltInCity() && a.Resources.IsEmpty() && (!a.Requirements.Contains("factions") || a.Requirements.Contains(Owner))).Select(a => a.ID).ToList();
            if (IsCastle)
                buildings = World.Buildings.Where(a => a.MinSettlementLevel <= StartLvl && a.Level == 1 && a.ForRandom && a.canBeBuiltInCastle() && a.Resources.IsEmpty() && (!a.Requirements.Contains("factions") || a.Requirements.Contains(Owner))).Select(a => a.ID).ToList();
            if (HasPort && IsCastle)
                buildings.Add("c_port");
            if (HasPort && !IsCastle)
                buildings.Add("port");
            return buildings.Count < 1 ? "" : Rndm.Pick(buildings);
        }

        internal string GenerateReligionsString()
        {
            var homeReligion = (from r in World.Factions where r.ID == HomeFaction select r.Religion).First();
            Dictionary<string, int> religions = new Dictionary<string, int>();
            foreach (var r in Hardcoded.Religions)
                religions.Add(r, 0);
            religions[homeReligion] = IsNewWorld ? 100 : Rndm.Int(Tuner.RegionHomeFactionReligionMin, Tuner.RegionHomeFactionReligionMax);
            while (religions.Sum(a => a.Value) != 100)
            {
                var pick = Rndm.Pick(Hardcoded.Religions);
                religions[pick] = religions[pick] + Rndm.Int(1, 100 - religions.Sum(a => a.Value));
            }
            return String.Join(" ", religions.Keys.Zip(religions.Values, (x, y) => x + " " + y));
        }
    }
}
