using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public class Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public Vector2Int(float X, float Y)
        {
            x = (int)X;
            y = (int)Y;
        }

        public static Vector2Int operator+ (Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return (a.x != b.x || a.y != b.y);
        }

        public static Vector2Int zero
        {
            get { return new Vector2Int(0, 0); }
        }

        public override string ToString()
        {
            return "[" + x + ", " + y + "]";
        }
    }
}
