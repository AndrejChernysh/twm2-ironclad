using Ironclad.Helper;

namespace Ironclad.Entities
{
    class Name
    {
        public string NameSet { get; set; }
        public string Type { get; set; }
        public string Txt { get; set; }
        public int ID { get; set; }
        public string NID { get; set; }

        public Name(string nameSet, string type, string txt, int id)
        {
            NameSet = nameSet;
            Type = type;
            Txt = txt;
            ID = id;
            NID = Translator.NToA("N" + id.ToString("D4"));
        }
    }
}
