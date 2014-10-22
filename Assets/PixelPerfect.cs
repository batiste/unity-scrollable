using UnityEngine;
using System.Collections;

public class PixelPerfect : MonoBehaviour
{
	public float floatable = 100.0f; //This can be PixelsPerUnit, or you can change it during runtime to alter the camera.
	
	void Update ()
	{
		this.camera.orthographicSize = Screen.height * gameObject.camera.rect.height / floatable / 2.0f;//- 0.1f;
	}
}
