using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FloorPalette
{
	BottomLeft,
	Left,
	TopLeft,
	Top,
	TopRight,
	Right,
	BottomRight,
	Bottom,
	Centre,
	Vertical,
	Horizontal,
	EndPieceLeft,
	EndPieceTop,
	EndPieceRight,
	EndPieceBottom,
	FullyEnclosed
}

public enum WallPalette
{
	BottomLeft,
	TopLeft,
	TopRight,
	BottomRight,
	Horizontal,
	Vertical,
	LeftT,
	TopT,
	RightT,
	BottomT,
	FourWay,
	Foundation,
	SinglePiece
}

public class MapPalette : MonoBehaviour
{

	public Sprite[] floorTiles = new Sprite[16];
	public Sprite[] wallTiles = new Sprite[13];

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
		
}
