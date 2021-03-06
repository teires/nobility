﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour {

	/*Controls camera through player input. Only supports horizontal motion, 
	 *but a vertical version may be desired?*/

	//MANUALS
	public bool PLAYER_INPUT = true;

	public float LEFTBOUND;
	public float RIGHTBOUND;
	public float INBOUND = 2;
	public float OUTBOUND = 0;
	public float SCROLL_SPEED = 1F;
	public float ZOOM_SPEED = .5F;
	public float LERP_SPEED = 1F;

	//GAME OBJECTS/COMPONENTS
	private Camera cam;
	private Camera farCam;
	private Camera nearCam;

	//STATIC VARS
	private Vector3 CAMERA_STARTINGPOS;
	private Vector3 ROTATE;

	//NONSTATIC VARS
	private float dir;
	private float zoom;
	private Vector3 movement;

	public Vector3 currentPoint {get; set;}
	public Vector3 rotate { get; set;}

	/*Init*/
	void Awake () {
		cam = this.gameObject.GetComponent<Camera> ();
		nearCam = GameObject.Find("Parallax Near Camera").GetComponent<Camera> ();
		farCam = GameObject.Find("Parallax Far Camera").GetComponent<Camera> ();
		CAMERA_STARTINGPOS = cam.transform.position;
		ROTATE = cam.transform.localRotation.eulerAngles;
		resetCamera ();
	}

	/*Check for edge of world*/
	private bool hitEdge(float dir, float zoom){
		float cam_pos = cam.transform.position.x;
		float cam_zoom = cam.transform.position.z;

		if (cam_pos <= CAMERA_STARTINGPOS.x - LEFTBOUND && dir < 0) {
			return true;
		} else if (cam_pos >= CAMERA_STARTINGPOS.x + RIGHTBOUND && dir > 0) {
			return true;
		} else if (cam_zoom >= CAMERA_STARTINGPOS.z + INBOUND && zoom > 0) {
			return true;
		} else if (cam_zoom <= CAMERA_STARTINGPOS.z - OUTBOUND && zoom < 0) {
			return true;
		}
		return false;
	}

	/*PARALLAX ZOOM*/
	private void GetFieldOfView() {

		//float a = cam.orthographicSize;
		float b = Mathf.Abs (cam.transform.position.z);

		//Set Ortho Zoom FOV
		//farCam.fieldOfView = Mathf.Atan(a/b) * Mathf.Rad2Deg * 2f;
		//nearCam.fieldOfView = farCam.fieldOfView;

		//Set Parallax Clipping Planes

		farCam.nearClipPlane = b;
		farCam.farClipPlane = cam.farClipPlane;
		nearCam.farClipPlane = b;
		nearCam.nearClipPlane = cam.nearClipPlane;
	}

	private Vector3 getMovement(bool input) {
		if (input == true) {
			dir = Input.GetAxis ("Horizontal");
			zoom = Input.GetAxis ("Vertical");
		}

		if (hitEdge(dir, zoom)) {
			return new Vector3(0, 0, 0);
		}

		//cam.orthographicSize += - zoom * ZOOM_SPEED;
		return new Vector3(dir*SCROLL_SPEED, 0, zoom * ZOOM_SPEED);
	}
		
	IEnumerator moveTo(Vector3 target) {
		if (cam.transform.position != target) {
			float i = 0;
			while (i < 1) {
				i += Time.deltaTime * LERP_SPEED;
				cam.transform.position = Vector3.Lerp (cam.transform.position, target, i);
				yield return new WaitForFixedUpdate ();
			}
		} else {
			yield break;
		}
	}
		
	IEnumerator rotateTo(Vector3 target) {
		if (cam.transform.eulerAngles != target){
			float i = 0;
			while (i < 1) {
				i += Time.deltaTime * LERP_SPEED;
				cam.transform.rotation = Quaternion.Slerp (cam.transform.rotation, Quaternion.Euler(target), i);
				yield return new WaitForFixedUpdate ();
			}
		}
	}

	public void resetCamera() {
		currentPoint = CAMERA_STARTINGPOS;
		rotate = ROTATE;
	}

	/* Update is called once per frame */
	void Update () {
		GetFieldOfView ();

		if (PLAYER_INPUT) {
			cam.transform.Translate (getMovement (PLAYER_INPUT));
		} else {
			StartCoroutine (rotateTo (rotate));
			StartCoroutine (moveTo (currentPoint));
		}
	}
}
