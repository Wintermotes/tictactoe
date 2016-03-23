using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text; 

public class GameAlgorithm : MonoBehaviour {

	public GameObject gameBoard; 
	private Board boardScript;
	private AIController controller; 
	private GameObject player; 
	private GameObject opponent; 
	private GameObject subtitle;
   

	// Dicts of board_fields, player_pieces and opp_pieces
	// Make this more clean
	private Dictionary<int, Transform> activeOpponentPieces = new Dictionary<int, Transform>();
	private Dictionary<int, Transform> activePlayerPieces = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> boardFieldsDict = new Dictionary<int, Transform>();

    private List<Transform> inactiveOpponentPieces = new List<Transform>();  
	
	private Transform boardField; 
	private Transform opponentPiece; 
	private Transform playerPieces;
	

	private List<int> playerChoices = new List<int>();
	private List<int> opponentChoices = new List<int>();
	private int playerScore; private int opponentScore;

	// Board parameters
	public int boardSizeX;
	public int boardSizeY;

	
	// Lights, vfx, animation and sound
	private Light sun; 
	public VFX vfx_script;  
	private Animator opponentAnimator; 


	// UI, debugging And dialogue
	public List<string> oppMessageList = new List<string>(); 
	private Text subtitle_text; private bool sayOnce; 
	private GameObject restartButton; 
	private StringBuilder builder = new StringBuilder();

	// Transform 
	TransformScript transformScript; 

	void Start(){
		AssignVariables();

		
		print("Controller: " + controller.ToString());
		boardScript.CreateBoard(boardSizeX, boardSizeY); 
	}
	void Update(){
		if(Input.GetKeyDown("space")){
			//ChangeTVScreen(); 
			transformScript.StartLerping(GameObject.Find("board_piece_botright").transform, Vector3.up); 
		}
	}

	public void update_game(RaycastHit hit, int person, Transform stone, int game_mode){
		// TODO: Make more flexible in terms of game_mode

		if(person == 1){
			playerStep(activePlayerPieces, stone, -1, hit);
		}

		updateBoardChoices(); 
		if(boardScript.checkForWin(playerChoices)){
			TriggerPlayerWin(); 
			return;
		}

		if(playerScore == 5) {
			vfx_script.triggerVictory(); 
		} else {
			StartCoroutine(opponentStep(2.0f, game_mode)); 
		}
	}

	public void playerStep(Dictionary<int, Transform> active_player_pieces, Transform stone, int board_field_index, RaycastHit hit){
		Vector3 board_pos = hit.transform.gameObject.transform.position;
		board_pos.y += 2.1f; // offset
        LightBoardPiece(hit.transform);
        hit.transform.gameObject.layer = 10; // Layer 10 = occupied
        stone.position = board_pos;

        int boardFieldIndex = findPieceByTransform(boardFieldsDict, hit.transform);
		

		foreach (Transform t in boardFieldsDict.Values){
			if (t.gameObject.layer == 0){
				t.gameObject.layer = 2; // Ignore raycast
			}
		}

		if (active_player_pieces.Count == 3){
			print ("Update player: Three cross pieces is on the board."); 
			int currentStoneIndex = findPieceByTransform(activePlayerPieces, stone);

			foreach(KeyValuePair <int, Transform> kvp in boardFieldsDict)
			{
				if(kvp.Key == currentStoneIndex)
                {
					kvp.Value.GetChild(0).gameObject.GetComponent<Light>().enabled = false;
					kvp.Value.gameObject.layer = 0; 
				}
			}

			boardScript.UpdateBoard(currentStoneIndex, "-1");
			active_player_pieces.Remove(currentStoneIndex);
			active_player_pieces.Add(board_field_index, stone); 
		} else {
			activePlayerPieces.Add(boardFieldIndex, stone); 
			playerChoices.Add(boardFieldIndex);
			boardScript.UpdateBoard(boardFieldIndex, "P");
			boardScript.PrintBoard(boardSizeX); 
		}

	}


