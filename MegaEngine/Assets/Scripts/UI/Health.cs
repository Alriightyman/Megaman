 using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	#region Variables
	
	// Unity Editor Variables
	public Image healthBar;
    
    // Public Properties
    public float MaximumHealth { get; set; }
	public float HurtingTimer { get; set; }
	public float HurtingDelay { get; set; }
	public bool IsHurting { get; set; }
	public bool IsDead { get; set; }
	public bool ShowHealthBar
    {
        get { return healthBar.transform.parent.gameObject.activeSelf; }
        set { healthBar.transform.parent.gameObject.SetActive(value); }
    }

    public float CurrentHealth
	{ 
		get
		{
			return currentHealth;
		} 
		set
		{ 
			if (value > MaximumHealth) { currentHealth = MaximumHealth; }
			else if (value < 0.0f) { currentHealth = 0.0f; }
			else if (value <= MaximumHealth && value >= 0.0f) { currentHealth = value; }
            healthBar.fillAmount = currentHealth / MaximumHealth;
		} 
	}

    // Protected Instance Variables
    private float tickAmount = 0.8f;
    [SerializeField] protected float startHealth = 100f;
	[SerializeField] protected float currentHealth = 100f;

	#endregion


	#region MonoBehaviour

	// Constructor
	protected void Awake ()
	{
        healthBar.fillAmount = startHealth / MaximumHealth;
        healthBar.transform.parent.gameObject.SetActive(false);
    }
	
	// Use this for initialization
	protected void Start ()
	{
		IsHurting = false;
        IsDead = false;
		MaximumHealth = 100.0f;
		HurtingDelay = 1.0f;
		
		currentHealth = startHealth;
        healthBar.fillAmount = startHealth / MaximumHealth;
    }
	#endregion
	
	
	#region Public Functions

	//
	public void Reset()
	{
		IsHurting = false;
		IsDead = false;
		MaximumHealth = 100.0f;
		HurtingDelay = 1.0f;
		
		currentHealth = startHealth;
        healthBar.fillAmount = startHealth / MaximumHealth;
    }
	
	// 
	public void ChangeHealth(float healthChange)
	{
        IsHurting = true;
        HurtingTimer = Time.time;
        currentHealth += healthChange * tickAmount;
        healthBar.fillAmount = currentHealth / MaximumHealth;

        if (currentHealth <= 0.0f)
        {
            IsDead = true;
        }
	}

	#endregion
}