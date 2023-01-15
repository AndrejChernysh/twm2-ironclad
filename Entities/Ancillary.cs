using System.Collections.Generic;

namespace Ironclad.Entities
{
    class Ancillary
    {
        public string IntName { get; set; }
        public string ExtName { get; set; }
        public bool IsTransferable { get; set; }
        public List<Effect> Effects { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ExcludeCultures { get; set; }
        public string ExcludeAncillaries { get; set; }
        public bool IsUnique { get; set; }

        public Ancillary(string intName, string extName, bool isTransferable, List<Effect> effects, string description, string image, string excludeCultures, string excludeAncillaries, bool isUnique)
        {
            IntName = intName;
            ExtName = extName;
            IsTransferable = isTransferable;
            Effects = effects;
            Description = description;
            Image = image;
            ExcludeCultures = excludeCultures;
            ExcludeAncillaries = excludeAncillaries;
            IsUnique = isUnique;
        }
    }
}
