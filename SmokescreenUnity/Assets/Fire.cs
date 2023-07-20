using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Player currentPlayer;
    // set in unity physics that this will ONLY collide with the player
    void Start()
    {
        transform.up = Vector3.up;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        currentPlayer.TakeDamage(2f * Time.deltaTime);
    }
}
