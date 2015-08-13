using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameAlgorithm : MonoBehaviour {

	public GameObject board; //Public, because you might want to switch board. 
	public GameObject vfx; 
	private ArrayList circle_pieces; 
	private Dictionary<int, Transform> board_pieces = new Dictionary<int, Transform>(); 

	private int[] playerChoices = new int[3];
	private int[] OpponentChoices = new int[3];
	int[,] winners2 = new int[,]
	{
		{0,1,2},
		{3,4,5},
		{6,7,8},
		{0,3,6},
		{1,4,7},
		{2,5,8},
		{0,4,8},
		{2,4,6}
	};


	
	/*	 GAME BOARD IS LIKE THIS: 	
	 * 		
	 * 		0    |  1   |  2 	
	 *	    ----------------
	 *		3    |  4   |  5
	 *		----------------
	 *		6    |  7   |  8
	 *      
	 * Algorithm: https://en.wikipedia.org/wiki/Minimax
	*/
	void Update(){
		if(Input.GetKeyDown("space")){
			vfx.GetComponent<VFX>().disableOrEnable(true);
		}
	}

	void Start(){
		// ASSIGNING AND INITIALIZING VARIABLES
		Debug.Log("Assigning...");
		GameObject temp_gameob = GameObject.Find("circle_pieces"); 
		int counter = 0; 
		foreach(Transform child in transform){
			board_pieces.Add(counter, child); 
			counter++; 
		}

		circle_pieces = new ArrayList(); 
		counter = 0; 
		foreach(Transform child in temp_gameob.transform){
			circle_pieces.Add(child); 
		}

		for(int i = 0; i<playerChoices.Length; i++){
			OpponentChoices[i] = -1; 
			playerChoices[i] = -1; 
		}
		Debug.Log("Player and opponent choice variables has been set.");

		// Some visuals
		vfx = GameObject.Find ("VFX"); 
		vfx.GetComponent<VFX>().disableOrEnable(false);

		Debug.Log("Assigning of variables done!"); 

		// DEBUGGING INFO: 
		/*Debug.Log("Board info: "); 
		foreach(KeyValuePair<int, Transform> board_piece in board_pieces)
		{
			Debug.Log(board_piece.Key + " : " + board_piece.Value.name);  
		}



		Debug.Log("Opponents Pieces info: "); 
		foreach(Transform circle_piece in circle_pieces){
			Debug.Log (circle_piece.name);
		}*/

		// Gives visual feedback when debugging the game algorithm
		int randomInt = Random.Range(0, 9); // Returns int between 0 and 8 
		Transform test_object = board_pieces[randomInt]; 
		//Debug.Log ("Name of testObject: " + test_object.name);

		Transform light_object = test_object.GetChild (0);
		//light_object.gameObject.GetComponent<Light>().enabled = true; 

		// Unit testing for a winning condition 
		playerChoices[0] = 0; 
		playerChoices[1] = 4; 
		playerChoices[2] = 8; 

		testAlgorithm(winners2, playerChoices); 

	}



/*  Function: testAlgorithm
 * 
 *	Tests any given algortihm by brute force, to check if a sequence of three is found. 
 *	 
 *  Input winners: A 2x2 array of win possibilities  
 *  Input playerBoard: Playerboard represented by an integer array
 * 
 */
	public void testAlgorithm(int[,] winners, int[] playerBoard, bool test = false){


		bool match = false; int counter = 0; 


		counter = 0; 
		Debug.Log ("Testing winners"); 
		Debug.Log ("-----------------------");
		for(int j = 0; j<winners.GetLength(0);j++) {
			for(int i = 0; i<playerChoices.Length; i++){
				Debug.Log ("Player number is: " + playerChoices[i]); 
				for(int g = 0; g<winners.GetLength(1); g++){	
				Debug.Log ("Testing against value: " + winners[j, g]); 
					if(playerChoices[i] == winners[j, g]){
						Debug.Log ("Matched!"); 
						match = true; 
						LightBoardPiece(board_pieces[playerChoices[i]]); 
					}
				}
				if(match){
					Debug.Log ("I had a match for " + playerChoices[i] + ", so incrementing score!");
					counter++; 
				}
				match = false;
			}
			if(counter == 3){
				Debug.Log("Lucky bitch. I'll get you next time."); 
				// TODO: Make some fancy visuals yo. 
				//vfx.GetComponent<VFX>().disableOrEnable(true);

			} 

			Debug.Log ("FINAL SCORE IS: " + counter + ", for winCondition at index: " + j); 
			Debug.Log ("-----------------------");
			Debug.Log (winners.GetLength(0)); 

			counter = 0; 
		}


	}
/* Function LightBoardPiece
 * 
 * Description: Enables a lightsource object, if it is a child of a given transform object. 
 * 
 * Input: Transform object which has a child, which has a ligth as a child. 
 * 
 * 
*/
	public void LightBoardPiece(Transform board_piece){
		board_piece.GetChild(0).gameObject.GetComponent<Light>().enabled = true; 
	}
	

}
