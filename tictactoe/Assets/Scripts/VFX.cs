using UnityEngine;
using System.Collections;

public class VFX : MonoBehaviour {

	static GameObject vfx; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void disableOrEnable(bool b){
		gameObject.SetActive(b); 
	}


}
