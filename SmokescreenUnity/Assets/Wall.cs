using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour
{
    public GameManager GM;
    float hp;
    public SpriteRenderer currentSprite;
    public Sprite standingSprite;
    public Sprite destroyedSprite;
    public bool playerCanPass;
    public bool onFire;
    public List<Wall> allNearbyWalls = new List<Wall>();
    void Start()
    {
        InvokeRepeating("Tick", 0, 1.0f);
        playerCanPass = false;
        currentSprite.sprite = standingSprite;
    }

    void Tick()
    {
        if (onFire)
        {

        }
    }
    void BeDestroyed()
    {
        playerCanPass = true;
        GetComponent<BoxCollider2D>().enabled = false;
        currentSprite.sprite = destroyedSprite;
    }

    void UpdateWallsAroundMe()
    {
        allNearbyWalls.Clear();
        foreach (Wall checkWall in GM.allWalls)
        {
            if (checkWall == this)
            {
                continue;
            }
            float xDiff = Mathf.Abs(checkWall.transform.position.x - transform.position.x);
            float yDiff = Mathf.Abs(checkWall.transform.position.y - transform.position.y);
            if (xDiff < 1.1f && yDiff < 1.1f)
            {
                allNearbyWalls.Add(checkWall);
            }
        }
    }
}
