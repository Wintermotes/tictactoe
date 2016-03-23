using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
	public StringBuilder builder = new StringBuilder();
	public int boardSize; 
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

	protected int bestScore;
	protected int bestToMove;
	protected int bestFromMove;
	public Boolean playedMaxPieces; 
	
	// Dictionary structure is: <board_field_index, player> where player = -1: empty, 0: player, 1: opponent
	public Dictionary<int, string> game_state = new Dictionary<int, string>();

	public virtual int[] getMove(List<int> choices) {
		return RandomMove();  
	}

	public void PrintBoard(int items_per_line){
		
		int counter = 0; string str = ""; 
		foreach(int index in this.game_state.Keys){
			str += index;
			if (index < 10){
				str += "  "; 
			}
			str += ":" + this.game_state[index] + " |";
			if (counter == items_per_line - 1){
                //print(str + " \n");
                str += "\n \n";
				str += "";
				counter = 0;
			} else {
				counter++;
			} 
		}
        //print(str);
		GameObject.FindGameObjectWithTag("gui_board_representation").GetComponent<Text>().text = str;
		//print(str); 
	}

	// Update the gameboard
	public void UpdateBoard(int key, string person){
		print ("Updating board with values: " + key + " and " + person + "\n"); 
		this.game_state[key] = person; 
	}
	
	// Creates an empty gameboard (-1 == no stones on board_field)
	public virtual void CreateBoard(int x, int y)
	{
		game_state.Clear();
		for (int i = 0; i < (x*y); i++) { 
			this.game_state.Add(i, "-1"); 
		}
		boardSize = x * y; 
	}


	public bool checkForWin(List<int> choices, bool test = false){
		if(!playedMaxPieces){
			return false; 
		} else {
			bool match = false; int counter = 0; 
			for(int j = 0; j<winners.GetLength(0);j++) {
				for(int i = 0; i<choices.Count; i++){
					for(int g = 0; g<winners.GetLength(1); g++){	
						if(choices[i] == winners[j, g]){
							match = true; 
						}
					}
					if(match){
						counter++; 
					}
					match = false;
				}
				if(counter == 3){
					return true; 
					
				} 
				counter = 0; 
		}
		}

		return false; 
	}
	

	protected Dictionary<int, int> generateMoves(){
		Dictionary<int, int> board_moves_list = new Dictionary<int, int>();
		foreach(KeyValuePair<int , string> board_field in game_state){
			if(board_field.Value == "-1"){
				board_moves_list.Add (board_field.Key, Int32.Parse(board_field.Value)); 
			}
		}
		print ("There are " + board_moves_list.Count + " moves to do on current table");
		PrintBoard(3);
		return board_moves_list; 
	}

	/*------------------------------------------ HELPER FUNCTIONS ----------------------------------------- */

	// Purely random approach
	public int[] RandomMove(){
		Dictionary<int, int> possible_moves = generateMoves(); // key  = board_field_value, value = score, 
		List<int> possible_moves_list = new List<int>();

		// Find random value from possible moves
		foreach(int key in possible_moves.Keys){
			possible_moves_list.Add(key); 
		}
		print("There are: " + possible_moves_list.Count + " moves."); 
		bestToMove = FindRandomMove(possible_moves_list);
		bestScore = 0;
		bestFromMove = FindRandomMove(possible_moves_list);

		return new int[] { bestToMove, bestFromMove, bestScore }; 
	}

	private int FindRandomMove(List<int> keys){
		if(keys.Count == 0){
			throw new System.ArgumentException("No possible moves on game board.", "keys");
		}

		List<int> board_moves_list = new List<int>(keys);
		int i = UnityEngine.Random.Range(0, board_moves_list.Count);
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

    public int getBoardSize() {
        return boardSize;
    }
}