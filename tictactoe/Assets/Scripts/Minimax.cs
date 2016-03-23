using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Minimax : Board {

    public int[] minimax(int depth, List<int> opponentChoices, bool playedMaxPieces = false){
        //TODO: Add possible moves with equal bestscore to list, and select randomly from it. 
        Dictionary<int, int> possibleMoves = generateMoves();

        int currentScore = 0;
        if (possibleMoves.Count == 0)
        {
            Console.WriteLine("NO MORE MOVES YO");
            return new int[] { -1, -1 };
        }
        else if (possibleMoves.Count == 0 || depth == 0)
        {
            // Gameover or depth reached, evaluate score
            bestScore = evaluateScores();
        }
        else if (playedMaxPieces)
        {
            print("PLAYED MAX PIECES");
            foreach (int piece in opponentChoices)
            {
                game_state[piece] = "-1";
                foreach (KeyValuePair<int, int> move in possibleMoves)
                {
                    game_state[move.Key] = "0"; // Try to place a piece as computer. 
                    currentScore = minimax(depth - 1, opponentChoices, true)[2];
                    //print ("Current score is: " + currentScore); 

                    if (currentScore > bestScore)
                    {
                        bestScore = currentScore;
                        bestToMove = move.Key;
                        bestFromMove = piece;
                        //print ("New bestScore : " + currentScore + "\n");
                        //print ("New best move: " + move + "\n"); 
                        //print ("Best piece to move " + piece + "\n"); 
                    }
                    game_state[move.Key] = "-1";
                }
                game_state[piece] = "0";
            }
        }
        else
        {
            foreach (KeyValuePair<int, int> move in possibleMoves)
            {
                //print ("Possible move to do is at index: " + move + ", and depth is: " + depth + "\n"); 
                game_state[move.Key] = "0"; // Try to place a piece as computer. 
                currentScore = minimax(depth - 1, opponentChoices)[2];

                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestToMove = move.Key;
                    //print ("New bestScore : " + currentScore + "\n");
                    //print ("New best move: " + move + "\n"); 
                }
                game_state[move.Key] = "-1";
            }
        }
        return new int[] { bestToMove, bestFromMove, bestScore};
    }

    public override int[] getMove(List<int> opponentChoices){
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

        if (opponentChoices.Count == 3)
        {
            print ("I have 3 pieces on the board"); 
            playedMaxPieces = true;
        }
        int[] result = minimax(1, opponentChoices, playedMaxPieces);
        //print ("Minimax came up with move" + result[0] + " at index: " + result[1] + ", which has score " + result[2]);
        //evaluateScores(); 
        return new int[] { result[0], result[1] }; 
    }

    protected int evaluateScores()
    {
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



    private int evaluateLine(int piece1, int piece2, int piece3)
    {
        int score = 0;
        // First cell:
        if (game_state[piece1] == "O")
        {
            score = 1;
        }
        else if (game_state[piece1] == "P")
        {
            score = -1;
        }

        // Second cell:
        // If piece2 is the computers: 
        if (game_state[piece2] == "O")
        {
            if (score == 1)
            {
                score = 10; // If score is one, that means first cell is mine
            }
            else if (score == -1)
            {
                return 0;
            }
            else
            {
                score = 1;
            }
            // else if piece2 is the players
        }
        else if (game_state[piece2] == "P")
        {
            if (score == -1)
            { // cell1 is opponent
                score = -10;
            }
            else if (score == 1)
            { // cell1 is mySeed
                return 0;
            }
            else
            {  // cell1 is empty
                score = -1;
            }
        }
        // Third cell: 
        if (game_state[piece3] == "-1")
        {
            if (score > 0)
            {
                score *= 10;
            }
            else if (score < 0)
            { // cell1 and/or cell2 is oppSeed
                return 0;
            }
            else
            { // cell1 and cell2 are empty
                score = 1;
            }
        }
        else if (game_state[piece3] == "P")
        {
            if (score < 0)
            {  // cell1 and/or cell2 is oppSeed
                score *= 10;
            }
            else if (score > 1)
            {  // cell1 and/or cell2 is mySeed
                return 0;
            }
            else
            {  // cell1 and cell2 are empty
                score = -1;
            }
        }
        //print ("After evaluating gamepiece 3, score is: " + score);
        //print ("Final score for board_field: " + piece1 + " , " + piece2 + " , " + piece3 + ":\n");
        //print (score + "\n");
        return score;

    }
}
