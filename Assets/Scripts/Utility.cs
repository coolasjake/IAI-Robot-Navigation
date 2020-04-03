using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary> Return true if all the axis of the vector are within the coresponding values of the bounds [exclusive] (for inclusive use !Outside). </summary>
    public static bool Inside(this Vector3 value, Vector3 lowerBounds, Vector3 upperBounds)
    {
        if (value.x.Inside(lowerBounds.x, upperBounds.x) &&
            value.y.Inside(lowerBounds.y, upperBounds.y) &&
            value.z.Inside(lowerBounds.z, upperBounds.z))
            return true;
        return false;
    }

    /// <summary> Return true if all the axis of the vector are outside the coresponding values of the bounds [exclusive] (for inclusive use !Inside). </summary>
    public static bool Outside(this Vector3 value, Vector3 lowerBounds, Vector3 upperBounds)
    {
        if (value.x.Outside(lowerBounds.x, upperBounds.x) ||
            value.y.Outside(lowerBounds.y, upperBounds.y) ||
            value.z.Outside(lowerBounds.z, upperBounds.z))
            return true;
        return false;
    }

    /// <summary> Return true if the x and z axis of the vector are outside the coresponding values of the bounds [exclusive] (for inclusive use !Inside). </summary>
    public static bool Outside2D(this Vector3 value, Vector3 lowerBounds, Vector3 upperBounds)
    {
        if (value.x.Outside(lowerBounds.x, upperBounds.x) ||
            value.z.Outside(lowerBounds.z, upperBounds.z))
            return true;
        return false;
    }

    /// <summary> Return true if the x and z axis of the vector are inside the coresponding values of the bounds [exclusive] (for inclusive use !Outside). </summary>
    public static bool Inside2D(this Vector3 value, Vector3 lowerBounds, Vector3 upperBounds)
    {
        if (value.x.Inside(lowerBounds.x, upperBounds.x) ||
            value.z.Inside(lowerBounds.z, upperBounds.z))
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
            value += Mathf.Abs(t);
            if (value >= target)
                return target;
            else
                return value;
        }
        else
        {
            value -= Mathf.Abs(t);
            if (value <= target)
                return target;
            else
                return value;
        }
    }

    /// <summary> Invert each axis of the vector (i.e. Vector.x = -Vector.x). </summary>
    public static Vector3 Invert(this Vector3 value)
    {
        return new Vector3(-value.x, -value.y, -value.z);
    }

    /// <summary> Replace the Y axis of this vector with the specified value. </summary>
    public static Vector3 FixedY(this Vector3 value, float newY)
    {
        return new Vector3(value.x, newY, value.z);
    }

    /// <summary> Multiply each axis of this vector by the corresponding axis of the given vector (creating a * operator cannot be done in this script). </summary>
    public static Vector3 MultipliedBy(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    /// <summary> Multiply each axis of this vector by the corresponding axis of the given vector (creating a * operator cannot be done in this script). </summary>
    public static Vector3Int MultipliedBy(this Vector3Int v1, Vector3 v2)
    {
        return new Vector3Int(Mathf.FloorToInt(v1.x * v2.x), Mathf.FloorToInt(v1.y * v2.y), Mathf.FloorToInt(v1.z * v2.z));
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
        Debug.LogError("Failed converting a string to a V2Int");
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
        Debug.LogError("Failed converting a string to a Rect");
        return Rect.zero;
    }
}

/// <summary> Helper class for managing an average. NOTE: Only tracks Total and Count. </summary>
public class Mean
{
    public float total = 0;
    public int count = 0;

    /// <summary> Add the value to the total and increase the count by 1 so the new average can be calculated. </summary>
    public void Add (float value)
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
