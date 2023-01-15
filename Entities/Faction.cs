using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ironclad.Entities
{
    public class Faction
    {
        public int StandardIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Adjective { get; set; }
        public string Culture { get; set; }
        public string Religion { get; set; }
        public string ColorPrimary { get; set; }
        public string ColorSecondary { get; set; }
        public bool DoesSeaInvasions { get; set; }
        public string LabelAI { get; set; }
        public int TreasuryMin { get; set; }
        public int TreasuryMax { get; set; }
        public int FixedIncomeMin { get; set; }
        public int FixedIncomeMax { get; set; }
        public List<string> Neighbours { get; set; }
        public List<string> InvasionTargets { get; set; }
        public int Order { get; set; }
        public string Capital { get; set; }
        public string NameSet { get; set; }
        public string LogoID { get; set; }
        public string SmallLogoID { get; set; }
        public int StartAgents { get; set; }
        public string p_r { get; set; }
        public string p_g { get; set; }
        public string p_b { get; set; }
        public string s_r { get; set; }
        public string s_g { get; set; }
        public string s_b { get; set; }
        public string CampaignDescription { get; set; }
        public string ShowcaseUnit { get; set; }
        public string Strength { get; set; }
        public string Weakness { get; set; }
        public string TitleLeader { get; set; }
        public string TitleHeir { get; set; }
        public string TitlePriest0 { get; set; }
        public string TitlePriest1 { get; set; }
        public string TitlePriest2 { get; set; }
        public string Accent { get; set; }
        public string NameShort { get; set; }
        public string MusicType { get; set; }
        public string Leader { get; set; }
        public bool HasPort { get; set; }
        public string Bonus { get; set; }

        public Faction(int iD, string intName, string extName, string adjective, string culture, string religion, string colorPrimary, string colorSecondary, string doesSeaInvasions, string labelAI, int treasuryMin, int treasuryMax, int fixedIncomeMin, int fixedIncomeMax, List<string> neighbours, int order, string capital, string nameset, string logoid, string smalllogoid, string startagents, string campaignDescription, string showcaseunit, string strength, string weakness, string titleleader, string titleheir, string titlepriest0, string titlepriest1, string titlepriest2, string accent, string shortname, string musicType, string leader, string bonus)
        {
            StandardIndex = iD;
            ID = intName;
            Name = extName;
            Adjective = adjective;
            Culture = culture;
            Religion = religion;
            ColorPrimary = colorPrimary.Trim();
            ColorSecondary = colorSecondary.Trim();
            DoesSeaInvasions = doesSeaInvasions.EqualsIgnoreCase("random") ? Rndm.Pick(true, false) : doesSeaInvasions.ToBool();
            LabelAI = labelAI;
            TreasuryMin = treasuryMin;
            TreasuryMax = treasuryMax;
            FixedIncomeMin = fixedIncomeMin;
            FixedIncomeMax = fixedIncomeMax;
            Neighbours = neighbours;
            InvasionTargets = new List<string>() {"NULL"};
            Order = order;
            Capital = capital;
            NameSet = nameset;
            LogoID = logoid;
            SmallLogoID = smalllogoid;
            StartAgents = Convert.ToInt32(startagents);
            p_r = colorPrimary.Trim().Split(" ")[0];
            p_g = colorPrimary.Trim().Split(" ")[1];
            p_b = colorPrimary.Trim().Split(" ")[2];
            s_r = colorSecondary.Trim().Split(" ")[0];
            s_g = colorSecondary.Trim().Split(" ")[1];
            s_b = colorSecondary.Trim().Split(" ")[2];
            CampaignDescription = campaignDescription;
            ShowcaseUnit = showcaseunit;
            Strength = strength;
            if (Culture == "middle_eastern")
                Strength = $"{Strength}\\nCheaper forts and towers";
            Weakness = weakness;
            if ((Religion != "catholic" && Religion != "orthodox") || ID == Hardcoded.PapalFaction)
                Weakness = $"{Weakness}\\nCannot have princesses";
            if (Culture == "mesoamerican")
                Weakness = $"{Weakness}\\nEach Building costs maintenance per turn";
            TitleLeader = titleleader == null ? "King" : titleleader;
            TitleHeir = titleheir == null ? "Prince" : titleheir;
            TitlePriest0 = titlepriest0 == null ? "Priest" : titlepriest0;
            TitlePriest1 = titlepriest1 == null ? "Bishop" : titlepriest1;
            TitlePriest2 = titlepriest2 == null ? "Cardinal" : titlepriest2;
            Accent = accent;
            NameShort = shortname;
            MusicType = musicType;
            Leader = leader;
            HasPort = false;
            Bonus = bonus;
        }

        internal string GetRandomName(bool isFemale = false)
        {
            var names = from n in World.Names where n.NameSet == NameSet && n.Type == "characters" select n.NID;
            if (isFemale)
                names = from n in World.Names where n.NameSet == NameSet && n.Type == "women" select n.NID;
            return Rndm.Pick(names);
        }

        internal string GetStartResourcesTxt()
        {
            var resources = new List<string>();
            var regions = World.Regions.Where(a => a.Owner == ID);
            foreach (var region in regions)
                resources.AddRange(region.Resources.Select(a => a.Name));
            resources = resources.Distinct().ToList();
            if (resources.Count > 0)
                return string.Join(", ", resources);
            return "None";
        }
        internal string GetConsumablesTxt()
        {
            var consumables = new List<string>();
            var regions = World.Regions.Where(a => a.Owner == ID);
            foreach (var region in regions)
                consumables.AddRange(region.Resources.Select(a => a.Consumable));
            consumables = consumables.Where(a => a != "NULL").Distinct().Select(a => Translator.Consumable(a)).ToList();
            if (consumables.Count > 0)
                return string.Join(", ", consumables);
            return "None";
        }

        internal string GetRandomFullName(bool isFemale=false, bool noSurname=false)
        {
            var names = from n in World.Names where n.NameSet == NameSet && n.Type == "characters" select n.NID;
            if (isFemale)
                names = from n in World.Names where n.NameSet == NameSet && n.Type == "women" select n.NID;
            var surnames = from n in World.Names where n.NameSet == NameSet && n.Type == "surnames" select n.NID;
            var fullname = noSurname ? Rndm.Pick(names) : $"{Rndm.Pick(names)} {Rndm.Pick(surnames)}";
            return fullname;
        }

        internal string GetUnitMaxCost(int maxCost)
        {
            var units = World.Units.Where(a => a.Factions.Contains(ID) && a.CostMoney < maxCost && !a.IsNaval && !a.IsGeneral).Select(a => a.IntName);
            return Rndm.Pick(units);
        }

        internal int GetPopulation()
        {
            var sum = 0;
            var populations = World.Regions.Where(a => a.Owner == ID).Select(a => a.StartPopMax - ((a.StartPopMax - a.StartPopMin) / 2)).ToList();
            foreach (var population in populations)
                sum = sum + population;
            return sum;
        }

        internal string GetSlaveUnitMaxCost(int maxCost, Faction subfaction=null)
        {
            var units = World.Units.Where(a => a.Factions.Contains(ID) && a.Factions.Contains("slave") && a.CostMoney < maxCost && !a.IsNaval && !a.IsGeneral).Select(a => a.IntName);
            if (subfaction != null)
                units = World.Units.Where(a => a.Factions.Contains(ID) && a.Factions.Contains("slave") && a.CostMoney < maxCost && !a.IsNaval && !a.IsGeneral && a.Factions.Contains(subfaction.ID)).Select(a => a.IntName);
            return Rndm.Pick(units);
        }

        internal string GetPeasant()
        {
            return World.Units.First(a => a.Factions.Contains(ID) && (a.IntName.EndsWith("Peasants") || a.IntName.EndsWith("Rabble") || a.IntName.EndsWith("Scouts"))).IntName;
        }
        internal string GetWeakestRangedUnit()
        {
            return World.Units.Where(a => a.Factions.Contains(ID) && a.Class == "missile").OrderBy(a => a.CostMoney).First().IntName;
        }
        internal string GetWeakestInfantryUnit()
        {
            return World.Units.Where(a => a.Factions.Contains(ID) && a.Category == "infantry" && !a.IsPeasant && a.Class != "missile").OrderBy(a => a.CostMoney).First().IntName;
        }
        internal string GetWeakestCavalryUnit()
        {
            try
            {
                var unit = World.Units.Where(a => a.Factions.Contains(ID) && a.Category == "cavalry").OrderBy(a => a.CostMoney).First().IntName;
            }
            catch
            {
                return null;
            }
            return World.Units.Where(a => a.Factions.Contains(ID) && a.Category == "cavalry").OrderBy(a => a.CostMoney).First().IntName;
        }
        internal string GetWeakestSiegeUnit()
        {
            try
            {
                var unit = World.Units.Where(a => a.Factions.Contains(ID) && a.Category == "siege" && a.Class == "missile").OrderBy(a => a.CostMoney).First().IntName;
            } catch
            {
                return null;
            }
            return World.Units.Where(a => a.Factions.Contains(ID) && a.Category == "siege" && a.Class == "missile").OrderBy(a => a.CostMoney).First().IntName;
        }

        internal int GetCountStartRegions()
        {
            return (from r in World.Regions where r.Owner == ID select r.CID).Count();
        }
        internal int GetCountStartCities()
        {
            return (from r in World.Regions where r.Owner == ID && !r.IsCastle select r.CID).Count();
        }
        internal int GetCountStartCastles()
        {
            return (from r in World.Regions where r.Owner == ID && r.IsCastle select r.CID).Count();
        }

        internal string GetGeneralsBodyguardUnit()
        {
            return World.Units.First(a => a.Factions.Contains(ID) && a.IsGeneral).IntName;
        }

        internal string GetCatapult()
        {
            try
            {
                return World.Units.First(a => a.IsCatapult && a.Factions.Contains(ID)).IntName;
            } catch
            {
                return World.Units.First(a => !a.IsNaval && !a.IsGeneral && a.Factions.Contains(ID)).IntName;
            }
        }
    }
}
