
using Ironclad.Helper;

namespace Ironclad.Entities
{
    public class Resource
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Model { get; set; }
        public string Icon { get; set; }
        public bool HasMine { get; set; }
        public bool IsNewWorld { get; set; }
        public string Consumable { get; set; }
        public string ConsumableText { get; set; }
        public int Occurances { get; set; }

        public Resource(string intName, string extName, int value, string model, string icon, bool hasMine, bool isNewWorld, string consumable, int min, int max)
        {
            ID = intName;
            Name = extName;
            Value = value;
            Model = model;
            Icon = icon;
            HasMine = hasMine;
            IsNewWorld = isNewWorld;
            Consumable = consumable == "NULL" ? "NULL" : consumable.Substring(0, 2);
            ConsumableText = consumable == "NULL" ? "NULL" : consumable;
            Occurances = Rndm.Int(min, max);
        }
    }
}
