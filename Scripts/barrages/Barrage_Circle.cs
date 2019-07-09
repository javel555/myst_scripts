using UnityEngine;
using System.Collections;

public class Barrage_Circle : MonoBehaviour {

	private GameObject PRFB_GUIDE;
	private GameObject PRFB_BULLET;

	private int state = 0;
	private float age;
	public float FIRE_CYCLE;
	private float fire_cycle_counter;


	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_BULLET = Resources.Load("Prefabs/BarrageBullets/Bullet_Storm") as GameObject;

	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		var player_position = mng.GOBJ_PLAYER.transform.position;
		var fixPosition = this.transform.position;
		fixPosition.y++;

		if(state == 0){
			mng.objects.InstantiateTiny(PRFB_GUIDE, fixPosition);
			state = 1;
		}else if(state == 1){
			if(age > 0.5f) state = 2;
		}else if(state == 2){
			if(fire_cycle_counter >= FIRE_CYCLE){
				fire_cycle_counter -= FIRE_CYCLE;

				var fireNum = 4;
				var spd = 6f;
				for(int i = 0; i < fireNum; i++){
					var putAngle = 360 / fireNum * i + age * -50f;
					var putPos = Quaternion.Euler(0,0,putAngle) * Vector3.up * 1.5f + fixPosition;

					var b = mng.objects.InstantiateTiny(PRFB_BULLET, putPos);
					var c = b.GetComponent<BarrageBullet>();
					c.Angle = putAngle + 90;
					c.Speed = spd;
				}
				// for(int i = 0; i < fireNum; i++){
				// 	var putAngle = 360 / fireNum * i + age * 50f;
				// 	var putPos = Quaternion.Euler(0,0,putAngle) * Vector3.up * 2f + fixPosition;

				// 	var b = mng.objects.InstantiateTiny(PRFB_BULLET, putPos);
				// 	var c = b.GetComponent<BarrageBullet>();
				// 	c.Angle = putAngle - 90;
				// 	c.Speed = spd;
				// }


			}
		}
	
	}

	private void LateUpdate() {
		age += Time.deltaTime;
		fire_cycle_counter += Time.deltaTime;
	}
}
