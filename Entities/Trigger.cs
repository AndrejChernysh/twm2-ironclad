using System;
using System.Collections.Generic;
using System.Text;

namespace Ironclad.Entities
{
    class Trigger
    {
        public string Name { get; set; }
        public string WhenToTest { get; set; }
        public List<String> Conditions { get; set; }
        public List<Effect> Effects { get; set; }

        public Trigger(string name, string whenToTest, List<string> conditions, List<Effect> effects)
        {
            Name = name;
            WhenToTest = whenToTest;
            Conditions = conditions;
            Effects = effects;
        }
    }
}
