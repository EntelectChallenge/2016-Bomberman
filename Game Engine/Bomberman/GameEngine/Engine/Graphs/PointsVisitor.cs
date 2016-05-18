using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

namespace GameEngine.Engine.Graphs
{
    public class PointsVisitor : GroupNodeVisitor<BombEntity>
    {
        private readonly HashSet<PlayerEntity> _playerEntities = new HashSet<PlayerEntity>();
        private int _totalPoints = 0;

        public override void Visit(GraphNode<BombEntity> node)
        {
            _totalPoints += node.Node.Points;
            _playerEntities.Add(node.Node.Owner);
        }

        public override void GroupVisitComplete(int groupId)
        {
            var aliveEntities = _playerEntities.Where(x => !x.Killed).ToList();

            if (aliveEntities.Any())
            {
                var pointsPerPlayer = _totalPoints;
                foreach (var playerEntity in aliveEntities)
                {
                    playerEntity.AddPoints(pointsPerPlayer);
                }
            }

            _playerEntities.Clear();
            _totalPoints = 0;
        }
    }
}
