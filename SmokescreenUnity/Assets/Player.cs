using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager GM;
    bool canSprint;
    bool canHammer;
    public float hp;
    float hpMax;
    public float air;
    float airMax;
    public float stamina;
    float staminaMax;
    public float water;
    float waterMax;
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
    }
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

        lookDirection = moveVector;
        playerRB.velocity = moveVector * speed;
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
        Instantiate(GM.sledgehammer, transform.position + lookDirection, transform.rotation);
        stamina -= 25f;
        StartCoroutine(SledgeCooldown());
    }
    IEnumerator SledgeCooldown()
    {
        canHammer = false;
        yield return new WaitForSeconds(1);
        canHammer = true;
    }
    public void TakeDamage(float amount)
    {
        hp -= Mathf.Abs(amount);
        if (hp <= 0f)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        hp = Mathf.Max(hpMax, hp + Mathf.Abs(amount));
    }
    void Die()
    {
        GM.GameOver();
        Destroy(this.gameObject);
    }
}
