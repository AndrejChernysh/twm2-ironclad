using Ironclad.Entities;
using Ironclad.Extensions;
using Remotion.Text.StringExtensions;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ironclad.Helper
{
    static class Generator
    {
        public static void GenerateGame()
        {
            File.Delete(Hardcoded.MAP);
            IO.Log("Generating Game");
            File.Delete(Settings.P(@"preferences\keys.dat"));
            foreach (string f in Directory.GetFiles(Settings.P(@"data\text"), "*.bin"))
                if (!f.Contains("battle.") && !f.Contains("strat.") && !f.Contains("tooltips.") && !f.Contains("shared.")) // Do not delete these as they are not getting regenerated
                    File.Delete(f);
            GenerateFactions();
            GenerateResources();
            GenerateBuildings();
            GenerateRegions();
            GenerateStratTxt();
            GenerateCampaign();
            GenerateMusicTypes();
            GenerateNames();
            GenerateRebelFactions();
            GenerateWinConditions();
            GenerateAccents();
            GenerateCampaignEvents();
            GenerateCampaignDescriptions();
            GenerateFactionTexts();
            GenerateTooltipsText();
            GenerateMercenaryPools();
            ScriptGenerator.GenerateAll();
            HEGenerator.GenerateHistoricEventsText();
        }

        private static void GenerateFactions()
        {
            StringBuilder c = new StringBuilder();
            c.Append(String.Concat(Enumerable.Repeat(";DO NOT REMOVE THIS LINE OTHERWISE THE GAME CRASHES\n", 5)));
            foreach (var f in World.Factions.OrderBy(a => a.Order).ToList())
            {
                c.Append($"\nfaction\t{f.ID}");
                c.Append($"\nculture\t{f.Culture}");
                c.Append($"\nreligion\t{f.Religion}");
                c.Append(f.ID == Hardcoded.PapalFaction ? "\nspecial_faction_type\tpapal_faction" : "");
                c.Append(f.ID == "slave" ? "\nspecial_faction_type\tslave_faction" : "");
                c.Append($"\nsymbol\tmodels_strat/nosymbol.CAS");
                c.Append($"\nrebel_symbol\tmodels_strat/nosymbol.CAS");
                c.Append($"\nprimary_colour\tred {f.p_r}, green {f.p_g}, blue {f.p_b}");
                c.Append($"\nsecondary_colour\tred {f.s_r}, green {f.s_g}, blue {f.s_b}");
                c.Append($"\nloading_logo\tloading_screen/symbols/symbol128_{f.ID}.tga");
                c.Append($"\nstandard_index\t{f.StandardIndex}");
                c.Append($"\nlogo_index\t{f.LogoID}");
                c.Append($"\nsmall_logo_index\t{f.SmallLogoID}");
                c.Append("\ntriumph_value\t5");
                if (f.ID == "slave") {
                    c.Append("\ncan_sap\tyes");
                    c.Append("\ncan_have_princess\tno");
                    c.Append("\nhas_family_tree\tno\n\n");
                } else {
                    c.Append("\ncustom_battle_availability\tyes");
                    if (f.ID != Hardcoded.PapalFaction)
                    {
                        c.Append("\nhorde_min_units\t10\nhorde_max_units\t20");
                        c.Append("\nhorde_max_units_reduction_every_horde\t10");
                        c.Append("\nhorde_unit_per_settlement_population\t250");
                        c.Append("\nhorde_min_named_characters\t2");
                        c.Append("\nhorde_max_percent_army_stack\t80");
                        c.Append("\nhorde_disband_percent_on_settlement_capture\t100");
                        c.Append(String.Concat(Enumerable.Repeat($"\nhorde_unit\t{f.GetUnitMaxCost(399)}", 3)));
                        c.Append(String.Concat(Enumerable.Repeat($"\nhorde_unit\t{f.GetPeasant()}", 3)));
                    }
                    c.Append("\ncan_sap\tno");
                    c.Append(Rndm.Chance(50) ? "\nprefers_naval_invasions\tno" : "\nprefers_naval_invasions\tyes");
                    c.Append((f.Religion == "catholic" || f.Religion == "orthodox") && f.ID != Hardcoded.PapalFaction ? "\ncan_have_princess\tyes" : "\ncan_have_princess\tno");
                    c.Append(f.ID == Hardcoded.PapalFaction ? "\nhas_family_tree\tno\n" : "\nhas_family_tree\tyes\n");
                }
            }
            File.WriteAllText(Hardcoded.DESCR_SM_FACTIONS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_SM_FACTIONS)} ({FO.GetSizeKB(Hardcoded.DESCR_SM_FACTIONS)} KB)");
        }
        private static void GenerateCampaignDescriptions()
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.Factions)
            {
                var neighbours = new List<string>() { };
                var neighboursTransl = new List<Faction>() { };
                foreach(var r in World.Regions.Where(a => a.Owner == f.ID))
                {
                    if (r.NeighbourRegions.Any())
                        foreach(var nr in r.NeighbourRegions)
                            if (!nr.Owner.Equals(f.ID))
                                neighbours.Add(nr.Owner);
                }
                if (neighbours.Any())
                    foreach (var n in neighbours.Distinct())
                        neighboursTransl.Add(World.Factions.First(a => a.ID == n));
                c.Append($"\n{{IMPERIAL_CAMPAIGN_{f.ID.ToUpper()}_TITLE}}{f.Name}");
                c.Append($"\n{{IMPERIAL_CAMPAIGN_{f.ID.ToUpper()}_DESCR}}" +
                    $"Capital: {World.Regions.First(a => a.CID == f.Capital).CityName}\\n" +
                    $"Culture: {Translator.IntToExCulture(f.Culture)}\\n" +
                    $"Religion: {Translator.IntToExReligion(f.Religion)}\\n" +
                    $"Cities: {f.GetCountStartCities()}\\n" +
                    $"Castles: {f.GetCountStartCastles()}\\n" +
                    $"Population: ~{f.GetPopulation()}\\n\\n" +
                    $"Resources:\\n{f.GetStartResourcesTxt()}\\n" +
                    $"Consumables:\\n{f.GetConsumablesTxt()}\\n\\n" +
                    $"Neighbours:\\n{string.Join(", ", neighboursTransl.Select(a => a.NameShort))}\\n" +
                    $"\\n{f.CampaignDescription}");
            }
            FO.WriteAllTextUCS2LEBOM(Hardcoded.CAMPAIGN_DESCRIPTIONS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.CAMPAIGN_DESCRIPTIONS)} ({FO.GetSizeKB(Hardcoded.CAMPAIGN_DESCRIPTIONS)} KB)");
        }
        private static void GenerateTooltipsText()
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n{Hardcoded.ContentTooltipsTxt}");
            foreach (var r in World.Resources)
            {
                var query = from b in World.Buildings join ch in World.BuildingChains on b.Chain equals ch.ID where b.Resources.Contains(r.ID) select ch.Name;
                var buildings = String.Join("\\n", query.Distinct());
                var btxt = buildings.Count() > 3 ? $"\\nEnables construction of:\\n{buildings}" : "";
                var rarity = Math.Round(Convert.ToDouble(World.ResourceOnMapCounts.First(a => a.Key == r.ID).Value) / Convert.ToDouble(World.ResourceOnMapCounts.Values.Sum()) * 100.0, 2);
                var consumable = r.Consumable == "NULL" ? "" : $" ({r.ConsumableText})";
                c.Append($"\n{{TMT_{r.ID.ToUpper()}_TOOLTIP}}{r.Name}{consumable}\\nRarity: {rarity}% of all resources\\nBase Trade Value: {r.Value}{btxt}\\n" +
                    $"Place merchant here to generate florins\\nPlace army here to loot if region owner is an enemy");
                c.Append($"\n{{TMT_{r.ID.ToUpper()}_MINE_TOOLTIP}}{r.Name}{consumable}\\nRarity: {rarity}% of all resources\\nBase Trade Value: {r.Value}{btxt}\\n" +
                    $"Place merchant here to generate florins\\nPlace army here to loot if region owner is an enemy");
            }
            c.Append("\n"); // Otherwise crash ingame - not applicable to all LEBOM files
            FO.WriteAllTextUCS2LEBOM(Hardcoded.TOOLTIPS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.TOOLTIPS)} ({FO.GetSizeKB(Hardcoded.TOOLTIPS)} KB)");
        }
        private static void GenerateFactionTexts()
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n{Hardcoded.expandedTxtOtherElements}");
            foreach (var faction in World.Factions)
            {
                var ppinfo = "This character can join and left political parties by chance and be promoted and demoted within the party ranks by having good or bad stats and winning or losing battles.\\n" +
                    "Make this general a governor by moving him in a settlement and gain at least -25% discount on constructing buildings.";
                c.Append($"\n{{{faction.ID.ToUpper()}}}{faction.Name}");
                foreach (var agent in Hardcoded.expandedTxtSingleWordElements)
                    c.Append($"\n{{EMT_{faction.ID.ToUpper()}_{agent.ToUpper()}}}{faction.Adjective} {agent.Capitalize()}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_GENERAL}}{faction.Adjective} Army");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_NAMED_CHARACTER}}{faction.Adjective} Family Member\\nDouble Left-click for more options (forced march, crusades, ...)\\n{ppinfo}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_NAMED_GENERAL}}{faction.Adjective} General");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_PRIEST}}{faction.Adjective} {faction.TitlePriest0}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_PRIEST_1}}{faction.Adjective} {faction.TitlePriest1}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_PRIEST_2}}{faction.Adjective} {faction.TitlePriest2}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_LARGE_TOWN}}{faction.Adjective} Large Town");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_LARGE_CITY}}{faction.Adjective} Large City");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_HUGE_CITY}}{faction.Adjective} Huge City");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_WOODEN_CASTLE}}{faction.Adjective} Motte and Bailey");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_STONE_KEEP}}{faction.Adjective} Wooden Castle");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_LARGE_CASTLE}}{faction.Adjective} Fortress");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_STAR_FORT}}{faction.Adjective} Star Fort");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FISHING_VILLAGE}}{faction.Adjective} Fishing Village");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_PORT}}{faction.Adjective} Port\\nPlace agent here to gather information\\nPlace army to blockade and take florins from enemy");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FACTION_LEADER}}{faction.Adjective} Faction Leader\\nDouble Left-click for more options (forced march, crusades, ...)\\n{ppinfo}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FACTION_HEIR}}{faction.Adjective} Faction Heir\\nDouble Left-click for more options (forced march, crusades, ...)\\n{ppinfo}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FACTION_LEADER_TITLE}}{faction.TitleLeader}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FACTION_HEIR_TITLE}}{faction.TitleHeir}");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FACTION_LEADER_NAME}}{faction.TitleLeader} %S");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FACTION_HEIR_NAME}}{faction.TitleHeir} %S");
                c.Append($"\n{{EMT_{faction.ID.ToUpper()}_FORMER_FACTION_LEADER_TITLE}}{faction.TitleLeader}");
                c.Append($"\n{{EMT_YOUR_FORCES_ATTACK_ARMY_{faction.ID.ToUpper()}}}Your forces attack an army of the {faction.Name}");
                c.Append($"\n{{EMT_YOUR_FORCES_ATTACK_NAVY_{faction.ID.ToUpper()}}}Your forces attack a navy of the {faction.Name}");
                c.Append($"\n{{EMT_YOUR_FORCES_AMBUSH_ARMY_{faction.ID.ToUpper()}}}Your forces ambush an army of the {faction.Name}");
                c.Append($"\n{{EMT_YOUR_FORCES_ATTACKED_ARMY_{faction.ID.ToUpper()}}}Your forces are attacked by an army of the {faction.Name}");
                c.Append($"\n{{EMT_YOUR_FORCES_ATTACKED_NAVY_{faction.ID.ToUpper()}}}Your forces are attacked by a navy of the {faction.Name}");
                c.Append($"\n{{EMT_YOUR_FORCES_AMBUSHED_ARMY_{faction.ID.ToUpper()}}}Your forces are ambushed by an army of the {faction.Name}");
                c.Append($"\n{{EMT_VICTORY_{faction.ID.ToUpper()}}}The {faction.Name} has won!");
                c.Append($"\n{{{faction.ID.ToUpper()}_STRENGTH}}{faction.Strength}");
                if (faction.Bonus != "NULL")
                    c.Append($"\\n{faction.Bonus}");
                c.Append($"\n{{{faction.ID.ToUpper()}_WEAKNESS}}{faction.Weakness}");
                c.Append($"\n{{{faction.ID.ToUpper()}_UNIT}}{faction.ShowcaseUnit}");
            }
            foreach (var r in World.Regions)
            {
                c.Append($"\n{{{r.CID}}}{r.CityName}");
                c.Append($"\n{{{r.CID}C}}{r.CityNameColonized}");
            }
            foreach (var r in World.Regions)
                foreach (var p in Settings.Parties)
                {
                    c.Append($"\n{{{r.CID}{p.Key}}}{r.CityName} [{p.Value.First()}]");
                    c.Append($"\n{{{r.CID}{p.Key}C}}{r.CityNameColonized} [{p.Value.First()}]");
                }
            c.Append("\n"); // Otherwise crash ingame - not applicable to all LEBOM files
            FO.WriteAllTextUCS2LEBOM(Hardcoded.EXPANDED, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.EXPANDED)} ({FO.GetSizeKB(Hardcoded.EXPANDED)} KB)");
        }
        private static void GenerateMercenaryPools()
        {
            StringBuilder c = new StringBuilder();
            foreach(var pool in World.MercPools.GroupBy(a => a.Name).Select(a => a.First()).ToList())
                c.Append($"\n{pool.GetDescrMercenariesEntry()}\n");
            File.WriteAllText(Hardcoded.DESCR_MERCENARIES, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_MERCENARIES)} ({FO.GetSizeKB(Hardcoded.DESCR_MERCENARIES)} KB)");
        }
        private static void GenerateAccents()
        {
            StringBuilder c = new StringBuilder();
            c.Append($"; DO NOT REMOVE OTHERWISE CRASH\n; DO NOT REMOVE OTHERWISE CRASH\n");
            foreach (var accent in World.Factions.Select(a => a.Accent).Distinct().ToList())
                c.Append($"\naccent {accent}\n\tfactions {String.Join(", ", World.Factions.Where(a => a.Accent == accent).Select(a => a.ID).ToList())}\n");
            File.WriteAllText(Hardcoded.DESCR_SOUNDS_ACCENTS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_SOUNDS_ACCENTS)} ({FO.GetSizeKB(Hardcoded.DESCR_SOUNDS_ACCENTS)} KB)");
        }
        private static void GenerateResources()
        {
            StringBuilder c = new StringBuilder();
            c.Append($";DO NOT REMOVE\n\nmine\t\t\t\tdata/models_strat/resource_mine.CAS\n\n");
            foreach (var resource in World.Resources.OrderByDescending(a => a.Value))
            {
                c.Append($"\n;{resource.Name}");
                c.Append($"\ntype\t{resource.ID}");
                c.Append($"\ntrade_value\t{resource.Value}");
                c.Append($"\nitem\t{resource.Model}");
                c.Append($"\nicon\t{resource.Icon}\n");
                if (resource.HasMine)
                    c.Append($"has_mine\n");
            }
            c.Append($"\n;DO NOT REMOVE\ntype\tgeneric\ntrade_value\t0\nitem\tdata/models_strat/resource_dogs.CAS\t; NOT ACTUALLY USED\nicon\tdata/ui/resources/resource_trade_goods.tga\n");
            File.WriteAllText(Hardcoded.DESCR_SM_RESOURCES, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_SM_RESOURCES)} ({FO.GetSizeKB(Hardcoded.DESCR_SM_RESOURCES)} KB)");
        }
        private static void GenerateCampaignEvents()
        {
            StringBuilder c = new StringBuilder();
            // Random plagues and fires
            foreach (var r in World.Regions.Where(a => !a.IsNewWorld && !a.IsUnreachable))
            {
                HEGenerator.Add($"{r.CID}_PLAGUE", $"Plague in {r.RegionName}", $"{r.CityName} in {r.RegionName} has been struck by an epidemic of a yet unknown infectious plague. Any person being within the settlement is in danger for the time being.||Plagues can spread if contaminated characters move to other cities.", "@7");
                HEGenerator.Add($"{r.CID}_FIRE", $"Fire in {r.CityName}", $"{r.CityName} in {r.RegionName} has been struck by a fire accident by a yet unknown cause of the tragedy.||For the time being, this settlement remains especially vulnerable against enemy attacks, since its people and garrison are busy cleaning up the mess.", "@59");
                c.Append($"\nevent\tplague {r.CID}_PLAGUE\ndate\t{Rndm.Int(1, 2000)}\nposition\t{r.Position.X} {r.Position.Y}\n");
                c.Append($"\nevent\tearthquake {r.CID}_FIRE\ndate\t{Rndm.Int(1, 500)}\nposition\t{r.Position.X} {r.Position.Y}\n");
            }
            // Historic Events
            foreach (var e in World.Events)
            {
                c.Append($"\nevent\thistoric\t{e.ID}\ndate\t{e.Season}{e.Turn}");
                if (e.Movie != "NULL")
                    c.Append($"\nmovie\t{e.Movie}");
                if (e.Picture != "@NULL")
                    HEGenerator.Add(e.ID, e.TxtTitle, e.TxtBody, e.Picture);
                else
                    HEGenerator.Add(e.ID, e.TxtTitle, e.TxtBody);
                c.Append($"\n");
            }
            // Black Plague
            var bdDate = World.Events.First(a => a.StartsPandemic).Turn + 2;
            var bds1 = World.Regions.Where(a => Tuner.bdStage1.Contains(a.ResourcePool)).ToList();
            var bds2 = World.Regions.Where(a => Tuner.bdStage2.Contains(a.ResourcePool)).ToList();
            var bds3 = World.Regions.Where(a => Tuner.bdStage3.Contains(a.ResourcePool)).ToList();
            var bds4 = World.Regions.Where(a => Tuner.bdStage4.Contains(a.ResourcePool)).ToList();
            var bds5 = World.Regions.Where(a => !a.IsNewWorld && !a.IsUnreachable).ToList();
            c.Append($"\n\nevent\tplague BLACKPLAGUE_FIRST\ndate\t{bdDate + 1}");
            HEGenerator.Add("BLACKPLAGUE_FIRST", "Cursed Mongols?", "More and more people begin to die around the Caspian Sea as it seems that God is punishing the Lands of the Mongols with Death.", "@67");
            foreach (var bd1 in bds1)
                c.Append($"\nposition\t{bd1.Position.X}, {bd1.Position.Y} ;{bd1.CityName}");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_SECOND\ndate\t{bdDate + 2}");
            HEGenerator.Add("BLACKPLAGUE_SECOND", "Death reaches Levante", "More and more people continue to die in great numbers, as traders and sailors seem to have brought the curse of death across the Muslim lands as well as Southern Europe.", "@67");
            foreach (var bd2 in bds2.Concat(bds1))
                c.Append($"\nposition\t{bd2.Position.X}, {bd2.Position.Y} ;{bd2.CityName}");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_THIRD\ndate\t{bdDate + 3}");
            HEGenerator.Add("BLACKPLAGUE_THIRD", "Death reaches Central Europe", "The Great Dying, as some are calling it, is not only continuing to kill off whole villages and many people in cities, it is also rapidly spreading to the North, as the first cases have been reported from Central and Western Europe.", "@67");
            foreach (var bd3 in bds3.Concat(bds2))
                c.Append($"\nposition\t{bd3.Position.X}, {bd3.Position.Y} ;{bd3.CityName}");
            c.Append($"\nmovie\tevent/black_death.bik");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_FOURTH\ndate\t{bdDate + 4}");
            HEGenerator.Add("BLACKPLAGUE_FOURTH", "Death reaches Northern Europe", "God does not spare the Northern Europeans from his punishment it seems - there are first cases of masses of dead people being reported across many of the Northern European cities.", "@67");
            foreach (var bd4 in bds4.Concat(bds3))
                c.Append($"\nposition\t{bd4.Position.X}, {bd4.Position.Y} ;{bd4.CityName}");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_FIFTH\ndate\t{bdDate + 5}");
            HEGenerator.Add("BLACKPLAGUE_FIFTH", "Death reaches Eastern Europe", "Until now, Eastern Europe was not affected from the Great Dying - however, this is now a thing of the past. Many deaths are being reported from most of the major cities.", "@67");
            foreach (var bd5 in bds5.Concat(bds4))
                c.Append($"\nposition\t{bd5.Position.X}, {bd5.Position.Y} ;{bd5.CityName}");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_SIXTH\ndate\t{bdDate + 6}");
            HEGenerator.Add("BLACKPLAGUE_SIXTH", "The Great Dying", "The World has come to it's darkest hour now, it seems that there will be no tomorrow for mankind on earth. Almost all of the communities are losing thousands of men, women and children every week.", "@67");
            foreach (var bd6 in Rndm.Pick(bds5, Convert.ToInt32(Math.Round(bds5.Count * 0.75))))
                c.Append($"\nposition\t{bd6.Position.X}, {bd6.Position.Y} ;{bd6.CityName}");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_SEVENTH\ndate\t{bdDate + 7}");
            HEGenerator.Add("BLACKPLAGUE_SEVENTH", "Hope!", "As the Great Dying continues, the first cities, most of them in the East, seem to have witnessed an end of Gods punishment. Many commmunities have become completely depopulated, but at last it seems that God is not intending to kill off every human on earth!", "@67");
            foreach (var bd7 in Rndm.Pick(bds5, Convert.ToInt32(Math.Round(bds5.Count * 0.50))))
                c.Append($"\nposition\t{bd7.Position.X}, {bd7.Position.Y} ;{bd7.CityName}");
            c.Append($"\n\nevent\tplague BLACKPLAGUE_EIGHTH\ndate\t{bdDate + 8}");
            HEGenerator.Add("BLACKPLAGUE_EIGHTH", "Great Dying fades off", "The positive trend of fewer dead every day is establishing with every passing week. In more and more communities, the dead are being buried or burned and the living are trying to reestablish some kind of a normal life.", "@67");
            foreach (var bd8 in Rndm.Pick(bds5, Convert.ToInt32(Math.Round(bds5.Count * 0.25))))
                c.Append($"\nposition\t{bd8.Position.X}, {bd8.Position.Y} ;{bd8.CityName}");
            File.WriteAllText(Hardcoded.DESCR_EVENTS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_EVENTS)} ({FO.GetSizeKB(Hardcoded.DESCR_EVENTS)} KB)");
        }
        private static void GenerateNames()
        {
            StringBuilder c = new StringBuilder();
            StringBuilder c2 = new StringBuilder();
            c.Append(String.Concat(Enumerable.Repeat(";DO NOT REMOVE THIS LINE OTHERWISE THE GAME CRASHES\n", 21)));
            foreach(var faction in World.Factions)
            {
                c.Append($"\n\nfaction: {faction.ID}");
                foreach (var type in World.Names.Select(a => a.Type).Distinct())
                {
                    c.Append($"\n\t{type}");
                    foreach (var name in World.Names.Where(a => a.NameSet == faction.NameSet && a.Type == type))
                    {
                        c.Append($"\n\t\t{name.NID}");
                        c2.Append($"\n{{{name.NID}}}{name.Txt}");
                    }
                }
            }
            c.Append("\n"); // Do not remove otherwise crash
            File.WriteAllText(Hardcoded.DESCR_NAMES, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_NAMES)} ({FO.GetSizeKB(Hardcoded.DESCR_NAMES)} KB)");
            FO.WriteAllTextUCS2LEBOM(Hardcoded.NAMES, c2.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.NAMES)} ({FO.GetSizeKB(Hardcoded.NAMES)} KB)");
        }
        private static void GenerateRebelFactions()
        {
            StringBuilder c = new StringBuilder();
            StringBuilder c2 = new StringBuilder();
            c.Append(String.Concat(Enumerable.Repeat(";DO NOT REMOVE THIS LINE OTHERWISE THE GAME CRASHES\n", 4)));
            foreach (var faction in World.RebelFactions)
            {
                c2.Append($"\n{{{faction.ID}}}{faction.Name}");
                c.Append($"\n\nrebel_type\t\t\t{faction.ID}");
                c.Append($"\ncategory\t\t\t{faction.Category}");
                c.Append($"\nchance\t\t\t{faction.Chance}");
                c.Append($"\ndescription\t\t\t{faction.ID}");
                foreach (var unit in faction.Units)
                    c.Append($"\nunit\t\t\t{unit}");
            }
            c.Append("\n");
            File.WriteAllText(Hardcoded.DESCR_REBEL_FACTIONS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_REBEL_FACTIONS)} ({FO.GetSizeKB(Hardcoded.DESCR_REBEL_FACTIONS)} KB)");
            FO.WriteAllTextUCS2LEBOM(Hardcoded.REBEL_FACTION_DESCR, c2.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.REBEL_FACTION_DESCR)} ({FO.GetSizeKB(Hardcoded.REBEL_FACTION_DESCR)} KB)");
        }
        private static void GenerateStratTxt()
        {
            StringBuilder c = new StringBuilder();
            c.Append($"\n{Hardcoded.ContentStratTxt}");
            foreach (var r in World.Resources)
            {
                c.Append($"\n{{SMT_RESOURCE_{r.ID.ToUpper()}}}{r.Name}");
                c.Append($"\n{{SMT_RESOURCE_{r.ID.ToUpper().Rem("S")}}}{r.Name}");
            }
            c.Append("\n"); // Otherwise crash ingame - not applicable to all LEBOM files
            FO.WriteAllTextUCS2LEBOM(Hardcoded.STRAT, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.STRAT)} ({FO.GetSizeKB(Hardcoded.STRAT)} KB)");
        }
        private static void GenerateWinConditions()
        {
            StringBuilder c = new StringBuilder();
            foreach (var f in World.Factions.Where(a => a.ID != "slave").ToList())
                c.Append($"{f.ID}\nhold_regions {f.Capital.Replace("C", "R")}\ntake_regions {World.Regions.Count(a => !a.IsUnreachable)/3}\nshort_campaign hold_regions\ntake_regions {World.Regions.Count(a => !a.IsUnreachable)/7}\noutlive {String.Join(" ", f.Neighbours)}\n\n");
            c.Append($"slave\nhold_regions R001\ntake_regions {World.Regions.Count}\nshort_campaign hold_regions\ntake_regions {World.Regions.Count}\noutlive {World.Factions.First().ID}\n\n");
            File.WriteAllText(Hardcoded.DESCR_WIN_CONDITIONS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_WIN_CONDITIONS)} ({FO.GetSizeKB(Hardcoded.DESCR_WIN_CONDITIONS)} KB)");
        }

        private static void GenerateCampaign()
        {
            var takenFleetPositions = new List<String>();
            StringBuilder c = new StringBuilder();
            c.Append(";Generated by Ironclad - DO NOT REMOVE THIS LINE OTHERWISE THE GAME CRASHES");
            c.Append("\ncampaign\timperial_campaign\nplayable");
            foreach (var faction in World.Factions.Where(a => !a.ID.Equals("slave")))
                c.Append($"\n\t{faction.ID}");
            c.Append($"\nend\nunlockable\nend\nnonplayable\n\tslave\nend\nstart_date\t{Tuner.StartDate} summer\nend_date\t{Tuner.EndDate} winter\ntimescale\t0.5");
            c.Append("\nmarian_reforms_disabled\nrebelling_characters_active\ngladiator_uprising_disabled\nnight_battles_enabled");
            c.Append($"\nbrigand_spawn_value {Tuner.RebelsSpawnRate}\npirate_spawn_value {Tuner.PiratesSpawnRate}\nfree_upkeep_forts {Tuner.FreeUpkeepForts}\n");
            foreach (var r in World.Regions)
            {
                r.ResourcePositions = r.GetAccessiblePositions(r.ResourceSlots);
                c.Append($"\n;Region: {r.RegionName} Pool: {r.ResourcePool}"); // Do not remove, needed for stats
                foreach (var (rp, i) in r.ResourcePositions.Select((v, i) => (v, i)).ToList())
                {
                    c.Append($"\nresource\t{r.Resources.ElementAt(i).ID},\t{rp.X},\t{rp.Y}");
                    c.Append(Rndm.Chance(10) ? $"\nresource\t{r.Resources.ElementAt(i).ID},\t{rp.X},\t{rp.Y}" : "");
                    c.Append(Rndm.Chance(1) ? $"\nresource\t{r.Resources.ElementAt(i).ID},\t{rp.X},\t{rp.Y}" : "");
                }
            }
            foreach (var f in World.Factions.OrderBy(a => a.Order))
            {
                var cap = World.Regions.First(a => a.Owner.Equals(f.ID) && a.CID.Equals(f.Capital));
                var noncaps = World.Regions.Where(a => a.Owner.Equals(f.ID) && !a.CID.Equals(f.Capital));
                c.Append($"\n\nfaction {f.ID}, {Rndm.Pick(Hardcoded.AIProfilesConstruction)} {Rndm.Pick(Hardcoded.AIProfilesRecruitment)}");
                c.Append($"\nai_label {f.LabelAI}\ndenari {Rndm.Int(f.TreasuryMin, f.TreasuryMax)}");
                c.Append($"\ndenari_kings_purse {Rndm.Int(f.FixedIncomeMin, f.FixedIncomeMax)}\n");
                c.Append($"\n{cap.GetDescrStratEntry()}\n");
                foreach (var region in noncaps)
                    c.Append($"\n{region.GetDescrStratEntry()}\n");
                if (f.ID != "slave")
                {
                    IO.Val(World.Names.Any(a => a.NameSet == f.NameSet && a.Txt == f.Leader && a.Type == "characters") || f.Leader == "NULL", $"Could not find fixed leader name {f.Leader} for {f.ID}");
                    var king = f.Leader == "NULL" ? f.GetRandomFullName(false, true) : World.Names.Where(a => a.NameSet == f.NameSet && a.Txt == f.Leader && a.Type == "characters").Select(a => a.NID).DefaultIfEmpty(f.GetRandomFullName()).First();
                    var queen = f.GetRandomFullName(true, true);
                    var age = f.ID == Hardcoded.PapalFaction ? 50 : Rndm.Int(40, 50);
                    var males = new List<string>() { king };
                    var females = new List<string>() { queen };
                    var teens = new List<string>();
                    var takennames = new List<string>() { king, queen };
                    var cnt = 0;
                    string newMale;
                    string newFemale;
                    string newTeen;
                    foreach (var noncap in noncaps)
                    {
                        cnt++;
                        if (cnt < 5) // No more than four children allowed otherwise crash
                        {
                            do {
                                newMale = f.GetRandomFullName();
                            } while (takennames.Contains(newMale));
                            takennames.Add(newMale);
                            males.Add(newMale);
                            do {
                                newFemale = f.GetRandomFullName(true, true);
                            } while (takennames.Contains(newFemale));
                            takennames.Add(newFemale);
                            females.Add(newFemale);
                        }
                    }
                    if (males.Count < 5)
                        foreach (var i in 1.To(5 - males.Count))
                        {
                            do {
                                newTeen = f.GetRandomFullName(true, true);
                            } while (takennames.Contains(newTeen));
                            takennames.Add(newTeen);
                            teens.Add(newTeen);
                        }
                    c.Append($"\n;{Translator.NIDToName(males.First())} ({cap.CityName})");
                    c.Append($"\ncharacter\t{males.First()}, named character, male, leader, age {age}, x {cap.Position.X} y {cap.Position.Y}");
                    c.Append($"\ntraits Factionleader 2, ReligionStarter 1, LoyaltyStarter 1, NightBattleCapable 1, PP{Rndm.Pick(Settings.Parties.Keys)} 4");
                    foreach (var trait in Hardcoded.GoodBadTraits)
                        if (Rndm.Chance(50))
                            c.Append($", {Rndm.RandomWord("Good", "Bad")}{trait} {Rndm.Int(1, 3)}");
                    var generalUnit = World.Units.First(a => a.Factions.Contains(f.ID) && a.IsGeneral).IntName;
                    c.Append($"\narmy\nunit\t{generalUnit}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                    foreach (int i in Enumerable.Range(1, Rndm.Int(1, 4)))
                        c.Append($"\nunit\t{f.GetUnitMaxCost(399)}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                    cnt = 0;
                    foreach (var r in noncaps)
                    {
                        cnt++;
                        if (cnt < 5)
                        {
                            if (cnt == 1) // is heir
                            {
                                c.Append($"\n\n;{Translator.NIDToName(males.ElementAt(cnt))} ({r.CityName})");
                                c.Append($"\ncharacter\t{males.ElementAt(cnt)}, named character, male, heir, age {age - 17 - cnt}, x {r.Position.X} y {r.Position.Y}");
                                c.Append($"\ntraits Factionheir 2, ReligionStarter 1, LoyaltyStarter 1, PP{Rndm.Pick(Settings.Parties.Keys)} 3");
                                if (Rndm.Chance(50))
                                    c.Append(", NightBattleCapable 1");
                            } else // not a heir
                            {
                                c.Append($"\n\n;{Translator.NIDToName(males.ElementAt(cnt))} ({r.CityName})");
                                c.Append($"\ncharacter\t{males.ElementAt(cnt)}, named character, male, age {age - 17 - cnt}, x {r.Position.X} y {r.Position.Y}");
                                c.Append($"\ntraits ReligionStarter 1, LoyaltyStarter 1, PP{Rndm.Pick(Settings.Parties.Keys)} 2");
                                if (Rndm.Chance(33))
                                    c.Append(", NightBattleCapable 1");
                            }
                            foreach (var trait in Hardcoded.GoodBadTraits)
                                if (Rndm.Chance(50))
                                    c.Append($", {Rndm.RandomWord("Good", "Bad")}{trait} {Rndm.Int(1, 3)}");
                            c.Append($"\narmy\nunit\t{generalUnit}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                            foreach (int i in Enumerable.Range(1, Rndm.Int(1, 4)))
                                c.Append($"\nunit\t{f.GetUnitMaxCost(399)}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                        }
                    }
                    // Add generals if starting cities exceed 5
                    if (noncaps.Count() + 1 > 5)
                    {
                        foreach (var noncap in noncaps.Skip(4))
                        {
                            var generalname = "";
                            do
                            {
                                generalname = f.GetRandomFullName();
                            } while (takennames.Contains(generalname));
                            takennames.Add(generalname);
                            c.Append($"\n\n;{Translator.NIDToName(generalname)} ({noncap.CityName})");
                            c.Append($"\ncharacter\t{generalname}, general, male, age 20, x {noncap.Position.X} y {noncap.Position.Y}\narmy");
                            foreach (int i in Enumerable.Range(1, Rndm.Int(1, 4)))
                                c.Append($"\nunit\t{f.GetUnitMaxCost(399)}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                        }
                    }
                    foreach (var pos in Rndm.Pick(World.AllPositions.Where(a => a.IsSuitableForResource && !a.IsCity && a.Owner == f.ID).ToList(), f.StartAgents))
                    {
                        var a = f.Culture == "mesoamerican" ? Rndm.Pick(Hardcoded.AgentsRecruitable.Where(a => a != "merchant")) : Rndm.Pick(Hardcoded.AgentsRecruitable);
                        var agentname = "";
                        do {
                            agentname = f.GetRandomFullName();
                        } while (takennames.Contains(agentname));
                        takennames.Add(agentname);
                        c.Append($"\n\n;{Translator.NIDToName(agentname)}");
                        c.Append($"\ncharacter\t{agentname}, {a}, male, age {Rndm.Int(20, 50)}, x {pos.X} y {pos.Y}");
                        if (a == "priest")
                            c.Append($"\ntraits Talent{a.Capitalize()} {Rndm.Int(2, 4)}, Education{a.Capitalize()} {Rndm.Int(3, 4)}, PriestLevel 1");
                        else
                            c.Append($"\ntraits Talent{a.Capitalize()} {Rndm.Int(1, 3)}, Education{a.Capitalize()} {Rndm.Int(1, 2)}");
                    }
                    if (f.HasPort)
                    {
                        Position fp;
                        var ok = false;
                        do
                        {
                            fp = Rndm.GetFleetPositionNearRegion(cap);
                            if (!takenFleetPositions.Contains($"{fp.X}|{fp.Y}"))
                            {
                                takenFleetPositions.Add($"{fp.X}|{fp.Y}");
                                ok = true;
                            }
                        } while (!ok);
                        var admiralname = "";
                        do {
                            admiralname = f.GetRandomFullName(false, true);
                        } while (takennames.Contains(admiralname));
                        takennames.Add(admiralname);
                        c.Append($"\n\n;{Translator.NIDToName(admiralname)}");
                        c.Append($"\ncharacter\t{admiralname}, admiral, male, age 20, x {fp.X} y {fp.Y}\narmy");
                        foreach (int i in 1.To(Rndm.Int(1, 2)))
                            c.Append($"\nunit\t{World.Units.Where(a => a.Factions.Contains(f.ID) && a.IsNaval).OrderBy(a => a.CostMoney).First().IntName}\texp 0 armour 0 weapon_lvl 0");
                    }
                    c.Append($"\n");
                    if (f.ID != Hardcoded.PapalFaction)
                    {
                        cnt = 0;
                        foreach (var female in females)
                        {
                            cnt++;
                            if (cnt == 1)
                            {
                                c.Append($"\n;{Translator.NIDToName(female)}");
                                c.Append($"\ncharacter_record\t{female}, \tfemale, age {age - 1}, alive, never_a_leader");
                            }
                            else
                            {
                                c.Append($"\n;{Translator.NIDToName(female)}");
                                c.Append($"\ncharacter_record\t{female}, \tfemale, age {age - 19 - cnt}, alive, never_a_leader");
                            }
                        }
                        cnt = 0;
                        foreach (var teen in teens)
                        {
                            cnt++;
                            c.Append($"\n;{Translator.NIDToName(teen)}");
                            c.Append($"\ncharacter_record\t{teen}, \tfemale, age {8 - (cnt * 2)}, alive, never_a_leader");
                        }
                        c.Append($"\n");
                        if (teens.Count > 0)
                        {
                            if (females.Count > 1)
                                c.Append($"\nrelative \t{males.First()}, \t{females.First()}, \t{String.Join(",\t", females.Skip(1))}, \t{String.Join(",\t", teens)},\tend");
                            else
                                c.Append($"\nrelative \t{males.First()}, \t{females.First()}, \t{String.Join(",\t", teens)},\tend");
                        }
                        else
                        {
                            c.Append($"\nrelative \t{males.First()}, \t{females.First()}, \t{String.Join(",\t", females.Skip(1))},\tend");
                        }
                        foreach (var mf in males.Skip(1).Zip(females.Skip(1), (m, f) => new { male = m, female = f }))
                            c.Append($"\nrelative \t{mf.male}, \t{mf.female},\tend");
                    }
                }
                else
                {
                    foreach (var r in World.Regions.Where(a => a.Owner == "slave"))
                    {
                        var sf = World.Factions.First(a => a.ID == r.HomeFaction);
                        var rebelgeneral = sf.GetRandomFullName();
                        c.Append($"\n\n;{Translator.NIDToName(rebelgeneral)} ({r.CityName})");
                        c.Append($"\ncharacter\tsub_faction {sf.ID}, {rebelgeneral}, named character, male, age {Rndm.Int(20, 40)}, x {r.Position.X} y {r.Position.Y}");
                        c.Append($"\ntraits ReligionStarter 1, LoyaltyStarter 1");
                        foreach (var t in Hardcoded.GoodBadTraits)
                            c.Append($", {Rndm.RandomWord("Good", "Bad")}{t} {Rndm.Int(1, 3)}");
                        var generalUnit = World.Units.First(a => a.Factions.Contains(sf.ID) && a.IsGeneral).IntName;
                        c.Append($"\narmy\nunit\t{generalUnit}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                        foreach (int i in Enumerable.Range(1, Rndm.Int(3, 8)))
                            c.Append($"\nunit\t{Rndm.Pick(World.RebelFactions.First(a => a.ID == r.Rebels).Units)}\texp {Rndm.Int(0, 3)} armour 0 weapon_lvl 0");
                    }
                }
            }
            c.Append("\n");
            foreach (var f1 in World.Factions.Where(a => a.ID != "slave"))
            {
                foreach (var f2 in World.Factions.Where(a => a.ID != "slave" && a.ID != f1.ID))
                {
                    var rs = f1.Religion != f2.Religion ? $"{Math.Round(Rndm.Dbl(-1.0, 0.0), 1)}" : $"{Math.Round(Rndm.Dbl(-0.1, 1.0), 1)}";
                    c.Append($"\nfaction_standings\t{f1.ID},\t{rs.Replace(",", ".")}\t{f2.ID}");
                }
                c.Append($"\nfaction_standings\t{f1.ID},\t-1\tslave");
                c.Append($"\nfaction_standings\tslave,\t-1\t{f1.ID}");
            }
            foreach (var f3 in World.Factions.Where(a => a.ID != "slave"))
                c.Append($"\nfaction_relationships\t{f3.ID}, at_war_with\tslave");
            c.Append($"\nfaction_relationships\tslave, at_war_with\t{String.Join(", ", World.Factions.Where(a => a.ID != "slave").Select(a => a.ID))}");
            c.Append($"\n\nscript\n{Hardcoded.CAMPAIGN.Split(@"\").Last()}\n");
            File.WriteAllText(Hardcoded.DESCR_STRAT, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_STRAT)} ({FO.GetSizeKB(Hardcoded.DESCR_STRAT)} KB)");
        }
        private static void GenerateRegions()
        {
            StringBuilder c = new StringBuilder();
            StringBuilder c2 = new StringBuilder();
            c.Append(";Generated by Ironclad\n;\n;\n;\n;\n");
            foreach(var r in World.Regions.OrderBy(a => a.CID).ToList())
            {
                var rgb = $"{r.RGB_R} {r.RGB_G} {r.RGB_B}";
                c.Append($"\n{r.RID}\n\tlegion: {r.CID.Replace("C", "L")}\n\t{r.CID}\n\t{r.HomeFaction}\n\t{r.Rebels}\n\t{rgb}\n\t{String.Join(", ", r.HiddenResources)}");
                c.Append($"\n\t5\n\t{Rndm.Int(r.FertilityMin, r.FertilityMax)}\n\treligions {{ {r.GenerateReligionsString()} }}");
                c2.Append($"\n{{{r.RID}}}{r.RegionName}\n{{{r.CID}}}{r.CityName}\n{{{r.CID.Replace("C","L")}}}{r.RegionName} ({r.CityName})");
            }
            File.WriteAllText(Hardcoded.DESCR_REGIONS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_REGIONS)} ({FO.GetSizeKB(Hardcoded.DESCR_REGIONS)} KB)");
            FO.WriteAllTextUCS2LEBOM(Hardcoded.IMPERIAL_CAMPAIGN_REGIONS_AND_SETTLEMENT_NAMES, c2.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.IMPERIAL_CAMPAIGN_REGIONS_AND_SETTLEMENT_NAMES)} ({FO.GetSizeKB(Hardcoded.IMPERIAL_CAMPAIGN_REGIONS_AND_SETTLEMENT_NAMES)} KB)");
        }
        private static void GenerateMusicTypes()
        {
            StringBuilder c = new StringBuilder();
            c.Append(String.Concat(Enumerable.Repeat(";DO NOT REMOVE THIS LINE OTHERWISE THE GAME CRASHES\n", 3)));
            foreach (var t in World.Regions.Select(a => a.MusicType).Distinct().ToList())
            {
                c.Append($"\nmusic_type {t}\n\tregions ");
                foreach (var r in World.Regions.Where(a => a.MusicType == t).ToList())
                    c.Append($"{r.RID} ");
                c.Append($"\n\tfactions ");
                foreach (var f in World.Factions.Where(a => a.MusicType == t).ToList())
                    c.Append($"{f.ID} ");
                c.Append("\n");
            }
            File.WriteAllText(Hardcoded.DESCR_SOUNDS_MUSIC_TYPES, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.DESCR_SOUNDS_MUSIC_TYPES)} ({FO.GetSizeKB(Hardcoded.DESCR_SOUNDS_MUSIC_TYPES)} KB)");
        }
        private static void GenerateBuildings()
        {
            StringBuilder c = new StringBuilder();
            StringBuilder c2 = new StringBuilder();
            c2.Append($"\n{{generic_repair}}Repair\n{{generic_repair_desc}}Repair\n{{generic_repair_desc_short}}Repair");
            c2.Append($"\n{{generic_dismantle}}Dismantle\n{{generic_dismantle_desc}}Dismantle\n{{generic_dismantle_desc_short}}Dismantle");
            c.Append($"\nhidden_resources {String.Join(" ", World.HiddenResources.Select(a => a.IntName).ToList())}\n");
            foreach(var bc in World.BuildingChains.OrderBy(a => a.BaseCostMoney).ThenBy(a => a.ID).ToList())
            {
                c2.Append($"\n{{{bc.ID}_name}}{bc.Name}");
                var bldngs = from b in World.Buildings where b.Chain.Equals(bc.ID) orderby b.Level ascending select b;
                c.Append($"\nbuilding {bc.ID}\n{{");
                if (bc.ConvertTo != "NULL")
                    c.Append($"\n\tconvert_to {bc.ConvertTo}");
                if (bc.Religion != "NULL")
                    c.Append($"\n\treligion {bc.Religion}");
                c.Append($"\n\tlevels {String.Join(" ", bldngs.Select(a => a.ID).ToList())}\n\t{{");
                foreach(var b in bldngs)
                {
                    c2.Append($"\n{{{b.ID}}}{b.Name}");
                    if (b.EffectDesc.Contains("*"))
                        c2.Append($"\n{{{b.ID}_desc}}{b.EffectDesc}\\n\\nMaterial: {bc.Material.Capitalize()}\\n\\n* May not apply to your faction\\n\\n{bc.Descr}");
                    else
                        c2.Append($"\n{{{b.ID}_desc}}{b.EffectDesc}\\n\\nMaterial: {bc.Material.Capitalize()}\\n\\n{bc.Descr}");
                    c2.Append($"\n{{{b.ID}_desc_short}}{b.EffectDesc}");
                    var q = from x in bldngs where x.Level == 1 select x.MinSettlementLevel;
                    var costLevel = q.First() + b.Level;
                    var costMoney = bc.BaseCostMoney * Tuner.BuildingCostMultiplier[costLevel];
                    b.CostMoney = Convert.ToInt32(costMoney);
                    var costRounds = bc.BaseCostRounds + costLevel - 1;
                    var q2 = from x in bldngs where x.Level == b.Level + 1 select x.ID;
                    var u = q2.Any() && bc.CanUpgrade ? "\t\t\t\t" + q2.First() + "\n" : "";
                    c.Append($"\n\t\t{b.ID} {bc.SettlementType.Rem("both")}");
                    if (b.Requirements != "NULL" && b.Resources.IsEmpty())
                        c.Append($" requires {b.Requirements}");
                    if (b.Requirements != "NULL" && !b.Resources.IsEmpty())
                        c.Append($" requires {b.Requirements} and resource {string.Join(" or resource ", b.Resources)}");
                    if (b.Requirements == "NULL" && !b.Resources.IsEmpty())
                        c.Append($" requires resource {string.Join(" or resource ", b.Resources)}");
                    c.Append($"\n\t\t{{");
                    if (b.CanConvert)
                        c.Append($"\n\t\t\tconvert_to {Convert.ToString(b.Level - 1)}");
                    var caps = b.Capabilities == "NULL" ? "" : b.Capabilities;
                    c.Append($"\n\t\t\tcapability\n\t\t\t{{\n\t\t\t\t{caps.Replace("\n", "\n\t\t\t\t")}\n\t\t\t}}");
                    if (b.FactionCapabilities != "NULL")
                        c.Append($"\n\t\t\tfaction_capability\n\t\t\t{{\n\t\t\t\t{b.FactionCapabilities.Replace("\n", "\n\t\t\t\t")}\n\t\t\t}}");
                    c.Append($"\n\t\t\tmaterial {bc.Material}\n\t\t\tconstruction {costRounds}\n\t\t\tcost {costMoney}");
                c.Append($"\n\t\t\tsettlement_min {Translator.CityLevelToCityLevelName(b.MinSettlementLevel, World.SettlementLevels)}\n\t\t\tupgrades\n\t\t\t{{\n{u}\t\t\t}}\n\t\t}}");
                }
                c.Append($"\n\t}}\n\tplugins\n\t{{\n\t}}\n}}");
            }
            File.WriteAllText(Hardcoded.EXPORT_DESCR_BUILDINGS, c.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.EXPORT_DESCR_BUILDINGS)} ({FO.GetSizeKB(Hardcoded.EXPORT_DESCR_BUILDINGS)} KB)");
            FO.WriteAllTextUCS2LEBOM(Hardcoded.EXPORT_BUILDINGS, c2.ToString().Replace("\n", "\r\n"));
            IO.Log($"Generated {FO.GetFileName(Hardcoded.EXPORT_BUILDINGS)} ({FO.GetSizeKB(Hardcoded.EXPORT_BUILDINGS)} KB)");
        }
    }
}
