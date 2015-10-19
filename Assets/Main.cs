using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	public Vector3 pos = new Vector3(0, 0, 0);
	public TileMap tilemap;

	void Awake() {
		Debug.Log ("Main Awake");
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Main Start");
		GameObject tilemap_obj = GameObject.Find ("TileMap");
		tilemap = tilemap_obj.GetComponent<TileMap>();
		Debug.Log (tilemap);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("up"))
			tilemap.pos.y -= 0.3f;
		
		if (Input.GetKey ("down"))
			tilemap.pos.y += 0.3f;

		if (Input.GetKey ("left"))
			tilemap.pos.x += 0.3f;
		
		if (Input.GetKey ("right"))
			tilemap.pos.x -= 0.3f;
	}
}
