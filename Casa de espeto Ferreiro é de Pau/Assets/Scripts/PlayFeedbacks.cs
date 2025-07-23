using UnityEngine;

public class PlayFeedbacks : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public TweenObject Tween;

    public void PlayParticle()
    {
        ParticleSystem.Play();
    }

    public void PlayTween()
    {
        Tween.PlayTween();
    }
}
