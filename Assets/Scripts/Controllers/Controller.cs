using UnityEngine;

// This script is required.
[RequireComponent(typeof(Pawn))]

public abstract class Controller : MonoBehaviour
{
    #region Fields
    [Header("Object & Component References")]

    [SerializeField, Tooltip("The Transform on this gameObject.")]
    protected Transform tf;

    [Tooltip("The Pawn on the Pawn's gameObject.")]
    protected Pawn pawn;

    [SerializeField, Tooltip("The Animator on this gameObject.")]
    protected Animator anim;
    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    protected virtual void Awake()
    {
        // If any of these are null, try to set them up.
        if (tf == null)
        {
            tf = transform;
        }

        if (pawn == null)
        {
            pawn = GetComponent<Pawn>();
        }

        if (anim == null)
        {
            // Get the animator from the pawn.
            anim = pawn.GetComponent<Animator>();
        }
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    #endregion Unity Methods


    #region Dev Methods

    #endregion Dev Methods
}
