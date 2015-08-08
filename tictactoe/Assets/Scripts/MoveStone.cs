using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class MoveStone : MonoBehaviour {

	private Vector3 mousePos; 
	private Vector3 wantedPos; 
	private bool carry = false; 
	private Rigidbody rb; 

	private GameObject subtitle; 
	private GameObject opponent; 

	
	void OnMouseDown(){
		Debug.Log (Input.mousePosition); 

		// This can be used on other objects
		/*
		if (carry) {
			rb = gameObject.GetComponent<Rigidbody>(); 
			rb.AddForce(0.0f, Input.mousePosition.y, 200.0f); 
		}*/


		carry = !carry; 


	}
	
	void Update(){
		if (carry) {
			mousePos = Input.mousePosition; 
			wantedPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10.0f));
			transform.position = wantedPos; 
			gameObject.layer = 2; // Layer 2 = ignoreRaycast
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit) && carry && Input.GetMouseButtonDown(0)){

			// TRANSFORMS 
			// TODO: make it only apply transform, if its a boardpiece. 
			Vector3 board_pos = hit.transform.gameObject.transform.position; 
			board_pos.y = board_pos.y += 2.1f;  
			gameObject.transform.position = board_pos;
			carry = !carry;

			// ANIMATION
			opponent = GameObject.FindGameObjectWithTag ("opponent");
			opponent.GetComponent<Animator>().SetInteger("anim_state", 1);

			//TODO: Trigger animation of opponent

			//Trigger subtitle
			GameObject subtitle = GameObject.FindGameObjectWithTag("subtitle"); 
			string name = "Cube: "; 
			subtitle.GetComponent<Text>().text = name + "Man, you are bad. Much worse than cylinder."; 
		}
	}


}
