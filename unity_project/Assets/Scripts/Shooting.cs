using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] protected GameObject shotPrefab;
	
	// Properties
	public bool CanShoot 	{ get; set; }
	public bool IsShooting 	{ get; set; }
	
	// Protected Instance Variables
	protected Vector3 shotPos;
	protected float shotSpeed = 20f;
	protected float delayBetweenShots = 0.2f;
	protected float shootingTimer;

	#endregion
	
	
	#region MonoBehaviour

	// Use this for initialization 
	protected void Start()
	{
		CanShoot = true;
		IsShooting = false;
	}

	// Update is called once per frame 
	protected void Update()
	{
		if (IsShooting == true)
		{
			if (Time.time - shootingTimer >= delayBetweenShots)
			{
				IsShooting = false;	
			}
		}
	}

	#endregion
	
	
	#region Public Functions

	//
	public void Reset()
	{
		CanShoot = true;
		IsShooting = false;
	}
	
	//
	public void Shoot(bool isTurningLeft)
	{
		IsShooting = true;
		shootingTimer = Time.time;
		shotPos = transform.position + transform.right * ((isTurningLeft == true) ? -1.6f : 1.6f);
		
		GameObject rocketObj = Instantiate(shotPrefab, shotPos, transform.rotation);
        Rigidbody2D rocketRBody = rocketObj.GetComponent<Rigidbody2D>();		
		Physics2D.IgnoreCollision(rocketRBody.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		
		Shot s = rocketRBody.GetComponent<Shot>();
		s.VelocityDirection = (isTurningLeft == true) ? -transform.right : transform.right;
		s.ShotSpeed = shotSpeed;
	}

	#endregion

}

