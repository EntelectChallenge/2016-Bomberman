using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Engine.Graphs
{
    public class GroupFinder<T>
    {
        private readonly Graph<T> _graph;

        private readonly HashSet<GraphNode<T>> _set = new HashSet<GraphNode<T>>();

        public GroupFinder(Graph<T> graph)
        {
            _graph = graph;
        }

        public List<IList<GraphNode<T>>> FindAllGroups()
        {
            return (from graphNode in _graph.NodeSet where !_set.Contains(graphNode) select FindGroup(graphNode).ToList()).Cast<IList<GraphNode<T>>>().ToList();
        }

        private IEnumerable<GraphNode<T>> FindGroup(GraphNode<T> rootNode)
        {
            var  stack = new Stack<GraphNode<T>>();
            stack.Push(rootNode);
            while (stack.Count != 0)
            {
                GraphNode<T> current = stack.Pop();
                if (_set.Contains(current)) continue;
                yield return current;
                _set.Add(current);
                foreach (var child in current.Neighbours)
                    stack.Push(child);
            }
        }
    }
}
