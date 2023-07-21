using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager GM;
    public bool isInside;
    public bool canSprint;
    public bool canHammer;
    public float hp;
    public float hpMax;
    public float air;
    public float airMax;
    public float stamina;
    public float staminaMax;
    public float water;
    public float waterMax;
    float speed = 3f;
    public Rigidbody2D playerRB;
    public GameObject playerSprite;
    public Vector3 lookDirection;
    // Start is called before the first frame update
    void Start()
    {
        canSprint = true;
        canHammer = true;
        InitStats();
        InvokeRepeating("UpdateFireLoudness", 0f, 0.5f);
    }

    void InitStats()
    {
        hpMax = 100f;
        hp = hpMax;
        airMax = 100f;
        air = airMax;
        staminaMax = 100f;
        stamina = staminaMax;
        waterMax = 100f;
        water = waterMax;
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardControls();
        UpdateAir();
    }
    void UpdateAir()
    {
        air = Mathf.Max(0f, air - 0.2f * Time.deltaTime);
        if (air <= 0f)
        {
            air = 0f;
            hp -= 5f * Time.deltaTime;
        }
    }
    public AudioSource fireSound;
    void UpdateFireLoudness()
    {
        float updatedVolume = 0f;
        float closest = 999f;
        foreach (Wall checkedWall in GM.allWalls)
        {
            if (!checkedWall.onFire)
            {
                continue;
            }
            float dist = Vector3.Distance(transform.position, checkedWall.transform.position);
            if (dist < closest)
            {
                closest = dist;
            }
        }
        updatedVolume = 1f / closest;
        if (closest > 10f)
        {
            updatedVolume = 0.1f;
        }
        if (closest == 999f)
        {
            updatedVolume = 0f;
        }
        fireSound.volume = updatedVolume;
    }

    int nextWalkSound = 0;
    public AudioSource walkSound1;
    public AudioSource walkSound2;
    public AudioSource walkSound3;
    void KeyboardControls()
    {
        bool sprint = false;
        Vector3 moveVector = new Vector3(0f, 0f, 0f);
        if (Input.GetKey("w"))
        {
            moveVector += transform.up;
        }
        if (Input.GetKey("s"))
        {
            moveVector -= transform.up;
        }
        if (Input.GetKey("d"))
        {
            moveVector += transform.right;
        }
        if (Input.GetKey("a"))
        {
            moveVector -= transform.right;
        }
        if (Input.GetKey("space"))
        {
            SprayWater();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (canSprint)
            {
                sprint = true;
            }
        }
        if (Input.GetKey("f"))
        {
            if (stamina >= 25f && canHammer)
            {
                Sledgehammer();
            }
        }

        if (Vector3.Magnitude(moveVector) > 0.1f)
        {
            lookDirection = moveVector;
            PlayWalkSound();
        }

        moveVector = Vector3.Normalize(moveVector);
        // use stamina to sprint
        if (sprint && stamina > 0f)
        {
            stamina = (stamina - 10f * Time.deltaTime);
            moveVector *= 1.5f;
            if (stamina < 0f)
            {
                StartCoroutine(SprintCD());
            }
        }
        else
        {
            stamina = Mathf.Min(staminaMax, stamina + 5f * Time.deltaTime);
        }

        playerRB.velocity = moveVector * speed;
    }
    void PlayWalkSound()
    {
        if (Time.frameCount < nextWalkSound)
        {
            return;
        }
        nextWalkSound = Time.frameCount + 20;

        int random = (int)Random.Range(0f, 2.99f);
        switch (random)
        {
            case 0:
                walkSound1.Play();
                break;
            case 1:
                walkSound2.Play();
                break;
            case 2:
                walkSound3.Play();
                break;
        }
    }
    IEnumerator SprintCD()
    {
        if (!canSprint)
        {
            yield break;
        }

        canSprint = false;
        while (stamina != staminaMax)
        {
            yield return null;
        }
        canSprint = true;
    }
    void SprayWater()
    {
        Debug.Log("spraying water");
        water = Mathf.Max(0f, water - 1f * Time.deltaTime);
    }
    void Sledgehammer()
    {
        Debug.Log("sledgehammer");
        GameObject newHammer = Instantiate(GM.sledgehammer, transform.position + lookDirection, transform.rotation);
        stamina -= 25f;
        StartCoroutine(SledgeCooldown());
        newHammer.transform.parent = transform;
    }
    IEnumerator SledgeCooldown()
    {
        canHammer = false;
        yield return new WaitForSeconds(1);
        canHammer = true;
    }

    public AudioSource damage1;
    public AudioSource damage2;
    int nextHurtSound = 0;
    public void TakeDamage(float amount)
    {
        if (hp < 0f)
        {
            return;
        }

        if (Time.frameCount > nextHurtSound)
        {
            PlayHurtSound();
            nextHurtSound = Time.frameCount + 60;
        }

        hp -= Mathf.Abs(amount);
        if (hp <= 0f)
        {
            Die();
        }
    }
    void PlayHurtSound()
    {
        if (Random.Range(0f, 1f) < 0.5f)
        {
            damage1.Play();
        }
        else
        {
            damage2.Play();
        }

    }
    public void Heal(float amount)
    {
        hp = Mathf.Max(hpMax, hp + Mathf.Abs(amount));
    }

    public void Die()
    {
        GM.PlayDeathSound();
        hp = -1f;
        GM.GameOver();
        Destroy(this.gameObject);
    }
}
