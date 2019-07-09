using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Barrage_Pillar : MonoBehaviour {

	private GameObject PRFB_GUIDE;
	private GameObject PRFB_BULLET;
	private AudioSource ADS_SE;

	private int state = 0;
	private int subState = 0;
	private int fireCount = 0;
	private float age;
	public float FIRE_CYCLE;
	private float fire_cycle_counter;

	private Rect PUT_AREA;
	private int PUT_NUMBER;

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_BULLET = Resources.Load("Prefabs/BarrageBullets/Bullet_Laser") as GameObject;

		ADS_SE = this.GetComponent<AudioSource>();

		PUT_NUMBER = 60;

		var currentPosition = this.transform.position;
		var width = 4;
		PUT_AREA = new Rect(
			currentPosition.x - width / 2,
			currentPosition.y,
			width,
			mng.WINDOW_TOP_LEFT.y
		);
		Debug.Log(PUT_AREA.yMax);
		Debug.Log(PUT_AREA.yMin);
	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		var player_position = mng.GOBJ_PLAYER.transform.position;
		var fixPosition = this.transform.position;
		fixPosition.y++;

		if(state == 0){
			if(fire_cycle_counter >= FIRE_CYCLE){
				fire_cycle_counter -= FIRE_CYCLE;

				if(fireCount % 4 == 0){
					mng.objects.InstantiateTiny(PRFB_GUIDE, new Vector2(
						PUT_AREA.x + PUT_AREA.width / PUT_NUMBER * fireCount,
						PUT_AREA.yMin
					));
				}

				var putPos = new Vector2(
					Random.Range(PUT_AREA.xMin, PUT_AREA.xMax),
					Random.Range(PUT_AREA.yMin, PUT_AREA.yMax)
				);
				var b = mng.objects.InstantiateTiny(PRFB_BULLET, putPos);
				var c = b.GetComponent<BarrageBullet>();
				c.Speed = 0;
				c.Angle = 180;
				c.lifeTime = 10;
				b.transform.SetParent(this.transform);
				b.transform.localScale *= 2f;

				fireCount++;
				if(fireCount > PUT_NUMBER){
					state = 1;
				}
			}
		}else if(state == 1){
			iTween.MoveTo(this.gameObject, iTween.Hash(
				"y", mng.WINDOW_BOTTOM_RIGHT.y,
				"time", 3f,
				"easeType", iTween.EaseType.easeOutBounce
			));
			iTween.MoveTo(this.gameObject, iTween.Hash(
				"y", mng.WINDOW_TOP_LEFT.y * -2,
				"time", 2f,
				"easeType", iTween.EaseType.easeInCubic,
				"delay", 3f
			));
			state = 2;
		}else if(state == 2){
			if(age > 3.1f){
				ADS_SE.Play();
				state = 3;
			}
		}
	}
	private void LateUpdate() {
		age += Time.deltaTime;
		fire_cycle_counter += Time.deltaTime;
	}
}
