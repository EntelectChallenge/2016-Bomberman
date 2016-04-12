using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Newtonsoft.Json;

namespace Domain.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameMap : IEnumerable<GameBlock>
    {
        [JsonIgnore]
        private readonly int _mapSeed;
        [JsonIgnore]
        private readonly int _mapHeight;
        [JsonIgnore]
        private readonly int _mapWidth;
        [JsonIgnore]
        private readonly GameBlock[,] _gameBlocks;
        [JsonIgnore]
        private readonly List<PlayerEntity> _registeredPlayers;

        /// <summary>
        /// A read only collection containing all of the player entities registered to this game map
        /// </summary>
        [JsonProperty]
        public IList<PlayerEntity> RegisteredPlayerEntities { get; private set; }
        [JsonProperty]
        public int CurrentRound { get; set; }

        public GameMap(int width, int height, int mapSeed)
        {
            _mapSeed = mapSeed;
            _mapWidth = width;
            _mapHeight = height;

            _registeredPlayers = new List<PlayerEntity>();
            RegisteredPlayerEntities = _registeredPlayers.AsReadOnly();

            _gameBlocks = new GameBlock[_mapWidth,_mapHeight];
            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                {
                    _gameBlocks[x, y] = new GameBlock(x+1, y+1);
                }
            }
        }

        /// <summary>
        /// Retrieves the game block at the specified X and Y location.  Game locations start at 1 up to game width/height
        /// </summary>
        /// <param name="x">X coordinates</param>
        /// <param name="y">Y coordinates</param>
        /// <returns>The game block found at the specified location</returns>
        /// <exception cref="LocationOutOfBoundsException">If the specified X or Y is out of bounds</exception>
        public GameBlock GetBlockAtLocation(int x, int y)
        {
            if (x <= 0)
                throw new LocationOutOfBoundsException(String.Format("Location of X ({0}) is smaller than or equal to 0", x));
            if(x > _mapWidth)
                throw new LocationOutOfBoundsException(String.Format("Location of X ({0}) is larger than map width {1}", x, _mapWidth));
            if (y <= 0)
                throw new LocationOutOfBoundsException(String.Format("Location of Y ({0}) is smaller than or equal to 0", y));
            if (y > _mapHeight)
                throw new LocationOutOfBoundsException(String.Format("Location of Y ({0}) is larger than map height {1}", y, _mapHeight));

            return _gameBlocks[x - 1, y - 1];
        }

        /// <summary>
        /// Returns the count of all the bombs in play for the current player
        /// </summary>
        /// <param name="player">The player whose bombs to locate</param>
        /// <returns>The amount of bombs placed on the map for the player</returns>
        public int GetPlayerBombCount(PlayerEntity player)
        {
            return GetPlayerBombs(player).Count;
        }

        /// <summary>
        /// Finds all of the bombs related to a player on the map
        /// </summary>
        /// <param name="player">The player entity</param>
        /// <returns>All of the bombs linked to the player</returns>
        public List<BombEntity> GetPlayerBombs(PlayerEntity player)
        {
            var bombs = new List<BombEntity>();
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var block = enumerator.Current;

                    if (block.Bomb != null && block.Bomb.Owner == player)
                        bombs.Add(block.Bomb);
                }
            }
            return bombs;
        }

        /// <summary>
        /// Registered a new player on the map at the specified location
        /// </summary>
        /// <param name="playerEntity">Player to Register</param>
        /// <param name="x">X Location</param>
        /// <param name="y">Y Location</param>
        /// <exception cref="LocationOutOfBoundsException">If the location is not valid</exception>
        public void RegisterPlayer(PlayerEntity playerEntity, int x, int y)
        {
            var gameBlock = GetBlockAtLocation(x, y);
            gameBlock.SetEntity(playerEntity);

            _registeredPlayers.Add(playerEntity);
        }

        /// <summary>
        /// The height of this game map
        /// </summary>
        [JsonProperty]
        public int MapHeight
        {
            get { return _mapHeight; }
        }

        /// <summary>
        /// The width of this game map
        /// </summary>
        [JsonProperty]
        public int MapWidth
        {
            get { return _mapWidth; }
        }

        /// <summary>
        /// All of the game blocks on this map
        /// </summary>
        [JsonProperty]
        public GameBlock[,] GameBlocks
        {
            get { return _gameBlocks; }
        }

        [JsonProperty]
        public int MapSeed
        {
            get { return _mapSeed; }
        }

        /// <summary>
        /// Gets the game block enumerator for this map,  the enumerator will enumerate over
        /// all of the game blocks, starting at y:1 and processing across the map for x.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GameBlock> GetEnumerator()
        {
            return new GameBlockEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
