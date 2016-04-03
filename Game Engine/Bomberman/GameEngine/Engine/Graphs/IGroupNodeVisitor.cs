using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Engine.Graphs
{
    public abstract class GroupNodeVisitor<T> : IVisitor<T>
    {
        public int CurrentGroupId { get; set; }

        public abstract void Visit(GraphNode<T> node);
        public abstract void GroupVisitComplete(int groupId);
    }
}
