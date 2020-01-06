using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] private GameObject shotPrefab;
    [SerializeField] private Transform shotSpawnPoint;
    [SerializeField] private float shotSpeed = 20f;
    [SerializeField] private float delayBetweenShots = 0.2f;

    // private Instance Variables
    private Vector3 shotPos;
    private float shootingTimer;
    private int shotCount = 0;
    private const int MaxShots = 3;

    // Properties
    public bool CanShoot 	{ get; set; }
	public bool IsShooting 	{ get; set; }
	
	#endregion
	
	
	#region MonoBehaviour

	// Use this for initialization 
	private void Start()
	{
		CanShoot = true;
		IsShooting = false;
	}

	// Update is called once per frame 
	private void Update()
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

	/// <summary>
    /// Reset properties back to defaults
    /// </summary>
	public void SetToDefaults()
	{
		CanShoot = true;
		IsShooting = false;
	}
	
	/// <summary>
    /// Shoot projectile
    /// </summary>
    /// <param name="isTurningLeft"></param>
	public void Shoot(bool isTurningLeft)
	{
        // can only shoot whatever maxshots is
        if (shotCount < MaxShots)
        {
            // add shot count and set IsShooting to true
            shotCount++;
            IsShooting = true;
            // set shot timer
            shootingTimer = Time.time;

            // get the shot position
            shotPos = new Vector3(transform.position.x + ((isTurningLeft == true) ? shotSpawnPoint.localPosition.x : -shotSpawnPoint.localPosition.x),
                                    transform.position.y + shotSpawnPoint.localPosition.y,
                                    0f);

            GameObject projectile = Instantiate(shotPrefab, shotPos, transform.rotation);
            Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(projectileRB.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            // get the script from the projectile and set the velocity direction, speed and parent
            Shot s = projectileRB.GetComponent<Shot>();
            s.VelocityDirection = (isTurningLeft == true) ? -transform.right : transform.right;
            s.ShotSpeed = shotSpeed;
            projectile.GetComponent<Shot>().parent = this;

            // Play firing sound
            GameEngine.SoundManager.Play(AirmanLevelSounds.SHOOTING);
        }
	}

    /// <summary>
    /// Decrements a 'shot'; never goes below zero
    /// </summary>
    public void ShotDestroyed()
    {
        shotCount--;
        if(shotCount < 0)
        {
            shotCount = 0;
        }
    }
	#endregion

}

