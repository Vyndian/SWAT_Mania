using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Fields
    [Header("Projectile")]

    [SerializeField, Tooltip("The time after instantiation before the projectile destroys itself.")]
    private float lifespan = 2.0f;

    // The damage this projectile will do to Health targets it hits. Set at istantiation by the Weapon.
    public float damage = 1.0f;


    [Header("Hit Particle Effect")]

    [SerializeField, Tooltip("The default Particle System (gameObject) to spawn if" +
        " what this projectile hits does not have a ProjectileHitOverride.")]
    private ParticleSystem defaultHitEffect;

    [SerializeField, Tooltip("The life of the particicle system effect that will" +
        " spawn when the projectile hits something.")]
    private float hitEffectDuration;
    
    
    [Header("Object & Component References")]

    [Tooltip("The Rigidbody on this projectile gameObject.")]
    public Rigidbody rb;

    [SerializeField, Tooltip("The Transform on this gameObject.")]
    private Transform tf;
    #endregion Fields


    #region Unity Methods
    // Called at instantiation.
    private void Awake()
    {
        // Start the lifespan timer to self-destroy the bullet.
        Destroy(gameObject, lifespan);

        if (tf == null)
        {
            tf = transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If any of these are not set up, try to set them up.
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Called when the bullet comes into contact with a collider.
    private void OnCollisionEnter(Collision collision)
    {
        // Attempt to get the Health from the collided object.
        Health hitObject = collision.gameObject.GetComponent<Health>();

        // If the object it collided with has Health,
        if (hitObject != null)
        {
            // then damage that object.
            hitObject.Damage(damage);
        }

        // Create the appropriate particle system hit effect.
        DoHitEffect(collision);

        // Destroy the bullet.
        Destroy(gameObject);
    }
    #endregion Unity Methods


    #region Dev Methods
    // Do the hit effect.
    private void DoHitEffect(Collision collision)
    {
        // Try to get a ProjectileHitEffect from the collision object.
        ProjectileHitOverride hitOverride = collision.gameObject.GetComponent<ProjectileHitOverride>();
        // Instantiate a hit effect based on whether one was found, and what its hitEffect is.
        ParticleSystem hitEffect = Instantiate(
            (hitOverride ? hitOverride.hitEffect : defaultHitEffect),
            collision.contacts[0].point,
            Quaternion.Inverse(tf.rotation)) as ParticleSystem;
        // Destroy the particle system after the appropriate amount on time.
        Destroy(hitEffect.gameObject, hitEffectDuration);
    }
    #endregion Dev Methods
}
