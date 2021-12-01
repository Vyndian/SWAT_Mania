using UnityEngine;

// Damage pickup. Takes health from player.

public class FirstAid_Biohazard : Pickup
{
    #region Fields
    [SerializeField, Tooltip("The amount of health to be taken from the player on pickup.")]
    private float damageValue = 20.0f;
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
        // Damages the player.
        player.health.Damage(damageValue);

        // Call the base method. Destroys this gameObject.
        base.OnPickup(player);
    }
    #endregion Dev Methods
}
