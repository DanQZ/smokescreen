using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public GameManager GM;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Objective collected");
        GM.uncollectedObjectives--;
        Destroy(this.gameObject);
    }
}
