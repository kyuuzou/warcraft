using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamera : SceneObject {

	public float LeftBound  { get; private set; }
	public float LowerBound { get; private set; }
	public float RightBound { get; private set; }
	public float UpperBound { get; private set; }

	public Vector2 Size { get; private set; }

	public override void InitializeExternals () {
		if (this.InitializedExternals) {
			return;
		}

		base.InitializeExternals ();

		Vector3 position = this.transform.position;
		float orthographicSize = this.Camera.orthographicSize;

		this.LeftBound  = position.x - orthographicSize * this.Camera.aspect;
		this.LowerBound = position.y - orthographicSize;
		this.RightBound = position.x + orthographicSize * this.Camera.aspect;
		this.UpperBound = position.y + orthographicSize;

		this.Size = new Vector2 (this.RightBound - this.LeftBound, this.UpperBound - this.LowerBound);
	}
}
