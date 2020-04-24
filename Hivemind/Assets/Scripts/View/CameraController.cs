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

        movement.x += (Input.GetAxis("Horizontal") * MovementSpeed) * Time.deltaTime;
        movement.z += (Input.GetAxis("Vertical") * MovementSpeed) * Time.deltaTime;
        
        if (Input.mousePosition.x < EdgeSize)
        {
            movement.x -= MovementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x > Screen.width - EdgeSize)
        {
            movement.x += MovementSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y < EdgeSize)
        {
            movement.z -= MovementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - EdgeSize)
        {
            movement.z += MovementSpeed * Time.deltaTime;
        }
        if(Time.timeScale == 0)
            movement *= Time.timeScale;

        transform.position += movement;
    }
}
