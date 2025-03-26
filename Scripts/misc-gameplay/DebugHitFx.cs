using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugHitFx : MonoBehaviour
{
    [Button]
    private void TestHitFx()
    {
        var highlightFx = GetComponent<HighlightEffect>();
        highlightFx.HitFX();
    }
}