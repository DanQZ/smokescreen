using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SledgehammerScript : MonoBehaviour
{
    bool collided = false;
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collided)
        {
            return;
        }
        if (collision.gameObject.tag != "wall")
        {
            return;
        }

        collision.gameObject.GetComponent<Wall>().TakeDamage(20f);
        collided = true;
    }
}
