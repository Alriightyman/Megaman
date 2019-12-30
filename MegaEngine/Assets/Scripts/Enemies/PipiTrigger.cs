using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;

public class PipiTrigger : MonoBehaviour//, IResetable
{
    #region MonoBehaviour

    protected void Awake()
    {
    }
    // Called when the Collider other enters the trigger.
    protected void OnTriggerEnter2D(Collider2D other )
	{
		if (other.tag == "Player")
		{
			transform.parent.gameObject.GetComponent<Pipi>().Attack();
			gameObject.GetComponent<Collider2D>().enabled = false;            
		}
	}

    #endregion
}
