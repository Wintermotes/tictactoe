using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour {

    public int turnsTaken;
    //TODO: Create doc.
    public virtual int[] getMove(List<int> choices) {
        int[] moves = { 0, 2 };
        return moves;
    }
}
