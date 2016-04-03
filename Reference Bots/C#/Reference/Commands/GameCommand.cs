using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference.Commands
{
    public enum GameCommand
    {
        MoveUp = 1,
        MoveLeft = 2,
        MoveRight = 3,
        MoveDown = 4,
        PlaceBomb = 5,
        TriggerBomb = 6,
        DoNothing = 7
    }
}
