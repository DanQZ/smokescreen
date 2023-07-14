using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour
{
    public GameManager GM;
    public int[] coordinates = new int[2];
    float hp;
    float wetness;
    public SpriteRenderer currentSprite;
    public Sprite standingSprite;
    public Sprite destroyedSprite;
    public bool playerCanPass;
    public bool onFire;
    public GameObject fireGO;
    public List<Wall> allNearbyWalls = new List<Wall>();
    void Awake()
    {
        coordinates = new int[2];
        hp = 100f;
        wetness = 0f;
        playerCanPass = false;
        currentSprite.sprite = standingSprite;
    }
    void Start()
    {
        InvokeRepeating("Tick", 0, 1.0f);
    }

    void Tick()
    {
        if (onFire)
        {
            SpreadFire();
            hp--;
            if (hp <= 0f)
            {
                BeDestroyed();
            }
        }
    }
    void SpreadFire()
    {
        foreach (Wall checkWall in allNearbyWalls)
        {
            float chance = Random.Range(0f, 1f);
            if (chance < 0.01f)
            {
                checkWall.SetOnFire();
            }
        }
    }
    public void SetOnFire()
    {
        if (hp <= 0f || onFire)
        {
            return;
        }
        if (wetness > 0f)
        {
            wetness = Mathf.Max(0f, wetness - 10f);
        }
        Debug.Log("new wall set on fire");
        onFire = true;
        fireGO = Instantiate(GM.fireObject, transform.position, transform.rotation);
        fireGO.transform.parent = transform;
        fireGO.GetComponent<Fire>().currentPlayer = GM.currentPlayer;
    }
    void GetWet(float amount)
    {
        wetness += amount;
        if (wetness > hp && onFire)
        {
            Extinguish();
        }
    }
    void Extinguish()
    {
        if (!onFire)
        {
            return;
        }
        onFire = false;
        Destroy(fireGO);
    }
    public void TakeDamage(float amount)
    {
        hp -= Mathf.Abs(amount);
        if (hp <= 0f)
        {
            BeDestroyed();
        }
    }
    void BeDestroyed()
    {
        Extinguish();
        playerCanPass = true;
        GetComponent<BoxCollider2D>().enabled = false;
        currentSprite.sprite = destroyedSprite;
    }

    public void UpdateWallsAroundMe()
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
                checkWall.allNearbyWalls.Add(this);
            }
        }
    }
    void UpdateWallsAroundMeTest()
    {
        allNearbyWalls.Clear();
        int xMin = coordinates[0] - 1;
        int xMax = coordinates[0] + 1;
        int yMin = coordinates[1] - 1;
        int yMax = coordinates[1] + 1;

        if (xMin < 0)
        {
            xMin = 0;
        }
        if (xMax > GM.mapSizeX - 1)
        {
            xMax = GM.mapSizeX - 1;
        }

        if (yMin < 0)
        {
            yMin = 0;
        }
        if (yMax > GM.mapSizeY - 1)
        {
            yMax = GM.mapSizeY - 1;
        }

        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                allNearbyWalls.Add(GM.mapGrid[x, y]);
            }
        }
    }
}
