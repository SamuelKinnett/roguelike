using UnityEngine;
using System.Collections;

public class HitResult : MonoBehaviour
{

	float duration, initialDuration;
	float posX, posY;
	float gridSize;
	bool displaying;
	bool finished;

	TextMesh textMesh;

	public void Initialise (float x, float y, string textToDisplay, float gridSize)
	{
		Vector3 tempTransform = this.transform.position;
		this.gridSize = gridSize;
		tempTransform.x = x * gridSize;
		tempTransform.y = y * gridSize + 2 * gridSize;
		tempTransform.z = -2;

		textMesh = this.GetComponent<TextMesh> ();

		textMesh.text = textToDisplay;
		textMesh.color = Color.white;
		textMesh.font = (Font)Resources.Load("Fonts/SDS_6x6");
		duration = 0.6f;
		initialDuration = duration;
		this.transform.position = tempTransform;
		this.displaying = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if (displaying) {
			duration -= Time.deltaTime;
			posY += gridSize * Time.deltaTime;

			Vector3 tempTransform = this.transform.position;
			tempTransform.y += gridSize * Time.deltaTime;
			this.transform.position = tempTransform;

			Color tempColor = textMesh.color;
			tempColor.a = (initialDuration * duration);
			if (tempColor.a < 0)
				tempColor.a = 0;
			Debug.Log (tempTransform.x + "," + tempTransform.y + "," + tempColor.a);
			textMesh.color = tempColor;

			if (duration <= 0) {
				finished = true;
			}
		}
	}

	public bool IsFinished ()
	{
		return finished;
	}

	public void DestroyHitResult ()
	{
		Destroy (gameObject);
		Destroy (this);
	}
}
