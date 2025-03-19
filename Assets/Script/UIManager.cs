using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] GameObject uiCanvas;

    [SerializeField] AudioSource[] soundEffectSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] Slider soundEffectSlider;
    [SerializeField] Slider musicSlider;

    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI progressTextUGUI;
    [SerializeField] GameObject settingPanal;
    [SerializeField] GameObject mainMenuPanal;

    public static UIManager Instance;
    private void Awake() {
        if (Instance == null) Instance = this;
        else Debug.LogError("There is more then one UIManager");

        if(SceneManager.GetActiveScene().buildIndex != 0)
            LevelManager.Instance.mainCharctere.OnPlayerDie += OnPlayerDie_OnPlayerDie;

        Data data = SaveSystem.Load();
        if (data != null) {
            soundEffectSlider.value = data.SoundEffectVolum;
            musicSlider.value = data.MusicVolum;
            if (SceneManager.GetActiveScene().buildIndex == 0) progressTextUGUI.text = "Max Score: " + data.MaxProgress.ToString();
        }
        else progressTextUGUI.text = "Max Score: 0";

        OnSoundEffectVolumeChange();
        OnMusicVolumeChange();
    }

    private void OnPlayerDie_OnPlayerDie() {
        ShowUI();
    }

    public void ShowUI() {
        uiCanvas.SetActive(true);
        progressTextUGUI.text = "Progress: " + LevelManager.Instance.mainCharctere.wallPassed;
        animator.CrossFade("Start", 0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnSoundEffectVolumeChange() {
        for (int i = 0; i < soundEffectSource.Length; i++) {
            soundEffectSource[i].volume = soundEffectSlider.value;
        }
    }
    public void OnMusicVolumeChange() {
        musicSource.volume = musicSlider.value;
    }
    public float GetMusicVolum() {
        return musicSlider.value;
    }
    public float GetSoundEffectVolum() {
        return soundEffectSlider.value;
    }
    public void Exit() {
        SaveSystem.Save();
        Application.Quit();
    }
    public void RePlay() {
        SaveSystem.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ShowSettingPanal() {
        settingPanal.gameObject.SetActive(true);
        mainMenuPanal.gameObject.SetActive(false);
    }
    public void ShowMainMenuPanal() {
        SaveSystem.Save();
        settingPanal.gameObject.SetActive(false);
        mainMenuPanal.gameObject.SetActive(true);
    }
    public void StartGame() {
        SaveSystem.Save();
        SceneManager.LoadScene(1);
    }
}
