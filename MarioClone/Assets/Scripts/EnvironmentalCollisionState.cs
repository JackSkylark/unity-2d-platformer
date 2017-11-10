using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalCollisionState {
	public bool WasOn;

	public bool IsOn;

	public bool FutureOn;

	public ByteBrosCollision2D Collision;

	public bool IsOnOrFutureOn
	{
		get
		{
			return this.IsOn || this.FutureOn;
		}
	}

	public bool WasOnButNotIsOn
	{
		get
		{
			return this.WasOn && !this.IsOn;
		}
	}

	public bool OnThisFrame
	{
		get
		{
			return !this.WasOn && this.IsOn;
		}
	}

	public bool OffThisFrame
	{
		get
		{
			return this.WasOn && !this.IsOn;
		}
	}

	public void Update()
	{
		this.WasOn = this.IsOn;
		this.IsOn = this.FutureOn;
		this.FutureOn = false;
	}
}
