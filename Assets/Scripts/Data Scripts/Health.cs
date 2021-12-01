using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    #region Fields
    [Header("Health")]

    [SerializeField, Tooltip("The object's maximum health")]
    private float maxHealth = 100;

    [SerializeField, Tooltip("The object's initial health. Object cannot start higher than maxHealth")]
    private float initialHealth = 100;

    // The object's current health. Initialized based on max and initial healths.
    public float currentHealth;


    [Header("Events")]

    [Tooltip("Raised every time the object is Damaged")]
    public UnityEvent onDamage;
    [Tooltip("Raised every time the object is healed")]
    public UnityEvent onHeal;
    [Tooltip("Raised once when the object's health reaches 0")]
    public UnityEvent onDie;


    [Header("Object & Component references")]

    [SerializeField, Tooltip("The PlayerData on the character this is attached to, if appropriate.")]
    private PlayerData playerData;

    [SerializeField, Tooltip("The AudioSource on this character")]
    private AudioSource audioSource;
    #endregion Fields


    #region Unity Methods
    // Called immediately when instantiated, before Start.
    private void Awake()
    {
        // Initialize the object's health.
        InitializeHealth();

        // Set this up.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If any of these are null, try to set them up.
        if (playerData == null)
        {
            // This will not work if the Health is on an object without a PlayerData on it. That's OK.
            playerData = GetComponent<PlayerData>();
        }
    }
    #endregion Unity Methods


    #region Dev Methods
    // Called once during Start to initialize the object's health.
    private void InitializeHealth()
    {
        // Get the higher of the initialHealth and 1.0f.
        float value = Mathf.Max(initialHealth, 1.0f);

        // Set the currentHealth to the value.
        SetCurrentHealth(value);
    }

    // Called to damage the object by the specified amount.
    public void Damage(float damage)
    {
        // Damage must be positive.
        damage = Mathf.Max(damage, 0.0f);

        // Determine new health, ensuring currentHealth stays between 0.0f and maxHealth.
        float newHealth = Mathf.Clamp((currentHealth - damage), 0.0f, maxHealth);
        
        // Apply the new health.
        SetCurrentHealth(newHealth);

        // Send a message to all other components of this gameObject to call OnDamage, if that method exists.
        SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);

        // Invoke OnDamage for this component.
        onDamage.Invoke();

        // If the object is dead,
        if (currentHealth == 0.0f)
        {
            // then send the OnDie message to other components.
            SendMessage("OnDie", SendMessageOptions.DontRequireReceiver);

            // Invoke OnDeath for this component.
            onDie.Invoke();
        }
    }

    // Convenience method to kill the object outright. Calls Damage() with maxHealth as the damage.
    public void Kill()
    {
        // Call Damage() with maxHealth as the damage to ensure death.
        Damage(maxHealth);
    }

    // Destroys the gameObject. Events cannot destroy without a function to call, only set inactive.
    public void DestroyObject(float delay)
    {
        Destroy(gameObject, delay);
    }

    // Called to heal the object's health by the specified amount.
    public void Heal(float healing)
    {
        // healing must be positive.
        healing = Mathf.Max(healing, 0.0f);

        // Determine new health, ensuring currentHealth stays between 0.0f and maxHealth.
        float newHealth = Mathf.Clamp((currentHealth + healing), 0.0f, maxHealth);

        // Apply the new health.
        SetCurrentHealth(newHealth);

        // Send a message to all other components of this gameObject to call OnHeal, if that method exists.
        SendMessage("OnHeal", SendMessageOptions.DontRequireReceiver);

        // Invoke OnHeal for this component.
        onHeal.Invoke();
    }

    // Play the Hurt sound.
    public void HurtSound()
    {
        audioSource.PlayOneShot(AudioManager.Instance.hurtSound);
    }


        #region Getters
    // Get the currentHealth.
    public float GetHealth()
    {
        return currentHealth;
    }

    // Get the object's current health percentage.
    public float GetHealthPercentage()
    {
        return (currentHealth / maxHealth);
    }
    #endregion Getters


        #region Setters
    // Sets the currentHealth.
    private void SetCurrentHealth(float newHealth)
    {
        // Ensure currentHealth does not go above maxHealth.
        currentHealth = Mathf.Min(newHealth, maxHealth);
        
    }
        #endregion Setters


    #endregion Dev Methods
}
