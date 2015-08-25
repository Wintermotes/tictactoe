using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text; 

public class Board : MonoBehaviour
{
	private StringBuilder builder = new StringBuilder();
	private int[,] winners = new int[,]
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
	
	// Dictionary structure is: <board_field_index, player>, where player = -1: empty, 0: player, 1: opponent
	private Dictionary<int, int> game_table = new Dictionary<int, int>();	

	public void PrintBoard(){
		int counter = 0; string str = ""; 
		foreach(int index in this.game_table.Keys){
			str += index + ":" + this.game_table[index] + " |"; 
			counter++; 
			if(counter==3){
				print (str + "\n"); 
				print ("-----------\n"); 
				str = ""; 
				counter = 0; 
			}
		}
	}

	// Update the gameboard
	public void UpdateBoard(int key, int person){
		//print ("Updating board with values: " + key + " and " + person + "\n"); 
		this.game_table[key] = person; 
	}
	
	// Creates an empty gameboard (-1 == no stones on board_field)
	public void CreateBoard()
	{
		game_table.Clear(); 
		for (int i = 0; i < 9; i++) { 
			this.game_table.Add(i, -1); 
		}


	}

	public bool checkForWin(List<int> choices, bool test = false){
		if(choices.Count < 3){
			print ("No need to go on, you havent placed 3 pieces yet"); 
			return false; 
		} else {
			// Check for winners
			print ("Checking for win..."); 
			bool match = false; int counter = 0; 

			for(int j = 0; j<winners.GetLength(0);j++) {
				for(int i = 0; i<choices.Count; i++){
					//Debug.Log ("Player number is: " + choices[i] + ". And counter is: " + counter); 
					for(int g = 0; g<winners.GetLength(1); g++){	
						//Debug.Log ("Testing against value: " + winners[j, g]); 
						if(choices[i] == winners[j, g]){
							//Debug.Log ("Matched!"); 
							match = true; 
							//LightBoardPiece(board_pieces[playerChoices[i]]); 
						}
					}
					if(match){
						//Debug.Log ("I had a match for " + choices[i] + ", so incrementing score!");
						counter++; 
					}
					match = false;
				}
				if(counter == 3){
					//Debug.Log("Lucky bitch. I'll get you next time. Counter: " + counter); 
					return true; 
					// TODO: Make some fancy visuals yo. 
					//vfx.GetComponent<VFX>().disableOrEnable(true);
					
				} 
				
				//Debug.Log ("FINAL SCORE IS: " + counter + ", for winCondition at index: " + j); 
				//Debug.Log ("-----------------------");			
				counter = 0; 

			
		}
		}

		return false; 
	}
	    
	public int[] CalculateComputerTurn(List<int> opponentChoices){

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
		


		/*	PSEUDO CODE:  
		 * 	1: Find all game states (possible board states in variable board states) for int depth 
		 * 	2: Rank, sort and evaluate those boardStates, by looking at: 
		 *  3a: The maximum score for players board and: 
		 *  3b: The minimum score for opponents board
		 * 	4: Return best boardstate, and best value for putting a stone. 
		 * 
		 */
		/*print ("------------------------------------------------\n");  
		print ("CALCULATING THE COMPUTERS TURN NOW WITH VALUES\n");
		print ("Player choices: " + PrintList(playerChoices) + "\n");
		print ("Opponent choices: " + PrintList(opponentChoices) + "\n"); 
		print ("------------------------------------------------\n"); 
		print ("Current gameboard is like this: \n"); 
		PrintBoard();*/



		// Score and sort game_board
		//best_key = EvaluateMoves ();
		//print ("Count of pos moves: " + possibleMoves.Count); 
		bool playedThreePieces = false;
		if(opponentChoices.Count == 3){
			//print ("I have 3 pieces on the board"); 
			playedThreePieces = true; 
		}
		int[] result = minimax(1, opponentChoices, playedThreePieces); 
		//print ("Minimax came up with move" + result[0] + ", which has " + result[1] + " as score."); 
		//evaluateScores(); 
		return new int[] {result[0], result[2]}; ; 
	}

	private int[] minimax(int depth, List<int> opponentChoices, bool playedMaxPieces = false){
		//TODO: Add possible moves with equal bestscore to list, and select randomly from it. 
		Dictionary<int, int> possibleMoves = generateMoves();

		int bestScore = -2; 
		int currentScore = 0;
		int bestMove = -1; 
		int bestPiece = -1; 

		if(possibleMoves.Count == 0){
			print ("NO MORE MOVES YO");
			return new int[] {-1, -1}; 
		} else if (possibleMoves.Count == 0 || depth == 0) {
			// Gameover or depth reached, evaluate score
			//print ("I reached end of depth, now i'll evauluate scores"); 
			bestScore = evaluateScores(); 
		} else if(playedMaxPieces) {
			print ("Played three pieces, so need to check them as well."); 
			foreach(int piece in opponentChoices){
				//print ("Setting piece " + piece + " to: -1"); 
				game_table[piece] = -1; 
				foreach(KeyValuePair<int, int> move in possibleMoves){ 
					//print ("Setting game_table " + move.Key + " to 0.");  
					game_table[move.Key] = 0; // Try to place a piece as computer. 
					
					currentScore = minimax(depth-1, opponentChoices, true)[1]; 
					//print ("Current score is: " + currentScore); 
					
					if(currentScore > bestScore){
						bestScore = currentScore; 
						bestMove = move.Key;  
						bestPiece = piece; 
						//print ("New bestScore : " + currentScore + "\n");
						//print ("New best move: " + move + "\n"); 
						//print ("Best piece to move " + piece + "\n"); 
					}
					game_table[move.Key] = -1;
				}
				game_table[piece] = 0; 
			}
		} else {
				foreach(KeyValuePair<int, int> move in possibleMoves){
					//print ("Possible move to do is at index: " + move + ", and depth is: " + depth + "\n"); 
					game_table[move.Key] = 0; // Try to place a piece as computer. 

					currentScore = minimax(depth-1, opponentChoices)[1]; 
					//print ("Current score is: " + currentScore); 

					if(currentScore > bestScore){
						bestScore = currentScore; 
						bestMove = move.Key;  
						//print ("New bestScore : " + currentScore + "\n");
						//print ("New best move: " + move + "\n"); 
					}
					game_table[move.Key] = -1;
				}
		}
		return new int[] {bestMove, bestScore, bestPiece}; 
	}

