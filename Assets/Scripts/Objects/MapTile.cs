using UnityEngine;
using System.Collections;

public enum TileVisibility
{
	unseen,
	seen,
	visible
}

public struct TileInfo
{
	public int x;
	public int y;
	public bool solid;
	public bool stairsUp;
	public bool stairsDown;
	public TileVisibility visibility;
	public float intensity;
	public bool active;
}

public class MapTile : ScriptableObject
{
	TileInfo info;
	Sprite sprite;
	GameObject worldTile;
	//The object representing the tile in worldspace

	TileVisibility oldVisibility;
	//Used so that the shading of the tile is only
	//updated as necessary

	bool initialised;	//Has the tile been initialised?

	void Update ()
	{

	}

	public MapTile ()
	{
		info.solid = true;
		info.stairsUp = false;
		info.stairsDown = false;
		info.visibility = TileVisibility.unseen;
		oldVisibility = TileVisibility.unseen;

	}

	/// <summary>
	/// This method is used to initialise the tile and to attach the sprite renderer
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void InitialiseTile(int x, int y) {
		info.x = x;
		info.y = y;
		info.active = false;

		worldTile = new GameObject ();
		worldTile.AddComponent<SpriteRenderer> ();

		//Set the spriterenderer to initially make the tile invisible (unseen)
		worldTile.GetComponent<SpriteRenderer> ().enabled = false;
		worldTile.name = "Tile [" + info.x + "," + info.y + "]";

		initialised = true;
	}


	/// <summary>
	/// This function is called when a tile needs to be updated or changed
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="solid">If set to <c>true</c> solid.</param>
	/// <param name="stairsUp">If set to <c>true</c> stairs up.</param>
	/// <param name="stairsDown">If set to <c>true</c> stairs down.</param>
	/// <param name="sprite">Sprite.</param>
	public void SetTile (int x, int y, bool solid, bool stairsUp, bool stairsDown, Sprite sprite)
	{

		if (!initialised)
			InitialiseTile (x, y);

		info.solid = solid;
		info.stairsUp = stairsUp;
		info.stairsDown = stairsDown;
		info.active = true;

		this.sprite = sprite;
		worldTile.GetComponent<SpriteRenderer> ().sprite = this.sprite;

		//Place the tile in world space
		//Essentially, we multiply the logical co-ordinate of the tile by the width of
		//the sprite in worldspace, giving us the correct world position. For example,
		//a tile at position (3, 5) in logical space with a sprite that measured 12 by
		//12 world units would be placed at (36, 60) in the world.

		Vector3 tilePosition = worldTile.transform.position;
		tilePosition.x = x * worldTile.GetComponent<SpriteRenderer> ().bounds.size.x;
		tilePosition.y = y * worldTile.GetComponent<SpriteRenderer> ().bounds.size.y;

		worldTile.transform.position = tilePosition;

		//Set the spriterenderer to initially make the tile invisible (unseen)
		worldTile.GetComponent<SpriteRenderer> ().enabled = false;
	}

	public void DestroyTile ()
	{
		Destroy (worldTile);
	}

	public void SetCollision (bool solid)
	{
		info.solid = solid;
	}

	public void SetStairsUp (bool stairsUp)
	{
		info.stairsUp = stairsUp;
	}

	public void SetStairsDown (bool stairsDown)
	{
		info.stairsDown = stairsDown;
	}

	public void SetVisibility (TileVisibility visibility)
	{
		info.visibility = visibility;

		if (info.visibility != oldVisibility) {
			switch (info.visibility) {

			case TileVisibility.seen:
				//The tile has previously been seen, but is now not visible,
				//so tint it to be darker
				worldTile.GetComponent<SpriteRenderer> ().color = new Color (0.2f, 0.2f, 0.2f);
				worldTile.GetComponent<SpriteRenderer> ().enabled = true;
				break;

			case TileVisibility.unseen:
				//For whatever reason, the tile is now unknown to the player.
				//Make it invisible.
				worldTile.GetComponent<SpriteRenderer> ().enabled = false;
				break;

			case TileVisibility.visible:
				//The tile is currently visible, apply tinting based on the intensity
				// value.
				worldTile.GetComponent<SpriteRenderer> ().color = new Color (info.intensity, info.intensity, info.intensity);
				worldTile.GetComponent<SpriteRenderer> ().enabled = true;
				break;
			}
			oldVisibility = info.visibility;
		}
	}

	public void SetLightIntensity(float intensity) {
		info.intensity = intensity;
	}

	public void SetInactive() {
		info.active = false;
		worldTile.GetComponent<SpriteRenderer> ().enabled = false;
	}

	public TileInfo GetInfo ()
	{
		return this.info;
	}
}

