using UnityEngine;
using System.Collections;
using System;

public class Spike : MonoBehaviour 
{
	#region Variables

	// Protected Instance Variables
	protected const float DAMAGE_AMOUNT = 10.0f;

    #endregion


    #region MonoBehaviour
    // 
    protected void OnCollisionStay2D(Collision2D collision) 
	{

            InflictDamage(collision.gameObject);
	}

    // 
    protected void OnTriggerStay2D(Collider2D other) 
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


    #region Protected Functions

    // 
    protected void InflictDamage(GameObject objectHit)
	{
		if (objectHit.tag == "Player")
		{
			GameEngine.Player.TakeDamage (DAMAGE_AMOUNT);
		}
	}

	#endregion
}
