using Ironclad.Extensions;
using Ironclad.Helper;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ironclad.Entities
{
    class Building
    {
        public string Chain { get; set; }
        public int Level { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Requirements { get; set; }
        public List<string> Resources { get; set; }
        public bool CanConvert { get; set; }
        public string Capabilities { get; set; }
        public string FactionCapabilities { get; set; }
        public int MinSettlementLevel { get; set; }
        public string EffectDesc { get; set; }
        public bool ForRandom { get; set; }
        public string MsgCompletionTitle { get; set; }
        public string MsgCompletionBody { get; set; }
        public string VideoCompletion { get; set; }
        public string PicCompletion { get; set; }
        public int CostMoney { get; set; }

        public Building(string chain, int level, string intName, string extName, string requirements, string resources, bool canConvert, string capabilities, string factionCapabilities, int minSettlementLevel, string factionBonus, string effectDesc, string completionMessageTitle, string completionMessageBody, string videoCompletion, string picCompletion)
        {
            Chain = chain;
            Level = level;
            ID = intName;
            Name = extName;
            if (requirements.Contains("religions")) {
                var givenReligion = requirements.Rem("religions", "{", "}", ",").Trim();
                requirements = $"factions {{ {String.Join(", ", World.Factions.Where(a => a.Religion == givenReligion).Select(a => a.ID).ToList())}, }}";
            }
            Requirements = Translator.ReplaceCulturesWithFactions(requirements);
            if (resources.Contains(","))
                Resources = resources.Rem(" ").Split(",").ToList();
            if (!resources.Contains(","))
                Resources = new List<string> { resources.Rem(" ") };
            if (resources.Contains("NULL"))
                Resources = new List<string> { };
            if (!Resources.IsEmpty())
                foreach (var r in Resources)
                    IO.Val(World.Resources.Any(a => a.ID == r), $"Resource {r} as requirement for building {ID} is invalid");
            CanConvert = canConvert;
            if (factionBonus != "NULL")
                capabilities = capabilities + "\n" + factionBonus;
            if (capabilities.Contains("\n"))
            {
                var listCapabilities = capabilities.Split("\n");
                var listCapabilitiesChecked = new List<string>();
                foreach (var singleCapability in listCapabilities)
                {
                    if (singleCapability.Contains("requires") && singleCapability.Contains("religions"))
                    {
                        var givenReligion = singleCapability.Split("religions")[1].Rem("{", "}", ",").Trim();
                        listCapabilitiesChecked.Add(singleCapability.Split("requires")[0] + $" requires factions {{ {String.Join(", ", World.Factions.Where(a => a.Religion == givenReligion).Select(a => a.ID).ToList())}, }}");
                    } else
                    {
                        listCapabilitiesChecked.Add(singleCapability);
                    }
                }
                capabilities = String.Join("\n", listCapabilitiesChecked);
            } else
            {
                if (capabilities.Contains("requires") && capabilities.Contains("religions"))
                {
                    var givenReligion = capabilities.Split("religions")[1].Rem("{", "}", ",").Trim();
                    capabilities = capabilities.Split("requires")[0] + $" requires factions {{ {String.Join(", ", World.Factions.Where(a => a.Religion == givenReligion).Select(a => a.ID).ToList())}, }}";
                }
            }
            Capabilities = capabilities;
            if (World.BuildingChains.First(a => a.ID == Chain).Religion == "catholic")
                Capabilities = $"{Capabilities}\npope_disapproval 1\npope_approval 1";
            FactionCapabilities = factionCapabilities;
            MinSettlementLevel = minSettlementLevel;
            ForRandom = World.BuildingChains.First(a => a.ID == Chain).ForRandom;
            EffectDesc = effectDesc == "NULL" ? Translator.BuildingEffectsToEffectDescs(Capabilities) : $"{Translator.BuildingEffectsToEffectDescs(Capabilities)}\\n{effectDesc}";
            if (EffectDesc == "")
                EffectDesc = " ";
            if (EffectDesc.StartsWith("\\n"))
                EffectDesc = EffectDesc.Remove(0, 2);
            MsgCompletionTitle = completionMessageTitle;
            MsgCompletionBody = completionMessageBody;
            VideoCompletion = videoCompletion == "NULL" ? "" : videoCompletion;
            PicCompletion = picCompletion;
        }
        public bool canBeBuiltInCastle()
        {
            if (World.BuildingChains.First(a => a.ID == Chain).SettlementType == "castle" || World.BuildingChains.First(a => a.ID == Chain).SettlementType == "both")
                return true;
            return false;
        }
        public bool canBeBuiltInCity()
        {
            if (World.BuildingChains.First(a => a.ID == Chain).SettlementType == "city" || World.BuildingChains.First(a => a.ID == Chain).SettlementType == "both")
                return true;
            return false;
        }
    }
}
