using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator animator;
    public Animator sword_animator;
    public AudioSource slash;
    public AudioSource death;
    public AudioSource bgm;
    public Rigidbody2D playerbody;
    static public bool death_once = false;
    static public Vector3 start_point;
    static public RigidbodyConstraints2D start_constraints;
    static public UnityEngine.Quaternion start_rotation;
    //public AudioSource jumpsound;

    public float runspeed = 20F;

    float horizontalmove = 0f;

    bool jump = false;

    public static bool attacking = false;

    public Collider2D sword_hitbox;

    private void Awake()
    {
        start_point = playerbody.transform.position;
        start_constraints = playerbody.constraints;
        start_rotation = playerbody.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalmove = Input.GetAxisRaw("Horizontal") * runspeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalmove));
        

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("Is_Jump", true);

        }
        if (Input.GetButtonDown("Fire1") && attacking == false && death_once == false)
        {
            sword_hitbox.enabled = true;
            Debug.Log(sword_hitbox.enabled);
            attacking = true;
            Invoke("returnhitbox", 0.4f);
            animator.SetBool("Attacking", true);
            sword_animator.SetBool("sword_slashing", true);
            slash.Play();


            Invoke("attack_revoke", 0.34f);
        }
    }

    void returnhitbox()
    {
        sword_hitbox.enabled = !sword_hitbox.enabled;
    }
    public void Onlanding()
    {
        animator.SetBool("Is_Jump", false);
        //CharacterController2D.double_jump = 2;

        //Debug.Log(CharacterController2D.double_jump);
    }

    private void attack_revoke()
    {
        attacking = false;
        animator.SetBool("Attacking", false);
        sword_animator.SetBool("sword_slashing", false);
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalmove * Time.fixedDeltaTime, false, jump);

        jump = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        //Debug.Log(PlayerMovement.attacking);

        if (other.gameObject.CompareTag("traps"))
        {
            Debug.Log(playerbody.transform.position);
            Debug.Log(start_point);
            playerbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            animator.SetBool("Dead", true);
            if(death_once == false)
            {
                death.Play();
                bgm.Stop();
                death_once = true;
                CharacterController2D.heart1.SetBool("Lost Heart", true);
                CharacterController2D.heart2.SetBool("Lost Heart", true);
                CharacterController2D.heart3.SetBool("Lost Heart", true);
                CharacterController2D.continuelives--;

                if (CharacterController2D.continuelives == -1)
                {
                    //Game Over
                    Invoke("GameOver", 2f);
                }
                else
                {
                    CharacterController2D.lives_text.text = "x" + " " + CharacterController2D.continuelives;

                    Invoke("restart", 2f);
                }
            }

            
            //PlayerMovement.attacking
            //Destroy(other.gameObject);
            //coin += 1;

            //coinget.Play();

            //cointext.text = "x" + " " + coin;

            //Debug.Log("I am dead");
        }
    }

    public void GameOver()
    {
        playerbody.constraints = PlayerMovement.start_constraints;
        playerbody.transform.rotation = PlayerMovement.start_rotation;
        SceneManager.LoadScene("Scenes/GameOver");
    }

    void restart()
    {
        animator.SetBool("Dead", false);
        playerbody.transform.position = start_point;
        playerbody.constraints = start_constraints;
        playerbody.transform.rotation = start_rotation;
        death_once = false;
        CharacterController2D.Lives = 3;
        CharacterController2D.heart1.SetBool("Lost Heart", false);
        CharacterController2D.heart2.SetBool("Lost Heart", false);
        CharacterController2D.heart3.SetBool("Lost Heart", false);
        bgm.Play();
    }
}
