using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MovementSpeed;
    public int EdgeSize;

    void Update()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) return;

        var movement = new Vector3();

        if (Input.mousePosition.x < EdgeSize || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x -= MovementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x > Screen.width - EdgeSize || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movement.x += MovementSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y < EdgeSize || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movement.z -= MovementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - EdgeSize || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movement.z += MovementSpeed * Time.deltaTime;
        }

        transform.position += movement;
    }
}
