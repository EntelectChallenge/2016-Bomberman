using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Reference.Serialization;

namespace Reference.Domain.Map
{
    public class GameMap
    {
        public int MapSeed { get; set; }
        public int MapHeight { get; set; }
        public int MapWidth { get; set; }
        public GameBlock[,] GameBlocks { get; set; }

        /// <summary>
        /// Retrieves the game block at the specified X and Y location.  Game locations start at 1 up to game width/height
        /// </summary>
        /// <param name="x">X coordinates</param>
        /// <param name="y">Y coordinates</param>
        /// <returns>The game block found at the specified location</returns>
        public GameBlock GetBlockAtLocation(int x, int y)
        {
            return GameBlocks[x - 1, y - 1];
        }

        public static GameMap FromJson(String map)
        {
            return JsonConvert.DeserializeObject<GameMap>(map, new JsonSerializerSettings()
            {
                Binder = new EntityTypeNameHandling()
            });
        }
    }
}
