using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public static class Utility
    {
        /// <summary> Return true if this float has a greater unsigned value than the comparison float. </summary>
        public static bool FurtherFromZero(this float f, float comparison)
        {
            if ((comparison > 0 && f > comparison) || (comparison < 0 && f < comparison))
                return true;
            return false;
        }

        /// <summary> Return true if this int has a greater unsigned value than the comparison int. </summary>
        public static bool FurtherFromZero(this int f, int comparison)
        {
            if ((comparison > 0 && f > comparison) || (comparison < 0 && f < comparison))
                return true;
            return false;
        }

        /// <summary> Return true if this float has a smaller unsigned value than the comparison float. </summary>
        public static bool CloserToZero(this float f, float comparison)
        {
            if ((comparison > 0 && f < comparison) || (comparison < 0 && f > comparison))
                return true;
            return false;
        }

        /// <summary> Return true if this int has a smaller unsigned value than the comparison int. </summary>
        public static bool CloserToZero(this int f, int comparison)
        {
            if ((comparison > 0 && f <= comparison) || (comparison < 0 && f >= comparison))
                return true;
            return false;
        }

        /// <summary> Return the normalized sign of the given value (1 / 0 / -1). </summary>
        public static float Sign(this float signOf)
        {
            if (signOf > 0)
                return 1;
            else if (signOf == 0)
                return 0;
            return -1;
        }

        /// <summary> Return the normalized sign of the given value (1 / 0 / -1). </summary>
        public static int Sign(this int signOf)
        {
            if (signOf > 0)
                return 1;
            else if (signOf == 0)
                return 0;
            return -1;
        }

        /// <summary> Return true if the float is outside the specified bounds [exclusive] (for inclusive use !Inside).
        public static bool Outside(this float value, float lowerBounds, float upperBounds)
        {
            if (value < lowerBounds || value > upperBounds)
                return true;
            return false;
        }

        /// <summary> Return true if the integer is outside the specified bounds [exclusive] (for inclusive use !Inside). </summary>
        public static bool Outside(this int value, int lowerBounds, int upperBounds)
        {
            if (value < lowerBounds || value > upperBounds)
                return true;
            return false;
        }

        /// <summary> Return true if the float is within the specified bounds [exclusive] (for inclusive use !Outside). </summary>
        public static bool Inside(this float value, float lowerBounds, float upperBounds)
        {
            if (value > lowerBounds && value < upperBounds)
                return true;
            return false;
        }

        /// <summary> Return true if the integer is within the specified bounds [exclusive] (for inclusive use !Outside). </summary>
        public static bool Inside(this int value, int lowerBounds, int upperBounds)
        {
            if (value > lowerBounds && value < upperBounds)
                return true;
            return false;
        }

        /// <summary> Return true if both the axis of the vector are outside the coresponding values of the bounds [exclusive] (for inclusive use !Inside). </summary>
        public static bool Outside(this Vector2Int value, int minX, int minY, int maxX, int maxY)
        {
            if (value.x.Outside(minX, maxX) ||
                value.y.Outside(minY, maxY))
                return true;
            return false;
        }

        /// <summary> Return true if both the axis of the vector are outside the coresponding values of the bounds [exclusive] (for inclusive use !Inside). </summary>
        public static bool Outside(this Vector2Int value, Vector2Int min, Vector2Int max)
        {
            if (value.Outside(min.x, min.y, max.x, max.y))
                return true;
            return false;
        }

        /// <summary> Move the value towards the target by a measure of 't'. </summary>
        public static int lerp(this int value, int target, int t)
        {
            if (value < target)
            {
                value += Math.Abs(t);
                if (value >= target)
                    return target;
                else
                    return value;
            }
            else
            {
                value -= Math.Abs(t);
                if (value <= target)
                    return target;
                else
                    return value;
            }
        }

        /// <summary> Trims the brackets ([{<>}]) from a string and then splits it by comma, or by white-space if no commas are found. </summary>
        public static Vector2Int StringToV2(this string origin)
        {
            origin = origin.Trim('[', ']', '(', ')', '{', '}', '<', '>', ' ');
            string[] vals = origin.Split(',');
            if (vals.Length == 2)
                return new Vector2Int(int.Parse(vals[0]), int.Parse(vals[1]));
            vals = origin.Split(' ');
            if (vals.Length == 2)
                return new Vector2Int(int.Parse(vals[0]), int.Parse(vals[1]));
            return Vector2Int.zero;
        }

        /// <summary> Trims the brackets ([{<>}]) from a string and then splits it by comma, or by white-space if no commas are found. </summary>
        public static Rect StringToRect(this string origin)
        {
            origin = origin.Trim('[', ']', '(', ')', '{', '}', '<', '>', ' ');
            string[] vals = origin.Split(',');
            if (vals.Length == 4)
                return new Rect(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]), float.Parse(vals[3]));
            vals = origin.Split(' ');
            if (vals.Length == 4)
                return new Rect(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]), float.Parse(vals[3]));
            return Rect.zero;
        }
    }

    /// <summary> Helper class for managing an average. NOTE: Only tracks Total and Count. </summary>
    public class Mean
    {
        public float total = 0;
        public int count = 0;

        /// <summary> Add the value to the total and increase the count by 1 so the new average can be calculated. </summary>
        public void Add(float value)
        {
            total += value;
            count += 1;
        }

        /// <summary> Returns the current average (total value divided by the number of values added). </summary>
        public float Average
        {
            get { return total / count; }
        }

        /// <summary> Add a value to the total and also return the new average. </summary>
        public float NewAverage(float value)
        {
            total += value;
            count += 1;
            return total / count;
        }
    }

}
