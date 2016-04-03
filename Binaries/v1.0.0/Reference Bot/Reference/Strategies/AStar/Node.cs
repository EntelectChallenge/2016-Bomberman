using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reference.Domain.Map;
using Reference.Domain.Map.Entities;

namespace Reference.Strategies.AStar
{
    public class Node
    {
        public int HCost { get; set; }
        public int GCost { get; set; }
        public int FCost { get { return HCost + GCost; } }
        public Node ParentNode { get; set; }

        private readonly Location _location;
        private readonly IEntity _nodeEntity;
        private readonly bool _exploding;
        private readonly BombEntity _bombEntity;


        public Node(GameBlock gameBlock)
        {
            this._location = gameBlock.Location;
            this._nodeEntity = gameBlock.PowerUp ?? gameBlock.Entity ?? gameBlock.Bomb;
            this._exploding = gameBlock.Exploding;
            this._bombEntity = gameBlock.Bomb;
        }

        public Location Location
        {
            get { return _location; }
        }

        public IEntity NodeEntity
        {
            get { return _nodeEntity; }
        }

        public bool Exploding
        {
            get { return _exploding; }
        }

        public int Points
        {
            get { return NodeEntity == null ? 0 : NodeEntity.GetPossiblePoints(); }
        }

        public bool IsBombPresent
        {
            get { return _bombEntity != null; }
        }

        public BombEntity BombEntity
        {
            get { return _bombEntity; }
        }
    }
}
