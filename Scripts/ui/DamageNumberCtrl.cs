using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumberCtrl : MonoBehaviour
{
    [SerializeField] private Vector2 randomSize;
    [SerializeField] private float moveDistanceY = 20f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private TextMeshProUGUI textCtrl;
    [SerializeField] private CanvasGroup fadeCtrl;

    private Sequence _seq;

    public void Show(int damageAmount)
    {
        textCtrl.text = $"{damageAmount / 100:F0}";
        StartCoroutine(TweenAnimation());
    }

    private IEnumerator TweenAnimation()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence();
        fadeCtrl.alpha = 1;
        var offset = RandomUtil.PointInBox(randomSize);
        var destination = transform.position + Vector3.up * moveDistanceY + offset;
        _seq.Insert(0, transform.DOScale(1.5f, moveDuration).From(1));
        _seq.Insert(0, transform.DOMove(destination, moveDuration));
        _seq.Insert(1, fadeCtrl.DOFade(0, fadeDuration));
        _seq.Play();
        yield return _seq.WaitForCompletion();
        PoolUtil.Despawn(gameObject);
    }
}