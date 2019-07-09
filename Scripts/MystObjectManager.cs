using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MystObjectManager : SingletonMonoBehaviour<MystObjectManager> {
	private static int OBJECT_POOL_SIZE;
  private GameObject[] object_collection;
	void Start(){
		OBJECT_POOL_SIZE = 10000;
		object_collection = new GameObject[OBJECT_POOL_SIZE];
	}

	public GameObject InstantiateTiny(GameObject prfb, Vector3 position){
		GameObject obj = null;
		for(int i=0; i < this.object_collection.Length; i++){
			var item = this.object_collection[i];
			if(item == null){
				obj = Instantiate(prfb, position, Quaternion.identity) as GameObject;
				this.object_collection[i] = obj;
				break;
			}
		}
		return obj;
	}

	public void DestroyAll(){
		// Debug.Log(this.object_collection.Count);
		foreach(GameObject obj in this.object_collection){
			if(obj != null)  Destroy(obj);
		}
		// Debug.Log(this.object_collection.Count);
		var bullets = GameObject.FindGameObjectsWithTag("Bullet(Enemy)");
    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
    foreach(var bullet in bullets){
      MystGameManager.Destroy(bullet);
    }
    foreach(var enemy in enemies){
      MystGameManager.Destroy(enemy);
    }
	}

	public void Update(){
	}
}
