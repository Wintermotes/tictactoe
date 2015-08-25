using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text; 

public class GameAlgorithm : MonoBehaviour {

	private GameObject player; 
	private GameObject opponent; 
	private GameObject subtitle;
	private GameObject tv_screen; 
	private int key; private int turnsTaken; 

	// Dicts of board_fields, player_pieces and opp_pieces
	public Board game_board; 
	private List<Transform> inactive_circle_pieces = new List<Transform>();
	private Dictionary<int, Transform> active_circle_pieces = new Dictionary<int, Transform>(); 
	private Dictionary<Transform, int> active_cross_pieces = new Dictionary<Transform, int>(); 
	private Dictionary<Transform, int> board_fields = new Dictionary<Transform, int>(); 


	private Transform board_field; 
	private Transform circle_piece; 

	private List<int> playerChoices = new List<int>();
	private List<int> opponentChoices = new List<int>();
	private int player_score; private int opponent_score; 

	
	// Lights, vfx, animation and sound
	private Light sun; 
	public VFX vfx_script;  
	private Animator opponentAnimator; 


	// UI, debugging And dialogue
	public List<string> oppMessageList = new List<string>(); 
	private Text subtitle_text; 
	private GameObject restartButton; 
	private StringBuilder builder = new StringBuilder();

	// Materials 
	Material[] screensavers; 
	private List<Material> screensaver_list = new List<Material>();


	void Awake(){
		game_board = GetComponent<Board>();  
		game_board.CreateBoard(); 
	}

	void Start(){
		AssignVariables(); 

	}
	void Update(){
		if(Input.GetKeyDown("space")){
			ChangeTVScreen(); 
		}
	}

	public void ChangeTVScreen(){
		int i = Random.Range(0, 4); 
		//Material material = screensavers[i];
		Material material = screensaver_list[i];

		Material[] temp_screensavers = tv_screen.GetComponent<MeshRenderer>().materials;
		temp_screensavers[3] = material; 
		tv_screen.GetComponent<MeshRenderer>().materials = temp_screensavers;
	}
	public void update_game(RaycastHit hit, int person, Transform stone){

		/*------------------------------------------ PLAYER STEP ----------------------------------------- */
		board_field = FindIndexInBoardfield(hit.transform.name); 
		LightBoardPiece(board_field); 
		game_board.UpdateBoard(key, person);
		board_field.gameObject.layer = 10; // Layer 10 = occupied

		if(person == 1){
			updatePlayer(active_cross_pieces, stone, key);

			// Transforming stone from raycast
			Vector3 board_pos = hit.transform.gameObject.transform.position; 
			board_pos.y += 2.1f; 
			stone.position = board_pos;

			// For animation, text and other stuff	
			GameObject temp_gb = GameObject.Find("board_pieces"); 
			print ("Changing layer on board_pieces!"); 
			foreach(Transform t in temp_gb.transform){
				if(t.gameObject.layer == 0){
					t.gameObject.layer = 2; 
				}
			}
		}

		// Clearing and updating choices at each move! 
		playerChoices.Clear(); opponentChoices.Clear(); 
		foreach(int index in active_cross_pieces.Values){
			playerChoices.Add(index); 
		}

		foreach(int index in active_circle_pieces.Keys){
			opponentChoices.Add(index); 
		}

		// Check for winning condition for the player
		if(game_board.checkForWin(playerChoices)){
			player_score++; 
			GameObject.FindGameObjectWithTag("player_score").GetComponent<Text>().text = "You: " + player_score; 
			ChangeTVScreen(); 
			restartButton.SetActive(true); 
			restartButton.transform.GetChild(0).GetComponent<Text>().text = "You won! Click here to grab another round"; 
			
			opponentAnimator.SetInteger("anim_state", 3);
			print(opponentAnimator.GetInteger("anim_state")); 
			subtitle_text.text = "Your TV: I lost! Go to hell."; 
			game_board.PrintBoard(); 
			//vfx_script.triggerLose(); 
			turnsTaken = 0; 
			return;
			 
		}

		if(player_score == 5) {
			vfx_script.triggerWin(); 
		} else {
		/*------------------------------------------ COMPUTER STEP ----------------------------------------- */
			StartCoroutine(AnimateComputerTurn(2.0f)); 
		}
	}

