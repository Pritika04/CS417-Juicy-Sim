using System.Collections;
using UnityEngine;

public class portalTp : MonoBehaviour
{
    public Camera portalCamera;
    public GameObject thePlayer;


    void OnTriggerEnter(Collider other)
    {
 
            Debug.Log("Trigger hit by: " + other.name);
            Debug.Log("Teleporting to: " + portalCamera.transform.position);
            thePlayer.transform.position = portalCamera.transform.position;
            Debug.Log("Player landed at: " + thePlayer.transform.position);


        //Transform playerTransform = player.transform;


        //obj.transform.rotation = portalCamera.transform.rotation;

    }

}