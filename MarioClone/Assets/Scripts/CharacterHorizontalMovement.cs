using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformMovement2D))]
public class CharacterHorizontalMovement : MonoBehaviour {
	private PlatformMovement2D _platformMovement2D;
	public float Acceleration;
	public float Decceleration;
	public float MaxSpeed;

	// Use this for initialization
	void Start () {
		_platformMovement2D = GetComponent<PlatformMovement2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var horizontalInput = Input.GetAxis("Horizontal");
		Vector2 localSpeed = _platformMovement2D.LocalSpeed;
		var offset = Acceleration * Time.deltaTime * Normalize(horizontalInput);
		var speed = horizontalInput * MaxSpeed;
		var speedAbs = Mathf.Abs(speed);
		localSpeed.x = ClampedAdd(localSpeed.x, offset, -speedAbs, speedAbs);


		var offset2 = Decceleration * Time.deltaTime * (float)((localSpeed.x <= 0f) ? -1 : 1);
		if (Normalize(speed) != Normalize(localSpeed.x))
		{
			localSpeed.x = ClampedSubtract(localSpeed.x, offset2, 0f, 0f);
		}
		else
		{
			localSpeed.x = ClampedSubtract(localSpeed.x, offset2, -speedAbs, speedAbs);
		}
		this._platformMovement2D.LocalSpeed = localSpeed;
	}

	public static float Normalize(float x)
	{
		if (x == 0f)
		{
			return x;
		}
		return Mathf.Sign(x);
	}

	public static float ClampedAdd(float start, float offset, float min, float max)
	{
		if (offset > 0f && start < max)
		{
			return Mathf.Min(max, start + offset);
		}
		if (offset < 0f && start > min)
		{
			return Mathf.Max(min, start + offset);
		}
		return start;
	}

	public static float ClampedSubtract(float start, float offset, float min, float max)
	{
		if (start < min)
		{
			return Mathf.Min(min, start - offset);
		}
		if (start > max)
		{
			return Mathf.Max(max, start - offset);
		}
		return start;
	}
}
