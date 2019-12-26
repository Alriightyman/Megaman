using UnityEngine;
using System.Collections;

public class Pipi : MonoBehaviour 
{
	#region Variables

	// Editor variables
	[SerializeField] private float speed = 10.0f;
	[SerializeField] private float dropDistance = 10f;
	// Private Instance Variables
	private Egg egg;
	private bool moving = false;
	private bool attacking = false;
	private float lifeSpan = 5.0f;
	private float lifeTimer;
	private float damage = 20.0f;

	#endregion


	#region MonoBehaviour

	// Constructor 
	protected void Awake ()
	{
		egg = gameObject.GetComponentInChildren<Egg>();
	}

	// Update is called once per frame
	protected void Update () 
	{
		if (moving == true)
		{
			transform.position += (-Vector3.right * speed * Time.deltaTime);
			
			if (Time.time - lifeTimer >= lifeSpan)
			{
				Destroy (gameObject);	
			}
		}
		
		if (attacking == true)
		{
			if (Mathf.Abs(GameEngine.Player.transform.position.x - transform.position.x) <= dropDistance)
			{
				egg.ReleaseEgg(speed);
				attacking = false;
			}
		}
	}
	
	//
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
		}
	}

	#endregion


	#region Public Functions
	
	//
	public void TakeDamage(float dam)
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
		Destroy (gameObject);
	}
	
	// 
	public void Attack()
	{
		lifeTimer = Time.time;
		moving = true;
		attacking = true;
	}

	//
	public void Reset()
	{

	}
	
	#endregion
}
