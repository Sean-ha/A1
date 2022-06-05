using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float panSpeed;
	[SerializeField] private float zoomSpeed;

	private Camera cam;

	private void Awake()
	{
		// TODO: Move this somewhere else maybe
		Application.targetFrameRate = 60;

		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		// Handle position
		float hori = Input.GetAxis("Horizontal");
		float verti = Input.GetAxis("Vertical");

		Vector3 offset = new Vector3(hori, verti, 0f);
		if (offset.magnitude > 1)
			offset.Normalize();

		offset *= panSpeed * Time.deltaTime * (cam.orthographicSize / 11f);

		Vector3 newPos = transform.position + offset;

		transform.position = newPos;

		// Handle zoom
		/*float scroll = Input.GetAxis("Mouse ScrollWheel");
		
		if (scroll < 0f) // Scroll forwards: zoom in
		{
			cam.orthographicSize += zoomSpeed;

			if (cam.orthographicSize > 60f)
				cam.orthographicSize = 60f;
		}
		else if (scroll > 0f) // Scroll backwards: zoom out
		{
			cam.orthographicSize -= zoomSpeed;

			if (cam.orthographicSize < 3f)
				cam.orthographicSize = 3f;
		}*/
	}
}
