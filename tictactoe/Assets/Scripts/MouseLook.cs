using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look (Debugged Edition)")]
public class MouseLook : MonoBehaviour {
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	public bool smoothLookX;
	private float sensivityXmemory;
	private float maximumSmoothX;
	private float minimumSmoothX;
	
	float rotationY = 0F;
	float rotationX = 0F;
	
	
	
	
	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			//float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
			
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
		
		//NEBUCH
		if (smoothLookX) {
			if (rotationX > maximumSmoothX) {
				sensitivityX = 1f;
			}
			
			if (rotationX < minimumSmoothX){
				
				sensitivityX = 1f;
			}
			
			if (rotationX > minimumSmoothX && rotationX < maximumSmoothX){
				
				sensitivityX = sensivityXmemory;
				
			} 
			
		}
		
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		//NEBUCH
		sensivityXmemory = sensitivityX;
		maximumSmoothX = maximumX - maximumX / 5;
		minimumSmoothX = minimumX + minimumX / (-5);
		
	}
}
