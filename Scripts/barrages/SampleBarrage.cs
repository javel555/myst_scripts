using UnityEngine;
using System.Collections;

public class SampleBarrage : MonoBehaviour {
	private GameObject prfb_bullet;
	private GameObject gobj_bullet_store;

	private float age = 0f;
	private int state = 0;

	private float fireCycle = 0.1f;
	private float fireCycleCounter = 0f;
	private int fireCount = 0;
	private float stay_start_time = 0f;

	// Use this for initialization
	void Start () {
		prfb_bullet = Resources.Load("Prefabs/BarrageBullets/Bullet_Storm") as GameObject;
		gobj_bullet_store = new GameObject();
	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		if(state == 0){
			if(fireCycleCounter >= fireCycle){
				fireCycleCounter -= fireCycle;

				var fixPosition = this.transform.position;
				fixPosition.y++;

				var diff =  mng.GOBJ_PLAYER.transform.position - fixPosition;
  			float rad = Mathf.Atan2(diff.y, diff.x);
        float deg = rad * Mathf.Rad2Deg - 90;

				var bullet = mng.objects.InstantiateTiny(prfb_bullet, fixPosition);
				bullet.transform.SetParent(gobj_bullet_store.transform);
				var bulletComp = bullet.GetComponent<BarrageBullet>();
				bulletComp.Angle = deg;
				bulletComp.Speed = 10f;

				fireCount++;
				if(fireCount > 8){
					fireCycleCounter = 0f;
					fireCount = 0;
					stay_start_time = age;
					state = 1;
				}

			}
		}else{
			if(age > stay_start_time + 2f){
				state = 0;
			}
		}
	}

	private void LateUpdate()
	{
		this.age += Time.deltaTime;
		this.fireCycleCounter += Time.deltaTime;
	}

	private void OnDestroy() {
		Destroy(this.gobj_bullet_store);
	}
}
