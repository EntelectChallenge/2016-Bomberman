using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;

namespace GameEngine.MapGenerator {
    public interface IPowerUpGenerator {

        Random Rand { get; set; }

        void GeneratePowerUps(GameMap map);
        bool IsMapSuitable(GameMap map);
    }
}
