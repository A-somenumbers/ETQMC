using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D o)
    {
        if (o.gameObject.CompareTag("Player"))
        {
            pickup();
        }
    }
    void pickup()
    {
        Debug.Log("Item Picked Up!");
    }
}
