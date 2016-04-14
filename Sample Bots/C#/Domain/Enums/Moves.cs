using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleBot.Domain.Enums
{
    enum Moves
    {
        DoNothing = 0,
        MoveUp = 1,
        MoveLeft = 2,
        MoveRight = 3,
        MoveDown = 4,
        PlaceBomb = 5,
        TriggerBomb = 6
    }
}
