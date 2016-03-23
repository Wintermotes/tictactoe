using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class MoveStone : MonoBehaviour {

	private Vector3 mousePos; 
	private Vector3 wantedPos; 
	private bool carry = false; 
	private bool waitForTime = true;  
	private Rigidbody rb; 
	private GameAlgorithm game;
    private Component controller; 

    public GameObject board_fields;
    public GameObject player_pieces;
    public int game_mode; 


	void OnMouseDown(){
		if(!enabled){
			return;
		}

		carry = !carry; 
	}
	
	void Update(){
		if (carry && gameObject.tag == "player_piece" || carry && gameObject.tag == "opponent_piece") {

			//print ("Moving stone"); 
			mousePos = Input.mousePosition; 
			wantedPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 20.0f));
			transform.position = wantedPos; 

			if(waitForTime){
				StartCoroutine(WaitXSeconds(0.5f)); 
				waitForTime = false; 
			}
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		// Only perform transform, if its on a board_piece, you carry a stone, and press the mouse. 
		if (Physics.Raycast(ray, out hit) && carry && Input.GetMouseButtonDown(0) && hit.transform.gameObject.tag == "board_piece" && hit.transform.gameObject.layer == 0){
			if(gameObject.tag == "player_piece"){
				gameObject.transform.parent = player_pieces.transform;
				foreach(Transform child in player_pieces.transform){
					child.tag = "player_piece";  
					child.GetComponent<MoveStone>().enabled = true; 
				}
			}

			// TRANSFORMS 
			carry = !carry;
            game = GameObject.FindGameObjectWithTag("game_algorithm").GetComponent<GameAlgorithm>(); 
			if(gameObject.tag == "opponent_piece"){
				game.update_game(hit, 0, gameObject.transform, game_mode); 
			} 
			else if(gameObject.tag == "player_piece")
			{
				game.update_game(hit, 1, gameObject.transform, game_mode); 
			} else {
				print("The gameobject tag is: " + gameObject.tag); 
			}
			waitForTime = true; 
		}
	}

	IEnumerator WaitXSeconds(float f) {
		gameObject.transform.parent = null;
		foreach(Transform child in player_pieces.transform){
			child.tag = "Untagged";  
			child.GetComponent<MoveStone>().enabled = false; 
		}

		gameObject.layer = 2; // Layer 2 = ignoreRaycast
		//gameObject.tag = "player_piece"; 

		yield return new WaitForSeconds(f);

		foreach(Transform t in board_fields.transform){
			if(t.gameObject.layer == 2){
				t.gameObject.layer = 0; 
			}
		}
	}


}
