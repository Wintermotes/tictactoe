using UnityEngine;
using System.Text; 
using UnityEngine.UI;
using System.Collections;

public class VFX : MonoBehaviour {

	static GameObject vfx; 
	private GameObject subtitle; 

	private GameObject sun; 

	// Ambient
	public float fadeTime = 0.0f;
	public Color startColor; 
	public Color endColor = Color.black; 
	
	void Start(){
		Cursor.visible = true; 
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){

		}
	}


	public void disableOrEnable(bool b){
		gameObject.SetActive(b); 
	}

	public void triggerLose(){
		RenderSettings.ambientLight = Color.black; 
		//GameObject.Find("sun").SetActive(false); 
		//Cursor.lockState = CursorLockMode.Locked; 
		//Cursor.visible = false; 
		Camera.main.GetComponent<MouseLook>().enabled = false; 

		subtitle = GameObject.FindGameObjectWithTag("subtitle");
		subtitle.GetComponent<Text>().text = "Your TV: I WIN MOTHERFUCKER!"; 

		// Trigger animation: 

		// Trigger sound

		// Trigger visible button
	}

	public void triggerVictory(){
		disableOrEnable(true); 
		//print ("You won!"); 
	}

	public IEnumerator FadeLight(Light lightComponent, bool fadeOut = false){
		float time = 0.0f;  

		if(!fadeOut){
			while (time < fadeTime && lightComponent.intensity > 0.35f){
				time+=Time.deltaTime;
				lightComponent.intensity += -0.01f; 
				yield return null;
			}
		} else {
			while (time < fadeTime && lightComponent.intensity < 0.8f){
				time+=Time.deltaTime;
				lightComponent.intensity += 0.01f; 
				yield return null;
			}
		}
	}
	

	public IEnumerator FadeText(Text textComponent, float seconds = 6.0f){
		float time = 0.0f; 
		Color c = new Color(0.0f, 0.0f, 0.0f, 1.0f); 

		yield return new WaitForSeconds(seconds); 
		while (time < fadeTime && textComponent.color.a > 0.0f){
			time+=Time.deltaTime;
			c.a += -0.01f; 
			textComponent.color = c; 
			yield return null;
		}

		textComponent.text = "";
		textComponent.color = new Color(0.0f, 0.0f, 0.0f, 1.0f); 


	}
}
