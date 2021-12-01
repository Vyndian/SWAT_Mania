using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Controller : Controller
{
    #region Fields
    [Header("Navigation")]

    [SerializeField, Tooltip("The Transform of the gameObject that this AI should be following & attacking.")]
    private Transform target;

    [SerializeField, Tooltip("The NavMeshAgent on this gameObject.")]
    private NavMeshAgent agent;


    [Header("Object & Component References")]

    [SerializeField, Tooltip("The EnemyData on this AI.")]
    private EnemyData data;
    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    protected override void Awake()
    {
        base.Awake();

        // If any of these are null, try to set them up.
        if (agent == null)
        {
            // Grab the NavMeshAgent component off of this gameObject.
            agent = GetComponent<NavMeshAgent>();
        }

        if (data == null)
        {
            data = GetComponent<EnemyData>();
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        if (target == null)
        {
            // Assume that we want to target the player if the target was not set up by designer.
            target = GameManager.GetPlayer().transform;
        }

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

        // If there is a player,
        if (GameManager.GetPlayer())
        {
            // then ensure the target is always set up correctly.
            target = GameManager.GetPlayer().transform;
        }
        // Else, there is no Player right now. DO NOTHING.
        else
        {
            return;
        }

        // Tell the pawn to move. It will change from world directions to local directions automatically.
        pawn.Move(DesiredVelocity(), data.maxMoveSpeed);
        /* NOTE: AT this point, both animator AND navMeshAgent are moving the enemy!
         * Within OnAnimatorMove(), this is resolved. */

        // If the enemy is aimed at the player well enough to attack with their weapon and within range,
        if (CheckAim() && CheckRange())
        {
            // then have the enemy attempt to attack.
            data.equippedWeapon.AttackStart();
        }
        // Else, the enemy is not aimed well enough.
        else
        {
            data.equippedWeapon.AttackEnd();
        }


        base.Update();
    }

    // Called after the animator has finished determining its changes.
    public void OnAnimatorMove()
    {
        // The animator determines how much to move, and we pass that velocity into the agent.
        agent.velocity = anim.velocity;
    }
    #endregion Unity Methods


    #region Dev Methods
    // Calculate the desired velocity via the NavMesh Agent.
    private Vector3 DesiredVelocity()
    {
        // Create a path to the target.
        agent.SetDestination(target.position);
        // Force the desiredVelocity to account for the NavMesh Agent's acceleration.
        Vector3 desiredVelocity = agent.desiredVelocity;
        desiredVelocity = Vector3.MoveTowards
            (
                desiredVelocity,
                agent.desiredVelocity,
                agent.acceleration * Time.deltaTime
            );

        // Return the result.
        return desiredVelocity;
    }

    // Check if the enemy is aimed at the player well enough to fire their weapon.
    private bool CheckAim()
    {
        // If the enemy has no weapon,
        if (data.equippedWeapon == null)
        {
            // then aim is automatically considered to be bad.
            return false;
        }

        // If the angle to the player is <= the equipped weapon's effective attack angle,
        if (AngleToPlayer() <= data.equippedWeapon.AttackAngle)
        {
            // then yes, the aim is good.
            return true;
        }
        // Else, the aim is bad.
        else
        {
            return false;
        }
    }

    // Check is the enemy is within range of the target.
    private bool CheckRange()
    {
        // If the enemy has no weapon,
        if (data.equippedWeapon == null)
        {
            // then the target is automatically considered to be out of range.
            return false;
        }

        // If the distance between the enemy and the target is within the weapon's effective range,
        if (Vector3.Distance(tf.position, target.position) <= data.equippedWeapon.effectiveRange)
        {
            // then the range is good.
            return true;
        }
        // Else, too far away. Range is bad.
        else
        {
            return false;
        }
    }

    // Calculates the angle from the enemy's forward to the target.
    private float AngleToPlayer()
    {
        // Get a vector from the enemy to the target.
        Vector3 targetDirection = target.position - tf.position;
        // Return the angle between targetDirection and enemy's forward.
        return Vector3.Angle(targetDirection, tf.forward);
    }
    #endregion Dev Methods
}
