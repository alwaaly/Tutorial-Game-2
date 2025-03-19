using System;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public class MainCharctere : MonoBehaviour {
    InputSystem_Actions actions;

    [SerializeField] float speed = 5;
    [SerializeField] float zoomSpeed = 5;
    [SerializeField] float rightCorrner;
    [SerializeField] float leftCorrner;
    [SerializeField] CinemachineCamera cam;

    [SerializeField] float ZoomFOV = 10;

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip hitDoorClip;
    [SerializeField] AudioClip hitWallclip;

    float defaultFOV = 40;
    float rightDir;

    public int wallPassed { get; private set; }
    public event Action OnPlayerDie;
    bool isCharecterDie;
    private void Awake() {
        actions = new();

        actions.Player.Move.performed += Move_performed;
        actions.Player.Move.canceled += Move_canceled;

        actions.Player.Zoom.started += Zoom_started;
        actions.Player.Zoom.canceled += Zoom_canceled;

        actions.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        audioTokenSource = new();
        HandelRunningSound(audioTokenSource.Token);
    }
    private void OnTriggerEnter(Collider other) {
        if (isCharecterDie) return;
        if (other.transform.TryGetComponent(out Door component)) {
            if (component.IsOpen) {
                component.Open();
                wallPassed++;
                if (wallPassed % 4 == 0) {
                    if (wallPassed % 8 == 0)
                        LevelManager.Instance.TheStateOFRandomness = (LevelManager.RandomState) UnityEngine.Random.Range(0,Enum.GetValues(typeof(LevelManager.RandomState)).Length);
                    LevelManager.Instance.Generate();
                }
            }
            else {
                dieAudioClip(hitDoorClip);
                Die();
            }
        }
        else {
            dieAudioClip(hitWallclip);
            Die();
        }
    }
    private void Die() {
        isCharecterDie = true;
        speed = 0;
        OnPlayerDie?.Invoke();
    }
    private void Zoom_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (tokenSource != null) tokenSource.Cancel();
        tokenSource = new();
        ZoomOut(tokenSource.Token);
    }

    private void Zoom_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if(tokenSource != null) tokenSource.Cancel();
        tokenSource = new();
        ZoomIn(tokenSource.Token);
    }

    private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        rightDir = context.ReadValue<Vector2>().x;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        rightDir = context.ReadValue<Vector2>().x;
    }
    private void Update() {
        transform.position += Vector3.right * rightDir * speed * Time.deltaTime;
        if (transform.position.x > rightCorrner) transform.position = new Vector3(rightCorrner, transform.position.y, transform.position.z);
        if (transform.position.x < leftCorrner) transform.position = new Vector3(leftCorrner, transform.position.y, transform.position.z);
    }

    CancellationTokenSource audioTokenSource;
    float runningSoundRepet;
    private async void HandelRunningSound(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            source.pitch = UnityEngine.Random.Range(1f, 2f);
            source.Play();
            runningSoundRepet = .3f;
            await Awaitable.WaitForSecondsAsync(runningSoundRepet);
        }
    }
    private void dieAudioClip(AudioClip clip) {
        audioTokenSource.Cancel();
        audioTokenSource.Dispose();

        source.clip = clip;
        source.Play();
    }


    float progress;
    CancellationTokenSource tokenSource;
    async void ZoomIn(CancellationToken token) {
        try {
            while (progress <= 1) {
                progress += Time.deltaTime * zoomSpeed;
                cam.Lens.FieldOfView = Mathf.Lerp(defaultFOV, ZoomFOV, progress);
                await Awaitable.NextFrameAsync(token);
            }
        }
        catch (OperationCanceledException) {}
    }
    async void ZoomOut(CancellationToken token) {
        try {
            while (progress >= 0) {
                progress -= Time.deltaTime * zoomSpeed;
                cam.Lens.FieldOfView = Mathf.Lerp(defaultFOV, ZoomFOV, progress);
                await Awaitable.NextFrameAsync(token);
            }
        }
        catch (OperationCanceledException) {}
    }
    private void OnDisable() {
        actions.Player.Move.performed -= Move_performed;
        actions.Player.Move.canceled -= Move_canceled;
        actions.Player.Zoom.started -= Zoom_started;
        actions.Player.Zoom.canceled -= Zoom_canceled;
        actions.Disable();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(new Vector3(rightCorrner, transform.position.y, transform.position.z),.1f);
        Gizmos.DrawWireSphere(new Vector3(leftCorrner, transform.position.y, transform.position.z),.1f);
    }
}
