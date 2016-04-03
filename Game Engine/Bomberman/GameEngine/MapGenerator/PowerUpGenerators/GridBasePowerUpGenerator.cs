using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;

// The idea with this one is to sub-divide each quadrant up into slightly lopsided sub-quadrants...
// On a 21x21 map a quadrant is 9x9, so for player one (top left) divide the 6 power ups into the sub-quadrants as follows:
// ....+++++
// .  .+   +
// . 1.+  1+
// ....+++++
// ----*****
// -  -*   *
// -  -*   *
// - 1-*  3*
// ----*****
// The power-up can spawn in any brick square inside that sub-quadrant
namespace GameEngine.MapGenerator.PowerUpGenerators {
    class GridBasePowerUpGenerator : IPowerUpGenerator {
        public Random Rand { get; set; }

        public void GeneratePowerUps(GameMap map) {
            throw new NotImplementedException();
        }

        public bool IsMapSuitable(GameMap map) {
            throw new NotImplementedException();
        }

    }
}
