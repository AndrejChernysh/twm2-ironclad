
namespace Ironclad.Entities
{
    class BuildingChain
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ConvertTo { get; set; }
        public string Religion { get; set; }
        public string SettlementType { get; set; }
        public int BaseCostRounds { get; set; }
        public int BaseCostMoney { get; set; }
        public string Material { get; set; }
        public bool CanUpgrade { get; set; }
        public bool ForRandom { get; set; }
        public string Descr { get; set; }
        public bool IsHidden { get; set; }

        public BuildingChain(string intName, string extName, string convertTo, string religion, string settlementType, int baseCostRounds, int baseCostMoney, string material, bool canUpgrade, bool forRandom, string descr, bool isHidden)
        {
            ID = intName;
            Name = extName;
            ConvertTo = convertTo;
            Religion = religion;
            SettlementType = settlementType;
            BaseCostRounds = baseCostRounds;
            BaseCostMoney = baseCostMoney;
            Material = material;
            CanUpgrade = canUpgrade;
            ForRandom = forRandom;
            Descr = descr;
            IsHidden = isHidden;
        }
    }
}
