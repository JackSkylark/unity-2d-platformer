using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public abstract class CharacterMotor2D : MonoBehaviour {
    private Collider2D _collider;
    public Collider2D Collider { get { return _collider; } }

    public Vector2 Position 
	{ 
		get { return transform.position; } 
		set { transform.position = value; } 
	}
    public float CollisionMargin = 0.01f;
    private Subject<ByteBrosCollision2D> _onCollision = new Subject<ByteBrosCollision2D>();
    public IObservable<ByteBrosCollision2D> OnCollision { get { return _onCollision.AsObservable(); } }

    public virtual void Awake() {
        _collider = this.GetComponent<Collider2D>();
    }

    public virtual void Start() {
        
    }

	public void OnCollisionNext(ByteBrosCollision2D collision) {
		_onCollision.OnNext(collision);
	}

    public MotionTestResult TestMovement(Vector2 delta) {
        return _collider.TestMotion(delta, CollisionMargin);
    }

    public void Move(Vector2 delta) {
        var motionTest = TestMovement(delta);
		if (motionTest.HasCollision) {
			var normal = motionTest.Collision.Normal;
			Position += motionTest.Distance * delta.normalized;
            _onCollision.OnNext(motionTest.Collision);
			var remainderMovementDelta = motionTest.Remainder * motionTest.Direction;

			var delta2 = normal.Slide(remainderMovementDelta);
			if ((double)delta2.magnitude < 0.004) 
			{
				return;
			}

			this.Move(delta2);
		}
		else {
			Position += delta;
		}
    }
}