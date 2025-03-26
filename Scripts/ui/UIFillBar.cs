using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIFillBar : MonoBehaviour
{
    [SerializeField, Required] private Image FillImage;
    [ShowInInspector, PropertyRange(0, 1), ShowIf(nameof(FillImage))]
    public float Ratio
    {
        get => FillImage ? FillImage.fillAmount : default;
        set
        {
            if (FillImage)
            {
                FillImage.fillAmount = value;
            }
        }
    }
}