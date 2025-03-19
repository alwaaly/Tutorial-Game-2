using UnityEngine;
using UnityEngine.EventSystems;
public class UI_ButtonSound : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {
    [SerializeField] AudioClip buttonDownSound;
    [SerializeField] AudioClip buttonUpSound;
    [SerializeField] AudioSource buttonSoundSource;
    public void OnPointerDown(PointerEventData eventData) {
        if (buttonDownSound != null) {
            buttonSoundSource.clip = buttonDownSound;
            buttonSoundSource.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (buttonUpSound != null) {
            buttonSoundSource.clip = buttonUpSound;
            buttonSoundSource.Play();
        }
    }
}
