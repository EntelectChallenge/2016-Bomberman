using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;
using Domain.Interfaces;
using Newtonsoft.Json;

namespace Domain.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameBlock
    {
        private readonly Location _location;

        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public IEntity Entity { get; private set; }
        [JsonProperty]
        public BombEntity Bomb { get; private set; }
        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public IPowerUpEntity PowerUp
        {
            get { return Entity != null ? null : PowerUpEntity; } }
        [JsonProperty]
        public bool Exploding { get { return ExplodingBombs.Count > 0; } }


        [JsonIgnore]
        private readonly HashSet<IEntity> _touchedByPlayers = new HashSet<IEntity>();
        [JsonIgnore]
        private readonly HashSet<BombEntity> _explodingBombs = new HashSet<BombEntity>();
        [JsonIgnore]
        public IPowerUpEntity PowerUpEntity { get; private set; }

        public GameBlock(int x, int y)
        {
            _location = new Location(x, y);
        }

        /// <summary>
        /// Sets a new entity on this game block. This will also update the entity location to the location of this game block
        /// </summary>
        /// <param name="entity">The entity to assign to this block</param>
        /// <exception cref="InvalidOperationException">If this block already contains an entity</exception>
        public void SetEntity(IEntity entity)
        {
            if (Entity != null && entity != null)
                throw new InvalidOperationException("Block already contains an entity " + Entity.ToString());

            Entity = entity;

            if(Entity != null)
            {
                Entity.Location = _location;
            }

            if (Entity != null && Entity.GetType() == typeof (PlayerEntity))
            {
                _touchedByPlayers.Add(Entity);
            }
        }

        /// <summary>
        /// Sets a power up entity on this game block.
        /// </summary>
        /// <param name="entity">The power up entity to assign to this block</param>
        /// <exception cref="InvalidOperationException">If this block already contains a power up entity</exception>
        public void SetPowerUpEntity(IPowerUpEntity entity)
        {
            if (PowerUpEntity != null && entity != null)
                throw new InvalidOperationException("Block already contains a power up entity " + PowerUpEntity.ToString());

            PowerUpEntity = entity;

            if (PowerUpEntity != null)
            {
                PowerUpEntity.Location = _location;
            }
        }

        /// <summary>
        /// Applies the power up to the player entity currently on this game block
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no player entity on this block</exception>
        /// <exception cref="InvalidOperationException">If there is no power up entity on this block</exception>
        public void ApplyPowerUp()
        {
            if (Entity == null || Entity.GetType() != typeof (PlayerEntity))
                throw new InvalidOperationException("Power up cannot be applied wihout a player present");

            if (PowerUpEntity == null)
                throw new InvalidOperationException("There is no power up entity on this game block " + Location);

            PowerUpEntity.PerformPowerUp((PlayerEntity)Entity);
            PowerUpEntity = null;
        }

        /// <summary>
        /// Plants a new bomb on this game block
        /// </summary>
        /// <param name="timer">The timer for the bomb</param>
        /// <exception cref="InvalidOperationException">If here is no player present on the current block</exception>
        /// <exception cref="InvalidOperationException">If the block already contains a bomb</exception>
        /// <exception cref="InvalidOperationException">If the timer is less than 1</exception>
        public void PlantBomb(int timer)
        {
            if (Entity == null || Entity.GetType() != typeof(PlayerEntity))
                throw new InvalidOperationException("Bomb required an owner");

            if (Bomb != null)
                throw new InvalidOperationException("Block already contains a bomb " + Entity.ToString());

            if (timer < 2)
                throw new InvalidOperationException("Bomb timer value should be larger than 1");

            var owner = ((PlayerEntity) Entity);
            Bomb = new BombEntity{
                Owner = owner,
                Location = _location,
                BombRadius = owner.BombRadius,
                BombTimer = timer
            };
        }

        /// <summary>
        /// Removed the bomb from this game block.  Only exploded bombs can be removed.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the current bomb has not yet exploded</exception>
        public void RemoveBomb()
        {
            if (Bomb != null && !Bomb.IsExploding)
                throw new InvalidOperationException("This bomb cannot be removed because it has not exploded yet " + Bomb);

            Bomb = null;
        }

        public bool HasBeenTouchedByPlayer(PlayerEntity player)
        {
            return _touchedByPlayers.Contains(player);
        }

        /// <summary>
        /// Returns the Map symbol for the entity on the this block.
        /// 1. If the block is exploding, the exploding symbol will be returned
        /// 2. If there is a bomb the bomb symbol is returned
        /// 3. If there is an entity on this block, the entity symbol is returned
        /// 4. If none of the above, and there is a power up on this block, retun the power up symbol
        /// </summary>
        /// <returns></returns>
        public char GetMapSymbol()
        {
            if (Exploding)
            {
                return '*';
            }
            if (Bomb != null)
            {
                return Bomb.GetMapSymbol();
            }
            if (Entity != null)
            {
                return Entity.GetMapSymbol();
            }
            if (PowerUpEntity != null)
            {
                return PowerUpEntity.GetMapSymbol();
            }

            return ' ';
        }

        /// <summary>
        /// The location for this block on the game map
        /// </summary>
        [JsonProperty]
        public Location Location
        {
            get { return _location; }
        }

        public HashSet<BombEntity> ExplodingBombs
        {
            get { return _explodingBombs; }
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2}, Entity:{3})", GetType().Name, Location.X, Location.Y, Entity);
        }
    }
}
