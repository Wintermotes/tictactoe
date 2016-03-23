using UnityEngine;
using System.Collections;

public class TransformScript : MonoBehaviour {

	public float timeTakenDuringLerp = 1f;
	
	/// How far the object should move when 'space' is pressed
	public float distanceToMove = 10;
	private bool isLerping;
	
	private Vector3 startPosition;
	private Vector3 endPosition;
	
	//The Time.time value when we started the interpolation
	private float timeStartedLerping;

	private Transform objectToMove; 

	public void StartLerping(Transform t, Vector3 wantedPosition)
	{
		isLerping = true; objectToMove = t; 
		timeStartedLerping = Time.time;
		
		//We set the start position to the current position, and the finish to 10 spaces in the 'forward' direction
		startPosition = t.position;
		endPosition = t.position + wantedPosition*distanceToMove;
	}
	
	//We do the actual interpolation in FixedUpdate(), since we're dealing with a rigidbody
	void FixedUpdate()
	{
		if(isLerping)
		{
			GameObject gb = GameObject.Find("board_piece_botright"); 
			//We want percentage = 0.0 when Time.time = _timeStartedLerping
			//and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
			//In other words, we want to know what percentage of "timeTakenDuringLerp" the value
			//"Time.time - _timeStartedLerping" is.
			float timeSinceStarted = Time.time - timeStartedLerping;
			float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
			
			//Perform the actual lerping.  Notice that the first two parameters will always be the same
			//throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
			//to start another lerp)
			objectToMove.transform.position = Vector3.Lerp (startPosition, endPosition, percentageComplete);
			
			//When we've completed the lerp, we set _isLerping to false
			if(percentageComplete >= 1.0f)
			{
				isLerping = false;
			}
		}
	}
}
