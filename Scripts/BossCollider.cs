using UnityEngine;
using System.Collections;

public class BossCollider : MonoBehaviour {

	private void OnTriggerStay2D(Collider2D other) {
		var mng = MystGameManager.Instance;
		if(other.tag != "Bullet(Player)") return;
		if(mng.boss_is_nodamage) return;
		if(mng.boss_state != 0) return;

		BarrageBullet b = other.GetComponent<BarrageBullet>();
		mng.boss_life -= b.power;
		StartCoroutine(DamageForBoss());

	}

  private IEnumerator DamageForBoss(){
		var mng = MystGameManager.Instance;
		var gobj_boss_pos = mng.GOBJ_BOSS.transform.position;
		gobj_boss_pos.y += 1f;
		mng.objects.InstantiateTiny(mng.PRFB_DAMAGE, gobj_boss_pos);
		mng.boss_is_nodamage = true;
		var sprite = mng.GOBJ_BOSS.GetComponent<SpriteRenderer>();
		int count = 30;
		bool forRed = true;
		for(int i= 0; i < count; i++){
			if(forRed){
				sprite.color = Color.red;
			}else{
				sprite.color = Color.white;
			}
			forRed = !forRed;
			yield return new WaitForSeconds(0.05f);
		}
		mng.boss_is_nodamage = false;
	}
}