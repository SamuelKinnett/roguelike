using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	public GameObject player;
	public GameObject obj_EntityManager;
	public float scrollSpeed;

	private PlayerController playerController;
	private EntityManager entityManager;
	private Rigidbody2D rigidBody;

	Vector3 defaultPosition;
	Vector3 currentPosition;
	Vector3 targetPosition;
	float playerWidth, playerHeight;

	// Use this for initialization
	void Start ()
	{
		playerController = player.GetComponent<PlayerController> ();
		entityManager = obj_EntityManager.GetComponent<EntityManager> ();
		rigidBody = this.GetComponent<Rigidbody2D> ();

		playerWidth = player.GetComponent<SpriteRenderer> ().bounds.size.x;
		playerHeight = player.GetComponent<SpriteRenderer> ().bounds.size.y;

		defaultPosition = new Vector3 (0, 0, -10);
		defaultPosition.x += (playerWidth / 2);
		defaultPosition.y += (playerHeight / 2);
	}
	
	// Update is called once per frame
	void Update ()
	{
		int[] playerPosition = entityManager.GetPlayerPosition ();
		targetPosition.x = playerPosition [0] * playerWidth;
		targetPosition.y = playerPosition [1] * playerHeight;

		currentPosition = this.transform.position;

		if (currentPosition != targetPosition) {
			rigidBody.velocity = new Vector2 (targetPosition.x - currentPosition.x,
				targetPosition.y - currentPosition.y) * scrollSpeed;
		}
	}
}
