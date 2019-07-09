using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MystEnemyHorming : EnemyBase {

	public float MoveSpeed;

	// Use this for initialization
	public override void Start () {
		base.Start();
		this.rigi_main.isKinematic = true;

	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();

		var mng = MystGameManager.Instance;

		var newPos = this.transform.position;
		var distance = mng.GOBJ_PLAYER.transform.position - newPos;
		float rad = Mathf.Atan2(distance.y, distance.x);
		float deg = rad * Mathf.Rad2Deg;
		if(deg < 0) deg += 360;
		var direction = Quaternion.Euler(0, 0, deg) * Vector2.right;
		newPos += direction * MoveSpeed * Time.deltaTime;

		this.transform.position = newPos;
	}
}
