using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;

namespace Domain.Enumerators
{
    public class GameBlockEnumerator : IEnumerator<GameBlock>, IEnumerator
    {
        private readonly GameMap _gameMap;
        private int currentX;
        private int currentY;

        public GameBlockEnumerator(GameMap gameMap)
        {
            _gameMap = gameMap;
            Reset();
        }

        public void Dispose()
        {
            //Nothing to dispose
        }

        public bool MoveNext()
        {
            if (currentY > _gameMap.MapHeight)
                return false;

            AssignNext();

            if (currentY > _gameMap.MapHeight)
                return false;

            return true;
        }

        private void AssignNext()
        {
            if (currentX <= _gameMap.MapWidth)
            {
                currentX++;
            }
            
            if (currentX > _gameMap.MapWidth)
            {
                currentX = 1;
                currentY++;
            }
            Current = currentY <= _gameMap.MapHeight ? _gameMap.GetBlockAtLocation(currentX, currentY) : null;
        }

        public void Reset()
        {
            currentX = 0;
            currentY = 1;
        }

        public GameBlock Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
