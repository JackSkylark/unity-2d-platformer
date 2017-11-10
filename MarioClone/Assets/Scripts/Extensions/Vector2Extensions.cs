using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions {

	public static float Dot (this Vector2 vec1, Vector2 vec2) {
		return Vector2.Dot(vec1, vec2);
	}

	public static Vector2 Slide (this Vector2 vec1, Vector2 vec2) {
		return vec2 - vec1 * Vector2.Dot(vec1, vec2);
	}

	public static bool WithinDegrees(this Vector2 normal1, Vector2 normal2, float degrees)
	{
		return normal1.Dot(normal2) >= Mathf.Cos(0.0174532924f * degrees); 
	}
}