	IEnumerator opponentStep(float f, int game_mode) {
		int bestMoveIndex; int bestPieceToMove;

		UnplayedPlayerPiecesInteraction(false, playerPieces); 
		PlayedPlayerPiecesInteraction(false);
		
		bestMoveIndex = controller.getMove(opponentChoices)[0];
		//bestPieceToMove = controller.getMove(opponentChoices)[1];
		

		if (activeOpponentPieces.Count == 3){
			//opponentPiece = activeOpponentPieces[bestPieceToMove];
           // boardFieldsDict[bestPieceToMove].gameObject.layer = 0;  
		} else {
			// Pick random piece of inactive circle pieces 
			bestPieceToMove = Random.Range(0, inactiveOpponentPieces.Count);
			opponentPiece = inactiveOpponentPieces[bestPieceToMove]; 
			inactiveOpponentPieces.RemoveAt(bestPieceToMove);
		}
		
		// Update the board and choices
		opponentChoices.Add(bestMoveIndex);
		activeOpponentPieces.Add(bestMoveIndex, opponentPiece);
		boardScript.UpdateBoard(bestMoveIndex, "O");
        controller.turnsTaken++;

        // Do visuals
        opponentAnimator.SetInteger("anim_state", 1);
		StartCoroutine(vfx_script.FadeLight(sun));

		// If computer win
		if(boardScript.checkForWin(opponentChoices)){
			// Internal variables
			controller.turnsTaken = 0; 
			opponentScore++; 

			// Animation and materials
			opponentAnimator.SetInteger("anim_state", 3);

			// Text
			GameObject.FindGameObjectWithTag("opponent_score").GetComponent<Text>().text = "Opponent: " + opponentScore; 

			subtitle_text.text = "Your tv: Now what's this...?"; 
			StartCoroutine(vfx_script.FadeText(subtitle_text, f)); 

			yield return new WaitForSeconds(2.5f);
			subtitle_text.text = "Your tv: Hah! Three Fin a row!"; 
			StartCoroutine(vfx_script.FadeText(subtitle_text, f)); 


			// Transform piece: 
			Vector3 destination = boardFieldsDict[bestMoveIndex].transform.position;
			destination.y += 15.0f; 
			opponentPiece.transform.position = destination;

			yield return new WaitForSeconds(1.5f); 
			restartButton.SetActive(true); 
			PlayedPlayerPiecesInteraction(true); 


			yield break; 
		} else {
			// Text
			// This is bad
			if(activeOpponentPieces.Count == 3 && !sayOnce){
				subtitle_text.text = "Now the real game begins. You can't add more pieces to the board";
				StartCoroutine(vfx_script.FadeText(subtitle_text, f)); 
				sayOnce = true; 
			} else {
				subtitle_text.text = oppMessageList[Random.Range(0, oppMessageList.Count)];
				StartCoroutine(vfx_script.FadeText(subtitle_text)); 
			}

			

			yield return new WaitForSeconds(f);
			// Do some transformations
			Vector3 destination = boardFieldsDict[bestMoveIndex].transform.position;
			destination.y += 15.0f;
			opponentPiece.transform.position = destination;
		}

		if(opponentScore == 5) {
			vfx_script.triggerLose(); 
			print ("Trigger resetgame"); 

		}
		

		StartCoroutine(vfx_script.FadeLight(sun, true)); 
		opponentAnimator.SetInteger("anim_state", 0);

		yield return new WaitForSeconds(f); 

		if(activePlayerPieces.Count == 3){
			PlayedPlayerPiecesInteraction(true);
		}
		else {
			UnplayedPlayerPiecesInteraction(true, playerPieces);
		}
	}

	/*------------------------------------------ GAME ALGORITHM FUNCTIONS ----------------------------------------- */
	public void updateBoardChoices() {
		playerChoices.Clear(); opponentChoices.Clear();
		foreach (int index in activePlayerPieces.Keys){
			playerChoices.Add(index);
		}

		foreach (int index in activeOpponentPieces.Keys){
			opponentChoices.Add(index);
		}
	}
	public void AssignVariables(){
        boardScript = gameBoard.GetComponent<Board>();
		controller = GameObject.FindGameObjectWithTag("opponent").GetComponent<AIController>();
        playerPieces = GameObject.FindGameObjectWithTag("player_pieces").transform;

        for (int i = 0; i < gameBoard.transform.childCount; i++) {
			boardFieldsDict.Add(i, gameBoard.transform.GetChild(i));
		}

		foreach (Transform child in GameObject.FindGameObjectWithTag("opponent_pieces").transform){
			inactiveOpponentPieces.Add(child); 
		}
		
		// Visual Effects 
		vfx_script = GameObject.FindGameObjectWithTag("vfx_controller").GetComponent<VFX>(); 
		vfx_script.disableOrEnable(false); 

		// TransformScript Object 
		transformScript = GameObject.FindGameObjectWithTag("transform_controller").GetComponent<TransformScript>(); 
		
		// GUI and Animation
		subtitle = GameObject.FindGameObjectWithTag("subtitle");
		subtitle_text = subtitle.GetComponent<Text>(); 
		
		opponent = GameObject.FindGameObjectWithTag ("opponent");
		opponentAnimator = opponent.GetComponent<Animator>();
		
		// Lights
		sun = GameObject.FindGameObjectWithTag("sun").GetComponent<Light>();


		// Dialogue
		//TODO: Move this to another component
		oppMessageList.Add("Your TV: Do you think this is a game?"); 
		oppMessageList.Add("Your TV: I see, you got me 'cornered'.");
		oppMessageList.Add("Your TV: I like you on acid girl.");
		oppMessageList.Add("Your TV: Why do you even try?");
		oppMessageList.Add("Your TV: Hmm, let me think..."); 
		sayOnce = false; 

		restartButton = GameObject.FindGameObjectWithTag("restart");  
		restartButton.SetActive(false); 
	}

