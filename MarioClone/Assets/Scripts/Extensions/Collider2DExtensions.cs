using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			var collision2d = new ByteBrosCollision2D {
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
	public ByteBrosCollision2D Collision { get; set; } 
	public Vector2 Direction { get; set; }
	public float Distance { get; set; }
	public float Remainder { get; set; }
}

public class ByteBrosCollision2D {
	public Collider2D Owner { get; set; }
	public Collider2D Target { get; set; }
	public Vector2 Position { get; set; }
	public Vector2 Normal { get; set; }
	public float Distance { get; set; }
}
