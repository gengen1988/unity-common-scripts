using HighlightPlus;
using UnityEngine;

public class HitFlashCtrl : MonoBehaviour
{
    private Hurtable _hurtable;
    private HighlightEffect _highlightEffect;

    private void Awake()
    {
        TryGetComponent(out _highlightEffect);
        _hurtable = GetComponentInParent<Hurtable>();
    }

    private void OnEnable()
    {
        _hurtable.OnHurtBegin += HandleHurtBegin;
    }

    private void OnDisable()
    {
        _hurtable.OnHurtBegin -= HandleHurtBegin;
    }

    private void HandleHurtBegin(HitInfo evt)
    {
        if (!evt.Participants.HurtEntity)
        {
            return;
        }

        _highlightEffect.HitFX();
    }
}