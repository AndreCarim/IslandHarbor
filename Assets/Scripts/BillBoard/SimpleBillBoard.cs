using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBillBoard : MonoBehaviour
{
    Vector3 cameraDir;

    // Update is called once per frame
    void Update()
    {   
        if(Camera.main){
            cameraDir = Camera.main.transform.forward;
            cameraDir.y = 0f; //we dont want it to move on the y axis

            transform.rotation = Quaternion.LookRotation(cameraDir);
        }
       
    }
}
