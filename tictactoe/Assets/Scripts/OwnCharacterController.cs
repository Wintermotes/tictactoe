using UnityEngine;
using System.Collections;

public class OwnCharacterController : MonoBehaviour {
	GameObject board_pieces; 
	bool your_turn; 

	// Use this for initialization
	void Start () {
		board_pieces = GameObject.Find("board_pieces"); 
		your_turn = true; 
	
	}
	
	// Update is called once per frame
	void OnMouseDown(){

	}
}
