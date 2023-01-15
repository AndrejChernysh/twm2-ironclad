using System;
using System.Collections.Generic;
using System.Text;

namespace Ironclad.Entities
{
    class Trait
    {
        public string IntName { get; set; }
        public string IntLevelName { get; set; }
        public string ExtLevelName { get; set; }
        public string Character { get; set; }
        public List<string> AntiTraits { get; set; }
        public int Threshold { get; set; }
        public List<Effect> Effects { get; set; }
        public int NoGoingBackLevel { get; set; }
        public string ExcludeCultures { get; set; }
        public string Description { get; set; }
        public string GainMessage { get; set; }
        public string LoseMessage { get; set; }

        public Trait(string intName, string intLevelName, string extLevelName, string character, List<string> antiTraits, int threshold, List<Effect> effects, int noGoingBackLevel, string excludeCultures, string description, string gainMessage, string loseMessage)
        {
            IntName = intName;
            IntLevelName = intLevelName;
            ExtLevelName = extLevelName;
            Character = character;
            AntiTraits = antiTraits;
            Threshold = threshold;
            Effects = effects;
            NoGoingBackLevel = noGoingBackLevel;
            ExcludeCultures = excludeCultures;
            Description = description;
            GainMessage = gainMessage;
            LoseMessage = loseMessage;
        }
    }
}
