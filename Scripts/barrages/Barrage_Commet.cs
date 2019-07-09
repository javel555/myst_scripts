using UnityEngine;
using System.Collections;

public class Barrage_Commet : MonoBehaviour {
	private GameObject PRFB_GUIDE;
	private GameObject PRFB_COMMET;

	private float age;
	private float state = 0;

	public float FIRE_CYCLE_BASE;
	private float FIRE_CYCLE;
	private float fire_cycle_counter;
	public float CYCLE_RANGE;

	private Vector2 emitt_position;

	// Use this for initialization
	void Start () {
		var mng = MystGameManager.Instance;

		PRFB_GUIDE =  Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_COMMET =  Resources.Load("Prefabs/BarrageBullets/Bullet_Red") as GameObject;

		FIRE_CYCLE = Random.Range(FIRE_CYCLE_BASE, CYCLE_RANGE);
		fire_cycle_counter = 0f;

		emitt_position = Vector2.zero;
		emitt_position.y = mng.WINDOW_TOP_LEFT.y + 6;
	}
	
	// Update is called once per frame
	void Update () {
		var mng = MystGameManager.Instance;
		if(state == 0){
			if(fire_cycle_counter >= FIRE_CYCLE){
				fire_cycle_counter -= FIRE_CYCLE;
				StartCoroutine(this.fall_commet());
				FIRE_CYCLE = Random.Range(FIRE_CYCLE_BASE, CYCLE_RANGE);
			}

			for(int i=0; i < this.transform.childCount; i++){
				var commet = this.transform.GetChild(i);
				var cmp = commet.GetComponent<BarrageBullet>();
				if(cmp.state == 0){
					if(commet.transform.position.y < -1){
						// 画面外に出たら弾ける
						int stone_num = 3;
						for(int j=0; j < stone_num; j++){
							var stone = mng.objects.InstantiateTiny(PRFB_COMMET, commet.transform.position);
							var stone_cmp = stone.GetComponent<BarrageBullet>();
							stone_cmp.Angle = Random.Range(-30, 30);
							stone_cmp.Speed = Random.Range(14f, 20f);
							stone_cmp.state = 1;
							stone.transform.SetParent(this.transform);
						}
						// 自分は消える
						Destroy(commet.gameObject);
					}
				}
				//// 弾け演出、出来たけど重すぎ
				// if(cmp.state == 1){
				// 	// 角度と速さから方向ベクトルを作る
				// 	var normalizedDeg = cmp.Angle -0;
				// 	var rad = normalizedDeg * Mathf.Deg2Rad;
				// 	var direction = new Vector2(
				// 		Mathf.Cos(rad), Mathf.Sin(rad)
				// 	);
				// 	var distance = direction * cmp.Speed;
				// 	Debug.Log(distance);

				// 	// 方向ベクトルに重力加速度を適応
				// 	distance.y -= 0.98f;
				// 	// 方向ベクトルを向きと速さに戻す
				// 	var newRad = Mathf.Atan2(distance.y, distance.x);
				// 	var newDeg = newRad * Mathf.Rad2Deg;
				// 	if(newDeg < 0) newDeg += 360f;
				// 	var newAngle = newDeg +0;
				// 	var newSpd = distance.magnitude;

				// 	cmp.Angle = newAngle;
				// 	cmp.Speed = newSpd;
				// }
			}
		}
	}

	private void LateUpdate(){
		age += Time.deltaTime;
		fire_cycle_counter += Time.deltaTime;
	}

	IEnumerator fall_commet(){
		var mng = MystGameManager.Instance;
    // ランダムな位置に予告
		emitt_position.x = Random.Range(-5, mng.WINDOW_BOTTOM_RIGHT.x + 5);
		mng.objects.InstantiateTiny(PRFB_GUIDE, emitt_position);
		yield return new WaitForSeconds(0.5f);
		// 弾発射
		var commet = mng.objects.InstantiateTiny(PRFB_COMMET, emitt_position);
		commet.transform.localScale = Vector2.one * 2;
		var cmp = commet.GetComponent<BarrageBullet>();
		cmp.Angle = Random.Range(135f, 175f);
		cmp.Speed = Random.Range(6f,10f);
		cmp.state = 0;
		cmp.transform.SetParent(this.transform);
	}
}
