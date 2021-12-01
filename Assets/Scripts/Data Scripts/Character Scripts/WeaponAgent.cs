using UnityEngine;

/* WeaponAgent is the parent for both PlayerData and EnemyData, so both can use weapons.
 * Therefore, WeaponAgent holds a lot of data variables that both players and enemies share.*/

public abstract class WeaponAgent : MonoBehaviour
{
    #region Fields
    [Header("Weapon")]

    [Tooltip("The weapon that this character has equipped.")]
    public Weapon equippedWeapon;


    [Header("Health")]

    // The player must have Health, and enemies MAY have Health.
    [Tooltip("The Health script attached to this character.")]
    public Health health;


    [Header("Speeds")]

    // The maximum movement speed of this character.
    [Tooltip("Max movement speed of this character.")]
    public float maxMoveSpeed = 8.0f;

    // The speed at which this character can turn their body.
    [Tooltip("Speed that this character can turn.")]
    public float turnSpeed = 90.0f;


    [Header("Animation Assistance")]

    [SerializeField, Tooltip("The name (as a string) of the anim int used for weapon stances.")]
    private string stanceParameter = "WeaponStance";


    // Only applicable for Players, but this way is accessible for the HumanoidPawn.
    [Header("Stamina")]

    // Whether or not the Player is currently sprinting.
    public bool isSprinting = false;


    [Header("Object & Component References")]

    [SerializeField, Tooltip("The Transform of this gameObject.")]
    protected Transform tf;

    [SerializeField, Tooltip("The tf of the WeaponContainer, where the weapon should be held.")]
    private Transform attachPoint;

    [Tooltip("The animator on this character.")]
    public Animator animator;

    [Tooltip("The Main Collider for this character.")]
    public Collider mainColl;

    [Tooltip("The Main Rigidbody for this character.")]
    public Rigidbody mainRB;

    // The OverheadCamera component on the camera that is following this Player.
    // Only applicable for Players, but this way is accessible for WeaponAgent typed vars (for HumanoidPawn).
    [Tooltip("Only applicable for Players, but accessible through WeaponAgent (base).")]
    public OverheadCamera overheadCam;

    #region Enum Definitions
    // Enum for weapon types, to help tell animator which weapon stance to use.
    public enum WeaponStance
    {
        Unarmed = 0,
        Rifle = 1,
        Handgun = 2
    }
    #endregion Enum Definitions
    #endregion Fields


    #region Unity Methods
    // Called immediately when the gameObject is instantiated.
    public virtual void Awake()
    {
        // If any of these are not set up, try to set them up.
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        // Set up these variables.
        if (mainColl == null)
        {
            mainColl = GetComponent<Collider>();
        }

        if (mainRB == null)
        {
            mainRB = GetComponent<Rigidbody>();
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        // If any of these are null, try to set them up.
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (tf == null)
        {
            tf = transform;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }
    #endregion Unity Methods


    #region Dev Methods
    // Creates a weapon from the Weapon prefab passed in and equips it.
    public virtual void EquipWeapon(Weapon weapon)
    {
        // Unequip any currently equipped weapon.
        UnequipWeapon();

        // Create the weapon as our equipped weapon.
        equippedWeapon = Instantiate(weapon);

        // Get the Transforms, then set its parent and location/rotation.
        Transform weapon_tf = equippedWeapon.transform;
        Transform prefab_tf = weapon.transform;
        weapon_tf.parent = attachPoint.transform;
        weapon_tf.localPosition = prefab_tf.localPosition;
        weapon_tf.localRotation = prefab_tf.localRotation;

        // Tell the animator which weapon stance to use.
        animator.SetInteger(stanceParameter, (int)equippedWeapon.weaponStance);

        // Set the layer of the weapon to that of this character.
        equippedWeapon.gameObject.layer = gameObject.layer;

        // Tell the weapon which Transform to match rotations with.
        equippedWeapon.rotationTarget = tf;

        // Tell the weapon that it has been equipped.
        equippedWeapon.OnEquip();
    }

    // Unequips the weapon, destroying it.
    public virtual void UnequipWeapon()
    {
        // If there is a weapon equipped,
        if (equippedWeapon != null)
        {
            // then tell the weapon it is being unequipped.
            equippedWeapon.OnUnequip();

            // Destroy the equipped weapon. Ensure the variable is now null.
            Destroy(equippedWeapon.gameObject);
            equippedWeapon = null;

            // Tell the animator to show characcter as unarmed.
            animator.SetInteger(stanceParameter, (int)WeaponStance.Unarmed);
        }
    }

    // Stop attacking with weapon immediately.
    public void StopAttack()
    {
        equippedWeapon.onAttackEnd.Invoke();
    }

    // Called via OnDeath event when the character dies.
    public virtual void HandleDeath()
    {
        // Stop attacking at once.
        StopAttack();
    }

    // Overridden by PlayerData.
    public virtual bool CanSprint()
    {
        // This should not be called at all. If it is, there is a problem. Log the error and return false.
        Debug.LogError("ERROR: CanSprint() should NOT be called on WeaponAgent superclass.");
        return false;
    }
    #endregion Dev Methods
}
