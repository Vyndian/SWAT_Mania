using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region Fields
    [Header("Object & Component References")]

    [SerializeField, Tooltip("The Health (on the Player or Enemy) that this HealthBar represents.")]
    private Health target;

    [SerializeField, Tooltip("The slider of this health bar.")]
    private Slider slider;

    [SerializeField, Tooltip("The TextMeshPro used for the percent shown on the PLAYER's health bar.")]
    private TextMeshProUGUI playerHealthValueTextMesh;


    #endregion Fields


    #region Unity Methods
    // Called immediately after being instantiated.
    private void Awake()
    {
        // If any of these are null, try to set them up.
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
    // Sets this health bar's target.
    public void SetTarget(Health newTarget)
    {
        target = newTarget;
    }

    // Update the health bar's percentage.
    public void UpdateHealthBar()
    {
        float percentage = target.GetHealthPercentage();
        slider.value = percentage;

        // If the playerHealthValueTextMesh is NOT null (if this is the Player),
        if (playerHealthValueTextMesh != null)
        {
            // then update the text to match the value.
            playerHealthValueTextMesh.text = (percentage * 100).ToString() + "%";
        }
    }

    /* Destroys the healthBar. This is to prevent the healthBar hanging around when the enemy
     * dies and goes Ragdoll. If not destroyed, it looks odd and blocks view of the dropped loot. */
    public void RemoveHealthBar()
    {
        Destroy(gameObject);
    }
    #endregion Dev Methods
}
