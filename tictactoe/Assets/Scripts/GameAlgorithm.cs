using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameAlgorithm : MonoBehaviour {

	public GameObject board; //Public, because you might want to switch board. 
	private ArrayList circle_pieces; 

	private Dictionary<int, Transform> board_pieces = new Dictionary<int, Transform>(); 

	
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

	void Start(){

		// ASSIGNING VARIABLES INTO LISTS
		GameObject temp_gameob = GameObject.Find("circle_pieces"); 
		int counter = 0; 
		Debug.Log("Assigning...");
		foreach(Transform child in transform){
			board_pieces.Add(counter, child); 
			counter++; 
		}

		circle_pieces = new ArrayList(); 
		counter = 0; 
		foreach(Transform child in temp_gameob.transform){
			circle_pieces.Add(child); 
		}

		// DEBUGGING INFO: 
		Debug.Log("Board info: "); 
		foreach(KeyValuePair<int, Transform> board_piece in board_pieces)
		{
			Debug.Log(board_piece.Key + " : " + board_piece.Value.name);  
		}

		Debug.Log("Opponents Pieces info: "); 
		foreach(Transform circle_piece in circle_pieces){
			Debug.Log (circle_piece.name);
		}
	}

}
