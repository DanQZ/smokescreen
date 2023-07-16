using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text playerStats;
    public Text gameStats;
    public GameObject playerPrefab;
    public GameObject objectivePrefab;
    public GameObject wallPrefab;
    public GameObject wallsParent;
    public CameraMotor mainCameraMotor;
    public int mapSizeX = 100;
    public int mapSizeY = 100;
    public Player currentPlayer;
    public GameObject fireObject;
    public GameObject sledgehammer;

    public GameObject building1;
    public List<Wall> allWalls = new List<Wall>();
    public Wall[,] mapGrid;
    List<GameObject> allObjectives = new List<GameObject>();
    public int uncollectedObjectives;


    bool gameStarted;

    void Awake()
    {
        gameStarted = false;
        mainCameraMotor.lookAt = this.gameObject.transform;
        mapGrid = new Wall[mapSizeX, mapSizeY];
    }
    void Start()
    {
        transform.position = new Vector3(0f, 0f, 0f);
        StartGame();
    }

    void StartGame()
    {
        uncollectedObjectives = 0;
        GameObject newPlayer = SpawnPlayer();
        currentPlayer = newPlayer.GetComponent<Player>();
        mainCameraMotor.lookAt = newPlayer.gameObject.transform;

        SpawnPremadeMap();

        //BuildMap();

        StartFire();
        SpawnObjective(currentPlayer.gameObject.transform.position + transform.up * 5f + transform.right * 5f);
        gameStarted = true;
    }
    void EndGame()
    {
        Destroy(currentPlayer.gameObject);
        mainCameraMotor.lookAt = this.gameObject.transform;
        gameStarted = false;
        foreach (Wall item in allWalls)
        {
            Destroy(item.gameObject);
        }
        allWalls.Clear();
        foreach (GameObject item in allObjectives)
        {
            Destroy(item);
        }
        allObjectives.Clear();
        gameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            UpdatePlayerStatText();
            UpdateGameStatsText();
        }
    }

    void CheckWinConditions()
    {

    }

    void UpdatePlayerStatText()
    {
        if (currentPlayer == null)
        {
            Debug.Log("Error: bullshit");
            return;
        }

        playerStats.text = $"Player Stats\n"
        + $"HP: {Mathf.Floor(currentPlayer.hp)}/{currentPlayer.hpMax}\n"
        + $"Air: {Mathf.Floor(currentPlayer.air)}/{currentPlayer.airMax}\n"
        + $"Stamina: {Mathf.Floor(currentPlayer.stamina)}/{currentPlayer.staminaMax}\n"
        ;
        if (!currentPlayer.canSprint)
        {
            playerStats.text += "Exhausted!";
        }
    }

    void UpdateGameStatsText()
    {
        gameStats.text = $"Current Game:\n"
        + $"Objectives Left: {uncollectedObjectives}\n"
        ;
        if (uncollectedObjectives <= 0)
        {
            gameStats.text += "Exit building ASAP";
        }
    }

    GameObject SpawnPlayer()
    {
        GameObject newPlayer = Instantiate(
            playerPrefab,
            transform.position + 3f * Vector3.up + 3f * Vector3.right,
            transform.rotation
            );
        newPlayer.GetComponent<Player>().GM = this;
        return newPlayer;
    }

    GameObject SpawnObjective(Vector3 position)
    {
        GameObject newObjective = Instantiate(objectivePrefab, position, transform.rotation);
        newObjective.GetComponent<Objective>().GM = this;
        uncollectedObjectives++;
        return newObjective;
    }

    void SpawnPremadeMap()
    {
        GameObject newMap = Instantiate(building1, transform.position, transform.rotation);
        foreach (Transform g in newMap.transform.GetComponentsInChildren<Transform>())
        {
            Wall checkedWall = g.gameObject.GetComponent<Wall>();
            if (checkedWall != null)
            {
                checkedWall.GM = this;
                checkedWall.UpdateWallsAroundMe();
                allWalls.Add(checkedWall);
            }
        }
    }

    void BuildMap()
    {
        Vector3 bottomLeft = new Vector3(0f, 0f, 0f);
        Vector3 bottomRight = new Vector3(10f, 0f, 0f);
        Vector3 topLeft = new Vector3(0f, 10f, 0f);
        Vector3 topRight = new Vector3(10f, 10f, 0f);


        BuildWalls(topLeft, Vector3.right, 10);
        BuildWalls(topRight, -1f * Vector3.up, 10);
        BuildWalls(bottomRight, -1f * Vector3.right, 10);
        BuildWalls(bottomLeft, Vector3.up, 10);
    }

    void StartFire()
    {
        allWalls[0].SetOnFire();
        allWalls[1].SetOnFire();
    }

    // no negative numbers
    void BuildWall(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            Debug.Log($"({x},{y}) is out of bounds");
            return;
        }
        Vector3 position = new Vector3((float)x, (float)y, 0f);
        GameObject newWall = Instantiate(wallPrefab, position, transform.rotation);
        newWall.transform.parent = wallsParent.transform;

        Wall newWallScript = newWall.GetComponent<Wall>();
        newWallScript.GM = this;
        if (mapGrid[x, y] != null)
        {
            Destroy(mapGrid[x, y].gameObject);
        }
        mapGrid[x, y] = newWallScript;

        //Debug.Log($"x:{x}, y:{y}");

        newWallScript.coordinates[0] = x;
        newWallScript.coordinates[1] = y;
        allWalls.Add(newWallScript);
        newWallScript.UpdateWallsAroundMe();
    }
    void BuildWalls(Vector3 startPos, Vector3 direction, int length)
    {
        Vector3 curPos = new Vector3(Mathf.Floor(startPos.x), Mathf.Floor(startPos.y), 0f);
        Vector3 change = Vector3.Normalize(direction);
        for (int i = 0; i < length; i++)
        {
            BuildWall((int)curPos.x, (int)curPos.y);
            curPos += change;
        }
    }
    public void CheckIfBuildingCollapse()
    {
        int wallsDestroyed = 0;
        foreach (Wall checkedWall in allWalls)
        {
            if (checkedWall.isDestroyed)
            {
                wallsDestroyed++;
            }
        }
        // if % of all walls are destroyed, collapse
        if ((float)wallsDestroyed > (float)allWalls.Count * 0.5f)
        {
            foreach (Wall checkedWall in allWalls)
            {
                if (!checkedWall.isDestroyed)
                {
                    checkedWall.BeDestroyed(false);
                }
            }
            if (currentPlayer.isInside)
            { // player immdiately dies if inside collapsing building
                currentPlayer.Die();
            }
        }
    }
    public void GameOver()
    {
        mainCameraMotor.lookAt = transform;
        if (uncollectedObjectives <= 0 && !currentPlayer.isInside)
        {
            Debug.Log("Mission success");
        }
        else
        {
            Debug.Log("Mission failed");
        }
        EndGame();
    }
}
