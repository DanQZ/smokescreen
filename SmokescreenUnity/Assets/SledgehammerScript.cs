using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SledgehammerScript : MonoBehaviour
{
    public List<Wall> allWallsCollided = new List<Wall>();
    void Start()
    {
        Destroy(this.gameObject, 0.25f);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "wall")
        {
            return;
        }
        // dont hit the same wall twice
        foreach (Wall checkedWall in allWallsCollided)
        {
            if (checkedWall == collision.gameObject.GetComponent<Wall>())
            {
                return;
            }
        }

        Debug.Log("hit a wall");

        Wall collidedWall = collision.gameObject.GetComponent<Wall>();
        allWallsCollided.Add(collidedWall);
        collidedWall.TakeDamage(33f);
    }
}
