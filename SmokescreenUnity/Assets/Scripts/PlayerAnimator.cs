using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator playerAnim;
    Rigidbody2D playerRB;
    Player playerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerRB = GetComponentInParent<Rigidbody2D>();
        playerScript = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerAnimations();
    }

    void PlayerAnimations()
    {
        if (playerRB.velocity.magnitude != 0)
        {
            playerAnim.SetBool("isWalking", true);
            playerAnim.SetFloat("moveX", playerScript.lookDirection.x);
            playerAnim.SetFloat("moveY", playerScript.lookDirection.y);
        }
        else if (playerRB.velocity.magnitude == 0)
        {
            playerAnim.SetBool("isWalking", false);
        }
    }
}
