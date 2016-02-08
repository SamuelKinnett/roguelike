using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	public SpriteRenderer spriteRenderer;
	private float worldWidth, worldHeight;

	// Use this for initialization
	void Start ()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		worldWidth = spriteRenderer.bounds.size.x;
		worldHeight = spriteRenderer.bounds.size.y;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	/// <summary>
	/// Sets the position of the player object in the game world
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SetPosition (int x, int y)
	{
		Vector3 tempTrans = this.transform.position;
		tempTrans.x = x * worldWidth;
		tempTrans.y = y * worldHeight;
		this.transform.position = tempTrans;
	}

}
