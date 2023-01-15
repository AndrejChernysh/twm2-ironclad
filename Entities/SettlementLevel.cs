using System;
using System.Collections.Generic;
using System.Text;

namespace Ironclad.Entities
{
    class SettlementLevel
    {
        public int Level { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Building { get; set; }
        public int PopMin { get; set; }
        public int PopMax { get; set; }

        public SettlementLevel(int level, string type, string name, string building, int popmin, int popmax)
        {
            Level = level;
            Type = type;
            Name = name;
            Building = building;
            PopMin = popmin;
            PopMax = popmax;
        }
    }
}
