using System;
using System.Collections.Generic;
using System.Text;

namespace Ironclad.Entities
{
    public class Effect
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Probability { get; set; }

        public Effect(string name, int level, int probability)
        {
            Name = name;
            Level = level;
            Probability = probability;
        }

        public Effect(string String, int probability)
        {
            if (String == "NULL") {
                Name = "NULL";
                Level = -1;
                Probability = -1;
            } else
            {
                if (String.Contains("AcquireAncillary"))
                {
                    Name = String;
                    Level = -1;
                } else
                {
                    Name = String.Split(" ")[0];
                    Level = Convert.ToInt32(String.Split(" ")[1]);
                }
                Probability = probability;
            }
        }
        public Effect(string String)
        {
            if (String == "NULL")
            {
                Name = "NULL";
                Level = -1;
                Probability = -1;
            }
            else
            {
                if (String.Contains("AcquireAncillary"))
                {
                    Name = String;
                    Level = -1;
                } else
                {
                    Name = String.Split(" ")[0];
                    Level = Convert.ToInt32(String.Split(" ")[1]);
                }
                Probability = 100;
            }
        }
    }
}
