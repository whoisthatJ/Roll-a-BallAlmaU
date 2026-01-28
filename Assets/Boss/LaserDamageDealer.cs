using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamageDealer : MonoBehaviour
{
    public bool isDamaging;
    private void OnTriggerStay(Collider other)
    {
        if (isDamaging && other.gameObject.tag == "Player")
            other.transform.localScale = other.transform.localScale * 0.99f;
    }
}
