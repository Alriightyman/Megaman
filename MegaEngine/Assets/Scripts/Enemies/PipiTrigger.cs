using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;

public class PipiTrigger : MonoBehaviour, IResetable
{
    #region MonoBehaviour

    protected void Awake()
    {
        GameEngine.GetResetableObjectList().Add(this);
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

    public void Reset()
    {
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
