using Ironclad.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Ironclad.Entities
{
    class Unit
    {
        public string IntName { get; set; }
        public string DictName { get; set; }
        public List<string> Factions { get; set; }
        public bool IsGeneral { get; set; }
        public int CostMoney { get; set; }
        public bool IsNaval { get; set; }
        public List<string> UnitModels { get; set; }
        public bool IsMercenary { get; set; }
        public bool IsCatapult { get; set; }
        public bool IsRecruitable { get; set; }
        public bool IsPeasant { get; set; }
        public string Class { get; set; }
        public string Category { get; set; }


        public Unit(string intName, string dictName, List<string> factions, bool isGeneral, int costMoney, bool isNaval, string unitModelsString, bool isMercenary, bool isCatapult, string unitClass, bool isPeasant, string category)
        {
            var tempListModels = new List<string>();
            IsMercenary = isMercenary;
            IntName = intName;
            DictName = dictName;
            Factions = factions;
            IsGeneral = isGeneral;
            CostMoney = IsMercenary ? costMoney * Tuner.MercenaryCostMultiplier : costMoney;
            IsNaval = isNaval;
            IsCatapult = isCatapult;
            IsPeasant = isPeasant;
            if (!unitModelsString.Contains(","))
                tempListModels.Add(unitModelsString);
            else
            {
                foreach (var unitModel in unitModelsString.Split(","))
                    tempListModels.Add(unitModel);
            }
            UnitModels = tempListModels;
            IsRecruitable = !(IsGeneral || IsMercenary || (Factions.First() == "slave" && Factions.Count == 1));
            Class = unitClass;
            Category = category;
        }
    }
}
