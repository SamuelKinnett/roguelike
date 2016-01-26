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
}

public class MapTile : MonoBehaviour
{
	TileInfo info;
	Sprite sprite;
	GameObject worldTile;
	//The object representing the tile in worldspace

	TileVisibility oldVisibility;
	//Used so that the shading of the tile is only
	//updated as necessary

	void Update ()
	{
		if (info.visibility != oldVisibility) {
			switch (info.visibility) {

			case TileVisibility.seen:
				//The tile has previously been seen, but is now not visible,
				//so tint it to be darker
				worldTile.GetComponent<SpriteRenderer> ().color = Color.grey;
				worldTile.GetComponent<SpriteRenderer> ().enabled = true;
				break;
			
			case TileVisibility.unseen:
				//For whatever reason, the tile is now unknown to the player.
				//Make it invisible.
				worldTile.GetComponent<SpriteRenderer> ().enabled = false;
				break;

			case TileVisibility.visible:
				//The tile is currently visible, apply no tinting
				worldTile.GetComponent<SpriteRenderer> ().color = Color.white;
				worldTile.GetComponent<SpriteRenderer> ().enabled = true;
				break;
			}
			oldVisibility = info.visibility;
		}
	}

	public MapTile ()
	{
		info.solid = true;
		info.stairsUp = false;
		info.stairsDown = false;
		info.visibility = TileVisibility.unseen;
		oldVisibility = TileVisibility.unseen;

	}

	public void CreateTile (int x, int y, bool solid, bool stairsUp, bool stairsDown, Sprite sprite)
	{
		info.x = x;
		info.y = y;
		info.solid = solid;
		info.stairsUp = stairsUp;
		info.stairsDown = stairsDown;

		this.sprite = sprite;

		worldTile = new GameObject ();
		worldTile.AddComponent<SpriteRenderer> ();
		worldTile.GetComponent<SpriteRenderer> ().sprite = this.sprite;

		//Place the tile in world space
		//Essentially, we multiply the logical co-ordinate of the tile by the width of
		//the sprite in worldspace, giving us the correct world position. For example,
		//a tile at position (3, 5) in logical space with a sprite that measured 12 by
		//12 world units would be placed at (36, 60) in the world.

		Vector3 tilePosition = worldTile.transform.position;
		tilePosition.x = x * worldTile.GetComponent<SpriteRenderer> ().bounds.size.x;
		tilePosition.y = y * worldTile.GetComponent<SpriteRenderer> ().bounds.size.y;
	
		//Set the spriterenderer to initially make the tile invisible (unseen)
		worldTile.GetComponent<SpriteRenderer> ().enabled = false;
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
	}

	public TileInfo GetInfo ()
	{
		return this.info;
	}
}

