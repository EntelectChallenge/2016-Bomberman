using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.MapGenerator
{
    public class CoordinateTranslator
    {
        private readonly int _width;
        private readonly int _height;

        public CoordinateTranslator(int height, int width)
        {
            this._height = height;
            this._width = width;
        }

        public int TranslateX(int x)
        {
            return _width - x;
        }

        public int TranslateY(int y)
        {
            return _height - y;
        }
    }
}
