using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Engine.Graphs
{
    public class GraphNode<T>
    {
        public T Node { get; private set; }
        public List<GraphNode<T>> Neighbours { get; private set; }

        public GraphNode(T node)
        {
            Node = node;
            Neighbours = new List<GraphNode<T>>();
        }

        public GraphNode<T> FindNode(NodeFinder<T> nodeFinder, T value)
        {
            return nodeFinder.FindNode(this, value);
        }

        public void Visit(IVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}
