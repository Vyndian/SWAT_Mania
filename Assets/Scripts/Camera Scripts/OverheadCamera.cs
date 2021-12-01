using UnityEngine;

// Attach this script to the Main Camera and it will follow the character pulled into the character variable.

public class OverheadCamera : MonoBehaviour
{
    #region Fields
    [Header("Camera Movement")]

    // The distance desired between the camera and the Player.
    [SerializeField] private Vector3 offset = new Vector3(0, 10, 0);

    // The  slower, default speed that the camera follows the Player.
    [SerializeField] private float moveSpeed_Default = 2.0f;

    // The speed that the camera follows the character while the character is sprinting.
    [SerializeField] private float moveSpeed_Sprinting = 4.5f;


    [Header("Object & Component References")]

    // The Transform of the Player.
    [SerializeField] private Transform player;

    // The Transform of this camera.
    [SerializeField] private Transform tf;
    #endregion Fields

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        // If any of these variables are not set up, set them up.

        if (tf == null)
        {
            tf = transform;
        }

        if (player == null)
        {
            player = GameManager.GetPlayer().transform;
        }
    }
    #endregion Unity Methods

    #region Dev Methods
    // Alter the camera boom's position appropriately to follow the character.
    public void FollowCharacter(bool isSprinting)
    {
        // The speed that the camera will follow the character.
        float moveSpeed;
        // Determine the speed by whether or not the character is sprinting.
        if (isSprinting)
        {
            moveSpeed = moveSpeed_Sprinting;
        }
        else
        {
            moveSpeed = moveSpeed_Default;
        }

        // Follow the character as they move, staying 10.0f units above, at moveSpeed/second.
        tf.position = Vector3.MoveTowards
            (
                tf.position,
                player.position + offset,
                moveSpeed * Time.deltaTime
            );
    }

    // Get a new reference to the Player (Called when Player respawns).
    public void FindPlayer()
    {
        // Get a new reference to the Player.
        player = GameManager.GetPlayer().transform;
        // Snap to the Player immediately.
        SnapToPlayer();
    }

    // Immediately move the camera to be over the Player (instead of over time).
    private void SnapToPlayer()
    {
        tf.position = new Vector3(player.position.x, player.position.y, player.position.z) + offset;
    }
    #endregion Dev Methods
}
