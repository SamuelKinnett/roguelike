using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class creates a text readout that slowly floats up and fades out, useful for
/// displaying combat results.
/// </summary>
public class HitResultManager : MonoBehaviour
{

	List<HitResult> hitResults;

	// Use this for initialization
	void Start ()
	{
		hitResults = new List<HitResult> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		bool removalNeeded = false;
		List<int> indicesToRemove = new List<int> ();

		for (int index = hitResults.Count - 1; index >= 0; --index) {
			if (hitResults [index].IsFinished ()) {
				removalNeeded = true;
				indicesToRemove.Add (index);
			}
		}

		if (removalNeeded) {
			foreach (int indexToRemove in indicesToRemove) {
				hitResults [indexToRemove].DestroyHitResult ();
				hitResults.RemoveAt (indexToRemove); 
			}
		}
	}

	public void CreateHitResult (float x, float y, string textToDisplay, float gridSize)
	{
		GameObject newWorldObject = new GameObject ();
		newWorldObject.AddComponent<TextMesh> ();
		newWorldObject.AddComponent<HitResult> ();

		Vector3 scale = newWorldObject.transform.localScale;
		scale.x = 0.125f;
		scale.y = 0.125f;
		newWorldObject.transform.localScale = scale;

		newWorldObject.GetComponent<HitResult> ().Initialise (x, y, textToDisplay, gridSize);
		hitResults.Add (newWorldObject.GetComponent<HitResult> ());
	}
}
