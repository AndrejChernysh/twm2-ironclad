using Ironclad.Extensions;
using Ironclad.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ironclad.Entities
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string GroundType { get; set; }
        public bool IsCity { get; set; }
        public bool IsPort { get; set; }
        public bool IsAccessible { get; set; }
        public int RegionID { get; set; }
        public string Owner { get; set; }
        public bool IsSuitableForResource { get; set; }
        public bool IsSea { get; set; }
        public bool IsHighSea { get; set; }

        public Position(int x, int y, string groundType, bool isCity, bool isPort, bool isAccessible, int regionID, string owner, bool isSuitableForResource, bool isSea, bool isHighSea) : this(x, y)
        {
            GroundType = groundType;
            IsCity = isCity;
            IsPort = isPort;
            IsAccessible = isAccessible;
            RegionID = regionID;
            Owner = owner;
            IsSuitableForResource = isSuitableForResource;
            IsSea = isSea;
            IsHighSea = isHighSea;
        }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
            if (x > -1 && y > -1)
            {
                var gt_x = x * 2 + 1;
                var gt_y = (y + 1) * 2;
                var c = Map.GroundTypes.GetPixel(gt_x, Map.GroundTypes.Height - gt_y);
                GroundType = "Impassable";
                if (c.R == 0 && c.G == 128 && c.B == 128)
                    GroundType = "FertileLow";
                if (c.R == 96 && c.G == 160 && c.B == 64)
                    GroundType = "FertileMedium";
                if (c.R == 101 && c.G == 124 && c.B == 0)
                    GroundType = "FertileHigh";
                if (c.R == 0 && c.G == 0 && c.B == 0)
                    GroundType = "Wilderness";
                if (c.R == 196 && c.G == 128 && c.B == 128)
                    GroundType = "MountainsHigh";
                if (c.R == 98 && c.G == 65 && c.B == 65)
                    GroundType = "MountainsLow";
                if (c.R == 128 && c.G == 128 && c.B == 64)
                    GroundType = "Hills";
                if (c.R == 0 && c.G == 64 && c.B == 0)
                    GroundType = "ForestDense";
                if (c.R == 0 && c.G == 128 && c.B == 0)
                    GroundType = "ForestSparse";
                if (c.R == 0 && c.G == 255 && c.B == 128)
                    GroundType = "Swamp";
                if (c.R == 64 && c.G == 0 && c.B == 0)
                    GroundType = "Ocean";
                if (c.R == 128 && c.G == 0 && c.B == 0)
                    GroundType = "SeaDeep";
                if (c.R == 196 && c.G == 0 && c.B == 0)
                    GroundType = "SeaShallow";
                if (c.R == 255 && c.G == 255 && c.B == 255)
                    GroundType = "Beach";
                c = Map.Features.GetPixel(x, Map.Features.Height - y - 1);
                IsSea = GroundType == "SeaShallow" || GroundType == "SeaDeep" || GroundType == "Ocean";
                IsHighSea = GroundType == "Ocean";
                var frgb = $"{c.R}|{c.G}|{c.B}";
                IsAccessible = !Hardcoded.InaccessibleGroundTypes.Contains(GroundType) && (frgb.Equals("0|0|0") || frgb.Equals("0|255|255"));
                IsSuitableForResource = !Hardcoded.InaccessibleGroundTypes.Contains(GroundType) && frgb.Equals("0|0|0");
                c = Map.Regions.GetPixel(x, Map.Regions.Height - y - 1);
                IsCity = $"{c.R}|{c.G}|{c.B}".Equals("0|0|0");
                IsPort = $"{c.R}|{c.G}|{c.B}".Equals("255|255|255");
                if (IsCity || IsPort)
                {
                    c = Map.Regions.GetPixel(x + 1, Map.Features.Height - 1 - y);
                    if (!World.Regions.Any(a => a.RGB_R == c.R && a.RGB_G == c.G && a.RGB_B == c.B))
                        c = Map.Regions.GetPixel(x - 1, Map.Features.Height - 1 - y);
                    if (!World.Regions.Any(a => a.RGB_R == c.R && a.RGB_G == c.G && a.RGB_B == c.B))
                        c = Map.Regions.GetPixel(x, Map.Features.Height - 1 - y + 1);
                    if (!World.Regions.Any(a => a.RGB_R == c.R && a.RGB_G == c.G && a.RGB_B == c.B))
                        c = Map.Regions.GetPixel(x, Map.Features.Height - 1 - y - 1);
                }
                if (World.Regions.Where(a => $"{a.RGB_R}|{a.RGB_G}|{a.RGB_B}".Equals($"{c.R}|{c.G}|{c.B}")).Count() == 1)
                {
                    RegionID = World.Regions.First(a => $"{a.RGB_R}|{a.RGB_G}|{a.RGB_B}".Equals($"{c.R}|{c.G}|{c.B}")).ID;
                    Owner = World.Regions.First(a => $"{a.RGB_R}|{a.RGB_G}|{a.RGB_B}".Equals($"{c.R}|{c.G}|{c.B}")).Owner;
                } else
                {
                    RegionID = 0;
                    Owner = "";
                }
                // Is potential resource on coast?
                if (IsSuitableForResource)
                {
                    try
                    {
                        var n = Map.Regions.GetPixel(x + 1, Map.Regions.Height - y - 1);
                        if ($"{n.R}|{n.G}|{n.B}".Equals("41|140|233") || $"{n.R}|{n.G}|{n.B}".Equals("41|140|235") || $"{n.R}|{n.G}|{n.B}".Equals("41|141|243"))
                            IsSuitableForResource = false;
                        n = Map.Regions.GetPixel(x - 1, Map.Regions.Height - y - 1);
                        if ($"{n.R}|{n.G}|{n.B}".Equals("41|140|233") || $"{n.R}|{n.G}|{n.B}".Equals("41|140|235") || $"{n.R}|{n.G}|{n.B}".Equals("41|141|243"))
                            IsSuitableForResource = false;
                        n = Map.Regions.GetPixel(x, Map.Regions.Height - y - 1 + 1);
                        if ($"{n.R}|{n.G}|{n.B}".Equals("41|140|233") || $"{n.R}|{n.G}|{n.B}".Equals("41|140|235") || $"{n.R}|{n.G}|{n.B}".Equals("41|141|243"))
                            IsSuitableForResource = false;
                        n = Map.Regions.GetPixel(x, Map.Regions.Height - y - 1 - 1);
                        if ($"{n.R}|{n.G}|{n.B}".Equals("41|140|233") || $"{n.R}|{n.G}|{n.B}".Equals("41|140|235") || $"{n.R}|{n.G}|{n.B}".Equals("41|141|243"))
                            IsSuitableForResource = false;
                    }
                    catch
                    {
                        IsSuitableForResource = false;
                    }
                }
            }
        }
        public string SaveToFile()
        {
            return $"{X};{Y};{GroundType};{IsCity};{IsPort};{IsAccessible};{RegionID};{Owner};{IsSuitableForResource};{IsSea};{IsHighSea}";
        }
        internal static Position LoadFromLine(string s)
        {
            var v = s.Split(";");
            return new Position(v[0].ToInt(), v[1].ToInt(), v[2], v[3].ToBool(), v[4].ToBool(), v[5].ToBool(), v[6].ToInt(), v[7], v[8].ToBool(), v[9].ToBool(), v[10].ToBool());
        }
    }
}
