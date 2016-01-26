using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to load all slices from a sprite into a single
/// array of sprites, which can then be retrieved using an index as
/// an argument.
/// </summary>
public class Spritesheet : MonoBehaviour
{

	public string filepath;
	//Path to the asset containing the sprites to be loaded.

	Sprite[] sprites;

	void Awake ()
	{
		sprites = Resources.LoadAll<Sprite> (filepath);
	}

	public Sprite GetSprite (int index)
	{
		return sprites [index];
	}
}
