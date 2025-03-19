using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour {
    public TextMeshProUGUI textUGUI;
    public Image Image;
    public bool IsOpen;
    [SerializeField] BoxCollider coll;
    private void OnValidate() {
        if (textUGUI == null) textUGUI = transform.parent.GetComponentInChildren<TextMeshProUGUI>();
        if (coll == null) coll = GetComponent<BoxCollider>();
        if(Image == null) Image = transform.parent.GetComponentInChildren<Image>();
    }

    private void OnEnable() {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        coll.enabled = true;
    }

    float progress;
    public async void Open() {
        LevelManager.Instance.DooraudioSource.Play();
        progress = 0;
        coll.enabled = false;
        while (progress <= 1) {
            progress += Time.deltaTime * 5;

            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, -90)), progress);

            await Awaitable.NextFrameAsync();
        }
    }
}
