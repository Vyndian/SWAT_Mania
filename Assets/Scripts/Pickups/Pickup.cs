using UnityEngine;

// Pickup is to be abstract. Different pickup types will inherit from it.

public abstract class Pickup : MonoBehaviour
{
    #region Fields
    [Header("Spin")]

    [SerializeField, Tooltip("Whether the pickup should spin.")]
    private bool spinPickup = true;

    [SerializeField, Tooltip("The vector that the pickup will spin every second.")]
    private Vector3 spinVector = new Vector3(50, 35, 50);


    [Header("Decay")]

    [SerializeField, Tooltip("Whether the pickup should disappear after some time if not picked up.")]
    private bool willDecay = true;

    [SerializeField, Tooltip("The amount of time before the pickup disappears if not picked up.")]
    private float lifespan = 60.0f;


    [Header("Object & Component references")]

    [SerializeField, Tooltip("The Transform of this gameObject.")]
    protected Transform tf;
    #endregion Fields


    #region Unity Methods
    // Called as soon as the object is instantiated.
    public void Awake()
    {
        // Set the pickup to the Player layer so only the Player can interact with it.
        gameObject.layer = LayerMask.NameToLayer("Pickups");

        // If the pickup is set to decay over time, call a delayed Destroy.
        if (willDecay)
        {
            Destroy(gameObject, lifespan);
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        // Spin the pickup gameObject if it is set to spin.
        if (spinPickup)
        {
            Spin();
        }
    }

    // Called automatically when another collider enters a trigger collider on this gameObject.
    private void OnTriggerEnter(Collider other)
    {
        // Attempt to get a PlayerData from the collision object.
        PlayerData player = other.gameObject.GetComponent<PlayerData>();

        // If the collision was with a player,
        if (player != null)
        {
            // Call OnPickup for the child script derived from this.
            OnPickup(player);
        }
    }
    #endregion Unity Methods


    #region Dev Methods
    // Call when the item is picked up. Base version runs last.
    public virtual void OnPickup(PlayerData player)
    {
        // Destroy the pickup object.
        Destroy(gameObject);
    }

    // Spin the gameObject.
    private void Spin()
    {
        tf.Rotate(spinVector * Time.deltaTime);
    }
    #endregion Dev Methods
}
