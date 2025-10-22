using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {
        return Mathf.Sqrt((a.x * a.x) + (a.y * a.y));
    }

    public static Vector2 Normalise(Vector2 a)
    {
        if (a != Vector2.zero)
        {
            return new Vector2(a.x / Magnitude(a), a.y / Magnitude(a));
        }
        return Vector2.zero;
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        lhs = Normalise(lhs);
        rhs = Normalise(rhs);

        return (lhs.x * rhs.x) + (lhs.y * rhs.y);
    }

    /// <summary>
    /// Returns the radians of the angle between two vectors
    /// </summary>
    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        return Mathf.Acos(Dot(lhs, rhs));
    }

    /// <summary>
    /// Translates a vector by X angle in degrees
    /// </summary>
    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        angle = angle * Mathf.Deg2Rad;

        Vector2 newPos = new Vector2((vector.x * Mathf.Cos(angle)) - (vector.y * Mathf.Sin(angle)), 
            (vector.x * Mathf.Sin(angle)) + (vector.y * Mathf.Cos(angle)));

        return newPos;
    }

    /// <summary>
    /// Translates a vector by X angle in degrees around a point
    /// </summary>
    public static Vector2 RotateVector(Vector2 offset, Vector2 localOrigin, float angle)
    {
        angle = angle * Mathf.Deg2Rad;

        Vector2 localOffset = offset - localOrigin;

        Vector2 rotatedOffset = new Vector2((localOffset.x * Mathf.Cos(angle)) - (localOffset.y * Mathf.Sin(angle)),
            (localOffset.x * Mathf.Sin(angle)) - (localOffset.y * Mathf.Cos(angle)));

        Vector2 newPos = localOrigin + rotatedOffset;

        return newPos;
    }

    public static int CircularClamp(int min, int max, int i)
    {
        int difference = max - min + 1;

        if (i > max)
        {
            return CircularClamp(min, max, i - difference);
        }
        else if (i < min)
        {
            return CircularClamp(min, max, i + difference);
        }
        else
        {
            return i;
        }
    }
}
