using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    #region Fields
    // SINGLETON
    public static UIManager Instance { get; private set; }


    [Header("Health & Lives")]

    [SerializeField, Tooltip("Prefab for enemy health bars.")]
    private HealthBar enemyHealthBarPrefab;

    [SerializeField, Tooltip("The health bar (script) on the main canvas to be assigned to the Player.")]
    private HealthBar playerHealthBar;

    [SerializeField, Tooltip("The TextMeshProUGUI of the number of lives remaining on the Player's HUD.")]
    private TextMeshProUGUI livesRemaingText;


    [Header("Weapon Icon Area")]

    [SerializeField, Tooltip("The Image used to show the weapon icon sprite for the Player's HUD.")]
    private Image weaponIconImage;

    [SerializeField, Tooltip("The Text Mesh Pro UGUI showing the number of rounds left in the current weapon.")]
    private TextMeshProUGUI ammoRemainingText;


    [Header("Score")]

    [SerializeField, Tooltip("The Text Mesh Pro UGUI of the score value on the normal HUD.")]
    private TextMeshProUGUI inGameScoreText;

    [SerializeField, Tooltip("The Text Mesh Pro UGUI of the score value on the win screen.")]
    private TextMeshProUGUI winScreenScoreText;


    [Header("Extra Menus and Screens")]

    [SerializeField, Tooltip("The GameObject holding the Pause Menu in the main canvas.")]
    private GameObject pauseMenu;

    [SerializeField, Tooltip("The GameObject holding the Lose Screen in the main canvas.")]
    private GameObject loseScreen;

    [SerializeField, Tooltip("The GameObject holding the Win Screen in the main canvas.")]
    private GameObject winScreen;

    [SerializeField, Tooltip("The GameObject holding the Settings Screen in the main canvas.")]
    private GameObject settingsScreen;


    [Header("Events")]

    [Tooltip("Invoked when the game is paused by the GM.")]
    public UnityEvent onPause;

    [Tooltip("Invoked when the game is resumed by the GM.")]
    public UnityEvent onResume;

    [Tooltip("Invoked by the GM when the game is lost.")]
    public UnityEvent onLose;

    [Tooltip("Invoked by the GM when the game is won.")]
    public UnityEvent onWin;
    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
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
    // Called on Start by PlayerData to register the primary health bar to them.
    public void RegisterPlayer(Health player)
    {
        // Set the primary health bar's target to the player.
        playerHealthBar.SetTarget(player);

        // Add listeners to the player's health to update the primary health bar when their health changes.
        player.onDamage.AddListener(playerHealthBar.UpdateHealthBar);
        player.onHeal.AddListener(playerHealthBar.UpdateHealthBar);

        // Force an update for the health bar immediately.
        playerHealthBar.UpdateHealthBar();
    }

    // Called on Awake by EnemyData to set up that healthBar.
    public void RegisterEnemy(Health enemy, HealthBar healthBar)
    {
        // Add listeners to this enemy's health to update their health bar when their health changes.
        enemy.onDamage.AddListener(healthBar.UpdateHealthBar);
        enemy.onHeal.AddListener(healthBar.UpdateHealthBar);
        // Add one more to destroy the healthBar when the enemy dies.
        enemy.onDie.AddListener(healthBar.RemoveHealthBar);
    }

    // Called when a player equips a weapon to update the weaponIconImage.
    public void UpdateWeaponIcon(Sprite newIcon)
    {
        weaponIconImage.sprite = newIcon;
    }

    // Called when the amount of ammo the Player has on hand changes.
    public void UpdateAmmoRemainingText(int newValue)
    {
        // Update the UI to the new value of ammo remaining.
        ammoRemainingText.text = newValue.ToString();
    }

    // Called by the GM when the Player's number of remaining lives changes.
    public void UpdateLivesRemainingText(int newValue)
    {
        livesRemaingText.text = newValue.ToString();
    }

    // Update the score values for the in game HUD and the win screen.
    public void UpdateScore(int newScore)
    {
        inGameScoreText.text = newScore.ToString();
        winScreenScoreText.text = newScore.ToString();
    }

    // Turns the pause screen either on or off.
    public void TogglePauseScreen(bool turnOn)
    {
        pauseMenu.SetActive(turnOn);
    }

    // Turns the lose screen either on or off.
    public void ToggleLoseScreen(bool turnOn)
    {
        loseScreen.SetActive(turnOn);
    }

    // Turns the win screen either on or off.
    public void ToggleWinScreen(bool turnOn)
    {
        winScreen.SetActive(turnOn);
    }

    // Turns on the settings screen from the in-game Pause screen.
    public void ActivateSettingsScreen()
    {
        pauseMenu.SetActive(false);
        settingsScreen.SetActive(true);
    }
    #endregion Dev Methods


    #region UI Callback Methods
    // Pause Screen -->

    // Called when the Player presses the Resume button on the Pause Screen.
    public void PauseScreen_OnResumeButtonClicked()
    {
        // Unpause the game.
        GameManager.Instance.TogglePause(false);
    }

    // Called when the Player presses the Quit button on the Pause Screen.
    public void PauseScreen_OnQuitButtonClicked()
    {
        // Exit this game to the Main Menu.
        GameManager.Instance.QuitGame();
    }

    // Called when the Player presses the Settings button on the Pause Screen.
    public void PauseScreen_OnSettingsButtonClicked()
    {
        ActivateSettingsScreen();
    }


    // Lose Screen -->

    // Called when the Player presses the Play Again button on the Lose Screen.
    public void LoseScreen_OnPlayAgainButtonClicked()
    {
        // Restart the game level.
        GameManager.Instance.RestartLevel();
    }

    // Called when the Player presses the Quit button on the Lose Screen.
    public void LoseScreen_OnQuitButtonClicked()
    {
        // Quit this game and go to the main menu.
        GameManager.Instance.QuitGame();
    }


    // Win Screen -->

    // Called when the Player presses the Play Again button on the Win Screen.
    public void WinScreen_OnPlayAgainButtonClicked()
    {
        // Have the GM restart the level.
        GameManager.Instance.RestartLevel();
    }

    // Called when the Player presses the Quit button on the Win Screen.
    public void WinScreen_OnQuitButtonClicked()
    {
        // Quit this game and go to the main menu.
        GameManager.Instance.QuitGame();
    }

    // Called when the Player presses the Credits button on the Win Screen.
    public void WinScreen_OnCreditsButtonClicked()
    {
        // Tell the GM to load the credits scene.
        GameManager.Instance.PlayCredits();
    }
    #endregion UI Callback Methods
}
