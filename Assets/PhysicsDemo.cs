using UnityEngine;

public class PhysicsDemo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       GetComponent<Rigidbody>().AddForce(Vector3.right*20);
        GetComponent<Rigidbody>().AddTorque(Vector3.right * 10);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