	public void resetGameVariables(){
		print ("resetGameVariables called"); 
		boardScript.CreateBoard(boardSizeX, boardSizeY);
		controller.turnsTaken = 0;


		// Boardfields and layers
		boardFieldsDict.Clear();
		for (int i = 0; i < gameBoard.transform.childCount; i++)
		{
			Transform child = gameBoard.transform.GetChild(i);
			child.gameObject.layer = 0;
			LightBoardPiece(child, false);
			boardFieldsDict.Add(i, child);

		}
		

		// Reassign player piece layers:  
		foreach(Transform child in playerPieces){
			child.gameObject.layer = 0; 
		}

		// Reassign inactive opponent pieces
		inactiveOpponentPieces.Clear();
		GameObject temp_gameob = GameObject.Find("circle_pieces"); 
		foreach(Transform child in temp_gameob.transform){
			inactiveOpponentPieces.Add(child); 
		}

		// Reset active pieces
		activeOpponentPieces.Clear(); 
		activePlayerPieces.Clear();  
				
		opponentChoices.Clear();  
		playerChoices.Clear();  

		// Animation and sound 
		opponentAnimator.SetInteger("anim_state", 0);
	}
	/*------------------------------------------ VISUAL, PLAYER CONTROLLER AND CAMERA FUNCTIONS ----------------------------------------- */
	

	public void PlayedPlayerPiecesInteraction(bool b){
		if(b){
			foreach(Transform t in activePlayerPieces.Values){
				t.gameObject.GetComponent<MoveStone>().enabled = true; 
				t.gameObject.layer = 0; 
			}
		} else {
			foreach(Transform t in activePlayerPieces.Values){
				t.gameObject.GetComponent<MoveStone>().enabled = false; 
				t.gameObject.layer = 2; 
			}
		}
	}

	public void UnplayedPlayerPiecesInteraction(bool b, Transform parentGameobject){
		foreach(Transform child in parentGameobject){
			child.GetComponent<MoveStone>().enabled = b; 
		}
	}

	public void TriggerPlayerWin() {
		playerScore++;
		GameObject.FindGameObjectWithTag("player_score").GetComponent<Text>().text = "You: " + playerScore;
		restartButton.SetActive(true);
		restartButton.transform.GetChild(0).GetComponent<Text>().text = "You won! Click here to grab another round";

		opponentAnimator.SetInteger("anim_state", 3);
		print(opponentAnimator.GetInteger("anim_state"));
		subtitle_text.text = "Your TV: I lost! Go to hell.";
		//vfx_script.triggerLose(); 
	}

	/*------------------------------------------ HELPER FUNCTIONS ----------------------------------------- */

	public void resetGame(){ 
		// This function is called fromr restartButton
		print ("resetGame called"); 
		restartButton.SetActive(false); 
		resetGameVariables(); 

		// Transform opponent and player pieces (and set default layers)
		GameObject boardpiece_botright = GameObject.Find("board_piece_botright"); 
		GameObject parent = GameObject.Find ("cross_pieces"); 

		Vector3 wanted_position = boardpiece_botright.transform.position; 
		Vector3 offset = new Vector3(17.0f, 0.0f, 0.0f); 
		wanted_position += offset; 

		// Transform player pieces
		foreach(GameObject child in GameObject.FindGameObjectsWithTag("player_piece")){
			// This should be animated
			child.gameObject.layer = 0; 
			child.transform.parent = parent.transform;

			wanted_position.y += 1.4f; 
			wanted_position.x += Random.Range(-4, 4); 
			wanted_position.z += Random.Range(-4, 4); 

			child.transform.position = wanted_position;
			//MovePiece(wanted_position, child.transform);  

		}

		// Transform opponent pieces
		wanted_position = GameObject.Find("board_piece_topleft").transform.position; 
		offset = new Vector3(-15.0f, 0.0f, 0.0f); 
		wanted_position += offset; 

		foreach(Transform child in GameObject.Find("circle_pieces").transform){
			child.transform.position = wanted_position;
			wanted_position.z += -4.0f; 
		}
	}

	public void GameOver(){
		//print ("Reset game called!"); 
		//print(Application.loadedLevel);
		//Application.LoadLevel(Application.loadedLevel);
	}
	

	public void LightBoardPiece(Transform board_piece, bool on = true){
			board_piece.GetChild(0).gameObject.GetComponent<Light>().enabled = on;
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

	IEnumerator MovePiece(Vector3 destination, Transform t, float duration = 0.0f){
		// Duration == time
		t.position = destination; 

		yield return null; 
	}

    public int findPieceByTransform(Dictionary<int, Transform> dict, Transform t) {
        foreach(KeyValuePair<int, Transform> kvp in dict)
        {
            if(t == kvp.Value)
            {
                return kvp.Key;
            }
        }

        return -1; 
    }


	
		


	

}
