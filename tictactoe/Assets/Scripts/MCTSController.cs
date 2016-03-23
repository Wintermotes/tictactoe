using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MCTSController : AIController {
	// Dicts of board_fields, player_pieces and opp_pieces
	public Board boardScript;
	public List<Transform> opponent_pieces = new List<Transform>();
	public List<Transform> player_pieces = new List<Transform>();
	private Dictionary<Transform, int> board_fields = new Dictionary<Transform, int>();

    public void Start(){
        boardScript = GameObject.FindGameObjectWithTag("board_fields").gameObject.GetComponent<Board>();
    }

    override
	public int[] getMove(List<int> choices){
        int bestMoveIndex = Random.Range(0, boardScript.getBoardSize());
        int bestPieceToMove = 0;

        while (!validateMoves(choices, bestMoveIndex)) {
            bestMoveIndex = Random.Range(0, boardScript.getBoardSize());
            print("BestMoveIndex (whileloop): " + bestMoveIndex);
        }

        print("BestMoveIndex : " + bestMoveIndex);

        int[] moves = { bestMoveIndex, bestPieceToMove };
		return moves;
	}

    public bool validateMoves(List<int>choices, int move)
    {
        foreach (int i in choices) {
            if (i == move)
                return false; 
        }
        return true; 
    }
	/*	PSEUDO CODE:  
	* 	create root node v0 with state s0 
	* 	    while within coputational budget do 
	*           v' <- TreePolicy(s(v0)
				delta <- DefaultPolicy(s(v'))
				Backpropagate(v', delta)
			return a(BestChild(v0))
	*  3b: The minimum score for opponents board
	* 	4: Return best boardstate, and best value for putting a stone. 
	* 
	*/




	// Update is called once per frame
}
