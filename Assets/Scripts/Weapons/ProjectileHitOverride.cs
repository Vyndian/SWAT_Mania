using UnityEngine;

public class ProjectileHitOverride : MonoBehaviour
{
    #region Fields
    [Header("Particle System for hit override")]

    [Tooltip("The ParticleSystem (gameObject) that will spawn when a projectile hits this gameObject.")]
    public ParticleSystem hitEffect;


    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    private void Awake()
    {
        // If any of these are null, try to set them up.
    }

    // Start is called before the first frame update
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

    }
    #endregion Unity Methods


    #region Dev Methods

    #endregion Dev Methods
}
