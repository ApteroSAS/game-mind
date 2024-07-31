using UnityEngine;

public class Q4_Wand : MonoBehaviour
{
    [SerializeField] private Material iceMaterial;
    [SerializeField] private AudioClip iceSound;

    private AudioSourceExtension audioSourceExtension;
    private bool soundPlayed = false;

    private void Awake()
    {
        audioSourceExtension = GetComponent<AudioSourceExtension>();
    }

    public void SetWandAttribute(PlayerAttribute playerAttribute)
    {
        if(playerAttribute == PlayerAttribute.Ice)
        {
            GetComponent<MeshRenderer>().material = iceMaterial;
            GetComponent<AudioSource>().clip = iceSound;
        }
    }

    public void AnimateWand(float currentValue)
    {
        if (!soundPlayed)
        {
            PlaySound();
        }
        currentValue = Mathf.Clamp(currentValue, 0, 30);
        transform.localEulerAngles = new Vector3(currentValue, 0, 0);
    }

    public void ResetWand()
    {
        transform.localEulerAngles = Vector3.zero;
        soundPlayed = false;
    }

    private void PlaySound()
    {
        audioSourceExtension.OnSoundPlayRandomPitch();
        soundPlayed = true;
    }
}
