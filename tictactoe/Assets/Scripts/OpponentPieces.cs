using UnityEngine;
using System.Text; 
using UnityEngine.UI;
using System.Collections;

public class OpponentPieces : MonoBehaviour {
	GameObject subtitle; 

	void OnMouseDown(){
		subtitle = GameObject.FindGameObjectWithTag("subtitle");
		subtitle.GetComponent<Text>().text = "Dont touch my pieces you dumbass!"; 
	}
}
