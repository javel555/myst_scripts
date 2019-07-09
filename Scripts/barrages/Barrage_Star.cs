using UnityEngine;
using System.Collections;

public class Barrage_Star : MonoBehaviour {
	
	private GameObject PRFB_GUIDE;
	private GameObject PRFB_BULLET;

	private int state = 0;
	private int subState = 0;
	private int fireCount = 0;
	private float age;
	public float FIRE_CYCLE;
	private float fire_cycle_counter;

	private Vector3[] putPositions = new Vector3[5];
	private Vector3 putPositionBase;

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_BULLET = Resources.Load("Prefabs/BarrageBullets/Bullet_Red") as GameObject;

		for(int i = 0; i < 5; i++){
			var putAngle = -360 / 5 * i;
			this.putPositions[i] = Quaternion.Euler(0,0,putAngle) * Vector3.up * 11f;
		}
		putPositionBase = mng.GOBJ_PLAYER.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		var fixPosition = this.transform.position;
		fixPosition.y++;

		if(state == 0){
			mng.objects.InstantiateTiny(PRFB_GUIDE, putPositionBase);
			state = 1;
		}else if(state == 1){
			if(age > 0.5f) state = 2;
		}else if(state == 2){
			var putNum = 6;
			if(fire_cycle_counter >= FIRE_CYCLE){
				fire_cycle_counter -= FIRE_CYCLE;
				
				int fromIndex = 0;
				int toIndex = 3;
				if(subState == 0){
					// 0-3
					// nop
				}else if(subState == 1){
					// 3-1
					fromIndex = 3;
					toIndex = 1;
				}else if(subState == 2){
					fromIndex = 1;
					toIndex = 4;
				}else if(subState == 3){
					fromIndex = 4;
					toIndex = 2;
				}else if(subState == 4){
					fromIndex = 2;
					toIndex = 0;
				}

				// put bullet
				var from = this.putPositions[fromIndex] + putPositionBase;
				var to = this.putPositions[toIndex] + putPositionBase;
				var putPos = Vector2.Lerp(from, to, (float)fireCount / putNum);
				var b = mng.objects.InstantiateTiny(PRFB_BULLET, putPos);
				b.transform.SetParent(this.transform);
				var c = b.GetComponent<BarrageBullet>();
				c.Speed = 0;
				c.lifeTime = 100;

				Vector3 diff = putPositionBase - (Vector3)putPos;
				float rad = Mathf.Atan2(diff.y, diff.x);
				float deg = rad * Mathf.Rad2Deg - 90;
				c.Angle = deg;

				if(fireCount == putNum){
					subState++;
					fireCount = 0;
					if(subState == 5){
						state++;
						subState = 0;
					}
				}else{
					fireCount++;
				}
			}
		}else if(state == 3){
			// ちょっと待つ
			if(age > 3f) state++;
		}else if(state == 4){
			// 弾移動
			for(int i=0; i < this.transform.childCount; i++){
				var b = this.transform.GetChild(i);
				var c = b.GetComponent<BarrageBullet>();
				c.Speed = 8f;
			}
			state = 5;
		}else if(state == 5){
			this.transform.DetachChildren();
			Destroy(this.gameObject, 1f);
		}

	}
	
	private void LateUpdate() {
		age += Time.deltaTime;
		fire_cycle_counter += Time.deltaTime;
	}
}
