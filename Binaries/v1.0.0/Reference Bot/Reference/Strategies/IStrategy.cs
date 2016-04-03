using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reference.Commands;
using Reference.Domain.Map;

namespace Reference.Strategies
{
    public interface IStrategy
    {
        GameCommand ExecuteStrategy(GameMap gameMap, char playerKey);
    }
}
