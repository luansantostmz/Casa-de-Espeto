using UnityEngine;

public class PlayAudioButton : BaseButton
{
    [SerializeField] AudioClip _clip;

    protected override void OnClick()
    {
        base.OnClick();

        AudioManager.Instance.PlaySFX(_clip);
    }
}
