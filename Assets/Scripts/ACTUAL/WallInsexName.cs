using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInsexName : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider target){
		Debug.Log ("2, 0");
		if (target.name == "12") {
			Debug.Log ("2, 0");
		}
	}


}//class
