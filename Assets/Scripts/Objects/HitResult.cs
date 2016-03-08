using UnityEngine;
using System.Collections;

/// <summary>
/// This class creates a text readout that slowly floats up and fades out, useful for
/// displaying combat results.
/// </summary>
public class HitResult : MonoBehaviour {

	float duration;
	float posX, posY;
	float gridSize;
	bool displaying;
	GameObject worldObject;
	TextMesh textMesh;

	// Use this for initialization
	void Start () {
		displaying = false;
		worldObject = new GameObject ();
		worldObject.AddComponent<TextMesh> ();
		textMesh = worldObject.GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (displaying) {
			duration -= Time.deltaTime;
			posY += gridSize * Time.deltaTime;

			Vector3 tempTransform = this.transform.position;
			tempTransform.y += gridSize * Time.deltaTime;
			this.transform.position = tempTransform;

			if (duration <= 0) {
				Destroy (worldObject);
				Destroy (this);
			}
		}
	}

	public void Display(float x, float y, string textToDisplay, PlayerController player) {
		Vector3 tempTransform = this.transform.position;
		gridSize = player.GetComponent<SpriteRenderer> ().bounds.size.x;
		tempTransform.x = x * gridSize;
		tempTransform.y = y * gridSize;

		textMesh.text = textToDisplay;
		duration = 1;
		this.transform.position = tempTransform;
	}
}
