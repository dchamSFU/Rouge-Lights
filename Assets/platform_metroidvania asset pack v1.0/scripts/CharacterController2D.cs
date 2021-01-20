using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	static public int Lives = 3;
	//Animator[] lives_tracker;

	static public Text lives_text;
	static public int current_level = 1;

	//Audio
	public AudioSource jumpsound;
	public AudioSource footstep;
	public AudioSource coinget;
	public AudioSource damage_taken;
	public AudioSource heal;
	public AudioSource winsound;

	public Animator animator;

	public AudioSource death;
	public AudioSource bgm;
	public Rigidbody2D playerbody;
	static public bool death_once = false;
	//public AudioSource slash;
	//public Rigidbody2D playerbody;

	//MISC
	public static int coin = 0;
	public static int continuelives = 3;
	//public static int double_jump = 2;
	Text cointext;
	Text wintext;
	static public Animator heart1;
	static public Animator heart2;
	static public Animator heart3;

	 
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{

		wintext = GameObject.Find("Canvas/WinText").GetComponent<Text>();
		cointext = GameObject.Find("Canvas/Text").GetComponent<Text>();
		lives_text = GameObject.Find("Canvas/Lives_text").GetComponent<Text>();
		heart1 = GameObject.Find("Canvas/Heart1").GetComponent<Animator>();
		heart2 = GameObject.Find("Canvas/Heart2").GetComponent<Animator>();
		heart3 = GameObject.Find("Canvas/Heart3").GetComponent<Animator>();
		//current_level = 1;

		lives_text.text = "x" + " " + continuelives;

		//lives_tracker = { heart3, heart2, heart1};

		//lives_tracker[1] = heart1;
		//lives_tracker[2] = heart2;
		//lives_tracker[3] = heart3;

		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void Start()
	{
		PlayerMovement.attacking = false;
		lives_text.text = "x" + " " + continuelives;
		cointext.text = "x" + " " + coin;
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}


		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if ((m_Grounded && jump && PlayerMovement.death_once == false)) //|| (jump && double_jump > 0))
		{
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			// Add a vertical force to the player.
			jumpsound.Play();
			m_Grounded = false;
			/*
			if(double_jump == 1)
			{
				//m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			}
			else
			{
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				//m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
			}
			double_jump--;*/
			
		}

		if ((m_Grounded && (move >= 0.01 || move < 0) && footstep.isPlaying == false && PlayerMovement.death_once == false))
		{
			footstep.Play();
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("coin"))
		{

			Destroy(other.gameObject);
			coin += 1;

			coinget.Play();

			cointext.text = "x" + " " + coin; 

			Debug.Log(coin);
		}
		else if (other.gameObject.CompareTag("health_potion") && PlayerMovement.attacking == true)
		{
			if(Lives <= 2 && coin >= 20)
			{
				Destroy(other.gameObject);
				Lives++;
				Animator[] lives_tracker = { heart1, heart2, heart3 };
				lives_tracker[Lives - 1].SetBool("Lost Heart", false);
				heal.Play();
				coin -= 20;
				cointext.text = "x" + " " + coin;

			}
		}
		else if (other.gameObject.CompareTag("WinPoint") && bgm.isPlaying == true)
		{
			//Win
			Debug.Log("Current level" + current_level);
			bgm.Stop();
			winsound.Play();
			wintext.text = "Level Complete!";
			playerbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
			animator.SetBool("WinDoNothing", true);
			current_level++;
			Debug.Log("Current level" + current_level);
			Lives = 3;
			heart1.SetBool("Lost Heart", false);
			heart2.SetBool("Lost Heart", false);
			heart3.SetBool("Lost Heart", false);
			Invoke("NextLevelLoad", 2f);
		}

	}

	public void NextLevelLoad()
	{
		winsound.Stop();
		bgm.Play();
		SceneManager.LoadScene("Scenes/Level" + current_level);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("enemy"))
		{
			//Take Damage
			Animator[] lives_tracker = { heart1, heart2, heart3 };
			lives_tracker[Lives - 1].SetBool("Lost Heart", true);
			Lives--;
			//heart1.SetBool("Lost Heart", true);
			
			if(Lives == 0)
			{
				//Die
				playerbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
				animator.SetBool("Dead", true);
				if (death_once == false)
				{
					death.Play();
					bgm.Stop();
					death_once = true;
					continuelives--;
					
					if(continuelives == -1)
					{
						//Game Over
						Invoke("GameOver", 2f);
					}
					else
					{
						lives_text.text = "x" + " " + continuelives;

						Invoke("restart", 2f);
					}
				}
			}
			
			damage_taken.Play();

			animator.SetBool("Took_damage", true);

			if (m_FacingRight)
			{
				m_Rigidbody2D.AddForce(new Vector2(-100f, 0f));
			}
			else
			{
				m_Rigidbody2D.AddForce(new Vector2(100f, 0f));
			}

			Invoke("revoke_damage", 0.2f);


			//Debug.Log("poop");
		}
	}

	public void GameOver()
	{
		playerbody.constraints = PlayerMovement.start_constraints;
		playerbody.transform.rotation = PlayerMovement.start_rotation;
		SceneManager.LoadScene("Scenes/GameOver");
	}

	void revoke_damage()
	{
		animator.SetBool("Took_damage", false);
	}

	void restart()
	{
		animator.SetBool("Dead", false);
		//animator.SetBool("WinDoNothing", false);
		playerbody.transform.position = PlayerMovement.start_point;
		playerbody.constraints = PlayerMovement.start_constraints;
		playerbody.transform.rotation = PlayerMovement.start_rotation;
		death_once = false;
		Lives = 3;
		heart1.SetBool("Lost Heart", false);
		heart2.SetBool("Lost Heart", false);
		heart3.SetBool("Lost Heart", false);
		bgm.Play();
	}

}
