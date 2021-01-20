using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hit_box : MonoBehaviour
{

	public Collider2D sword_hitbox;
	public AudioSource slime_death_sound;

	private void Awake()
	{
		sword_hitbox.enabled = !sword_hitbox.enabled;
	}
	private void OnTriggerStay2D(Collider2D other)
	{

		Debug.Log(PlayerMovement.attacking);

		if (other.gameObject.CompareTag("enemy") && PlayerMovement.attacking == true)
		{

			//PlayerMovement.attacking
			
			other.enabled = false;
			other.gameObject.GetComponent<Animator>().SetBool("Dead", true);
			slime_death_sound.Play();
			other.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
			//Destroy(other.gameObject);
			//coin += 1;

			//coinget.Play();

			//cointext.text = "x" + " " + coin;

			Debug.Log("ded");
		}
	}




}
