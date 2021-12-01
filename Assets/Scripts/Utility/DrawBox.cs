using UnityEngine;

public class DrawBox : MonoBehaviour
{
    #region Fields
    [Header("Box Gizmo Settings")]

    [SerializeField, Tooltip("The scale of the box you'd like drawn.")]
    private Vector3 scale = new Vector3(1, 1, 1);

    [SerializeField, Tooltip("The color of the box you'd like drawn.")]
    private Color color = Color.cyan;

    [SerializeField, Tooltip("The length of the ray used to show spawn direction.")]
    private float rayLength = 0.8f;


    [Header("Object & Component References")]

    // The Transform on this gameObject.
    private Transform tf;
    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    private void Awake()
    {
        // If any of these are not set up, try to set them up.
    }

    // Start is called before the first frame update
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (tf != transform)
        {
            tf = transform;
        }

        Gizmos.matrix = Matrix4x4.TRS(tf.position, tf.rotation, Vector3.one);
        Gizmos.color = Color.Lerp(color, Color.clear, 0.5f);
        Gizmos.DrawCube(Vector3.up * scale.y / 2.0f, scale);
        Gizmos.color = color;
        Gizmos.DrawRay(Vector3.zero, Vector3.forward * rayLength);
    }
    #endregion Unity Methods


    #region Dev Methods

    #endregion Dev Methods
}
