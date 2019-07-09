using UnityEngine;
using System.Collections;

public class Barrage_StormTiny : MonoBehaviour {

	private GameObject PRFB_BULLET;
	private GameObject PRFB_GUIDE;

	private float age = 0f;
	private int state = 0;
	private float BULLET_CYCLE;
	private float bullet_cycle_counter;
	private float BULLET_SPD;

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_BULLET = Resources.Load("Prefabs/BarrageBullets/Bullet_Storm") as GameObject;
		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;

		BULLET_CYCLE = 0.2f;
		BULLET_SPD = 34f;

		bullet_cycle_counter = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		if(state == 0){
			mng.objects.InstantiateTiny(PRFB_GUIDE, this.transform.position);
			state = 1;
		}else if(state == 1){
			if(age > 0.2f) state = 2;
		}else if(state == 2){
			if(bullet_cycle_counter >= BULLET_CYCLE){
				bullet_cycle_counter -= BULLET_CYCLE;

				var bullet = mng.objects.InstantiateTiny(PRFB_BULLET, this.transform.position);
				var bulletComponent = bullet.GetComponent<BarrageBullet>();
				bulletComponent.Speed = BULLET_SPD;
			}
			if(age > 2f) state = 3;
		}else if(state == 3){
			Destroy(this.gameObject);
		}
	}
	void LateUpdate(){
		age += Time.deltaTime;
		bullet_cycle_counter += Time.deltaTime;
	}
}
