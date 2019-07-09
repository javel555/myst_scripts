using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MystEnemyHopper : EnemyBase {

	public Vector2 MoveVecter;
	// public Vector2 HopperVecter;
	private float age = 0;
	// private float hop_cycle_counter = 0;
	// public float HOP_CYCLE = 0;
	// private float progress = 0;
	// public float SPEED = 0;
	public float HEIGHT = 0;
	public float CYCLE = 0;
	// 備忘録：子のBarrageをY-1移動しておかないと発射位置がずれる

	// Use this for initialization
	private Vector2 original;
	public override void Start () {
		base.Start();
		this.rigi_main.isKinematic = true;
		this.original = this.transform.position;
		// base.mainFunc = new IEnumerator[1];
		// base.mainFunc[0] = this.Move();
	}
	
	// Update is called once per frame
	public override void Update(){
		base.Update();

		var newPos = this.original;
		newPos += (Vector2)MoveVecter * Time.deltaTime;
		this.original = newPos;

		var hopp = HEIGHT * Mathf.Sin(age * CYCLE);
		if(hopp < 0) hopp *= -1;
		newPos.y += hopp;

		// if(HOP_CYCLE != 0){
			
		// }else{
		// }

		this.transform.position = newPos;
		
	}
	// IEnumerator Move () {
	// 	var startX = this.transform.position.x;
	// 	while(true){
	// 		float x = this.transform.position.x;
	// 		float y = HEIGHT * (x - 1f) * (x - 2f);
	// 		var newPos = new Vector2(
	// 			x + SPEED * Time.deltaTime,
	// 			y
	// 		);
	// 		this.transform.position = newPos;
	// 		yield return new WaitForSeconds(HOP_CYCLE);
	// 	}
	// }
	public override void LateUpdate(){
		base.LateUpdate();
		age += Time.deltaTime;
	}
}
