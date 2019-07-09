using UnityEngine;
using System.Collections;

public class BossAction {

	MystGameManager MNG;
	GameObject PRFB_GUIDE;
	GameObject PRFB_BREAK;

	// Phase1
	GameObject PRFB_STORM;
	GameObject PRFB_MEN_TARGET_LASER;
	GameObject PRFB_LASER;

	GameObject PRFB_BARRAGE_SAMPLE;
	GameObject PRFB_STORM_TOP;
	GameObject PRFB_STORM_BOTTOM;
	GameObject PRFB_TARGET;
	GameObject PRFB_LASER_WALL_TOP;
	GameObject PRFB_LASER_WALL_BOTTOM;
	GameObject PRFB_ROTATE;
	GameObject PRFB_STRATE;

	GameObject PRFB_WAVE;
	GameObject PRFB_CIRCLE;
	GameObject PRFB_MEN_TARGET2WAY;
	GameObject PRFB_MEN_HORMING_TARGET2WAY;
	GameObject PRFB_2WAY;

	GameObject PRFB_STAR;
	GameObject PRFB_PILLAR;
	GameObject PRFB_MEN_HOPPER;
	
	public int boss_state;
	float age;
	float action_counter;

	Coroutine currentCoroutine;

	public BossAction(){
		MNG = MystGameManager.Instance;
		PRFB_GUIDE = Resources.Load("Prefabs/Effects/Guide") as GameObject;
		PRFB_BREAK = Resources.Load("Prefabs/Effects/Damage") as GameObject;

		PRFB_STORM = Resources.Load("Prefabs/Barrage/Storm_Tiny") as GameObject;
		PRFB_MEN_TARGET_LASER = Resources.Load("Prefabs/Barrage/WithEnemy/MystEnemy_Target_Laser") as GameObject;
		PRFB_LASER = Resources.Load("Prefabs/Barrage/Target_Laser") as GameObject;
		
		PRFB_BARRAGE_SAMPLE = Resources.Load("Prefabs/Barrage/SampleBarrage") as GameObject;
		PRFB_STORM_TOP =      Resources.Load("Prefabs/Barrage/Storm_Top") as GameObject;
		PRFB_STORM_BOTTOM =   Resources.Load("Prefabs/Barrage/Storm_Bottom") as GameObject;
		PRFB_TARGET =         Resources.Load("Prefabs/Barrage/Target_Way") as GameObject;

		PRFB_LASER_WALL_TOP =    Resources.Load("Prefabs/Barrage/Laser_Wall_Top") as GameObject;
		PRFB_LASER_WALL_BOTTOM = Resources.Load("Prefabs/Barrage/Laser_Wall_Bottom") as GameObject;
		PRFB_ROTATE = Resources.Load("Prefabs/Barrage/Rotate_Fixed") as GameObject;
		PRFB_STRATE = Resources.Load("Prefabs/Barrage/Strate_Quik") as GameObject;

		PRFB_WAVE = Resources.Load("Prefabs/Barrage/Wave") as GameObject;
		PRFB_CIRCLE = Resources.Load("Prefabs/Barrage/Circle") as GameObject;
		PRFB_MEN_TARGET2WAY = Resources.Load("Prefabs/Barrage/WithEnemy/MystEnemy_Target2Way") as GameObject;
		PRFB_MEN_HORMING_TARGET2WAY = Resources.Load("Prefabs/Barrage/WithEnemy/MystEnemyHorming_Target2Way") as GameObject;

		PRFB_STAR = Resources.Load("Prefabs/Barrage/Star") as GameObject;
		PRFB_PILLAR = Resources.Load("Prefabs/Barrage/Pillar") as GameObject;
		PRFB_MEN_HOPPER = Resources.Load("Prefabs/Barrage/WithEnemy/MystEnemyHopper_R") as GameObject;

		age = 0f;
		action_counter = 0f;
		boss_state = 0;
	}

	public void Start(){
		if(this.currentCoroutine != null) MNG.StopCoroutine(currentCoroutine);
		if(boss_state == 0){
		  this.currentCoroutine = MNG.StartCoroutine(this.FirstAction());
		}else if(boss_state == 1){
			this.currentCoroutine = MNG.StartCoroutine(this.SecondAction());
		}else{
			this.currentCoroutine = MNG.StartCoroutine(this.ThirdAction());
		}
	}
	public void Stop(){
		iTween.Stop(MNG.GOBJ_BOSS);
		if(this.currentCoroutine != null) MNG.StopCoroutine(currentCoroutine);
	}