	private int evaluateScores(){
		int score = 0; 
		score += evaluateLine(0, 1, 2);  // row 0
		score += evaluateLine(3, 4, 5);  // row 1
		score += evaluateLine(6, 7, 8);  // row 2

		score += evaluateLine(0, 3, 6);  // col 0
		score += evaluateLine(1, 4, 7);  // col 1
		score += evaluateLine(2, 5, 8);  // col 2

		score += evaluateLine(0, 4, 8);  // diagonal
		score += evaluateLine(6, 4, 2);  // alternate diagonal
		//print ("Im done evaluating scores. Final score is: " + score + "\n"); 
		//print ("UNIT TESTING ENDING HERE"); 
		return score;
	}



	private int evaluateLine(int piece1, int piece2, int piece3){
		int score = 0; 
		// First cell:
		//print ("GAME PIECE 1 IS: " + game_table[piece1]); 
		if(game_table[piece1] == 0){
			score = 1; 
		} else if(game_table[piece1] == 1){
			score = -1; 
		}
		//print ("After evaluating gamepiece 1, score is: " + score); 

		// Second cell:
		// If piece2 is the computers: 
		//print ("GAME PIECE 2 IS: " + game_table[piece2]); 
		if(game_table[piece2] == 0){
			//print ("CHECKING PIECE 2!"); 
			if(score == 1){
				//print ("INCREMENTING BY 10!"); 
				score = 10; // If score is one, that means first cell is mine
			} else if (score == -1){
				return 0; 
			} else {
				score = 1; 
			}
		// else if piece2 is the players
		} else if (game_table[piece2] == 1) {
			if (score == -1) { // cell1 is opponent
				score = -10;
			} else if (score == 1) { // cell1 is mySeed
				return 0;
			} else {  // cell1 is empty
				score = -1;
			}
		}
		//print ("After evaluating gamepiece 2, score is: " + score);

		//print ("GAME PIECE 3 IS: " + game_table[piece3]); 
		// Third cell: 
		if(game_table[piece3] == 0){
			if(score > 0){
				score *= 10; 
			} else if( score < 0) { // cell1 and/or cell2 is oppSeed
				return 0; 
			} else { // cell1 and cell2 are empty
				score = 1; 
			}
		} else if (game_table[piece3] == 1) {
			if (score < 0) {  // cell1 and/or cell2 is oppSeed
				score *= 10;
			} else if (score > 1) {  // cell1 and/or cell2 is mySeed
				return 0;
			} else {  // cell1 and cell2 are empty
				score = -1;
			}
		}
		//print ("After evaluating gamepiece 3, score is: " + score);
		//print ("Final score for board_field: " + piece1 + " , " + piece2 + " , " + piece3 + ":\n");
		//print (score + "\n");
		return score; 

	}

	private Dictionary<int, int> generateMoves(){
		Dictionary<int, int> board_moves_list = new Dictionary<int, int>();

		foreach(KeyValuePair<int , int> board_field in game_table){
			if(board_field.Value == -1){
				board_moves_list.Add (board_field.Key, board_field.Value); 
			}
		}
		//print ("There are " + board_moves_list.Count + " moves to do on current table"); 
		return board_moves_list; 
	}


	/*------------------------------------------ HELPER FUNCTIONS ----------------------------------------- */

	// Purely random approach
	public int EvaluateMoves(){
		Dictionary<int, int> possible_moves = generateMoves(); // key  = board_field_value, value = score, 
		List<int> possible_moves_list = new List<int>();
		int best_move; 
		
		// Find random value from possible moves
		foreach(int key in possible_moves.Keys){
			possible_moves_list.Add(key); 
		}
		print ("There are: " + possible_moves_list.Count + " moves."); 
		best_move = FindRandomMove(possible_moves_list); 
		return best_move; 
	}

	private int FindRandomMove(List<int> keys){
		if(keys.Count == 0){
			throw new System.ArgumentException("No possible moves on game board.", "keys");
		}

		List<int> board_moves_list = new List<int>(keys);
		int i = Random.Range(0, board_moves_list.Count);
		int j = board_moves_list[i]; 

		return j; 
	}

	private string PrintList(List<int> list){
		builder.Length = 0; 
		foreach (int i in list)
		{
			builder.Append(i).Append(", ");
		}
		string result = builder.ToString();
		return result; 
	}
}