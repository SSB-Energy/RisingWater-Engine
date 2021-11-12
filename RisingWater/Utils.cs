using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    public class Utils
    {
        /// <summary>
        /// screen dimensions
        /// </summary>
        public static Vector2 ScreenDims => Program.RATIO * Program.SCALE;

        /// <summary>
        /// The screen height in tile count
        /// </summary>
        public const float HEIGHT_TILES = 18;
        /// <summary>
        /// The screen width in tile count
        /// </summary>
        public const float WIDTH_TILES = 32;

        /// <summary>
        /// The tile size
        /// </summary>
        public static float TileSize => ScreenDims.Y / HEIGHT_TILES;

        /// <summary>
        /// Returns a vector whose dimensions you can pass into render functions
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 Renderable(Vector2 value) => value * TileSize;
    }
}
