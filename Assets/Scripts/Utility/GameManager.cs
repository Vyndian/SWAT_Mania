using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    #region Fields
    // SINGLETON
    public static GameManager Instance { get; private set; }


    [Header("The Player")]

    [SerializeField, Tooltip("The PREFAB for the Player.")]
    private PlayerData playerPrefab;

    private static PlayerData m_Player;


    [Header("The HUD")]

    [Tooltip("The Health Bar slider.")]
    public Slider healthBar;

    [Tooltip("The Stamina Bar slider.")]
    public Slider staminaBar;


    [Header("Pausing")]

    [Tooltip("Whether or not the game is paused.")]
    public bool isPaused = false;

    [Tooltip("The primary key used to pause the game.")]
    public KeyCode primaryPauseKey = KeyCode.Escape;


    [Header("Spawning")]

    [SerializeField, Tooltip("Where the Player should start this level.")]
    private Transform playerStartLocation;
    
    [SerializeField, Tooltip("How long between the Player dieing and being respawned.")]
    private float playerRespawnDelay = 4.0f;


    [Header("Lives")]

    [SerializeField, Tooltip("The number of lives that the Player starts with. Cannot respawn if < 1.")]
    private int initialLives = 2;

    // The actual number of lives left.
    private int livesLeft;


    [Header("Scene Info")]

    [SerializeField, Tooltip("The EXACT name of the Main Menu scene.")]
    private string mainMenuSceneName = "MainMenu";

    [SerializeField, Tooltip("The EXACT name of the Credits scene.")]
    private string creditsSceneName = "Credits";


    [Header("Scoring")]

    [SerializeField, Tooltip("The number of points the Player needs in order to beat the level.")]
    private int scoreWinCondition = 50;

    // The Player's current score, earned by killing the enemies.
    private int currentScore = 0;

    // The highest score yet achieved.
    public int highScore { get; private set; }


    [Header("Other Object & Component References")]

    [Tooltip("The OverheadCamera script on the main camera that should follow the player.")]
    public OverheadCamera overheadCamera;
    #endregion Fields


    #region Unity Methods
    // Called when instantiated.
    private void Awake()
    {
        // SINGLETON
        // If there isn't already an instance of this class,
        if (Instance == null)
        {
            // then this will the the only allows instance.
            Instance = this;
        }
        // Else, a new GameManager is trying to instantiate, but there can only be one.
        else
        {
            // Destroy this GameManager and end the Awake method.
            Destroy(this);
            return;
        }

        // Spawn the player on the start point and save a reference to their data.
        SpawnPlayer();
    }

    // Start is called before the first frame update
    public void Start()
    {
        // Set initial lives.
        SetLivesLeft(initialLives);

        // Load the previously saved game state.
        //LoadSavedData();
    }

    // Update is called once per frame
    public void Update()
    {
        // Take in and handle all player input related to GM tasks.
        HandleInput();
    }
    #endregion Unity Methods


    #region Dev Methods
    // Spawn the player.
    private void SpawnPlayer()
    {
        // Spawn the Player.
        m_Player = Instantiate(playerPrefab, playerStartLocation.position, playerStartLocation.rotation).
            GetComponent<PlayerData>();
        // Set up the references needed for the Player.
        m_Player.SetUpReferences();
        // Add a listener for when the Player dies.
        m_Player.health.onDie.AddListener(HandlePlayerDeath);
    }

    // Called when the player dies via OnDie event.
    private void HandlePlayerDeath()
    {
        // Remove this listener from the OnDie event.
        m_Player.health.onDie.RemoveListener(HandlePlayerDeath);

        // If the Player has a life to consume,
        if (livesLeft > 0)
        {
            // then consume a list and invoke the SpawnPlayer method after the appropriate respawn delay.
            SetLivesLeft(livesLeft - 1);
            Invoke("SpawnPlayer", playerRespawnDelay);
        }
        // Else, the Player did not have any more lives left.
        else
        {
            GameManager.Instance.LoseGame();
        }
    }

    // Called every frame to handle player input for GM-related tasks.
    private void HandleInput()
    {
        // If the player just pressed the primary pause key,
        if (Input.GetKeyDown(primaryPauseKey))
        {
            // Then toggle whether the game is paused.
            TogglePause(!isPaused);
        }
    }

    // Called whenever the Player earns or loses points toward their score.
    // Positive input increases the score. Negative lowers the score.
    public void ChangeScore(int scoreChange)
    {
        // Apply the change, minimum of 0.
        currentScore = Mathf.Max((currentScore + scoreChange), 0);
        // Change the highScore if applicable.
        highScore = Mathf.Max(currentScore, highScore);
        // Tell the UIManager to update the UI.
        UIManager.Instance.UpdateScore(currentScore);

        // Check if the game has been won.
        if (currentScore >= scoreWinCondition)
        {
            WinGame();
        }
    }

    // Toggles whether the game is paused.
    public void TogglePause(bool pauseGame)
    {
        // Do the pause toggle.
        DoPause(pauseGame);

        // If we are pausing the game,
        if (pauseGame)
        {
            // Invoke the onPause event on the UIManager.
            UIManager.Instance.onPause.Invoke();
        }
        // Else, we are unpausing the game.
        else
        {
            // Invoke the onResume event on the UIManager.
            UIManager.Instance.onResume.Invoke();
        }
    }

    // Do the actual implementation of pausing and unpausing.
    private void DoPause(bool turnOn)
    {
        isPaused = turnOn;
        Time.timeScale = Convert.ToInt32(!turnOn);
    }

    // Restart the current level.
    public void RestartLevel()
    {
        // Ensure that the game is NOT paused.
        DoPause(false);

        // RestartLevel the current level.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // The Player loses the game.
    public void LoseGame()
    {
        // Pause the game (without invoking the normal onPause event).
        DoPause(true);

        // Invoke the onLose event on the UIManager.
        UIManager.Instance.onLose.Invoke();
    }

    // The Player wins the game.
    public void WinGame()
    {
        // Pause the game (without invoking the normal onPause event).
        DoPause(true);

        // Invoke the UIManager's onWin event.
        UIManager.Instance.onWin.Invoke();
    }

    // Quits this game, taking the Player back to the main menu.
    public void QuitGame()
    {
        // Ensure the game is unpaused.
        DoPause(false);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Loads the Credits scene.
    public void PlayCredits()
    {
        // Ensure the game is unpaused.
        DoPause(false);

        SceneManager.LoadScene(creditsSceneName);
    }


        #region Getters
    public static PlayerData GetPlayer()
    {
        // Return reference to the player.
        return m_Player;
    }

    public int GetLivesLeft()
    {
        return livesLeft;
    }
    #endregion Getters

        #region Setters
    // Sets the number of lives left and updates the HUD.
    private void SetLivesLeft(int newVal)
    {
        livesLeft = newVal;
        UIManager.Instance.UpdateLivesRemainingText(livesLeft);
    }
        #endregion Setters
    #endregion Dev Methods
}
