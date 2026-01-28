using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public GameObject coin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
   
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.position;
        transform.eulerAngles += Vector3.up * Input.GetAxis("Mouse X") + Vector3.right * Input.GetAxis("Mouse Y");
        float rotX = transform.eulerAngles.x;
        if (rotX > 180)
            rotX -= 360;
        rotX = Mathf.Clamp(rotX, -30, 30);
        transform.eulerAngles = new Vector3(rotX, transform.eulerAngles.y, transform.eulerAngles.z);
        
    }
}
