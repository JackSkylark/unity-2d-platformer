using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collision2DExtensions {
    public static bool IsGroundCollision(
        this ByteBrosCollision2D collision,
        float maxAngle)
    {
        return collision.Normal.WithinDegrees(Vector2.up, maxAngle);
    }

    public static bool IsWallLeftCollision(
        this ByteBrosCollision2D collision, 
        float maxAngle)
	{
		return collision.Normal.WithinDegrees(Vector2.right, maxAngle);
	}

    public static bool IsWallRightCollision(
        this ByteBrosCollision2D collision, 
        float maxAngle)
	{
		return collision.Normal.WithinDegrees(Vector2.left, maxAngle);
	}
}