	public void Update(){
		// Debug.Log(this.mainCoroutine);
		if(MNG.boss_life <= (MNG.BOSS_LIFE_MAX / 3 * 2) && boss_state == 0){
			this.Stop();
			// 出した弾を消す
			var bullets = GameObject.FindGameObjectsWithTag("Bullet(Enemy)");
			var enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(var bullet in bullets){
				var showEffect = Random.Range(0,2);
				if(showEffect == 0) MNG.objects.InstantiateTiny(PRFB_BREAK, bullet.transform.position);
				MystGameManager.Destroy(bullet);
			}
			foreach(var enemy in enemies){
				MystGameManager.Destroy(enemy);
			}

			boss_state = 1;
			this.currentCoroutine = MNG.StartCoroutine(this.SecondAction());
		}

		if(MNG.boss_life <= (MNG.BOSS_LIFE_MAX / 3) && boss_state == 1){
			this.Stop();

			// 出した弾を消す
			var bullets = GameObject.FindGameObjectsWithTag("Bullet(Enemy)");
			var enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(var bullet in bullets){
				var showEffect = Random.Range(0,2);
				if(showEffect == 0) MNG.objects.InstantiateTiny(PRFB_BREAK, bullet.transform.position);
				MystGameManager.Destroy(bullet);
			}
			foreach(var enemy in enemies){
				MystGameManager.Destroy(enemy);
			}

			boss_state = 2;
			this.currentCoroutine = MNG.StartCoroutine(this.ThirdAction());
		}

	}

	public IEnumerator FirstAction(){
		// フェーズ1
		yield return new WaitForSeconds(0.2f);
		for(;;){
			// 竜巻攻撃
			var STORM_PUT_NUMBER = 10;
			for(int i=0; i < STORM_PUT_NUMBER + 1; i++){
				var putPos = new Vector2(
					(MNG.WINDOW_BOTTOM_RIGHT.x / STORM_PUT_NUMBER) * (STORM_PUT_NUMBER - i),
					MNG.WINDOW_BOTTOM_RIGHT.y
				);
				MNG.objects.InstantiateTiny(PRFB_STORM, putPos);
				yield return new WaitForSeconds(0.01f);
			}
			yield return new WaitForSeconds(2f);

			// 敵発射
			var ENEMY_PUT_NUMBER = 10;
			for(int i=0; i < ENEMY_PUT_NUMBER; i++){
				var randomY = Random.Range(2f, MNG.WINDOW_TOP_LEFT.y - 2f);
				var enemyPos = new Vector2(MNG.WINDOW_BOTTOM_RIGHT.x, randomY);
				MNG.objects.InstantiateTiny(PRFB_GUIDE, enemyPos);
				yield return new WaitForSeconds(0.2f);
				MNG.objects.InstantiateTiny(PRFB_MEN_TARGET_LASER, enemyPos);
			}

			// 早苗さんランダム移動
			// 左3分の1の範囲を適当に
			var MOVE_NUMBER = 2;
			for(int i=0; i < MOVE_NUMBER; i++){

				// 自機狙い弾
				var laser = MNG.objects.InstantiateTiny(PRFB_LASER, MNG.FixedBossPosition());
				laser.transform.SetParent(MNG.GOBJ_BOSS.transform);
				var laser_comp = laser.GetComponent<Barrage_Target>();
				laser_comp.FIRE_CYCLE = 2f;
				yield return new WaitForSeconds(2f);

				var move_to_x = Random.Range(
					MNG.WINDOW_BOTTOM_RIGHT.x / 3 * 2,
					MNG.WINDOW_BOTTOM_RIGHT.x - 2);
				var move_to_y = Random.Range(
					MNG.WINDOW_BOTTOM_RIGHT.y + 2,
					MNG.WINDOW_TOP_LEFT.y - 2);

				iTween.MoveTo(MNG.GOBJ_BOSS, iTween.Hash(
					"x", move_to_x, "y", move_to_y,
					"time", 1f, "easeType", iTween.EaseType.easeInCubic
				));
				yield return new WaitForSeconds(5f);
			}
		}
	}

	public IEnumerator SecondAction(){
		// フェーズ2
		yield return new WaitForSeconds(0.4f);
		MNG.objects.InstantiateTiny(PRFB_WAVE, MNG.FixedBossPosition());

		for(;;){
			var MOVE_NUMBER = 4;
			for(int i=0; i< MOVE_NUMBER; i++){
				MNG.objects.InstantiateTiny(PRFB_MEN_TARGET2WAY, MNG.FixedBossPosition());
				var move_to_x = Random.Range(
					MNG.WINDOW_BOTTOM_RIGHT.x / 4 * 3,
					MNG.WINDOW_BOTTOM_RIGHT.x - 2);
				var move_to_y = Random.Range(
					MNG.WINDOW_BOTTOM_RIGHT.y + 2,
					MNG.WINDOW_TOP_LEFT.y - 2);
				iTween.MoveTo(MNG.GOBJ_BOSS, iTween.Hash(
					"x", move_to_x, "y", move_to_y,
					"time", 0.6f, "easeType", iTween.EaseType.easeOutCubic
				));
				yield return new WaitForSeconds(0.8f);
			}

			var target = MNG.objects.InstantiateTiny(PRFB_TARGET, MNG.FixedBossPosition());
			target.transform.SetParent(MNG.GOBJ_BOSS.transform);
			var move_to_x_long = Random.Range(
				MNG.WINDOW_BOTTOM_RIGHT.x / 4 * 3,
				MNG.WINDOW_BOTTOM_RIGHT.x - 2);
			var move_to_y_long = Random.Range(
				MNG.WINDOW_BOTTOM_RIGHT.y + 2,
				MNG.WINDOW_TOP_LEFT.y - 2);
			iTween.MoveTo(MNG.GOBJ_BOSS, iTween.Hash(
				"x", move_to_x_long, "y", move_to_y_long,
				"time", 13f, "easeType", iTween.EaseType.linear
			));
			yield return new WaitForSeconds(15f);
		}

	}

