using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
public class Head : MonoBehaviour {
    [SerializeField] Rigidbody rb;
    [SerializeField] NoiseSettings hitNoise;
    [SerializeField] CinemachinePanTilt panTilt;
    [SerializeField] CinemachineBasicMultiChannelPerlin perlin;
    [SerializeField] VolumeProfile profile;
    private void OnValidate() {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }
    private void Awake() {
        LevelManager.Instance.mainCharctere.OnPlayerDie += MainCharctere_OnPlayerDie;
    }

    private void MainCharctere_OnPlayerDie() {
        rb.isKinematic = false;
        rb.useGravity = true;
        transform.parent = null;
        rb.AddForce(Vector3.forward * 1000);
        perlin.NoiseProfile = hitNoise;

        panTilt.enabled = false;
        profile.TryGet(out DepthOfField component);
        component.active = true;

        profile.TryGet(out Vignette vignette);
        vignette.intensity.Override(.4f);
        vignette.smoothness.Override(1);

        StopDeathAnimation();
    }
    async void StopDeathAnimation() {
        await Awaitable.WaitForSecondsAsync(2);
        rb.isKinematic = true;
        rb.useGravity = false;
        perlin.AmplitudeGain = 0;
    }
    private void OnDisable() {
        LevelManager.Instance.mainCharctere.OnPlayerDie -= MainCharctere_OnPlayerDie;
        profile.TryGet(out DepthOfField component);
        component.active = false;


        profile.TryGet(out Vignette vignette);
        vignette.intensity.Override(.2f);
        vignette.smoothness.Override(.2f);
    }
}
