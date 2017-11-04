using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KinematicBody2D : MonoBehaviour {
	const float MAX_WALKABLE_SLOPE_ANGLE = 45f;
	const float SLIDE_CANCEL_THRESHOLD = 1.0f;

	private Collider2D _collider2D;
	public Vector2 Velocity;
	public float Gravity = 10;
	public float CollisionMargin = 0.01f;

	public Vector2 Position 
	{ 
		get { return transform.position; } 
		set { transform.position = value; } 
	}

	void Start () 
	{
		_collider2D = this.GetComponent<Collider2D>();
	}

	void FixedUpdate () 
	{
		Velocity.y -= Gravity * Time.deltaTime;

		var motion = Velocity * Time.deltaTime;
		Move(motion);
	}

	private void Move(Vector2 delta) 
	{
		var motionTest = _collider2D.TestMotion(delta);
		if (motionTest.HasCollision) {
			var normal = motionTest.Collision.Normal;
			var num = motionTest.Distance - CollisionMargin;

			Debug.Log(num);

			Position += num * delta.normalized;

			var remainderMovementDelta = motionTest.Remainder * motionTest.Direction;
			if (WithinDegrees(normal, Vector2.up, MAX_WALKABLE_SLOPE_ANGLE)) {
				Velocity.y = 0f;
			}

			var delta2 = normal.Slide(remainderMovementDelta);
			if ((double)delta2.magnitude < 0.004) {
				return;
			}
			this.Move(delta2);
		}
		else {
			Position += delta;
		}
	}

	public static float Wrap180(float angle)
	{
		while (angle >= 180f)
		{
			angle -= 360f;
		}
		while (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	private static float DegreeToRadian(float angle)
	{
		return Mathf.PI * angle / 180.0f;
	}

	private static float RadianToDegree(float angle)
	{
		var degree = angle * (180.0f / Mathf.PI);
		return degree < 0
			? degree + 360
			: degree;
	}

	private static bool WithinDegrees(Vector2 normal1, Vector2 normal2, float degrees)
	{
		return Vector3.Dot(normal1, normal2) >= Mathf.Cos(0.0174532924f * degrees);
	}
}

public static class Vector2Extensions {
	public static float Dot (this Vector2 vec1, Vector2 vec2) {
		return Vector2.Dot(vec1, vec2);
	}

	public static Vector2 Slide (this Vector2 vec1, Vector2 vec2) {
		return vec2 - vec1 * Vector2.Dot(vec1, vec2);
	}
}

public static class Collider2DExtensions {
	
	public static MotionTestResult TestMotion (
		this Collider2D collider,
		Vector2 motion) 
	{
		var collisionBox = collider.GetCollisionBox();

		var raycastHit2d = Physics2D.BoxCast(
			origin: collisionBox.center,
			size: collisionBox.size,
			angle: 0,
			direction: motion.normalized,
			distance: motion.magnitude
		);

		if (raycastHit2d) {
			var collision2d = new KinematicCollision2D {
				Owner = collider,
				Target = raycastHit2d.collider,
				Position = raycastHit2d.point,
				Normal = raycastHit2d.normal,
				Distance = raycastHit2d.distance
			};

			return new MotionTestResult {
				HasCollision = true,
				Collision = collision2d,
				Direction = motion.normalized,
				Distance = raycastHit2d.distance,
				Remainder = motion.magnitude - raycastHit2d.distance
			};
		}

		return new MotionTestResult {
			HasCollision = false,
			Collision = null,
			Direction = motion.normalized,
			Distance = motion.magnitude,
			Remainder = 0f
		};
	}

	public static Rect GetCollisionBox (this Collider2D collider) {
		return new Rect(
			x: collider.bounds.min.x,
			y: collider.bounds.min.y,
			width: collider.bounds.size.x,
			height: collider.bounds.size.y
		);
	}
}

public class MotionTestResult {
	public bool HasCollision { get; set; }
	public KinematicCollision2D Collision { get; set; } 
	public Vector2 Direction { get; set; }
	public float Distance { get; set; }
	public float Remainder { get; set; }
}

public class KinematicCollision2D {
	public Collider2D Owner { get; set; }
	public Collider2D Target { get; set; }
	public Vector2 Position { get; set; }
	public Vector2 Normal { get; set; }
	public float Distance { get; set; }
}

