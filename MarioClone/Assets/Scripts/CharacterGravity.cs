using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : MonoBehaviour {
	private PlatformMovement2D _movementState;
	public float GravityAngle;
	public float GravityStrength = 26f;
	public float MaxFallSpeed = 38f;

	void Awake() {
		_movementState = GetComponent<PlatformMovement2D>();
	}

	public Vector2 TransformVelocity(Vector2 initialSpeed) {
		if (!_movementState.Ground.IsOn) {
			return ApplyGravity(initialSpeed, GravityStrength, MaxFallSpeed);
		}

		return initialSpeed;
	}

	private static Vector2 ApplyGravity(Vector2 initialSpeed, float gravityStrength, float maxFallSpeed) 
	{
		var ySpeed = initialSpeed.y - gravityStrength * Time.deltaTime;
		ySpeed = Mathf.Max(-maxFallSpeed, ySpeed);

		return new Vector2(initialSpeed.x, ySpeed);
	}
}
