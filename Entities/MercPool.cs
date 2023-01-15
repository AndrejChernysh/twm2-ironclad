using System;
using System.Linq;
using System.Text;

namespace Ironclad.Entities
{
    class MercPool
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Experience { get; set; }
        public decimal ReplenishMin { get; set; }
        public decimal ReplenishMax { get; set; }
        public int Maximum { get; set; }
        public int Initial { get; set; }

        public MercPool(string name, string unit, int experience, decimal replenishMin, decimal replenishMax, int maximum, int initial)
        {
            Name = name;
            Unit = unit;
            Experience = experience;
            ReplenishMin = replenishMin;
            ReplenishMax = replenishMax;
            Maximum = maximum;
            Initial = initial;
        }

        public string GetDescrMercenariesEntry()
        {
            var sb = new StringBuilder();
            var regions = from r in World.Regions where r.MercPool == Name select r.RID;
            sb.Append($"pool {Name}\n\tregions {string.Join(" ", regions)}");
            foreach(var e in World.MercPools.Where(a => a.Name == Name).ToList())
                sb.Append($"\n\tunit {e.Unit}\texp {e.Experience} cost {World.Units.First(a => a.IntName.Equals(e.Unit)).CostMoney} replenish {e.ReplenishMin} - {e.ReplenishMax} max {e.Maximum} initial {e.Initial}");
            return sb.ToString();
        }
    }
}
