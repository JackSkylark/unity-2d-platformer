using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlatformMovement2D : CharacterMotor2D {
	public Vector2 GroundContactNormal;
	public Vector2 WallLeftContactNormal;
	public Vector2 WallRightContactNormal;
	public Vector2 CeilingContactNormal;
	private Vector2 _groundContactNormal;
	private Vector2 _wallLeftContactNormal;
	private Vector2 _wallRightContactNormal;
	private Vector2 _ceilingContactNormal;
	public Vector2 LocalSpeed;
	public float GravityAngle;
	private CharacterMotor2D _motor;
	private CharacterGravity _gravity;
	public EnvironmentalCollisionState Ground = new EnvironmentalCollisionState();
	public EnvironmentalCollisionState Ceiling = new EnvironmentalCollisionState();
	public EnvironmentalCollisionState WallLeft = new EnvironmentalCollisionState();
	public EnvironmentalCollisionState WallRight = new EnvironmentalCollisionState();	
	public IObservable<ByteBrosCollision2D> OnCollisionGroundEvent;
	public IObservable<ByteBrosCollision2D> OnCollisionWallLeftEvent;
	public IObservable<ByteBrosCollision2D> OnCollisionWallRightEvent;
	public IObservable<ByteBrosCollision2D> OnLandOnGroundEvent;
	public bool IsJumping { get { return LocalSpeed.y > 0f; } }
	public bool IsInAir { get { return !IsOnGround; } }
	public bool IsOnCeiling { get { return LocalSpeed.y >= 0f && Ceiling.IsOn; } }
	public bool IsOnGround { get { return LocalSpeed.y <= 0.0001f && Ground.IsOn; } }
	public bool IsOnGroundOrCeiling { get { return IsOnGround || IsOnCeiling; } }
	public bool HasWallLeft { get { return LocalSpeed.x <= 0f && WallLeft.IsOn; } }
	public bool HasWallRight { get { return LocalSpeed.x >= 0f && WallRight.IsOn; } }

	public override void Awake () {
		base.Awake();
		_motor = GetComponent<CharacterMotor2D>();
		_gravity = GetComponent<CharacterGravity>();

		OnCollisionGroundEvent = _motor.OnCollision
			.Where(x => x.IsGroundCollision(60f));

		OnLandOnGroundEvent = OnCollisionGroundEvent
			.Where(x => !Ground.IsOn);

		OnCollisionWallLeftEvent = _motor.OnCollision
			.Where(x => x.IsWallLeftCollision(30f));

		OnCollisionWallRightEvent = _motor.OnCollision
			.Where(x => x.IsWallRightCollision(30f));
	}

	// Use this for initialization
	public override void Start () {
		base.Start();
		OnCollisionGroundEvent.Subscribe(OnGroundCollision);
		OnCollisionWallLeftEvent.Subscribe(OnWallLeftCollision);
		OnCollisionWallRightEvent.Subscribe(OnWallRightCollision);
	}

	private void OnGroundCollision(ByteBrosCollision2D collision) {
		_groundContactNormal += collision.Normal.normalized;
		Ground.FutureOn = true;
	}

	private void OnWallLeftCollision(ByteBrosCollision2D collision) {
		_wallLeftContactNormal = collision.Normal.normalized;
		WallLeft.FutureOn = true;
	}

	private void OnWallRightCollision(ByteBrosCollision2D collision) {
		_wallRightContactNormal = collision.Normal.normalized;
		WallRight.FutureOn = true;
	}

	void UpdateRays() {
		if (Ground.IsOnOrFutureOn)
		{
			var testResults = Collider.TestMotion(Vector2.down * CollisionMargin);
			if (testResults.HasCollision) {
				OnCollisionNext(testResults.Collision);
			}
		}
	}

	void UpdateNormals() {
		if (!WallRight.IsOn) {
			WallRightContactNormal = Vector3.left;
		}
		if (!Ground.IsOn) {
			GroundContactNormal = Vector3.up;
		}
		if (!WallLeft.IsOn) {
			WallLeftContactNormal = Vector3.right;
		}
		if (!Ceiling.IsOn) {
			CeilingContactNormal = Vector3.down;
		}
		if (_groundContactNormal.sqrMagnitude != 0f)
		{
			GroundContactNormal = _groundContactNormal.normalized;
		}
		if (_ceilingContactNormal.sqrMagnitude != 0f)
		{
			CeilingContactNormal = _ceilingContactNormal.normalized; 
		}
		if (_wallLeftContactNormal.sqrMagnitude != 0f)
		{
			WallLeftContactNormal = _wallLeftContactNormal.normalized;
		}
		if (_wallRightContactNormal.sqrMagnitude != 0f)
		{
			WallRightContactNormal = _wallRightContactNormal.normalized;
		}

		_groundContactNormal = Vector2.zero;
		_ceilingContactNormal = Vector2.zero;
		_wallRightContactNormal = Vector2.zero;
		_wallLeftContactNormal = Vector2.zero;
	}

	void FixedUpdate() {
		LocalSpeed = _gravity.TransformVelocity(LocalSpeed);

		if (HasWallLeft || HasWallRight) {
			LocalSpeed.x = Mathf.Clamp(LocalSpeed.x, -12f, 12f);
			LocalSpeed.x *= 0.9f;

			if (LocalSpeed.x < 0.5f) {
				LocalSpeed.x = 0f;
			}
		}

		if (IsOnGround && LocalSpeed.y < 0f) {
			LocalSpeed.y = 0f;
		}

		if (IsOnCeiling && LocalSpeed.y > 0f) {
			LocalSpeed.y = 0f;
		}

		Move(LocalSpeed * Time.deltaTime);
		UpdateRays();
		
		// Correct For Approximate Movement

		if (IsOnGround) {
			Debug.DrawRay(transform.position, GroundContactNormal * -0.4f, Color.red);
			var groundNormalTest = TestMovement(GroundContactNormal * -0.4f);
			if (groundNormalTest.HasCollision) {
				this.Position += groundNormalTest.Distance * groundNormalTest.Direction;
				OnCollisionNext(groundNormalTest.Collision);
			}

			if (HasWallLeft) {
				var wallLeftTest = TestMovement(WallLeftContactNormal * -0.4f);
				if (wallLeftTest.HasCollision) {
					OnCollisionNext(wallLeftTest.Collision);
				}
			}

			if (HasWallRight) {
				var wallRightTest = TestMovement(WallRightContactNormal * -0.4f);
				if (wallRightTest.HasCollision) {
					OnCollisionNext(wallRightTest.Collision);
				}
			}
		}

		PostFixedUpdate();
		UpdateNormals();
	}

	public void PostFixedUpdate() {
		Ground.Update();
		WallLeft.Update();
		WallRight.Update();
	}
}
