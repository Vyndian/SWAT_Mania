using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Fields
    [Header("Spawn Logic")]

    [SerializeField, Tooltip("The method this spawner should use for spawns.")]
    private SpawnMode spawnMode = SpawnMode.Once;

    [SerializeField, Tooltip("The delay between spawns, for either Continuous or Maintain.")]
    private float spawnDelay = 4.0f;

    [SerializeField, Tooltip("Whether the spawned unit should be random." +
        "If false, will choose the first item in the aray.")]
    private bool useRandomPrefab = false;

    // The time since the last spawn occured.
    private float timeSinceSpawn = 0.0f;

    // Whether the Maintain mode is currently in the process of spawning a delayed unit.
    private bool spawningDelayedUnit = false;

    // The number of units currently active.
    private int numActiveUnits = 0;


    [Header("Maintain Mode Variables")]

    [SerializeField, Min(1), Tooltip("The number of units allowed active at once for Maintain mode.")]
    private int numActiveUnitsAllowed = 1;

    [SerializeField, Tooltip("Whether to spawn the first unit immediately (Maintain mode only).")]
    private bool spawnFirstImmediately = true;


    [Header("Limit Total Spawns Over Life (Maintain & Continuous modes)")]

    [SerializeField, Tooltip("Whether the total number of units allowed to spawn should be limited.")]
    private bool limitTotalSpawns = true;

    [SerializeField, Tooltip("If Limited Total Spawns is true," +
        "Spawner will destroy itself after spawning this many units over its life.")]
    private int numTotalUnitsAllowed = 10;

    // The number of total units ever spawned by this spawner.
    private int numTotalUnitsSpawned = 0;


    [Header("Object & Component References")]

    [SerializeField, Tooltip("All the prefabs that this spawner should create from.")]
    private GameObject[] spawnPrefabs;

    [SerializeField, Tooltip("The Transform on this gameObject.")]
    private Transform tf;

    [SerializeField, Tooltip("The Transform with the location where the spawn should occur.")]
    private Transform spawnLocation;


        #region Enum Definitions
    // Enum definition for the different spawn modes.
    // Once: Spawns one unit immediately, then destroys itself.
    // Continuous: Continues to spawn over time.
    // Maintain: Operates as Continuous until a certain number of spawn reached.
    //     Maintains that number of spawns by spawning more when some are desroyed.
    // OnCommand: Spawns units only when commanded to.
    public enum SpawnMode { Once, Continuous, Maintain, OnCommand }
    #endregion Enum Definitions
    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    private void Awake()
    {
        // If the spawnPrefabs array is null, log an error and destroy this gameObject.
        if (spawnPrefabs.Length == 0)
        {
            Debug.LogError("Spawner has no Prefabs to choose from. Destroying this object.");
            Destroy(gameObject);
        }

        // If any of these are null, try to set them up.
    }

    // Start is called before the first frame update
    private void Start()
    {
        // If any of these are null, try to set them up.
        if (tf == null)
        {
            tf = transform;
        }

        // If this spawner is set to only spawn one unit,
        if (spawnMode == SpawnMode.Once)
        {
            // then spawn one unit.
            SpawnUnit();
            // Destroy this spawner.
            DestroySelf();
        }
        // Else, if set to Maintain mode and also should spawn the first unit immediately,
        else if (spawnMode == SpawnMode.Maintain && spawnFirstImmediately)
        {
            // then spawn that unit.
            SpawnUnit();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // If the mode is set to Continuous,
        if (spawnMode == SpawnMode.Continuous)
        {
            // then perform the Continuous protocols.
            Continuous();
        }
        // Else, if set to Maintain,
        else if (spawnMode == SpawnMode.Maintain)
        {
            // then perform the Maintain protocols.
            Maintain();
        }
    }
    #endregion Unity Methods


    #region Dev Methods
    // Spawn a unit at the spawnLocation.
    public void SpawnUnit()
    {
        // Instantiate a unit at the spawnLocation.
        GameObject unit = Instantiate(ChoosePrefab(), spawnLocation.position, spawnLocation.rotation);

        // If the spawnMode is anything other than Once or OnCommand,
        if (spawnMode != SpawnMode.Once && spawnMode != SpawnMode.OnCommand)
        {
            // then increment the number of active units.
            numActiveUnits++;
            // Increment the total number of units ever spawned by this spawner.
            numTotalUnitsSpawned++;

            // If the number of total units spawned is limited and has reached its limit,
            if (limitTotalSpawns && numTotalUnitsSpawned >= numTotalUnitsAllowed)
            {
                // then destroy this spawner.
                DestroySelf();
            }

            // Try to get a Health from the unit.
            Health unitHealth = unit.GetComponent<Health>();
            // If the unit has health,
            if (unitHealth != null)
            {
                // then add a listener to that unit that calls HandleUnitDeath when it dies.
                unitHealth.onDie.AddListener(HandleUnitDeath);
            }

            // Reset the time since the last spawn.
            timeSinceSpawn = 0.0f;
        }
    }

    // Perform all steps necessary for the Continuous mode each frame.
    private void Continuous()
    {
        // Track the time since last spawn.
        timeSinceSpawn += Time.deltaTime;

        // If enough time has passed to spawn another unit,
        if (timeSinceSpawn >= spawnDelay)
        {
            // then spawn another unit.
            SpawnUnit();
        }
    }

    // Perform all steps necessary for the Maintain mode each frame.
    private void Maintain()
    {
        // If there aren't too many units already, && not already spawning a delayed unit,
        if (numActiveUnits < numActiveUnitsAllowed && !spawningDelayedUnit)
        {
            // then spawn a delayed unit.
            StartCoroutine(nameof(DelayedSpawn));
        }
    }

    // Spawn a unit only after a certain amount of time.
    private IEnumerator DelayedSpawn()
    {
        // Currently spawning a delayed unit.
        spawningDelayedUnit = true;

        // Until enough time has passed,
        while (timeSinceSpawn < spawnDelay)
        {
            // track the time.
            timeSinceSpawn += Time.deltaTime;

            // Yield.
            yield return null;
        }
        // Once enough time has passed,
        // no longer spawning delayed unit. Spawn the unit.
        spawningDelayedUnit = false;
        SpawnUnit();
    }

    // Called when a unit spawned by this spawner dies (except for Once mode).
    private void HandleUnitDeath()
    {
        // Decrement the number of active units.
        numActiveUnits--;
    }

    // Destroys the spawner gameObject.
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    // Returns a prefab from the spawnPrefabs array. Random is useRandomPrefab is true.
    private GameObject ChoosePrefab()
    {
        // If set to always use the first prefab in the array,
        if (!useRandomPrefab)
        {
            // then simply return the first element of the array.
            return spawnPrefabs[0];
        }
        // Else, is set to do it randomly.
        else
        {
            return spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
        }
    }
    #endregion Dev Methods
}
