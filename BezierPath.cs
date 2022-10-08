using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierPath
{
    [System.Serializable]
    public struct BezierNode
    {
        public BezierCurve curve;
        public float speedMultiplier;
        public Tweening.TweenType tween;
    }
    [SerializeField]
    private BezierNode[] nodes;
    public BezierNode[] Nodes { get { return nodes; } }
    /// <summary>
    /// NOTES:
    /// FORMATIONS:
    ///     If you want a formation that:
    ///         + changes angle and breaks formation, make a formation that staggers their spawn.
    ///         + changes angle and does not break formation, set Direction to be Static_angle and have a bunch of enemies with offsets be spawned at the same T
    ///         + does not change angle and does not break formation, set Direction to be Static_Angle and have a bunch of enemies with offsets be spawned at the same T
    ///         + fans out, have the Path Expand
    ///     If you want a path that is:
    ///         + A singular point, then overlay all anchors and points on top of each other. This is beneficial for:
    ///             + A pause in the path
    ///             + A galaga base formation that sways side to side and pulsates. Set the path as a single point
    ///         
    /// </summary>
    public enum Direction
    {
        FORWARD,
        ANGLEOFFSET,
        STATIC_ANGLE
    }

    protected Direction directionFacing;
    protected float angleOffset;
    [SerializeField]
    public Vector3 startPoint;
   
    // Start is called before the first frame update
    public Vector3 VelocityAtTime(float t, Tweening.TweenType tween)
    {
        return nodes[Mathf.FloorToInt(t)].curve.GetVelocityAtTime(Mathf.Min(Tweening.GetValue(tween, t % 1.0f), nodes.Length));
    }

    public Vector3 LocationAtTime(float t, Tweening.TweenType tween)
    {
        return nodes[Mathf.FloorToInt(t)].curve.GetLocationAtTime(Mathf.Min(Tweening.GetValue(tween, t % 1.0f), nodes.Length));
    }
}

[System.Serializable]
public class BezierCurve
{
    [SerializeField]
    protected Vector3 anchorA, anchorB, point1, point2;

    public Vector3 AnchorA { get { return anchorA; } set { anchorA = value; } }
    public Vector3 AnchorB { get { return anchorB; } set { anchorB = value; } }
    public Vector3 Point1 { get { return point1; } set { point1 = value; } }
    public Vector3 Point2 { get { return point2; } set { point2 = value; } }
    
    public static Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    public static Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 cd = Vector3.Lerp(c, d, t);
        return QuadraticLerp(ab, bc, cd, t);
    }

    public Vector3 GetLocationAtTime(float t)
    {
        return CubicLerp(anchorA, point1, point2, anchorB, t);
    }

    public static Vector3 GetVelocityAtTime(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = a * ((-3 * t * t) + (6 * t) - 3);
        Vector3 p1 = b * ((9 * t * t) - (12 * t) + 3);
        Vector3 p2 = c * ((-9 * t * t) + (6 * t));
        Vector3 p3 = d * (3 * t * t);
        return p0 + p1 + p2 + p3;
        
    }


    public Vector3 GetVelocityAtTime(float t)
    {
        //Formula for the derivative of Bernstein Polynomial form.
        Vector3 p0 = anchorA * ((-3 * t * t) + (6 * t) - 3);
        Vector3 p1 = point1 * ((9 * t * t) - (12 * t) + 3);
        Vector3 p2 = point2* ((-9 * t * t) + (6 * t));
        Vector3 p3 = anchorB * (3 * t * t);
        return p0 + p1 + p2 + p3;
    }

    private Vector3 RotateVectorBy90(Vector3 rot, bool clockwise)
    {
        if (clockwise)
        {
            return new Vector3(rot.y, -rot.x);
        }
        return new Vector3(-rot.y, rot.x);
    }
    
}


[RequireComponent(typeof(Rigidbody2D))]

