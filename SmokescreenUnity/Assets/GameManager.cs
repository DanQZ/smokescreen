using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float FPS = 60f;
    public GameObject MainMenuUI;
    public GameObject InGameUI;
    public GameObject GameOverUI;
    public Text playerStats;
    public Text gameStats;
    public Text gameOverText;
    public GameObject playerPrefab;
    public GameObject pickupPrefab;
    public GameObject wallPrefab;
    public GameObject wallsParent;
    public CameraMotor mainCameraMotor;
    public int mapSizeX = 100;
    public int mapSizeY = 100;
    public Player currentPlayer;
    public GameObject fireObject;
    public GameObject sledgehammer;

    public GameObject building1;
    public GameObject building2;
    public GameObject building3;
    public List<Wall> allWalls = new List<Wall>();
    public Wall[,] mapGrid;
    List<GameObject> allObjectives = new List<GameObject>();
    public int primaryObjectivesLeft;
    public int secondaryObjectivesLeft;

    public AudioSource deathSound;
    bool gameStarted;

    void Awake()
    {
        Application.targetFrameRate = 60; // sets frame rate to 60fps
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above
        gameStarted = false;
        mainCameraMotor.lookAt = this.gameObject.transform;
        mapGrid = new Wall[mapSizeX, mapSizeY];
    }
    void Start()
    {
        transform.position = new Vector3(0f, 0f, 0f);
        InGameUI.SetActive(false);
        GameOverUI.SetActive(false);
    }

    public void StartGame()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(true);
        primaryObjectivesLeft = 0;
        GameObject newPlayer = SpawnPlayer();
        currentPlayer = newPlayer.GetComponent<Player>();
        mainCameraMotor.lookAt = newPlayer.gameObject.transform;

        SpawnPremadeMap();

        //BuildMap();


        StartFire();

        Vector3 objectivePos = currentPlayer.gameObject.transform.position + transform.up * 8f + transform.right * 5f;
        SpawnObjective(objectivePos, "primary");
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

    public void GoToMainMenu()
    {

    }

    float timer = 0f;
    void Update()
    {
        if (gameStarted)
        {
            UpdatePlayerStatText();
            UpdateGameStatsText();
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = timer % 1f;
                RandomFireChance();
            }
        }
    }

    void RandomFireChance()
    {
        float random = Random.Range(0f, 1f);
        if (random < 0.02f) // % chance per sec
        {
            int random2 = (int)Random.Range(0f, (float)(allWalls.Count - 0.01f));
            allWalls[random2].SetOnFire();
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
        + $"Primary Objectives Left: {primaryObjectivesLeft}\n"
        + $"Secondary Objectives Left: {secondaryObjectivesLeft}\n";
        if (primaryObjectivesLeft <= 0)
        {
            gameStats.text += "Primary Objectives completed\nExit building ASAP";
        }
    }

    GameObject SpawnPlayer()
    {
        GameObject newPlayer = Instantiate(
            playerPrefab,
            new Vector3(-2f, -2f, 0f),
            transform.rotation
            );
        newPlayer.GetComponent<Player>().GM = this;
        return newPlayer;
    }

    GameObject SpawnObjective(Vector3 position, string objectiveType)
    {
        GameObject newObjective = Instantiate(pickupPrefab, position, transform.rotation);
        Objective obScript = newObjective.GetComponent<Objective>();
        obScript.GM = this;
        obScript.type = objectiveType;
        primaryObjectivesLeft++;
        return newObjective;
    }

    void SpawnPremadeMap()
    {
        GameObject newMap = null;
        int random = (int)Random.Range(0f, 2.99f);
        switch (random)
        {
            default:
                Debug.Log("error map picking");
                break;
            case 0:
                newMap = Instantiate(building1, transform.position, transform.rotation);
                break;
            case 1:
                newMap = Instantiate(building2, transform.position, transform.rotation);
                break;
            case 2:
                newMap = Instantiate(building3, transform.position, transform.rotation);
                break;
        }
        MapScript newMapScript = newMap.GetComponent<MapScript>();
        newMapScript.currentPlayer = currentPlayer;
        newMapScript.GM = this;

        CheckAndMovePremadeMap(newMap);
    }

    void CheckAndMovePremadeMap(GameObject map)
    {
        Wall bottomLeftWall = null;
        Vector3 bottomLeft = new Vector3(-10f, -10f, 0f);
        float leastDistance = 999f;

        // adds all walls to list, correctly moves map relative to world center
        // also finds all pickups in case there are custom objectives
        // removes any MapScript.cs accidentally left in as children
        foreach (Transform g in map.transform.GetComponentsInChildren<Transform>())
        {
            Wall checkedWall = g.gameObject.GetComponent<Wall>();
            if (checkedWall != null)
            {
                checkedWall.GM = this;
                checkedWall.UpdateWallsAroundMe();
                allWalls.Add(checkedWall);
                if (bottomLeftWall == null)
                {
                    bottomLeftWall = checkedWall;
                    continue;
                }
                float checkedDistance = Vector3.Distance(checkedWall.transform.position, bottomLeft);
                if (checkedDistance < leastDistance)
                {
                    bottomLeftWall = checkedWall;
                    leastDistance = checkedDistance;
                }
                checkedWall.gameObject.layer = LayerMask.NameToLayer("Wall");
            }
            Objective checkedObjective = g.gameObject.GetComponent<Objective>();
            if (checkedObjective != null)
            {
                checkedObjective.GM = this;
                switch (checkedObjective.type)
                {
                    default:
                        break;
                    case "primary":
                        primaryObjectivesLeft++;
                        break;
                    case "secondary":
                        secondaryObjectivesLeft++;
                        break;
                }
            }

            // for accidental leave ins
            if (g.gameObject == map)
            {
                continue;
            }
            MapScript checkMapScript = g.gameObject.GetComponent<MapScript>();
            if (checkMapScript != null)
            {
                Destroy(checkMapScript);
            }
            PolygonCollider2D checkCollider = g.gameObject.GetComponent<PolygonCollider2D>();
            if (checkCollider != null)
            {
                Destroy(checkCollider);
            }
        }
        Vector3 bottomLeftTo000 = Vector3.zero - bottomLeftWall.transform.position;
        map.transform.position += bottomLeftTo000;
    }

    void StartFire()
    {
        int wallIndex = (int)Random.Range(0f, (float)allWalls.Count - 0.01f);
        allWalls[wallIndex].secretImmune = false;
        allWalls[wallIndex].SetOnFire();
        foreach (Wall nearbyWall in allWalls[wallIndex].allNearbyWalls)
        {
            nearbyWall.SetOnFire();
        }
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
            CollapseBuilding();
        }
    }

    public AudioSource collapseSound;
    public void CollapseBuilding()
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
        collapseSound.Play();
        Invoke("GameOver", 2f);
    }
    public void GameOver()
    {
        InGameUI.SetActive(false);
        GameOverUI.SetActive(true);
        mainCameraMotor.lookAt = transform;
        if (primaryObjectivesLeft <= 0 && currentPlayer.hp > 0f)
        {
            gameOverText.text = "Mission Success!";
        }
        else
        {
            gameOverText.text = "Mission Failed!";
        }
        EndGame();
    }
    public void PlayDeathSound()
    {
        deathSound.Play();
    }
}
