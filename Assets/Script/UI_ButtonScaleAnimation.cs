using System.Threading;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonScaleAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] float toSize = 1.2f;
    [SerializeField] RectTransform rect;
    [SerializeField] float animationSpeed = 4;
    [SerializeField] AnimationCurve curve;
    Vector3 defaultSize;
    private void OnValidate() {
        if (rect == null) rect = transform.GetComponent<RectTransform>();
    }
    private void Awake() {
        defaultSize = rect.localScale;
        tokenSource = new();
    }
    private void OnEnable() {
        rect.localScale = defaultSize;
        tokenSource = new();
    }
    public void OnPointerEnter(PointerEventData eventData) {
        try {
            tokenSource.Cancel();
            tokenSource = new();
            GetBiggerAnimation(tokenSource.Token);
        }
        catch (ObjectDisposedException) { }
    }

    public void OnPointerExit(PointerEventData eventData) {
        try {
            tokenSource.Cancel();
            tokenSource = new();
            ToDefaultAnimation(tokenSource.Token);
        }
        catch (ObjectDisposedException) { }
    }
    float progress;
    CancellationTokenSource tokenSource;
    async void GetBiggerAnimation(CancellationToken token) {
        try {
            while (progress <= 1) {
                progress += Time.deltaTime * animationSpeed;
                rect.localScale = Vector3.Lerp(defaultSize, defaultSize * toSize, curve.Evaluate(progress));
                await Awaitable.NextFrameAsync(token);
            }
        }
        catch (OperationCanceledException) {
        }
    }
    async void ToDefaultAnimation(CancellationToken token) {
        try {
            while (progress >= 0) {
                progress -= Time.deltaTime * animationSpeed;
                rect.localScale = Vector3.Lerp(defaultSize, defaultSize * toSize, curve.Evaluate(progress));
                await Awaitable.NextFrameAsync(token);
            }
        }
        catch (OperationCanceledException) {
        }
    }
    private void OnDisable() {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }
}
