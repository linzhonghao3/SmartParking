using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    
    public GameObject carCamera;
    public GameObject mainCamera;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) {
            carCamera.GetComponent<AudioListener>().enabled = true;
            carCamera.GetComponent<Camera>().enabled = true;
            mainCamera.GetComponent<AudioListener>().enabled = false;
            mainCamera.GetComponent<Camera>().enabled = false;
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            mainCamera.GetComponent<AudioListener>().enabled = true;
            mainCamera.GetComponent<Camera>().enabled = true;
            carCamera.GetComponent<AudioListener>().enabled = false;
            carCamera.GetComponent<Camera>().enabled = false;
            
        }
    }

    public void Exit() {
        Application.Quit();
    }
}
