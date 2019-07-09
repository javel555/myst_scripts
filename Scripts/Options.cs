using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D other) {
		var other_layer = LayerMask.LayerToName(other.gameObject.layer);
		if(other_layer == "EnemyBullet" ){
			BarrageBullet b = other.GetComponent<BarrageBullet>();
			if(b != null && b.is_deletable == true)
				Destroy(other.gameObject);
		}
	}
}
