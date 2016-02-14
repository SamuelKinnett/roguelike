using UnityEngine;
using System.Collections;

public class SearchPerameters : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Point StartLocation { get; set; }
    public Point EndLocation { get; set; }
    public bool[,] Map { get; set; }
}
