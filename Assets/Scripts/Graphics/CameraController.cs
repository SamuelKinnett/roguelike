﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{

	public GameObject player;
	public GameObject obj_EntityManager;
	public GameObject obj_GameController;
	public GameObject obj_MapManager;
	public GameObject obj_Shroud;

	public float scrollSpeed;
	public float zoomSpeed;

	private PlayerController playerController;
	private GameController gameController;
	private MapManager mapManager;
	private EntityManager entityManager;
	private Rigidbody2D rigidBody;
	private Image shroud;

	bool showShroud;
	float targetShroudAlpha;
	int currentFloor;

	Vector3 defaultPosition;
	Vector3 mapPosition;
	Vector3 currentPosition;
	Vector3 targetPosition;

	float defaultOrtho;
	//The default orthographic size that the camera uses when
	// focused on the player
	float defaultOrthoZoom;
	//A value that lets the player choose their own custom
	// zoom level, within limits. It corresponds to the number of map tiles that take
	// up half of the screen height.
	float mapOrtho;
	//The orthographic size used when the camera is focused on the
	// map
	float orthoSize;
	//The current target orthographic size
	float playerWidth, playerHeight;
	//Width and height of the player sprite in world
	// units. Since all tiles, both terrain and mobs, are the same size, this can be
	// used in calculations besides those pertaining to the player sprite.
	float hitTimer;
	//This timer is used to shake the camera for a time after being hit.
	int mapWidth, mapHeight;

	// Use this for initialization
	void Start ()
	{
		playerController = player.GetComponent<PlayerController> ();
		gameController = obj_GameController.GetComponent<GameController> ();
		mapManager = obj_MapManager.GetComponent<MapManager> ();
		entityManager = obj_EntityManager.GetComponent<EntityManager> ();
		rigidBody = this.GetComponent<Rigidbody2D> ();
		shroud = obj_Shroud.GetComponent<Image> ();

		playerWidth = player.GetComponent<SpriteRenderer> ().bounds.size.x;
		playerHeight = player.GetComponent<SpriteRenderer> ().bounds.size.y;

		defaultPosition = new Vector3 (0, 0, -10);
		defaultPosition.x += (playerWidth / 2);
		defaultPosition.y += (playerHeight / 2);

		//Orthographic level of the camera represents the number of in-game units half of the vertical screen
		// size takes. In this case, we make sure it always shows 10 squares in height.
		defaultOrtho = 5 * playerHeight;
		defaultOrthoZoom = 5;

		//And we'll set the map ortho such that it can contain the entire map, with a 1 tile padding around the edge
		mapWidth = mapManager.mapWidth + 2;
		mapHeight = mapManager.mapHeight + 2;

		mapOrtho = (mapHeight * playerHeight) / 2;
		mapPosition = new Vector3 (0, 0, -10);
		mapPosition.x = (mapWidth / 2) * playerWidth;
		mapPosition.y = (mapHeight / 2) * playerHeight;

		currentFloor = gameController.GetCurrentFloor ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentPosition = this.transform.position;

		float distanceToTarget = Mathf.Sqrt (Mathf.Pow (targetPosition.x - currentPosition.x, 2)
		                         + Mathf.Pow (targetPosition.y - currentPosition.y, 2));
		targetShroudAlpha = distanceToTarget - 0.1f;

		Color tempColor = shroud.color;

		if (gameController.GetCurrentFloor () != currentFloor) {
			currentFloor = gameController.GetCurrentFloor ();
			tempColor.a = 1;
			shroud.color = tempColor;
		}

		if (showShroud) {
			if (shroud.color.a != targetShroudAlpha)
			if (shroud.color.a > targetShroudAlpha) {
				if (shroud.color.a - 0.01f > targetShroudAlpha)
					tempColor.a -= 0.01f;
				else if (shroud.color.a - 0.01f <= targetShroudAlpha)
					tempColor.a = targetShroudAlpha;
			}
		} else {
			if (shroud.color.a - 0.01f > 0)
				tempColor.a -= 0.01f;
			else
				tempColor.a = 0;
		}
		shroud.color = tempColor;

		if (Input.GetKey (KeyCode.M)) {
			//Zoom out to view the whole map
			// pause input during this time.
			gameController.Pause ();
			showShroud = false;
			targetPosition = mapPosition;
			orthoSize = mapOrtho;
		} else {
			//Move to the player's position
			showShroud = true;
			gameController.Resume ();
			if (Input.GetKeyDown (KeyCode.Period)) {
				//Decreases the default zoom level, thereby zooming in
				if (defaultOrthoZoom > 2) {
					--defaultOrthoZoom;
				}
				defaultOrtho = defaultOrthoZoom * playerHeight;
			} else if (Input.GetKeyDown (KeyCode.Comma)) {
				//Increases the default zoom level, thereby zooming out
				if (defaultOrthoZoom < 10 && defaultOrthoZoom < mapHeight) {
					++defaultOrthoZoom;
				}
				defaultOrtho = defaultOrthoZoom * playerHeight;
			}
			int[] playerPosition = entityManager.GetPlayerPosition ();
			orthoSize = defaultOrtho;
			targetPosition.x = playerPosition [0] * playerWidth + playerWidth / 2;
			targetPosition.y = playerPosition [1] * playerHeight + playerHeight / 2;
		}

		if (Camera.main.orthographicSize != orthoSize) {
			//If we're not at the target zoom level, smoothly interpolate between the current and target zoom levels
			// to achieve a smooth zooming effect.
			Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, orthoSize, Time.deltaTime * zoomSpeed);
			if (Mathf.Abs (Camera.main.orthographicSize - orthoSize) < 0.001f) {
				//Stop the camera from potentially zooming forever by snapping to the level after a certain point
				Camera.main.orthographicSize = orthoSize;
			}
		}

		if (currentPosition != targetPosition) {
			if (Camera.main.orthographicSize == defaultOrtho) {
				//If we're at the default zoom level, move the camera at the normal speed
				rigidBody.velocity = new Vector2 (targetPosition.x - currentPosition.x,
					targetPosition.y - currentPosition.y) * scrollSpeed;
			} else {
				//Otherwise, multiply the scrollspeed further by the otho level to account for the scaling of the map
				rigidBody.velocity = new Vector2 (targetPosition.x - currentPosition.x,
					targetPosition.y - currentPosition.y) * scrollSpeed * Camera.main.orthographicSize;
			}
		}

		if (hitTimer > 0) {
			Vector3 tempPosition = Camera.main.transform.position;
			tempPosition.x = targetPosition.x + (Random.value - 1) / 10;
			tempPosition.y = targetPosition.y + (Random.value - 1) / 10;
			Camera.main.transform.position = tempPosition;
			hitTimer -= Time.deltaTime;
		}

	}

	public void AddHitEffect (float seconds)
	{
		hitTimer = seconds;
	}
}
