using UnityEngine;
using System.Collections;

public class Barrage_Storm : MonoBehaviour {
	private GameObject PRFB_EMITTER;
	private GameObject PRFB_BULLET;
	private GameObject PRFB_GUIDE;
	private Vector2 EMITTER_UNDER_POSITION;
	private Vector2 EMITTER_TOP_POSITION;
	private Vector2 emitter_position;

	private float age = 0f;
	private int state = 0;
	public bool is_top = false;

	private float EMITTER_CYCLE;
	private float emitter_cycle_counter;
	private int EMITTER_NUMBER;
	private int emitter_count;
	private float EMITTER_SPD;

	private float BULLET_CYCLE;
	private float bullet_cycle_counter;
	private float BULLET_SPD;

	private GameObject[] emitter_collection;
	// private ArrayList bullet_collection = new ArrayList();

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_EMITTER = Resources.Load("Prefabs/BarrageBullets/Bullet_Storm_Emitter") as GameObject;
		PRFB_BULLET = Resources.Load("Prefabs/BarrageBullets/Bullet_Storm") as GameObject;

		EMITTER_CYCLE = 0.3f;
		EMITTER_NUMBER = 5;
		EMITTER_SPD = 3f;

		BULLET_CYCLE = 0.2f;
		BULLET_SPD = 34f;

		EMITTER_TOP_POSITION = new Vector2(
			mng.WINDOW_BOTTOM_RIGHT.x + 2,
			mng.WINDOW_TOP_LEFT.y);
		EMITTER_UNDER_POSITION = new Vector2(
			mng.WINDOW_BOTTOM_RIGHT.x + 2,
			mng.WINDOW_BOTTOM_RIGHT.y);

		emitter_cycle_counter = 0f;
		emitter_count = 0;
		emitter_collection = new GameObject[EMITTER_NUMBER];
		emitter_position = EMITTER_UNDER_POSITION;
		if(is_top) emitter_position = EMITTER_TOP_POSITION;

		bullet_cycle_counter = 0f;

	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		if(state == 0){
			var pos = emitter_position;
			pos.x = mng.WINDOW_BOTTOM_RIGHT.x;
			mng.objects.InstantiateTiny(PRFB_GUIDE, pos);
			state = 1;
		}else if(state == 1){
			if(age > 0.5f) state = 2;
		}else if(state == 2){
			// 竜巻玉弾
			if(emitter_cycle_counter >= EMITTER_CYCLE){
				emitter_cycle_counter -= EMITTER_CYCLE;

				var bullet = mng.objects.InstantiateTiny(PRFB_EMITTER, emitter_position);
				emitter_collection[emitter_count] = bullet;
				var bulletComp = bullet.GetComponent<BarrageBullet>();
				bulletComp.Angle = 90;
				bulletComp.Speed = EMITTER_SPD;
				bulletComp.lifeTime *= 2;

				emitter_count++;
				if(emitter_count >= EMITTER_NUMBER){
					emitter_cycle_counter = 0f;
					emitter_count = 0;
					state = 3;
				}
			}
		}else if(state == 3){
			bool is_clear = true;
			foreach(GameObject emitter in emitter_collection){
				if (emitter != null) { is_clear = false;}
			}
			if(is_clear){
				Destroy(this.gameObject);
			}
		}

    if( bullet_cycle_counter >= BULLET_CYCLE){
			bullet_cycle_counter -= BULLET_CYCLE;
			
			foreach(GameObject emitter in emitter_collection){
				if(emitter == null) continue;

				var b = mng.objects.InstantiateTiny(PRFB_BULLET, emitter.transform.position);
				var c = b.GetComponent<BarrageBullet>();
				b.transform.SetParent(emitter.transform);
				if(this.is_top){c.Angle = 180;}

				c.Speed = BULLET_SPD;
			}
		}

	}

	void LateUpdate()
	{
		age += Time.deltaTime;
		emitter_cycle_counter += Time.deltaTime;
		bullet_cycle_counter += Time.deltaTime;
	}
	
}
