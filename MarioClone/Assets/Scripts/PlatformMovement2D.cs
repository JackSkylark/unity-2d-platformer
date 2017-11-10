using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlatformMovement2D : CharacterMotor2D {
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

		OnLandOnGroundEvent.Subscribe(x => 
			Debug.Log(x.Position));
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

		PostFixedUpdate();
	}

	public void PostFixedUpdate() {
		Ground.Update();
		WallLeft.Update();
		WallRight.Update();
	}

	
}
