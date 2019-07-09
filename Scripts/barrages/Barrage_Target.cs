using UnityEngine;
using System.Collections;

public class Barrage_Target : MonoBehaviour {
	[SerializeField]
	public GameObject PRFB_BULLET;
	private GameObject PRFB_GUIDE;
	
	private float age;
	private float state = 0;
	
	[SerializeField]
	public float FIRE_CYCLE;
	private float fire_cycle_counter;
	[SerializeField]
	public int FIRE_NUMBER;
	private int fire_count;
	[SerializeField]
	public int FIRE_WAY;
	[SerializeField]
	public int FIRE_ANGLE_WIDTH;
	[SerializeField]
	public float FIRE_SPD;
	[SerializeField]
	public bool IS_FIXED;
	[SerializeField]
	public float FIXED_DEG;
	[SerializeField]
	public float FIXED_DEG_STEP;
	private float end_age = 0;
	public float REPEAT_WAIT;

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;

		fire_cycle_counter = 0f;
		fire_count = 0;
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
			fire_cycle_counter = 0f;
			fire_count = 0;
		}else if(state == 1){
			if(age > 0.5f) state = 2;
		}else if(state == 2){
			if(fire_cycle_counter >= FIRE_CYCLE){
				fire_cycle_counter -= FIRE_CYCLE;

				float deg = FIXED_DEG + 90f;
				FIXED_DEG += FIXED_DEG_STEP;
				if(!IS_FIXED){
					var diff = player_position - fixPosition;
					float rad = Mathf.Atan2(diff.y, diff.x);
					deg = rad * Mathf.Rad2Deg - 90;
				}

				for(int i=0; i < FIRE_WAY; i++){
					var b = mng.objects.InstantiateTiny(PRFB_BULLET, fixPosition);
					var c = b.GetComponent<BarrageBullet>();
					c.Speed = FIRE_SPD;

					if(FIRE_WAY == 1){
						c.Angle = deg;
					}else{
						float fire_angle_unit = (float)FIRE_ANGLE_WIDTH / (float)(FIRE_WAY - 1);
						float fire_start_angle = deg - (FIRE_ANGLE_WIDTH / 2f);
						c.Angle = fire_start_angle + (fire_angle_unit * i);
					}
				}

				fire_count++;
				if(fire_count > FIRE_NUMBER){
					state = 3;
					end_age = age;
				}
			}
		}else if(state == 3 && REPEAT_WAIT > 0){
			if(age > end_age + REPEAT_WAIT){
				age -= (end_age + REPEAT_WAIT);
				state = 0;
			}
		}
	}

	private void LateUpdate() {
		age += Time.deltaTime;
		fire_cycle_counter += Time.deltaTime;
	}
}
