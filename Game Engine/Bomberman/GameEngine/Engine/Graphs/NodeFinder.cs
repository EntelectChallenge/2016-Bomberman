using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Engine.Graphs
{
    public class NodeFinder<T>
    {
        private readonly HashSet<GraphNode<T>> _visitedNodes = new HashSet<GraphNode<T>>();

        public GraphNode<T> FindNode(GraphNode<T> from, T value)
        {
            if (_visitedNodes.Contains(from))
                return null;

            _visitedNodes.Add(from);

            return @from.Node.Equals(value) ? @from : @from.Neighbours.Select(neighbour => neighbour.FindNode(this, value)).FirstOrDefault(found => found != null);
        }
    }
}
