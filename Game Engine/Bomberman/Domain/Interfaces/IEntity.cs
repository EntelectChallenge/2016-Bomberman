using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;

namespace Domain.Interfaces
{
    public interface IEntity
    {
        /// <summary>
        /// The location of this entity on the map.  Is set once an entity is registered to a block
        /// </summary>
        Location Location { get; set; }
        
        /// <summary>
        /// The map symbol representing this entity
        /// </summary>
        /// <returns>The symbol to be used on the map</returns>
        char GetMapSymbol();

        /// <summary>
        /// If this entity can be destroyed by bombs
        /// </summary>
        /// <returns></returns>
        bool IsDestructable();

        /// <summary>
        /// If a bomb blast should stop once it collides with this entity
        /// </summary>
        /// <returns></returns>
        bool StopsBombBlast();

    }
}
