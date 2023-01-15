using Ironclad.Helper;
using System.Drawing;

namespace Ironclad.Entities
{
    internal class Map
    {
        public static Bitmap Regions = TgaDecoder.FromFile(Hardcoded.MAP_REGIONS);
        public static Bitmap GroundTypes = TgaDecoder.FromFile(Hardcoded.MAP_GROUND_TYPES);
        public static Bitmap Features = TgaDecoder.FromFile(Hardcoded.MAP_FEATURES);
        public static Bitmap Climates = TgaDecoder.FromFile(Hardcoded.MAP_CLIMATES);
        public static Bitmap Fog = TgaDecoder.FromFile(Hardcoded.MAP_FOG);
        public static Bitmap Heights = TgaDecoder.FromFile(Hardcoded.MAP_HEIGHTS);
        public static int Width = Regions.Width;
        public static int Height = Regions.Height;
    }
}
