using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [Header("Set Player Look At")]
    [SerializeField] public static Transform target;
    [SerializeField] public static Transform look;

    [Header("Offset Camera")]
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;


    // Update is called once per frame
    void FixedUpdate()
    {
        //position of the camera,
        // rotation of the camera
        // when having offset
        Vector3 dPos = target.position + position;
        Vector3 dRot = new Vector3(target.rotation.x + rotation.x, target.rotation.y + rotation.y, target.rotation.z + rotation.z);

        //Vector3 sPos = Vector3.Lerp(transform.position, dPos, speed * Time.deltaTime);
        //set pos and rot of camera
        transform.position = dPos;
        transform.LookAt(look.position);
        transform.rotation = Quaternion.Euler(dRot);

    }
}
