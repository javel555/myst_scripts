using UnityEngine;
using System.Collections;

public class BulletEmitter : MonoBehaviour {

	public float rotate;
	private Vector3 rotate_value;
	public float scale;
	private Vector3 scale_value;
	
	void Start() {
		rotate_value = new Vector3(0,0,rotate);
		scale_value  = new Vector3(scale, scale, scale);
		if(scale < 0){
			this.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
		}
	}


	void Update () {
		transform.Rotate(rotate_value);
		transform.localScale -= scale_value * Time.deltaTime;
		if( transform.localScale.x < 0.1f){
			Destroy(this.gameObject);
		}
	}
}
