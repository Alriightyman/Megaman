using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] protected GameObject shotPrefab;
    [SerializeField] private Transform shotSpawnPoint;
    [SerializeField] protected float shotSpeed = 20f;
    [SerializeField] protected float delayBetweenShots = 0.2f;

    // Properties
    public bool CanShoot 	{ get; set; }
	public bool IsShooting 	{ get; set; }
	
	// Protected Instance Variables
	protected Vector3 shotPos;
	protected float shootingTimer;
    private int shotCount = 0;
    private const int MaxShots = 3;
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
        if (shotCount < MaxShots)
        {
            shotCount++;
            IsShooting = true;

            shootingTimer = Time.time;
            shotPos = new Vector3(transform.position.x + ((isTurningLeft == true) ? shotSpawnPoint.localPosition.x : -shotSpawnPoint.localPosition.x),
                                    transform.position.y + shotSpawnPoint.localPosition.y,
                                    0f);

            GameObject rocketObj = Instantiate(shotPrefab, shotPos, transform.rotation);
            Rigidbody2D rocketRBody = rocketObj.GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(rocketRBody.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            Shot s = rocketRBody.GetComponent<Shot>();
            s.VelocityDirection = (isTurningLeft == true) ? -transform.right : transform.right;
            s.ShotSpeed = shotSpeed;
            rocketObj.GetComponent<Shot>().parent = this;

            GameEngine.SoundManager.Play(AirmanLevelSounds.SHOOTING);
        }
	}

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

