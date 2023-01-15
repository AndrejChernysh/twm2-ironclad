using System.Collections.Generic;
using System.Linq;

namespace Ironclad.Entities
{
    class RebelFaction
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Chance { get; set; }
        public List<string> Units { get; set; }

        public RebelFaction(string intName, string extName, string category, int chance, List<string> units)
        {
            ID = intName;
            Name = extName;
            Category = category;
            Chance = chance;
            Units = units.Select(a => a.Trim()).ToList();
        }
    }
}
