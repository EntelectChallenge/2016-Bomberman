using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Reference.Commands;
using Reference.Domain.Map;
using Reference.Domain.Map.Entities;
using Reference.Domain.Map.Entities.PowerUps;

namespace Reference.Strategies.AStar
{
    public class NodeMap
    {
        private readonly char _playerKey;
        private readonly GameMap _gameMap;
        private readonly List<Node> _nodes;
        private readonly Node _playerNode;
        public NodeMap(GameMap map, char playerKey)
        {
            _playerKey = playerKey;
            _gameMap = map;
            _nodes = new List<Node>();

            for (var y = 1; y < map.MapHeight; y++)
            {
                for (var x = 1; x < map.MapWidth; x++)
                {
                    var block = map.GetBlockAtLocation(x, y);
                    if (block.Entity is IndestructibleWallEntity)
                        continue;

                    _nodes.Add(new Node(block));
                }
            }

            _playerNode = _nodes.FirstOrDefault(x => x.NodeEntity is PlayerEntity && ((PlayerEntity)x.NodeEntity).Key == playerKey);
        }

        public List<Node> Nodes
        {
            get { return _nodes; }
        }

        public Node PlayerNode
        {
            get { return _playerNode; }
        }

        public PlayerEntity Player
        {
            get { return ((PlayerEntity) PlayerNode.NodeEntity); }
        }

        public int AvailableBombs
        {
            get
            {
                return Player.BombBag - PlayerBombs.Count;
            }
        }

        public List<BombEntity> PlayerBombs
        {
            get
            {
                var bombs = new List<BombEntity>();
                for (int x = 1; x < _gameMap.MapWidth; x++)
                {
                    for (int y = 1; y < _gameMap.MapHeight; y++)
                    {
                        var entity = _gameMap.GetBlockAtLocation(x, y).Bomb;
                        if (entity == null) continue;
                        if (entity.Owner.Key == _playerKey)
                            bombs.Add(entity);
                    }
                }

                return bombs;
            }
        } 

        public Node GetNodeAtLocation(int x, int y)
        {
            return Nodes.FirstOrDefault(n => n.Location.X == x && n.Location.Y == y);
        }

        public List<Node> GetNeighbours(Node centre)
        {
            var neighbours = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x != 0 && y != 0)
                        continue;

                    var nodeAtLocation = GetNodeAtLocation(centre.Location.X + x, centre.Location.Y + y);

                    if(nodeAtLocation != null && nodeAtLocation != centre)
                        neighbours.Add(nodeAtLocation);
                }
            }

            return neighbours;
        }

        public bool IsInExplosionRange(Node node)
        {
            if (node.IsBombPresent)
            {
                return true;
            }

            for (int x = 1; x <= _gameMap.MapWidth; x++)
            {
                var nodeAtLocation = GetNodeAtLocation(x, node.Location.Y);

                if (nodeAtLocation == node || nodeAtLocation == null)
                    continue;

                if (IsInExplosionRange(node, nodeAtLocation))
                    return true;
            }

            for (int y = 1; y <= _gameMap.MapWidth; y++)
            {
                var nodeAtLocation = GetNodeAtLocation(node.Location.X, y);

                if (nodeAtLocation == node || nodeAtLocation == null)
                    continue;

                if (IsInExplosionRange(node, nodeAtLocation))
                    return true;
            }

            return false;
        }

        private bool IsInExplosionRange(Node startNode, Node node)
        {
            if (node.IsBombPresent)
            {
                var bomb = node.BombEntity;
                var bomx = Math.Abs(bomb.Location.X - startNode.Location.X);
                var bomy = Math.Abs(bomb.Location.Y - startNode.Location.Y);
                if ((bomx <= bomb.BombRadius && bomb.Location.Y == startNode.Location.Y))
                {
                    return true;
                }

                if (bomy <= bomb.BombRadius && bomb.Location.X == startNode.Location.X)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetNodePenalty(Node node)
        {
            if (node == PlayerNode)
                return 0;

            if(node.IsBombPresent)
                return 110;

            if (node.NodeEntity is DestructibleWallEntity)
            {
                if (IsInExplosionRange(PlayerNode))
                    return IsInExplosionRange(node) ? 250 : 30;

                return IsInExplosionRange(node) ? 150 : -10;
            }

            if (node.NodeEntity == null && IsInExplosionRange(PlayerNode))
                return IsInExplosionRange(node)  ? 50 : -50;

            if (IsInExplosionRange(node) || node.Exploding)
                return 200;

            if (node.NodeEntity is SuperPowerUp)
                return -85;

            if (node.NodeEntity is IPowerUp)
                return -55;

            if (node.NodeEntity is PlayerEntity)
                return 50;

            return 0;
        }
    }
}
