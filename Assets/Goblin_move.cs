using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_move : MonoBehaviour
{
	public float speed;
	private bool MoveRight = true;
	public Collider2D goblinbody;
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;

	// Use this for initialization

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		// Use this for initialization
		if (goblinbody.enabled == true)
		{
			if (MoveRight)
			{
				//Vector3 targetVelocity = new Vector2(speed * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				//m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, 0);

				transform.Translate(2 * Time.deltaTime * speed, 0, 0);
				transform.localScale = new Vector2(1, 1);
			}
			else
			{
				transform.Translate(-2 * Time.deltaTime * speed, 0, 0);
				transform.localScale = new Vector2(-1, 1);
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Wall Hit");
		if (other.gameObject.CompareTag("turn"))
		{

			if (MoveRight)
			{
				MoveRight = false;
			}
			else
			{
				MoveRight = true;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log("Player Hit");
		if (other.gameObject.CompareTag("Player"))
		{

			if (MoveRight)
			{
				MoveRight = false;
			}
			else
			{
				MoveRight = true;
			}
		}
	}
}
