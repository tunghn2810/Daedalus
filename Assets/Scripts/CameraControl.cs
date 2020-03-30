using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Attributes
    float horzSpeed = 2;
    float vertSpeed = 2;

    float yaw = 0;
    float pitch = 0;

    float minFOV = 50;
    float maxFOV = 90;
    float sensitivity = -10;

    // Update is called once per frame
    void Update()
    {
        //Only rotate when holding down right mouse button
        if (Input.GetMouseButton(1))
        {
            yaw += horzSpeed * Input.GetAxis("Mouse X");
            pitch -= vertSpeed * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        //Zoom in/out using the mouse scrollwheel
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFOV, maxFOV);
        Camera.main.fieldOfView = fov;
    }
}
