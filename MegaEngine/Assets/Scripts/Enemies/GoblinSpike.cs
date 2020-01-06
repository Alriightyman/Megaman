using UnityEngine;
using System.Collections;
using System;

public class GoblinSpike : MonoBehaviour 
{
	#region Variables

	// private Instance Variables
	private const float DAMAGE_AMOUNT = 2f;

    #endregion


    #region MonoBehaviour
    // 
    private void OnCollisionStay2D(Collision2D collision) 
	{

            InflictDamage(collision.gameObject);
	}

    // 
    private void OnTriggerStay2D(Collider2D other) 
	{
            InflictDamage(other.gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "shot")
        {
            var boxcollider = collision.gameObject.GetComponent<BoxCollider2D>();
            if (boxcollider != null)
            {
                boxcollider.enabled = false;
            }
            var shot = collision.gameObject.GetComponent<Shot>();
            var velocity = shot.VelocityDirection;
            shot.VelocityDirection = new Vector3(-velocity.x, Math.Abs(velocity.x), velocity.z);
            GameEngine.SoundManager.Play(AirmanLevelSounds.LANDING);

        }
    }

    #endregion


    #region private Functions

    // 
    private void InflictDamage(GameObject objectHit)
	{
		if (objectHit.tag == "Player")
		{
			GameEngine.Player.TakeDamage (DAMAGE_AMOUNT);
		}
	}

	#endregion
}