	public void updatePlayer(Dictionary<Transform, int> active_player_pieces, Transform stone, int board_field_index){
		if(active_cross_pieces.Count == 3){
			int previous_stone_index = active_cross_pieces[stone];
			foreach(KeyValuePair <Transform, int> kvp in board_fields){
				if(kvp.Value == previous_stone_index){
					kvp.Key.GetChild(0).gameObject.GetComponent<Light>().enabled = false;
					kvp.Key.gameObject.layer = 0; 
				}
			}

			// For the game algorithm
			game_board.UpdateBoard(active_cross_pieces[stone], -1);
			active_cross_pieces.Remove(stone);
			active_cross_pieces.Add(stone, board_field_index); 


		} else {
			active_cross_pieces.Add(stone, board_field_index); 
			playerChoices.Add(board_field_index); 
		}

	}


	IEnumerator AnimateComputerTurn(float f) {
		game_board = GetComponent<Board>();
		bool usedAllPieces = false; int i; int j; 
		
		if(inactive_circle_pieces.Count == 0 && !usedAllPieces){
			print ("Used all pieces, assigning variable again"); 
			GameObject temp_gameob = GameObject.Find("circle_pieces"); 
			foreach(Transform child in temp_gameob.transform){
				inactive_circle_pieces.Add(child); 
			}
			usedAllPieces = true; 
		} 
		
		i = Random.Range(0, inactive_circle_pieces.Count);
		circle_piece = inactive_circle_pieces[i]; 
		
		if(!usedAllPieces){
			inactive_circle_pieces.RemoveAt(i); // So we use all the pieces first
		}
		
		// Choose boardfield
		if(turnsTaken > 5){
			print ("Turns taken great than 5"); 
			i = game_board.EvaluateMoves(); 
			print ("i: " + i); 
		} else {
			i = game_board.CalculateComputerTurn(opponentChoices)[0]; 
			turnsTaken++; 
		}


		// j = best piece to move 
		j = game_board.CalculateComputerTurn(opponentChoices)[1];
		
		// When we have 3 pieces on the board
		if(active_circle_pieces.Count == 3){
			circle_piece = active_circle_pieces[j]; 
			foreach(KeyValuePair <Transform, int> kvp in board_fields){
				if(kvp.Value == j){
					kvp.Key.gameObject.layer = 0; 
				}
			}
		} else {
			active_circle_pieces.Add(i, circle_piece); 
		}
		
		if(j>-1){
			game_board.UpdateBoard(j, -1); 
			active_circle_pieces.Add(i, active_circle_pieces[j]); 
			active_circle_pieces[j].gameObject.layer = 0; 
			active_circle_pieces.Remove(j); 
			
		}
		
		board_field = transform.GetChild(i); 
		board_field.gameObject.layer = 10;
		
		// Update the board and choices
		opponentChoices.Remove(j); 
		opponentChoices.Add(i); 
		game_board.UpdateBoard(i, 0);

		// Do visuals
		opponentAnimator.SetInteger("anim_state", 1);
		StartCoroutine(vfx_script.FadeLight(sun)); 

		if(game_board.checkForWin(opponentChoices)){
			opponent_score++; 
			ChangeTVScreen(); 
			GameObject.FindGameObjectWithTag("opponent_score").GetComponent<Text>().text = "Opponent: " + opponent_score; 
			restartButton.SetActive(true); 

			opponentAnimator.SetInteger("anim_state", 3);
			print(opponentAnimator.GetInteger("anim_state")); 
			subtitle_text.text = "Your TV: I won! Too bad."; 
			game_board.PrintBoard(); 
			//vfx_script.triggerLose(); 
			turnsTaken = 0; 
			yield break; 
		} else {
			subtitle_text.text = oppMessageList[Random.Range(0, oppMessageList.Count)];
			StartCoroutine(vfx_script.FadeText(subtitle_text)); 
			yield return new WaitForSeconds(f);
		}


		// Do some transformations
		Vector3 destination = board_field.transform.position;
		destination.y += 15.0f; 
		circle_piece.transform.position = destination;

		if(opponent_score == 5) {
			vfx_script.triggerLose(); 
			print ("Trigger resetgame"); 

		}
		
		if(active_cross_pieces.Count == 3){
			// Integrate sound? 
			
			// Make all but active pieces ignore raycast
			foreach(Transform active_player_piece in active_cross_pieces.Keys){
				active_player_piece.parent = null; 
				active_player_piece.gameObject.layer = 0; 
			}
			
			GameObject temp_gb = GameObject.Find("cross_pieces"); 
			foreach(Transform child in temp_gb.transform){
				child.GetComponent<MoveStone>().enabled = false; 
			}
		}
		StartCoroutine(vfx_script.FadeLight(sun, true)); 
		opponentAnimator.SetInteger("anim_state", 0);
		print ("Done calling AnimateComputerTurn"); 
		
	}
	
