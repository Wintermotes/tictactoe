using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class MoveStone : MonoBehaviour {

	private Vector3 mousePos; 
	private Vector3 wantedPos; 
	private bool carry = false; 
	private bool waitForTime = true;  
	private Rigidbody rb; 
<<<<<<< HEAD
	private GameAlgorithm game;
    private Component controller; 

    public GameObject board_fields;
    public GameObject player_pieces;
    public int game_mode; 


=======
	private GameAlgorithm game; 

	//TODO: Move this to char controller. Right now, you can carry multiple pieces. 
>>>>>>> b5436829e9413476f7cf662121799f06d4a231a8
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
<<<<<<< HEAD
				gameObject.transform.parent = player_pieces.transform;
				foreach(Transform child in player_pieces.transform){
=======
				GameObject temp_gb = GameObject.Find("cross_pieces"); 
				gameObject.transform.parent = temp_gb.transform;
				foreach(Transform child in temp_gb.transform){
>>>>>>> b5436829e9413476f7cf662121799f06d4a231a8
					child.tag = "player_piece";  
					child.GetComponent<MoveStone>().enabled = true; 
				}
			}

<<<<<<< HEAD
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
=======

			// TRANSFORMS 
			carry = !carry;
		
			game = GameObject.FindGameObjectWithTag("game_algorithm").GetComponent<GameAlgorithm>(); 
			if(gameObject.tag == "opponent_piece"){
				game.update_game(hit, 0, gameObject.transform); 
			} 
			else if(gameObject.tag == "player_piece")
			{
				game.update_game(hit, 1, gameObject.transform); 
			} else {
				print("The gameobject tag is: " + gameObject.tag); 
			}


			// SOUNDS

			// MATERIALS
>>>>>>> b5436829e9413476f7cf662121799f06d4a231a8
			waitForTime = true; 
		}
	}

	IEnumerator WaitXSeconds(float f) {
		gameObject.transform.parent = null;
<<<<<<< HEAD
		foreach(Transform child in player_pieces.transform){
=======
		GameObject temp_gb = GameObject.Find("cross_pieces"); 

		foreach(Transform child in temp_gb.transform){
>>>>>>> b5436829e9413476f7cf662121799f06d4a231a8
			child.tag = "Untagged";  
			child.GetComponent<MoveStone>().enabled = false; 
		}

		gameObject.layer = 2; // Layer 2 = ignoreRaycast
		//gameObject.tag = "player_piece"; 

		yield return new WaitForSeconds(f);

<<<<<<< HEAD
		foreach(Transform t in board_fields.transform){
=======
		temp_gb = GameObject.Find("board_pieces"); 
		foreach(Transform t in temp_gb.transform){
>>>>>>> b5436829e9413476f7cf662121799f06d4a231a8
			if(t.gameObject.layer == 2){
				t.gameObject.layer = 0; 
			}
		}
	}


}
