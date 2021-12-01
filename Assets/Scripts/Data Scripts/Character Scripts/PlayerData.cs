using System;
using UnityEngine;
using UnityEngine.UI;

// The PlayerData holds data about the player that can be accessed from other scripts.
// This way, all data specific to this player is in one spot.
// Derives from WeaponAgent to have ability to use weapons.

// Theese scripts are required.
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Player_InputController))]

public class PlayerData : WeaponAgent
{
    #region Fields
    // Stamina is for players only, not enemies.
    [Header("Stamina")]

    // The player's maximum stamina.
    [SerializeField] private float maxStamina = 100.0f;

    // The player's curent stamina.
    private float currentStamina;

    // The rate at which stamina drops when sprinting (per second).
    [SerializeField] private float staminaUseRate = 10.0f;

    // The number of seconds that player must wait to start regaini9ng stamina once they stop sprinting.
    [SerializeField] private float staminaRecoveryDelay = 1.0f;

    // The number of seconds since the player stopped sprinting (until staminaRecoverDelay time is reached).
    private float timeStaminaRecoveryDelayed = 0.0f;

    // The amount of stamina to be recovered (per second) when recovering stamina.
    [SerializeField] private float staminaRecoveryRate = 20.0f;


    [Header("Death Settings")]

    [SerializeField, Tooltip("How long after death before the Player's body gameObject is destroyed." +
        " This is separate and independent from the time before Player is respawned.")]
    private float bodyDestructionDelay = 4.0f;


    [Header("Object & Component references")]

    [SerializeField, Tooltip("The Slider for the Stamina Bar on the UI HUD.")]
    private Slider staminaBar;

    [SerializeField, Tooltip("The Weapon the player starts with. Leave blank to start unarmed.")]
    private Weapon defaultWeapon;
    #endregion Fields

    #region Unity Methods
    // Called immediately when the gameObject is instantiated.
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        // If there is a default weapon assigned,
        if (defaultWeapon != null)
        {
            // then equip that weapon.
            EquipWeapon(defaultWeapon);
        }

        // Register the player's health bar.
        UIManager.Instance.RegisterPlayer(health);

        // Initialize currentStamina to equal maxStamina.
        currentStamina = maxStamina;

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        // If sprinting,
        if (isSprinting)
        {
            // then use stamina.
            UseStamina();
        }

        // Recover stamina (as appropriate).
        RecoverStamina();

        base.Update();
    }
    #endregion Unity Methods

    #region Dev Methods
    // Returns bool for whether or not the character can sprint at the moment.
    public override bool CanSprint()
    {
        // If there is stamina left,
        if (currentStamina > 0)
        {
            // and if the player is not already sprinting,
            if (!isSprinting)
            {
                // then the player can sprint. Player is now sprinting.
                isSprinting = true;
                // Reset the time since the player started the stamina recovery delay.
                timeStaminaRecoveryDelayed = 0.0f;
            }

            // Player will not be sprinting. Use stamina.
            UseStamina();
            // Return true. The player can sprint.
            return true;
        }
        // Else, they cannot sprint.
        else
        {
            isSprinting = false;
            return false;
        }
    }

    // Called each frame that the character is sprinting to lower the stamina.
    public void UseStamina()
    {
        // Set the current stamina equal to itself myself staminaUseRate / second, minimum of 0.
        currentStamina -= staminaUseRate * Time.deltaTime;

        if (currentStamina < 0)
        {
            currentStamina = 0;
        }

        // Update the stamina bar.
        UpdateStaminaBar();
    }

    // Called every frame. Determines if stamina should be recovered, and does so.
    private void RecoverStamina()
    {
        // If the player is not sprinting, and stamina is not at maximum,
        if (!isSprinting && currentStamina < maxStamina)
        {
            // and if the player's stamina recovery has been delayed enough,
            if (timeStaminaRecoveryDelayed >= staminaRecoveryDelay)
            {
                // then recover stamina, with a max of maxStamina.
                currentStamina += staminaRecoveryRate * Time.deltaTime;

                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }

                // Update the stamina bar.
                UpdateStaminaBar();
            }
            // Else, the player's stamina recovery has not yet been delayed enough.
            else
            {
                // Increase the time since 
                timeStaminaRecoveryDelayed += Time.deltaTime;
            }
        }
    }

    // Updates the stamina bar on the HUD.
    private void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina / maxStamina;
    }

    // Called to get the current stamina.
    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    // Call this via events when the player dies.
    public override void HandleDeath()
    {
        // Toggle the Ragdoll.
        GetComponent<HumanoidPawn>().ToggleRagdoll(true);
        // Remove the Monobehaviors this Player won't need anymore.
        Destroy(this);
        Destroy(GetComponent<Health>());
        Destroy(GetComponent<Player_InputController>());
        Destroy(GetComponent<HumanoidPawn>());

        // Destroy this gameObject after a delay. The delay must be positive.
        Destroy(gameObject, Math.Abs(bodyDestructionDelay));

        base.HandleDeath();
    }

    // Set up all the references needed for this Player from the GM.
    public void SetUpReferences()
    {
        // Set references to gameObjects as necessary.
        staminaBar = GameManager.Instance.staminaBar;
        overheadCam = GameManager.Instance.overheadCamera;

        // Tell the camera to get a new reference to the Player.
        overheadCam.FindPlayer();
    }

    // Overrides the WeaponAgent's EquipWeapon method.
    public override void EquipWeapon(Weapon weapon)
    {
        base.EquipWeapon(weapon);
        
        equippedWeapon.isEquippedByPlayer = true;

        UIManager.Instance.UpdateWeaponIcon(equippedWeapon.weaponIcon);
    }
    #endregion Dev Methods
}
