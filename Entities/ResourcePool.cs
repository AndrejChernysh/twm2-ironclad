using System;
using System.Collections.Generic;
using System.Text;

namespace Ironclad.Entities
{
    class ResourcePool
    {
        public string Name { get; set; }
        public bool HasGold { get; set; }
        public bool HasSilver { get; set; }
        public bool HasSpices { get; set; }
        public bool HasSilk { get; set; }
        public bool HasIvory { get; set; }
        public bool HasSulfur { get; set; }
        public bool HasTin { get; set; }
        public bool HasIron { get; set; }
        public bool HasMarble { get; set; }
        public bool HasDyes { get; set; }
        public bool HasSugar { get; set; }
        public bool HasCoal { get; set; }
        public bool HasCamels { get; set; }
        public bool HasAmber { get; set; }
        public bool HasElephants { get; set; }
        public bool HasWine { get; set; }
        public bool HasTimber { get; set; }
        public bool HasChocolate { get; set; }
        public bool HasFurs { get; set; }
        public bool HasSlaves { get; set; }
        public bool HasTextiles { get; set; }
        public bool HasCotton { get; set; }
        public bool HasDogs { get; set; }
        public bool HasWool { get; set; }
        public bool HasGrain { get; set; }
        public bool HasTobacco { get; set; }
        public bool HasFish { get; set; }

        public ResourcePool(string name, string hasGold, string hasSilver, string hasSpices, string hasSilk, string hasIvory, string hasSulfur, string hasTin, string hasIron, string hasMarble, string hasDyes, string hasSugar, string hasCoal, string hasCamels, string hasAmber, string hasElephants, string hasWine, string hasTimber, string hasChocolate, string hasFurs, string hasSlaves, string hasTextiles, string hasCotton, string hasDogs, string hasWool, string hasGrain, string hasTobacco, string hasFish)
        {
            Name = name;
            HasGold = hasGold == "1";
            HasSilver = hasSilver == "1";
            HasSpices = hasSpices == "1";
            HasSilk = hasSilk == "1";
            HasIvory = hasIvory == "1";
            HasSulfur = hasSulfur == "1";
            HasTin = hasTin == "1";
            HasIron = hasIron == "1";
            HasMarble = hasMarble == "1";
            HasDyes = hasDyes == "1";
            HasSugar = hasSugar == "1";
            HasCoal = hasCoal == "1";
            HasCamels = hasCamels == "1";
            HasAmber = hasAmber == "1";
            HasElephants = hasElephants == "1";
            HasWine = hasWine == "1";
            HasTimber = hasTimber == "1";
            HasChocolate = hasChocolate == "1";
            HasFurs = hasFurs == "1";
            HasSlaves = hasSlaves == "1";
            HasTextiles = hasTextiles == "1";
            HasCotton = hasCotton == "1";
            HasDogs = hasDogs == "1";
            HasWool = hasWool == "1";
            HasGrain = hasGrain == "1";
            HasTobacco = hasTobacco == "1";
            HasFish = hasFish == "1";
        }
        public List<string> getResourceIDs()
        {
            var result = new List<string>() { };
            if (HasGold)
                result.Add("gold");
            if (HasSilver)
                result.Add("silver");
            if (HasSpices)
                result.Add("spices");
            if (HasSilk)
                result.Add("silk");
            if (HasIvory)
                result.Add("ivory");
            if (HasSulfur)
                result.Add("sulfur");
            if (HasTin)
                result.Add("tin");
            if (HasIron)
                result.Add("iron");
            if (HasMarble)
                result.Add("marble");
            if (HasDyes)
                result.Add("dyes");
            if (HasSugar)
                result.Add("sugar");
            if (HasCoal)
                result.Add("coal");
            if (HasCamels)
                result.Add("camels");
            if (HasAmber)
                result.Add("amber");
            if (HasElephants)
                result.Add("elephants");
            if (HasWine)
                result.Add("wine");
            if (HasTimber)
                result.Add("timber");
            if (HasChocolate)
                result.Add("chocolate");
            if (HasFurs)
                result.Add("furs");
            if (HasSlaves)
                result.Add("slaves");
            if (HasTextiles)
                result.Add("textiles");
            if (HasCotton)
                result.Add("cotton");
            if (HasDogs)
                result.Add("dogs");
            if (HasWool)
                result.Add("wool");
            if (HasGrain)
                result.Add("grain");
            if (HasTobacco)
                result.Add("tobacco");
            if (HasFish)
                result.Add("fish");
            return result;
        }
    }
}
