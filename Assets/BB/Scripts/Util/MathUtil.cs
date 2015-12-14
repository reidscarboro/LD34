using UnityEngine;
using System.Collections;

public class MathUtil : MonoBehaviour {

    public static Vector2 polarToCartesian(Vector2 polar) {
        return new Vector2(polar.x * cosDegrees(polar.y), polar.x * sinDegrees(polar.y));
    }

    public static float sinDegrees(float radians) {
        return Mathf.Sin((radians * Mathf.PI) / 180);
    }

    public static float cosDegrees(float radians) {
        return Mathf.Cos((radians * Mathf.PI) / 180);
    }
}
