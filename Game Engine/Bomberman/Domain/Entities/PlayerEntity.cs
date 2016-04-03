using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Interfaces;
using Newtonsoft.Json;

namespace Domain.Entities
{
    public class PlayerEntity : BaseEntity
    {
        private int _points;

        public String Name { get; set; }
        public char Key { get; set; }

        public int Points
        {
            get { return _points + MapCoveragePoints; }
            private set { _points = value; }
        }

        public bool Killed { get; set; }

        public int BombBag { get; set; }
        public int BombRadius { get; set; }

        [JsonIgnore]
        public int MapCoveragePoints { get; set; }

        [JsonIgnore]
        public int KilledRound { get; set; }

        public void AddPoints(int points)
        {
            _points += points;
        }

        public void RemovePoints(int points)
        {
            _points -= points;
        }

        public override char GetMapSymbol()
        {
            return Key;
        }

        public override bool IsDestructable()
        {
            return true;
        }

        public override bool StopsBombBlast()
        {
            return false;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2}, Key:{3})", GetType().Name, Location.X, Location.Y, Key);
        }
    }
}
