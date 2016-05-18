using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Engine.Graphs
{
    public class Graph<T>
    {
        public List<GraphNode<T>> NodeSet { get; private set; }

        public Graph()
        {
            NodeSet = new List<GraphNode<T>>();
        }

        public void AddNode(T node)
        {
            if (FindNode(node) == null)
                NodeSet.Add(new GraphNode<T>(node));
        }

        public void ConnectNodes(T from, T to)
        {
            var fromNode = FindNode(from);
            var toNode = FindNode(to);

            fromNode.Neighbours.Add(toNode);
            toNode.Neighbours.Add(fromNode);
        }

        public GraphNode<T> FindNode(T value)
        {
            var nodeFinder = new NodeFinder<T>();
            return NodeSet.Select(graphNode => nodeFinder.FindNode(graphNode, value)).FirstOrDefault(found => found != null);
        }

        public void VisitAllNodes(IVisitor<T> visitor)
        {
            foreach (var graphNode in DepthFirstTraversalAll())
            {
                graphNode.Visit(visitor);
            }
        }

        public void VisitGroups(GroupNodeVisitor<T> groupNodeVisitor)
        {
            var finder = new GroupFinder<T>(this);
            var groups = finder.FindAllGroups();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                groupNodeVisitor.CurrentGroupId = i;

                foreach (var node in group)
                {
                    groupNodeVisitor.Visit(node);
                }

                groupNodeVisitor.GroupVisitComplete(i);
            }
        }

        public IEnumerable<GraphNode<T>> DepthFirstTraversalAll()
        {
            var set = new HashSet<GraphNode<T>>();
            var stack = new Stack<GraphNode<T>>();
            foreach (var node in NodeSet)
            {
                stack.Push(node);
            }
            while (stack.Count != 0)
            {
                GraphNode<T> current = stack.Pop();
                if (set.Contains(current)) continue;
                yield return current;
                set.Add(current);
                foreach (var child in current.Neighbours)
                    stack.Push(child);
            }
        }
    }
}
