using UnityEngine;

public class PlayFeedbacks : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public TweenObject Tween;
    public AudioClip SFX;

    public void PlayParticle()
    {
        ParticleSystem.Play();
    }

    public void PlayTween()
    {
        Tween.PlayTween();
    }

    public void PlaySFX()
    {
        AudioManager.Instance.PlaySFX(SFX);
    }
}
