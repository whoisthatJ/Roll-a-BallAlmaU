using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    public float speed = 45f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, speed) * Time.deltaTime);
    }
}
