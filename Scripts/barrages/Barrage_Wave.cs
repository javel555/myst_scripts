using UnityEngine;
using System.Collections;

public class Barrage_Wave : MonoBehaviour {

	private GameObject PRFB_GUIDE;
	private GameObject PRFB_LASER;

	private float age;
	private float state = 0;

	public float FIRE_CYCLE;
	private float fire_cycle_counter;
	public float FIRE_NUM;

	private float emmit_position_unit_y;

	private float noise_seed;

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE =  Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_LASER =  Resources.Load("Prefabs/BarrageBullets/Bullet_Laser_Vartical") as GameObject;

		fire_cycle_counter = 0f;
		emmit_position_unit_y = mng.WINDOW_TOP_LEFT.y / FIRE_NUM;
		FIRE_NUM += 1;

		noise_seed = this.gameObject.GetInstanceID();
	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;

		if(state == 0){
			for(int i=0; i<FIRE_NUM; i++){
				mng.objects.InstantiateTiny(PRFB_GUIDE,
				  new Vector2(mng.WINDOW_BOTTOM_RIGHT.x, emmit_position_unit_y * i));
			}
			state = 1;
		}else if(state == 1){
			if(age > 0.5f) state = 2;
		}else if(state == 2){
			if(fire_cycle_counter >= FIRE_CYCLE){
				fire_cycle_counter -= FIRE_CYCLE;
				var empty_slot = Mathf.PerlinNoise(noise_seed, age * 0.2f);
				empty_slot = Mathf.RoundToInt(empty_slot * FIRE_NUM);
				int range = 2;

				for(int i=0; i<FIRE_NUM; i++){
					if(i >= empty_slot - range && i <= empty_slot + range) continue;

					var laser = mng.objects.InstantiateTiny(PRFB_LASER,
						new Vector2(mng.WINDOW_BOTTOM_RIGHT.x, emmit_position_unit_y * i));
					var cmp = laser.GetComponent<BarrageBullet>();
					cmp.Angle = 90;
					cmp.Speed = 6f;
					cmp.lifeTime = 20f;
				}
			}
		}
	}

	
	void LateUpdate(){
		age += Time.deltaTime;
		fire_cycle_counter += Time.deltaTime;
	}
}
