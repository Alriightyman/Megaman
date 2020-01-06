 using UnityEngine;
using UnityEngine.UI;
using Extensions;

public class Health : MonoBehaviour
{
	#region Variables
	
	// Unity Editor Variables
	[SerializeField] public Image healthBar;
    
    // Public Properties
    public float MaximumHealth { get; set; }
	public float HurtingTimer { get; set; }
	public float HurtingDelay { get; set; }
	public bool IsHurting { get; set; }
	public bool IsDead { get; set; }
    public bool IsFull { get { return currentHealth / MaximumHealth == 1f; } }
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

    // private Instance Variables
    [SerializeField] private float startHealth = 28f;
	[SerializeField] private float currentHealth = 28f;

	#endregion


	#region MonoBehaviour

	// Constructor
	private void Awake ()
	{
        healthBar.fillAmount = startHealth / MaximumHealth;
    }
	
	// Use this for initialization
	private void Start ()
	{
		IsHurting = false;
        IsDead = false;
		MaximumHealth = 28f;
		HurtingDelay = 1.0f;
		
		currentHealth = startHealth;
        healthBar.fillAmount = startHealth / MaximumHealth;
    }
	#endregion
	
	
	#region Public Functions

	//
	public void Restart()
	{
		IsHurting = false;
		IsDead = false;
		MaximumHealth = 28f;
		HurtingDelay = 1.0f;
		
		currentHealth = startHealth;
        healthBar.fillAmount = startHealth / MaximumHealth;
    }
	
	// 
	public void ChangeHealth(float healthChange)
	{
        IsHurting = true;
        HurtingTimer = Time.time;

        AddHealth(healthChange);

        if (currentHealth <= 0.0f)
        {
            IsDead = true;
        }
	}

    public void AddHealth(float amountToAdd)
    {
        currentHealth += amountToAdd;

        currentHealth = currentHealth < MaximumHealth ? currentHealth : MaximumHealth;
        
        healthBar.fillAmount = currentHealth / MaximumHealth;
    }

	#endregion
}