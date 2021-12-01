using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    #region Fields
    // Whether or not the gun can currently shoot.
    private bool canShoot = true;

    [Header("Fire Rate (Single shot)")]

    [SerializeField, Tooltip("If false, bullet shoots as fast as can pull the trigger.")]
    private bool limitedFireRate = false;

    [SerializeField, Tooltip("How many rounds gun can fire / second.")]
    private float roundsPerMinute = 45.0f;

    // Seconds to fire each round. Calculated in Start (60 / roundsPerMinute).
    private float fireRate_Single;

    // How long it has been (in seconds) since the last time the gun was fired.
    private float timeSinceFired = 0.0f;

    [Header("FireRate (Burst Fire)")]

    [SerializeField, Min(1), Tooltip("The number of rounds fired per burst.")]
    private int roundsPerBurst = 3;

    [SerializeField, Min(1), Tooltip("How many seconds it should take to complete the burst.")]
    private float burstTime = 0.25f;

    [SerializeField, Min(1), Tooltip("The number of bursts the weapon can fire in one minute.")]
    private float burstsPerMinute = 15.0f;

    // Seconds BETWEEN each burst, from start to start. Calculated in Start (60 / roundsPerMinute - burstTime).
    private float fireRate_Burst;

    // Seconds between each round DURING a burst. Calculated in Start (burstTime / roundsPerBurst).
    private float burstSpeed;


    [Header("Projectile Settings")]

    [SerializeField, Tooltip("The Bullet prefab that this gun fires as a projectile.")]
    private Projectile projectilePrefab;

    [SerializeField, Tooltip("Transform of the location projectiles should spawn (the barrel).")]
    private Transform barrel;

    [SerializeField, Tooltip("The damage that each projectile will deal to Health objects they hit.")]
    private float projectileDamage = 10.0f;

    [SerializeField, Tooltip("The amount of force with which the projectile is propelled from the barrel.")]
    private float muzzleVelocity = 1000.0f;

    [SerializeField, Tooltip("Whether or not the gun should have bullet spread.")]
    private bool hasSpread = true;

    [SerializeField, Tooltip("The amount of variance in initial trajectory of the bullets.")]
    private float spread = 1.2f;


    [Header("Ammo Settings")]

    [SerializeField, Tooltip("The number of rounds this gun starts with when equipped.")]
    private int numRoundsDefault = 30;

    [SerializeField, Tooltip("The number of rounds used with each bullet fired " +
        "(almost always 1, DO NOT account for rounds per burst).")]
    private int roundsUsedPerShot = 1;

    // The current number of rounds this gun has.
    private int currentRoundsLeft = 0;


    [Header("Muzzle Flash")]

    [SerializeField, Tooltip("The ParticleSystem for the muzzle flash. It should be a child of the barrel.")]
    private ParticleSystem muzzleFlashParticle;
    #endregion Fields


    #region Unity Methods
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        // Calculate the number of seconds between each single shot from the roundsPerMinute.
        fireRate_Single = 60 / roundsPerMinute;

        // Calculate the number of seconds between each burch from the burstsPerMinute and burstTime.
        fireRate_Burst = (60 / burstsPerMinute) - burstTime;

        // Calculate the number of seconds between each round during a burst.
        burstSpeed = burstTime / roundsPerBurst;

        // Ensure the UI is correct for the Player.
        if (isEquippedByPlayer)
        {
            UIManager.Instance.UpdateAmmoRemainingText(currentRoundsLeft);
        }

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    #endregion Unity Methods


    #region Dev Methods
    // Called at the beginning of the attack with this weapon.
    public override void AttackStart()
    {
        // Always invoke the onAttackStart event.
        // Other methods called from the event, as decided by designers.
        onAttackStart.Invoke();

        base.AttackStart();
    }

    // Called at the end of the attack with this weapon.
    public override void AttackEnd()
    {
        // Always invoke the onAttackEnd event.
        // Other methods called from the event, as decided by designers.
        onAttackEnd.Invoke();

        base.AttackEnd();
    }

    // Fires a bullet from the gun, projecting it out from the barrel.
    // Potentially affected by fireRate.
    public void FireSingleBullet()
    {
        // If the gun is ready to shoot, and there are enough rounds left to fire a shot,
        if (canShoot && currentRoundsLeft >= roundsUsedPerShot)
        {
            // then fire a bullet.
            InstantiateBullet();

            // If the fire rate is limited,
            if (limitedFireRate)
            {
                // then the gun is no longer ready to shoot until firRate seconds have passed.
                canShoot = false;
                // Start the timer until gun can shoot again.
                StartCoroutine(CantFire(fireRate_Single));
            }
        }
        // Else, the rifle cannot shoot because it was fired too recently.
        // Do nothing.
    }

    // Fires a burst of rounds from the gun, projected from the barrel.
    // Potentially affected by roundsPerMinute.
    public void FireBurst()
    {
        // If the gun is ready to shoot, and there are enough rounds to fire a burst,
        if (canShoot && currentRoundsLeft >= (roundsUsedPerShot * roundsPerBurst))
        {
            // then do the burst fire.
            DoBurst();

            // If the fire rate is limited,
            if (limitedFireRate)
            {
                // then the gun cannot shoot until enough time has passed.
                canShoot = false;
                // Start the delay until gun can shoot again.
                StartCoroutine(CantFire(fireRate_Burst));
            }
        }
        // Else, the rifle cannot shoot because it was fired too recently.
        // Do nothing.
    }

    // Do the actual burst.
    private void DoBurst()
    {
        // Once, for each round that should be fired per burst,
        for (int i = 0; i < roundsPerBurst; i++)
        {
            // call a delayed shot, with burstSpeed between each round.
            StartCoroutine(DelayedShot(burstSpeed * i));
        }
    }

    // Delay the firing of a bullet a certain amount of time, then fire.
    private IEnumerator DelayedShot(float delay)
    {
        float timer = 0.0f;
        // Until the delay has been satisfied,
        while (timer < delay)
        {
            // track the time.
            timer += Time.deltaTime;
            // Yield.
            yield return null;
        }
        // Once delay has been satisfied, instantiate a bullet.
        InstantiateBullet();
    }

    // Instantiates a bullet.
    private void InstantiateBullet()
    {
        // Adjust the number of rounds left, as some are being used.
        ChangeCurrentRounds(-roundsUsedPerShot);

        // Create a Projectile (bullet) at the barrel.
        Projectile projectile = Instantiate
            (
                projectilePrefab,
                barrel.position,
                GetTrajectory()
            ) as Projectile;

        // Assign the projectile its damage. Match its layer to the gun's layer.
        projectile.damage = projectileDamage;
        projectile.gameObject.layer = gameObject.layer;

        // Add force to the bullet to make it "shoot" out of the barrel.
        projectile.rb.AddRelativeForce
            (
                Vector3.forward * muzzleVelocity,
                ForceMode.VelocityChange
            );

        // If the muzzleFlashParticle isn't null,
        if (muzzleFlashParticle != null)
        {
            // then emit a particle.
            muzzleFlashParticle.Emit(1);
        }

        // If this gun makes an attackSound,
        if (doesPlayAttackSound)
        {
            // then play the attackSound.
            AttackSound();
        }
    }

    // Calculates and returns the bullet's initial trajectory.
    private Quaternion GetTrajectory()
    {
        // If this weapon uses bullet spread,
        if (hasSpread)
        {
            // then include spread in the calculation.
            return barrel.rotation * Quaternion.Euler(Random.onUnitSphere * spread);
        }
        // Else, no dot use the spread.
        else
        {
            return barrel.rotation;

        }
    }

    // Prevents firing the gun for a set period of time.
    private IEnumerator CantFire(float delay)
    { 
        // Until enough time has passed for the gun to fire,
        while (timeSinceFired < delay)
        {
            // Track the time since last fired.
            timeSinceFired += Time.deltaTime;
            // then do nothing this frame.
            yield return null;
        }

        // Once enough time has passed,
        // Reset the timer.
        timeSinceFired = 0.0f;
        // The gun can now shoot.
        canShoot = true;
    }

    // Adjusts the number of rounds currently available by the amount provided.
    // Positive numbers add to the rounds left, negative numbers decrease.
    private void ChangeCurrentRounds(int change)
    {
        // Apply the change, minimum of 0 rounds left (though that should never be necessary).
        currentRoundsLeft = Mathf.Max((currentRoundsLeft + change), 0);

        // If this is equipped by the Player,
        if (isEquippedByPlayer)
        {
            // then update the HUD.
            UIManager.Instance.UpdateAmmoRemainingText(currentRoundsLeft);
        }
    }

    // Called when the weapon is being equipped.
    public override void OnEquip()
    {
        // Initialize the current rounds left.
        ChangeCurrentRounds(numRoundsDefault);

        base.OnEquip();
    }

    // Called when the weapon is being unequipped.
    public override void OnUnequip()
    {
        // Set the ammount of ammo available to 0.
        ChangeCurrentRounds(-currentRoundsLeft);

        base.OnUnequip();
    }
    #endregion Dev Methods
}
