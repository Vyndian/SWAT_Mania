using UnityEngine;

// Health pickup. Adds health to the player, immediately (heal).

public class FirstAid_White : Pickup
{
    #region Fields
    [SerializeField, Tooltip("The amount of health to be healed by the player on pickup.")]
    private float healValue = 20.0f;
    #endregion Fields


    #region Unity Methods
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    #endregion Unity Methods


    #region Dev Methods
    // Called when a player triggers the collider.
    public override void OnPickup(PlayerData player)
    {
        // Heal the player.
        player.health.Heal(healValue);

        // Call the base method. Destroys this gameObject.
        base.OnPickup(player);
    }
    #endregion Dev Methods
}
