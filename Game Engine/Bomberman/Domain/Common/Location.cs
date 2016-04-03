using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Location
    {
        public readonly int _x;
        public readonly int _y;

        public Location(int x, int y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// The X location on the game map, value between 1 and game map width
        /// </summary>
        [JsonProperty]
        public int X
        {
            get { return _x; }
        }

        /// <summary>
        /// The Y location on the game map, value between 1 and game map height
        /// </summary>
        [JsonProperty]
        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Checks if the two location objects point to the same X and Y coordinates
        /// </summary>
        /// <param name="location">The location to compare</param>
        /// <returns>True if the locations have the same X and Y</returns>
        public bool IsSameCoordinates(Location location)
        {
            return _x == location._x && _y == location._y;
        }

        /// <summary>
        /// Checks if the location is on an even block for both X and Y coordinates
        /// </summary>
        /// <returns></returns>
        public bool IsEven()
        {
            return _x%2 == 0 && _y%2 == 0;
        }

        /// <summary>
        /// Checks if this location is on an odd block for both X and Y coordinates
        /// </summary>
        /// <returns></returns>
        public bool IsOdd()
        {
            return _x % 2 != 0 && _y % 2 != 0;
        }

        public override string ToString()
        {
            return String.Format("Location(X:{0},Y:{1})", X, Y);
        }
    }
}
