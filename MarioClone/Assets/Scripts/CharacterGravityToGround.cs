using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravityToGround : MonoBehaviour {
    private PlatformMovement2D _movementState;

    void Awake() {
        _movementState = GetComponent<PlatformMovement2D>();
    }

	public Vector2 TransformVelocity(Vector2 initialSpeed) {
		return initialSpeed;
	}
}


