using UnityEngine;
using System.Collections;

public class OptionAnimation : MonoBehaviour {

	public bool is_enable_anime;
	private float seta;

	// Use this for initialization
	void Start () {
		this.is_enable_anime = true;
		this.seta = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!this.is_enable_anime) return;
		var parent_position = this.transform.parent.transform.position;

		var length = 0.4f;
		this.seta += Time.deltaTime * 6f;
		var newPos = this.transform.position;
		newPos.x = parent_position.x + Mathf.Cos(seta) * length * 2f;
		newPos.y = parent_position.y + Mathf.Sin(seta * 1.1f) * length;

		var shake_seta = Time.time * 30f;
		newPos.y = newPos.y + Mathf.Sin(shake_seta) * 0.05f;

		this.transform.position = newPos;
	}
}
