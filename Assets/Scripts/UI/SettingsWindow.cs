using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SettingsWindow : MonoBehaviour
{
    #region Fields
    [Header("Volume Area")]

    [SerializeField, Tooltip("The Slider for the Master Volume setting.")]
    private Slider masterVolumeSlider;

    [SerializeField, Tooltip("The Slider for the Sound Volume setting.")]
    private Slider soundVolumeSlider;

    [SerializeField, Tooltip("The Slider for the Music Volume setting.")]
    private Slider musicVolumeSlider;


    [Header("Graphics Area")]

    [SerializeField, Tooltip("The TMP Dropdown for the Resolution setting.")]
    private TMP_Dropdown resolutionDropdown;

    [SerializeField, Tooltip("The Toggle for the Fullscreen setting.")]
    private Toggle fullScreenToggle;

    [SerializeField, Tooltip("The TMP Dropdown for the Video Quality setting.")]
    private TMP_Dropdown videoQualityDropdown;


    [Header("Buttons")]

    [SerializeField, Tooltip("The Button script on the Apply button.")]
    private Button applyButton;


    [Header("Other Object & Component References")]

    [SerializeField, Tooltip("The gameObject holding the UI menu that should be opened when" +
        " the settings close.")]
    private GameObject previousMenu;
    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    private void Awake()
    {
        // Build the appropriate setting options for the resolution and video quality settings.
        BuildResolutions();
        BuildVideoQualities();
    }

    // Called every time the gameObject is enabled (SetActive to false, then true, also same as Start).
    private void OnEnable()
    {
        // Apply the currently saved preferences to the settings.
        ApplyPreferencesToUI();
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
    // Builds the Resolutions Dropdown menu with the appropriate options.
    private void BuildResolutions()
    {
        // Clear the resolution settings dropdown of all current options.
        resolutionDropdown.ClearOptions();
        // Create a list to hold the options we will get from Screen.
        List<string> resolutions = new List<string>();

        // Iterate through the resolution options from Screen.
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            // Add each resolution to the list, formated like this:
            // 1234 x 987
            resolutions.Add
                (string.Format("{0} x {1}", Screen.resolutions[i].width, Screen.resolutions[i].height));
        }

        // Actually add them to the dropdown as options.
        resolutionDropdown.AddOptions(resolutions);
    }

    // Builds the Video Quality Dropdown menu with the appropriate options.
    private void BuildVideoQualities()
    {
        // Clear the video quality settings dropdown of all current options.
        videoQualityDropdown.ClearOptions();
        // Add the appropriate settings.
        videoQualityDropdown.AddOptions(QualitySettings.names.ToList());
    }

    // Update all current settings to the values saved in PlayerPrefs and via Unity.
    private void ApplyPreferencesToUI()
    {
        // Apply Player Prefs to the volume sliders.
        masterVolumeSlider.value =
            PlayerPrefs.GetFloat(AudioManager.Instance.masterVolume_PrefName, masterVolumeSlider.maxValue);
        soundVolumeSlider.value =
            PlayerPrefs.GetFloat(AudioManager.Instance.soundVolume_PrefName, soundVolumeSlider.maxValue);
        musicVolumeSlider.value =
            PlayerPrefs.GetFloat(AudioManager.Instance.musicVolume_PrefName, soundVolumeSlider.maxValue);

        // Find the correct value in the resolutions dropdown for the current resolution.
        string currentRes =
                string.Format("{0} x {1}", Screen.currentResolution.width, Screen.currentResolution.height);
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutionDropdown.options[i].text == currentRes)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        // Apply Unity's auto-saved preferences to fullscreen and video quality.
        fullScreenToggle.isOn = Screen.fullScreen;
        videoQualityDropdown.value = QualitySettings.GetQualityLevel();
        // Resolution settings are saved & applied automatically.

        // Apply those changes.
        ApplyChoicesToGame(false);
    }

    // Applies the current setting values. Can also save the values to preferences.
    private void ApplyChoicesToGame(bool saveToPrefs)
    {
        // No changes have been made anymore, so ensure the Apply button is not interactable.
        applyButton.interactable = false;

        AudioManager.Instance.ChangeBusVolumes
            (AudioManager.Instance.VolumeToDecibel(masterVolumeSlider.value),
            AudioManager.Instance.VolumeToDecibel(soundVolumeSlider.value),
            AudioManager.Instance.VolumeToDecibel(musicVolumeSlider.value));

        int resolutionValueIndex = resolutionDropdown.value;
        int width = Screen.width;
        int height = Screen.height;
        for (int i = 0; i < Screen.resolutions.Length; i ++)
        {
            if (i == resolutionValueIndex)
            {
                width = Screen.resolutions[i].width;
                height = Screen.resolutions[i].height;
            }
        }

        Screen.SetResolution(width, height, fullScreenToggle.isOn);

        if (saveToPrefs)
        {
            // Save the current values as Player Preferences (the ones that Unity doesn't save automatically).
            SavePrefs();
        }
    }

    // Save the current values as Player Preferences (the ones that Unity doesn't save automatically).
    private void SavePrefs()
    {
        PlayerPrefs.SetFloat(AudioManager.Instance.masterVolume_PrefName, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(AudioManager.Instance.soundVolume_PrefName, soundVolumeSlider.value);
        PlayerPrefs.SetFloat(AudioManager.Instance.musicVolume_PrefName, musicVolumeSlider.value);
        PlayerPrefs.SetInt(AudioManager.Instance.audioSettingsSaved_PrefName, 1);
    }

    // Closes the Settings menu, opening up the appropriate previous menu.
    private void CloseSettingsMenu()
    {
        previousMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    #endregion Dev Methods


    #region UI Callback Methods
    // Called when the Player clicks on the Back button of the Settings Menu.
    public void SettingsMenu_OnBackButtonClicked()
    {
        // Close the Settings menu, going back to the previous menu.
        CloseSettingsMenu();
    }

    // Called when the Player clicks on the Back button of the Settings Menu.
    public void SettingsMenu_OnApplyButtonClicked()
    {
        // Apply.
        ApplyChoicesToGame(true);
    }
    #endregion UI Callback Methods
}