public static class Tweening
{
    /// <summary>
    /// Takes in a tween type and float and tweens it according to the given tween type
    /// </summary>
    public static float GetValue(TweenType Tween, float t)
    {
        return Tween switch
        {
            TweenType.LINEAR => t,
            TweenType.EASE_IN_SIN => EaseInSin(t),
            TweenType.EASE_OUT_SIN => EaseOutSin(t),
            TweenType.EASE_IN_QUAD => EaseInQuad(t),
            TweenType.EASE_OUT_QUAD => EaseOutQuad(t),
            TweenType.EASE_IN_CUBIC => EaseInCubic(t),
            TweenType.EASE_OUT_CUBIC => EaseOutCubic(t),
            TweenType.EASE_IN_QUART => EaseInQuart(t),
            TweenType.EASE_OUT_QUART => EaseOutQuart(t),
            TweenType.EASE_IN_QUINT => EaseInQuint(t),
            TweenType.EASE_OUT_QUINT => EaseOutQuint(t),
            TweenType.EASE_IN_OUT_SIN => EaseInOutSin(t),
            TweenType.EASE_IN_OUT_QUAD => EaseInOutQuad(t),
            TweenType.EASE_IN_OUT_CUBIC => EaseInOutCubic(t),
            TweenType.EASE_IN_OUT_QUART => EaseInOutQuart(t),
            TweenType.EASE_IN_OUT_QUINT => EaseInOutQuint(t),
            TweenType.EASE_IN_CIRC => EaseInCirc(t),
            TweenType.EASE_OUT_CIRC => EaseOutCirc(t),
            TweenType.EASE_IN_OUT_CIRC => EaseInOutCirc(t),
            TweenType.EASE_OUT_IN_QUAD=> EaseOutInQuad(t),
            TweenType.EASE_OUT_IN_CUBIC=> EaseOutInCubic(t),
            TweenType.EASE_OUT_IN_QUART=> EaseOutInQuart(t),
            TweenType.EASE_OUT_IN_QUINT=> EaseOutInQuint(t),
            TweenType.EASE_OUT_IN_CIRC=> EaseOutInCirc(t),
            _ => t,
        };
    }


    public enum TweenType
    {
        LINEAR,
        EASE_IN_SIN,
        EASE_OUT_SIN,
        EASE_IN_OUT_SIN,
        EASE_IN_QUAD,
        EASE_OUT_QUAD,
        EASE_IN_OUT_QUAD,
        EASE_OUT_IN_QUAD,
        EASE_IN_CUBIC,
        EASE_OUT_CUBIC,
        EASE_IN_OUT_CUBIC,
        EASE_OUT_IN_CUBIC,
        EASE_IN_QUART,
        EASE_OUT_QUART,
        EASE_IN_OUT_QUART,
        EASE_OUT_IN_QUART,
        EASE_IN_QUINT,
        EASE_OUT_QUINT,
        EASE_IN_OUT_QUINT,
        EASE_OUT_IN_QUINT,
        EASE_IN_CIRC,
        EASE_OUT_CIRC,
        EASE_IN_OUT_CIRC,
        EASE_OUT_IN_CIRC


    }
    //REMEMBER: To mirror a curve, just do 1 - before the function.
    public static float EaseInSin(float t)
    {
        return 1 - Mathf.Cos(t * Mathf.PI / 2f);
    }

    public static float EaseOutSin(float t)
    {
        return Mathf.Sin(t * Mathf.PI / 2f);
    }

    public static float EaseInOutSin(float t) {

        return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
        }

    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    public static float EaseOutQuad(float t)
    {
        return 1 - ((1 - t) * (1 - t));
    }

    public static float EaseInOutQuad(float t)
    {
        return t < .5f ? (2 * t * t) : 1 - (Mathf.Pow(-2 * t + 2, 2) / 2);
    }

    public static float EaseOutInQuad(float t)
    {
        return t > .5f ? (2 * t * t) : 1 - (Mathf.Pow(-2 * t + 2, 2) / 2);
    }
    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }

    public static float EaseOutCubic(float t)
    {
        return 1 - (Mathf.Pow(1 - t, 3));
    }

    public static float EaseInOutCubic(float t)
    {
        return t < .5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }

    public static float EaseOutInCubic(float t)
    {
        return t > .5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
    public static float EaseInQuart(float t)
    {
        return t * t * t * t;
    }

    public static float EaseOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);
    }

    public static float EaseInOutQuart(float t)
    {
        return t < .5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
    }

    public static float EaseOutInQuart(float t)
    {
        return t > .5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
    }
    public static float EaseInQuint(float t)
    {
        return t * t * t * t * t;
    }

    public static float EaseOutQuint(float t)
    {
        return 1 - Mathf.Pow(1 - t, 5);
    }

    public static float EaseInOutQuint(float t)
    {
        return t < .5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }

    public static float EaseOutInQuint(float t)
    {
        return t > .5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }

    public static float EaseInCirc(float t)
    {
        return 1 - Mathf.Sqrt(1 - (t * t));
    }

    public static float EaseOutCirc(float t)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
    }

    public static float EaseInOutCirc(float t)
    {
        return t < 5
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;

    }

    public static float EaseOutInCirc (float t)
    {
        return t > 5
           ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
           : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
    }


}

