using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.Collections.Generic;

namespace Ironclad.Entities
{
    class Event
    {
        public string ID { get; set; }
        public string Scale { get; set; }
        public int Year { get; set; }
        public int Turn { get; set; }
        public string TxtTitle { get; set; }
        public string TxtBody { get; set; }
        public int Chance { get; set; }
        public bool Winter { get; set; }
        public string Picture { get; set; }
        public bool IsPerCity { get; set; }
        public bool IsPerFaction { get; set; }
        public Region Plague { get; set; }
        public Region Earthquake { get; set; }
        public string Movie { get; set; }
        public Position Volcano { get; set; }
        public bool StartsPandemic { get; set; }
        public string Season { get; set; }
        public string SpawnFaction { get; set; }
        public List<string> SpawnUnits { get; set; }
        public List<string> SpawnRIDs { get; set; }
        public string SpawnGeneralNameID { get; set; }
        public int SpawnUnitsExpMin { get; set; }
        public int SpawnUnitsExpMax { get; set; }
        public int SpawnUnitsArmMin { get; set; }
        public int SpawnUnitsArmMax { get; set; }

        public Event(string scale, string YoT, string textTitle, string textBody, int chance, bool winter, string pic, bool perCity, bool perFaction, string plague, string earthquake, string movie, string vp, bool startsPandemic, string spawnFaction, string spawnUnits, string spawnRID, string spawnGeneralName, string spawnUnitsExpRange, string spawnUnitsArmRange)
        {
            // YoT = Year or Turn
            if (scale != "Year" && scale != "Turn")
                scale = "Year"; // Default value
            Scale = scale;
            TxtTitle = textTitle;
            TxtBody = textBody;
            Chance = chance;
            Winter = winter;
            Season = Winter ? "winter " : "";
            Picture = "@" + pic;
            IsPerCity = perCity;
            IsPerFaction = perFaction;
            Movie = movie;
            StartsPandemic = startsPandemic;
            Volcano = vp == "NULL" ? null : new Position(Convert.ToInt32(vp.Rem(" ").Split(",")[0]), Convert.ToInt32(vp.Rem(" ").Split(",")[1]));
            Plague = plague == "NULL" ? null : Translator.ToRegion(plague);
            Earthquake = earthquake == "NULL" ? null : Translator.ToRegion(earthquake);
            SpawnGeneralNameID = spawnGeneralName;
            ID = textTitle.Replace(" ", "_").Rem(".", "!", "?", "-", "'").ToUpper();
            if (YoT.Contains("-"))
                YoT = Rndm.Int(Convert.ToInt32(YoT.Split("-")[0]), Convert.ToInt32(YoT.Split("-")[1])).ToString();
            if (Scale == "Turn")
            {
                Turn = Convert.ToInt32(YoT);
                Year = Tuner.StartDate + Convert.ToInt32(YoT);
            } else
            {
                Year = Convert.ToInt32(YoT);
                Turn = (Convert.ToInt32(YoT) - Tuner.StartDate);
            }
            SpawnFaction = spawnFaction;
            if (spawnUnits != "NULL")
            {
                if (spawnUnits.Contains(","))
                {
                    var units = spawnUnits.Split(",");
                    SpawnUnits = new List<string>();
                    foreach (var unit in units)
                    {
                        Validator.IsUnit(unit.Trim(), $"Event {ID}");
                        SpawnUnits.Add(unit.Trim());
                    }
                }
                else
                {
                    Validator.IsUnit(spawnUnits.Trim(), $"Event {ID}");
                    SpawnUnits = new List<string>() { spawnUnits.Trim() };
                }
            }
            if (spawnRID != "NULL")
            {
                if (spawnRID.Contains(","))
            {
                var rids = spawnRID.Split(",");
                SpawnRIDs = new List<string>();
                foreach (var rid in rids)
                    SpawnRIDs.Add(rid.Trim());
            }
            else
                SpawnRIDs = new List<string>() { spawnRID.Trim() };
            }
            if (SpawnFaction != "NULL")
            {
                Validator.IsFaction(SpawnFaction);
                foreach (var checkRID in SpawnRIDs)
                    Validator.IsRID(checkRID);
            }
            if (spawnUnitsArmRange.Contains("-"))
            {
                SpawnUnitsArmMin = spawnUnitsArmRange.Split("-")[0].ToInt();
                SpawnUnitsArmMax = spawnUnitsArmRange.Split("-")[1].ToInt();
            }
            if (spawnUnitsExpRange.Contains("-"))
            {
                SpawnUnitsExpMin = spawnUnitsExpRange.Split("-")[0].ToInt();
                SpawnUnitsExpMax = spawnUnitsExpRange.Split("-")[1].ToInt();
            }
        }
    }
}
