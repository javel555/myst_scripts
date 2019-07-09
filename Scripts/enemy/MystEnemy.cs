using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MystEnemy : EnemyBase {

	public Vector2 MoveVecter;
	// 備忘録：子のBarrageをY-1移動しておかないと発射位置がずれる

	// Use this for initialization
	public override void Start () {
		base.Start();
		this.rigi_main.isKinematic = true;
		// base.mainFunc = new IEnumerator[1];
		// base.mainFunc[0] = this.Move();
	}
	
	// Update is called once per frame
	public override void Update(){
		base.Update();

		var newPos = this.transform.position;
		newPos += (Vector3)MoveVecter * Time.deltaTime;
		this.transform.position = newPos;
		
	}
	// IEnumerator Move () {
	// 	while(true){

	// 	}
	// }
}
