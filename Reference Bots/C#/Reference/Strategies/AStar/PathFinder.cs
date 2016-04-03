using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reference.Domain.Map.Entities;
using Reference.Domain.Map.Entities.PowerUps;

namespace Reference.Strategies.AStar
{
    public class PathFinder
    {
        private readonly NodeMap _nodeMap;
        private readonly List<Node> _openNodes = new List<Node>();
        private readonly ISet<Node> _closedNodes = new HashSet<Node>();

        public PathFinder(NodeMap nodeMap)
        {
            _nodeMap = nodeMap;
        }

        public List<Node> FindBestPath(Node startNode, Node endNode)
        {
            foreach (var node in _nodeMap.Nodes)
            {
                node.ParentNode = null;
                node.GCost = 0;
                node.HCost = 0;
            }

            _openNodes.Add(startNode);

            while (_openNodes.Count > 0)
            {
                var currentNode = _openNodes[0];

                for (int i = 1; i < _openNodes.Count; i++)
                {
                    if (_openNodes[i].FCost < currentNode.FCost ||
                        (_openNodes[i].FCost == currentNode.FCost && _openNodes[i].HCost < currentNode.HCost))
                    {
                        currentNode = _openNodes[i];
                    }
                }

                _openNodes.Remove(currentNode);
                _closedNodes.Add(currentNode);

                if (currentNode == endNode)
                    break;

                foreach (var neighbour in _nodeMap.GetNeighbours(currentNode))
                {
                    if (neighbour.Exploding || _closedNodes.Contains(neighbour))
                        continue;

                    var movementCost = currentNode.GCost + Distance(currentNode, neighbour) + _nodeMap.GetNodePenalty(currentNode);
                    if (movementCost < neighbour.GCost || !_openNodes.Contains(neighbour))
                    {
                        neighbour.GCost = movementCost;
                        neighbour.HCost = Distance(neighbour, endNode);
                        neighbour.ParentNode = currentNode;

                        if (!_openNodes.Contains(neighbour))
                        {
                            _openNodes.Add(neighbour);
                        }
                    }
                }
            }

            return GetPath(startNode, endNode);
        }

        public List<Node> GetPath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }

            path.Reverse();

            return path;
        }

        public int Distance(Node nodeA, Node nodeB)
        {
            var distanceX = Math.Abs(nodeA.Location.X - nodeB.Location.X);
            var distanceY = Math.Abs(nodeA.Location.Y - nodeB.Location.Y);

            if (distanceX > distanceY)
                return distanceY + (distanceX - distanceY);
            return distanceX + (distanceY - distanceX);
        }
    }
}
