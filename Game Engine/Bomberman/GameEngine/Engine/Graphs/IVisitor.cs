using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Engine.Graphs
{
    public interface IVisitor<T>
    {
        void Visit(GraphNode<T> node);
    }
}
