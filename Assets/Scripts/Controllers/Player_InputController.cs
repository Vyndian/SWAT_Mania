using UnityEngine;

// The input controller just takes input from the player, and tells the Pawn what that input was.
// The Pawn is what actually controls the character using root motion.

public class Player_InputController : Controller
{
    #region Fields
    [Header("Controls")]

    // The key used to sprint with.
    [SerializeField, Tooltip("The key used to sprint")] private KeyCode sprintKey = KeyCode.LeftShift;

    // The key used to walk with.
    [SerializeField, Tooltip("The key used to walk")] private KeyCode walkKey = KeyCode.LeftAlt;

    // The key used to attack with the equiped weapon.
    [SerializeField, Tooltip("The key used to attack")] private KeyCode attackKey = KeyCode.Mouse0;


    [Header("Object/Component references")]

    // References the CharacterData on this character, which holds all character-specific data.
    [SerializeField] private PlayerData data;

    // The camera that will follow this player.
    [SerializeField] private Camera cam;
    #endregion Fields

    #region Unity Methods
    // Called immeidately after being instantiated.
    protected override void Awake()
    {
        base.Awake();

        // If any of these variables are not yet set up, set them up.
        if (data == null)
        {
            data = GetComponent<PlayerData>();
        }

        if (tf == null)
        {
            tf = transform;
        }

        if (pawn == null)
        {
            pawn = GetComponent<Pawn>();
        }

        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        // If the game is paused,
        if (GameManager.Instance.isPaused)
        {
            // then return. Do nothing.
            return;
        }

        // Get all movement input and act appropriately.
        GetMovementInput();

        // Get all input relating to combat actions.
        GetCombatInput();

        base.Update();
    }
    #endregion Unity Methods

    #region Dev Methods
    // Abstracts all movement input gathering code to keep Update clean.
    private void GetMovementInput()
    {
        // Get whether or not the player is trying to sprint.
        bool sprint = Input.GetKey(sprintKey);

        // If not trying to sprint,
        if (!sprint)
        {
            // Tell the CharacterData that the player is not sprinting.
            data.isSprinting = false;
        }

        // Take the user's input for direction and whether or not they are sprinting/walking and such.
        // Tell the Pawn to move the player, accounting for speed/sprint/walk.
        pawn.Move(GetVelocity(), data.maxMoveSpeed, sprint, Input.GetKey(walkKey));

        // Get the point that the mouse is aiming at and tell the Pawn to turn the player
        // towards that point over time.
        pawn.TurnOverTime(GetMouseTarget(), data.turnSpeed);
    }

    // Called every frame to move the player based on their input.
    // NOTE: No longer inverts here! Inverting is done in HumanoidPawn's Move().
    private Vector3 GetVelocity()
    {
        // Create a new Vector3 to hold the user's current input.
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Return the input.
        return input;
    }

    // Gets the location that the mouse is pointing at (on a place at the character's feet).
    private Vector3 GetMouseTarget()
    {
        // Create a mathmatical plane perpendicular to the player at their feet (true 0 of their position).
        Plane plane = new Plane(Vector3.up, tf.position);
        // Create a ray from the mouse's position toward that plane, angled as if from the camera.
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // Check how far the ray travelled to interset with the plane.
        if (plane.Raycast(ray, out float distance))
        {
            // Return the location where they intersected.
            return ray.GetPoint(distance);
        }
        // Else, something went wrong.
        else
        {
            // Log the problem.
            Debug.LogError("The raycast failed to find the plane for " + gameObject.name);

            // Return a point directly in front of where the player is currently facing.
            return tf.rotation.eulerAngles + tf.forward;
        }
    }

    // Abstracts all combat input gathering code to keep Update clean.
    private void GetCombatInput()
    {
        // If the player has a weapon,
        if (data.equippedWeapon != null)
        {
            // and if the player just pressed the fire key,
            if (Input.GetKeyDown(attackKey))
            {
                // then tell the player's weapon to start the attack.
                data.equippedWeapon.AttackStart();
            }
            // Else, if they just let go of that key,
            else if (Input.GetKeyUp(attackKey))
            {
                // then tell the player's weapon to stop the attack.
                data.equippedWeapon.AttackEnd();
            }
        }
    }
    #endregion Dev Methods
}
