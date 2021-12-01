using UnityEngine;

public class WeaponPickup : Pickup
{
    #region Fields
    [Header("Weapon Pickup")]

    // The prefab must have a Weapon script (or derived from Weapon).
    [SerializeField, Tooltip("The Prefab of the weapon that this pickup should spawn.")]
    private Weapon weaponPickup;

    // Holds reference to the Transform of the visuals created st Start().
    private Transform visuals;
    #endregion Fields


    #region Unity Methods
    // Start is called before the first frame update
    public override void Start()
    {
        // Instantiate the weaponPickup for visuals of the pickup. Set its parent and location/rotation.
        visuals = Instantiate(weaponPickup).transform;
        visuals.parent = tf;
        visuals.position = tf.position;
        visuals.rotation = tf.rotation;

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {


        base.Update();
    }
    #endregion Unity Methods


    #region Dev Methods
    public override void OnPickup(PlayerData player)
    {
        player.EquipWeapon(weaponPickup);

        base.OnPickup(player);
    }
    #endregion Dev Methods
}
