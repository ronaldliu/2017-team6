﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

  public float speed = 6f;

  Vector3 movement;
  Animator anim;
  Rigidbody playerRBody;
  Camera playerCam;

  int floorMask;
  float camRayLength = 100f;
  float camHorizontalAngle = 0f;

	// Initializes player variables on startup
	void Awake () {
    floorMask = LayerMask.GetMask("Floor");

		anim = GetComponentInChildren<Animator>();
    playerRBody = GetComponent<Rigidbody>();

    // Get the Main Character from the scene
    playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
    camHorizontalAngle = playerCam.GetComponent<PlayerCamera>().horizontalRotation;
	}

  // Runs all necessary updates for the player
  void FixedUpdate()
  {
    // Get the horizontal and vertical directions for movement input
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    // Move the player
    Move(h,v);

    // Turn the player
    Turning();

    // Animate the player base on direction of movement
    Animating(h,v);
  }

  // Moves the player the specified direction
	void Move(float h, float v)
  {
    movement.Set(h, 0f, v);

    movement = movement.normalized * speed * Time.deltaTime;
    movement = Quaternion.Euler(0, camHorizontalAngle, 0) * movement;

    playerRBody.MovePosition(transform.position + movement);
  }

  // Rotates the player base on the Camera position and Mouse position
  void Turning ()
  {
    // Get horizontal and vertical directions for aim input
    float h = Input.GetAxis("AimX");
    float v = Input.GetAxis("AimY");

    Quaternion newRotation;

    if (h != 0 || v != 0) {
      float angle = Mathf.Atan2(h, v * -1) * Mathf.Rad2Deg;
      angle += camHorizontalAngle;

      transform.rotation = Quaternion.Euler(0, angle, 0);
    } else {
      Ray camRay = playerCam.ScreenPointToRay (Input.mousePosition);

      RaycastHit floorHit;

      // Get the position of the hit on the floor.
      if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
        Vector3 playerToMouse = floorHit.point - transform.position;
        playerToMouse.y = 0f;

        newRotation = Quaternion.LookRotation(playerToMouse);
        playerRBody.MoveRotation(newRotation);
      }
    }
  }

	void Animating(float h, float v)
	{
		Vector3 relativeMoveDir = new Vector3 (h, 0, v);
		relativeMoveDir = (Quaternion.Inverse(playerRBody.rotation) * relativeMoveDir);

		anim.SetFloat ("ForwardMovement", relativeMoveDir.z);
		anim.SetFloat ("RightMovement", relativeMoveDir.x);
	}
}
