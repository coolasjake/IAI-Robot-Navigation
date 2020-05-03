using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary> (Replica of Unity class) Uses an X and Y value to represent a 2D vector. This version uses ints instead of the normal floats. </summary>
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

        /// <summary> Shorthand for (0, 0). </summary>
        public static Vector2Int zero
        {
            get { return new Vector2Int(0, 0); }
        }

        /// <summary> Returns x and y inside square brackets seperated by a comma, e.g. [-1, 5] </summary>
        public override string ToString()
        {
            return "[" + x + ", " + y + "]";
        }
    }
}