	public IEnumerator ThirdAction(){
		// フェーズ３
		yield return new WaitForSeconds(0.4f);
		MNG.objects.InstantiateTiny(PRFB_STAR, MNG.FixedBossPosition());
		var move_to_x_long = Random.Range(
			MNG.WINDOW_BOTTOM_RIGHT.x / 4 * 3,
			MNG.WINDOW_BOTTOM_RIGHT.x - 2);
		var move_to_y_long = Random.Range(
			MNG.WINDOW_BOTTOM_RIGHT.y + 2,
			MNG.WINDOW_TOP_LEFT.y - 2);
		iTween.MoveTo(MNG.GOBJ_BOSS, iTween.Hash(
			"x", move_to_x_long, "y", move_to_y_long,
			"time", 1f, "easeType", iTween.EaseType.linear
		));
		yield return new WaitForSeconds(1f);

		for(;;){
			// カエル
			var ENEMY_PUT_NUMBER = 20;
			for(int i=0; i < ENEMY_PUT_NUMBER; i++){
				var randomY = Random.Range(2f, MNG.WINDOW_TOP_LEFT.y - 2f);
				var enemyPos = new Vector2(MNG.WINDOW_TOP_LEFT.x, randomY);
				MNG.objects.InstantiateTiny(PRFB_GUIDE, enemyPos);
				yield return new WaitForSeconds(0.1f);
				MNG.objects.InstantiateTiny(PRFB_MEN_HOPPER, enemyPos);
			}

			yield return new WaitForSeconds(1f);

			move_to_x_long = Random.Range(
				MNG.WINDOW_BOTTOM_RIGHT.x / 4 * 3,
				MNG.WINDOW_BOTTOM_RIGHT.x - 2);
			move_to_y_long = Random.Range(
				MNG.WINDOW_BOTTOM_RIGHT.y + 2,
				MNG.WINDOW_TOP_LEFT.y - 2);
			iTween.MoveTo(MNG.GOBJ_BOSS, iTween.Hash(
				"x", move_to_x_long, "y", move_to_y_long,
				"time", 6f, "easeType", iTween.EaseType.linear
			));

			// 星
			var STAR_PUT_NUMBER = 2;
			for(int i=0; i < STAR_PUT_NUMBER; i++){
				MNG.objects.InstantiateTiny(PRFB_STAR, MNG.FixedBossPosition());
				yield return new WaitForSeconds(3f);
			}

			yield return new WaitForSeconds(2f);

			move_to_x_long = Random.Range(
				MNG.WINDOW_BOTTOM_RIGHT.x / 4 * 3,
				MNG.WINDOW_BOTTOM_RIGHT.x - 2);
			move_to_y_long = Random.Range(
				MNG.WINDOW_BOTTOM_RIGHT.y + 2,
				MNG.WINDOW_TOP_LEFT.y - 2);
			iTween.MoveTo(MNG.GOBJ_BOSS, iTween.Hash(
				"x", move_to_x_long, "y", move_to_y_long,
				"time", 6f, "easeType", iTween.EaseType.linear
			));

			var circle = MNG.objects.InstantiateTiny(PRFB_CIRCLE, MNG.FixedBossPosition());
			circle.transform.SetParent(MNG.GOBJ_BOSS.transform);

			// 自機狙い柱を５～６回集中的に
			var PILLER_PUT_NUMBER = Random.Range(4,6);
			var pillerY = MNG.WINDOW_TOP_LEFT.y;
			for(int i=0; i < PILLER_PUT_NUMBER; i++){
				var player_position = MNG.GOBJ_PLAYER.transform.position;
				var putPos = new Vector2(player_position.x, pillerY);
				MNG.objects.InstantiateTiny(PRFB_PILLAR, putPos);
				yield return new WaitForSeconds(1f);
			}
			yield return new WaitForSeconds(8f);
			MystGameManager.Destroy(circle);
			yield return new WaitForSeconds(3f);
		}
	}

	public void LateUpdate(){
		age += Time.deltaTime;
		action_counter += Time.deltaTime;
	}

	// private GameObject SetBarrage(GameObject prfb, Vector3 pos){
	// 	var mng = MystGameManager.Instance;
	// 	var temp = mng.objects.InstantiateTiny(prfb, pos);
	// 	return temp;
	// }
}
