using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image FillImage;

    private CanvasGroup _cg;

    private void Awake()
    {
        TryGetComponent(out _cg);
    }

    public void SetRatio(float value)
    {
        FillImage.fillAmount = value;
    }

    public void SetAlpha(float value)
    {
        _cg.alpha = value;
    }
}