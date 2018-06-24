using UnityEngine;
using System.Collections;

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