	/*------------------------------------------ GAME ALGORITHM FUNCTIONS ----------------------------------------- */

	public void AssignVariables(){
		// Boardfields
		game_board = GetComponent<Board>(); 
		key = -1; 
		int counter = 0;

		foreach(Transform child in transform){
			board_fields.Add(child, counter); 
			child.gameObject.layer = 2; 
			counter++; 
		}
		
		// Circle Pieces (i.e the opponent pieces for now)
		foreach(Transform child in GameObject.Find("circle_pieces").transform){
			inactive_circle_pieces.Add(child); 
		}
		
		// Visual Effects Object
		vfx_script = GameObject.Find("VFX").GetComponent<VFX>(); 
		vfx_script.disableOrEnable(false); 
		
		// GUI and Animation
		subtitle = GameObject.FindGameObjectWithTag("subtitle");
		subtitle_text = subtitle.GetComponent<Text>(); 
		
		opponent = GameObject.FindGameObjectWithTag ("opponent");
		opponentAnimator = opponent.GetComponent<Animator>();
		
		// Lights
		sun = GameObject.Find("sun").GetComponent<Light>();

		// Materials 
		tv_screen = GameObject.FindGameObjectWithTag("tv_screen"); 
		for(int i = 1; i<5; i++){
			string s = "tv_screensaver_" + i.ToString(); 
			//print ("String: " + s); 
			Material screensaver = Resources.Load(s, typeof(Material)) as Material;
			screensaver_list.Add(screensaver); 
			//print (screensaver.name); 
		}

		// Dialogue
		oppMessageList.Add("Your TV: Do you think this is a game?"); 
		oppMessageList.Add("Your TV: I see, you got me 'cornered'.");
		oppMessageList.Add("Your TV: I like you on acid girl.");
		oppMessageList.Add("Your TV: Why do you even try?");
		oppMessageList.Add("Your TV: Hmm, let me think..."); 
		
		restartButton = GameObject.FindGameObjectWithTag("restart");  
		restartButton.SetActive(false); 
	}

	public Transform FindIndexInBoardfield(string name){
		foreach(Transform field in board_fields.Keys){
			if (name == field.name){
				print ("Found a match!"); 
				key = board_fields[field]; 
				board_field = field; 
				break; 
			}
		}
		return board_field; 
	} 

	public void ResetGameVariables(){
		game_board.CreateBoard(); 
		game_board.PrintBoard(); 
		int counter = 0; 

		// Boardfields and layers
		board_fields.Clear(); 
		foreach(Transform child in transform){
			board_fields.Add(child, counter);
			child.gameObject.layer = 0; 
			LightBoardPiece(child, false); 
			counter++; 
		}

		// Player pieces
		inactive_circle_pieces.Clear();

		active_circle_pieces.Clear(); 
		active_cross_pieces.Clear();  
				
		opponentChoices.Clear();  
		playerChoices.Clear();  

		// Animation and sound 
		opponentAnimator.SetInteger("anim_state", 0);
	}

	/*------------------------------------------ HELPER FUNCTIONS ----------------------------------------- */

	public void resetGame(){ 
		restartButton.SetActive(false); 
		ResetGameVariables(); 
		ChangeTVScreen(); 
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


	
		


	

}
