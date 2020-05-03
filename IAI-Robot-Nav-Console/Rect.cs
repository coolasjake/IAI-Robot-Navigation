using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Copy of the Rect struct from Unity. Similar to a tuple. </summary>
    public class Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public Rect(float X, float Y, float W, float H)
        {
            x = X;
            y = Y;
            width = W;
            height = H;
        }

        public static Rect zero
        {
            get { return new Rect(0, 0, 0, 0); }
        }
    }
}
