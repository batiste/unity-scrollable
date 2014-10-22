using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int size_x;
	public int size_y;
	public float tileSize = 1.0f;

	public Texture2D terrainTiles;
	public int tileResolution;

	private Vector3 center;
	private Vector3 pos = new Vector3(0, 0, 0);
	Rect[] tiles;

	private MeshFilter mesh_filter;
	private Vector3 mesh_trans = new Vector3(0, 0, 0); 

	void Awake() {
		Debug.Log ("Awake");
		Application.targetFrameRate = 5;
	}

	// Use this for initialization
	void Start () {
		mesh_filter = GetComponent<MeshFilter>();
		tiles = GetTiles();
		float ratio = 2 * Camera.main.orthographicSize / Screen.height;
		float wratio = ratio * Camera.main.aspect;
		size_x = (int)(2 + (wratio * Screen.width / tileSize));
		size_y = (int)(2 + (ratio * Screen.height / tileSize));
		BuildMesh();
		center = new Vector3(-tileSize * size_x / 2, -tileSize * size_y / 2, 0);
		center = center + new Vector3(-tileSize, -0, 0);
		transform.position = pos + center;
	}

	void Update () {
		pos.x += 1;
		pos.y += 1;
		Vector3 trans = new Vector3(
			pos.x % tileSize, 
			pos.y % tileSize, 0);
		transform.position = trans + center;
		if (trans != mesh_trans) {
			UVMapping ();
		}
		mesh_trans = trans;
	}
	
	public Rect[] GetTiles() {
		
		int numTilesPerRow = terrainTiles.width / tileResolution;
		int numRows = terrainTiles.height / tileResolution;

		float x_offset = (float) tileResolution / terrainTiles.width;
		float y_offset = (float) tileResolution / terrainTiles.height;

		Rect[] tiles = new Rect[numTilesPerRow * numRows];

		for(int y=0; y < numRows; y++) {
			for(int x=0; x < numTilesPerRow; x++) {
				// Rect(x, y, width, height)
				tiles[y * numTilesPerRow + x] = new Rect(x_offset * x, y_offset * y, x_offset, y_offset);
			}
		}

		return tiles;
	}

	public Rect GetTile(int x, int y) {
		int index = Mathf.Abs(x + y) % tiles.Length;
		return tiles[index];
	}

	public void UVMapping() {
		Vector2[] uv = mesh_filter.sharedMesh.uv;
		int x, y;
		int x_offset = (int)(((int)pos.x / (int)tileSize));
		int y_offset = (int)(((int)pos.y / (int)tileSize));
		for (y=0; y < size_y; y++) {
			for (x=0; x < size_x; x++) {
				int index = (y * size_x + x);
				int tileIndex = 4 * index;
				Rect tile = GetTile(x - x_offset, y - y_offset);
				uv[ tileIndex + 0 ] = new Vector2( tile.x, tile.y );
				uv[ tileIndex + 1 ] = new Vector2( tile.x + tile.width, tile.y );
				uv[ tileIndex + 2 ] = new Vector2( tile.x + tile.width, tile.y + tile.height );
				uv[ tileIndex + 3 ] = new Vector2( tile.x, tile.y + tile.height );
			}
		}
		// this seems necessary
		mesh_filter.sharedMesh.uv = uv;
	}

	public void BuildMesh() {
		
		int numTiles = size_x * size_y;
		int numTriangles = numTiles * 2;
		int numVertices = 4 * numTiles;

		// Generate the mesh data
		Vector3[] vertices = new Vector3[numVertices];
		Vector3[] normals = new Vector3[numVertices];
		Vector2[] uv = new Vector2[numVertices];

		int[] triangles = new int[ numTriangles * 3 ];
		
		int x, y;
		for(y=0; y < size_y; y++) {
			for(x=0; x < size_x; x++) {
				float _x = (float)x * tileSize;
				float _y = (float)y * tileSize;
				int index = (y * size_x + x);
				int tileIndex = 4 * index;

				vertices[ tileIndex + 0 ] = new Vector3( _x, _y );
				vertices[ tileIndex + 1 ] = new Vector3( _x + tileSize, _y );
				vertices[ tileIndex + 2 ] = new Vector3( _x + tileSize, _y + tileSize );
				vertices[ tileIndex + 3 ] = new Vector3( _x, _y + tileSize );

				Rect tile = GetTile(x, y);

				uv[ tileIndex + 0 ] = new Vector2( tile.x, tile.y );
				uv[ tileIndex + 1 ] = new Vector2( tile.x + tile.width, tile.y );
				uv[ tileIndex + 2 ] = new Vector2( tile.x + tile.width, tile.y + tile.height );
				uv[ tileIndex + 3 ] = new Vector2( tile.x, tile.y + tile.height );

				normals[ tileIndex + 0 ] = new Vector3( 0, 0, -1 );
				normals[ tileIndex + 1 ] = new Vector3( 0, 0, -1 );
				normals[ tileIndex + 2 ] = new Vector3( 0, 0, -1 );
				normals[ tileIndex + 3 ] = new Vector3( 0, 0, -1 );

				int triangleIndex = 6 * index;

				triangles[triangleIndex + 0] = tileIndex;
				triangles[triangleIndex + 1] = tileIndex + 2;
				triangles[triangleIndex + 2] = tileIndex + 1;

				triangles[triangleIndex + 3] = tileIndex;
				triangles[triangleIndex + 4] = tileIndex + 3;
				triangles[triangleIndex + 5] = tileIndex + 2;

			}
		}
		Debug.Log ("Done Verts!");

		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;

		//terrainTiles.filterMode = FilterMode.Point;
		//terrainTiles.wrapMode = TextureWrapMode.Clamp;
		//terrainTiles.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = terrainTiles;

		Debug.Log ("Done Mesh!");

	}

	
}
