using UnityEngine;

public class Footstep : MonoBehaviour
{
    #region Fields
    [Header("Audio for Footstep")]

    [SerializeField, Tooltip("The audio source to play through.")]
    private AudioSource source;

    [SerializeField, Tooltip("The footstep audio clip.")]
    private AudioClip clip;
    #endregion Fields


    #region Unity Methods
    
    #endregion Unity Methods


    #region Dev Methods
    public void AnimationEventFootstep()
    {
        source.PlayOneShot(clip);
    }
    #endregion Dev Methods
}
