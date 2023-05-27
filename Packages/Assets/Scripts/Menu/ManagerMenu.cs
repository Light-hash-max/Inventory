using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;
using LinkedSquad.Interactions.Audio;

public class ManagerMenu : MonoBehaviour
{
    [SerializeField] GameObject PanelMainMenu;
    [SerializeField] GameObject PanelSetting;
    [SerializeField] GameObject PanelPrivacy;
    [SerializeField] GameObject PanelPause;
    [SerializeField] GameObject CameraMenu;
    [SerializeField] GameObject CanvasMobile;
    [SerializeField] GameObject CanvasGame;
    [SerializeField] GameObject ButtonPrivacy;
    [SerializeField] GameObject ButtonExit;
    [SerializeField] GameObject Controller;
    [SerializeField] GameObject Croshair;
    [SerializeField] GameObject TimelineEnableBool;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider EffectsSlider;
    [SerializeField] AudioMixer Mixer;
    [SerializeField] UnityEvent PauseEvent;
    [SerializeField] UnityEvent ResumeEvent;
    [SerializeField] Slider SensitivitySlider;
    private float Sensitivity = 2f;

    bool MainMenu = true;
    bool PauseGameBool;
    bool PauseMenuBool;
    bool MobileControl;
    int FisrtStartGame = 0;


    // Start is called before the first frame update
    private void Start ()
    {
        
        FisrtStartGame = PlayerPrefs.GetInt("FirstStart", FisrtStartGame);
#if UNITY_STANDALONE
        {
            MobileControl = false;
            CanvasMobile.SetActive(false);
            ButtonPrivacy.SetActive(false);
            ButtonExit.SetActive(true);
            Croshair.SetActive(true);
        }
#endif
#if UNITY_ANDROID
        {
            MobileControl = true;
            CanvasMobile.SetActive(false);
        }
#endif
#if UNITY_IOS
            {
            MobileControl = true;
            CanvasMobile.SetActive(false);
            }
#endif

        if (FisrtStartGame == 0 && MobileControl == true)
        {
            FisrtStartGame = 1;
            PanelMainMenu.SetActive(false);
            PanelPrivacy.SetActive(true);
            PlayerPrefs.SetInt("FirstStart", FisrtStartGame);
        }
 
        Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
        MusicSlider.value = PlayerPrefs.GetFloat("Music", 0.7f);
        float EffectsVolume = PlayerPrefs.GetFloat("Effects", 1f);
        EffectsSlider.value = EffectsVolume;
        Mixer.SetFloat("Effects", Mathf.Log10(EffectsVolume) * 20);

        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2f);
        SensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 2f);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PauseGameBool && !PauseMenuBool && MainMenu == false && !TimelineEnableBool.activeSelf)
            {
                PauseMenu();
            }
        }
    }

    public void ChangeMusicVolume(float NewVolume)
    {
        PlayerPrefs.SetFloat("Music", NewVolume);
        Mixer.SetFloat("Music", Mathf.Log10(NewVolume) * 20);
    }

    public void ChangeEffectsVolume(float NewVolume)
    {
        PlayerPrefs.SetFloat("Effects", NewVolume);
        Mixer.SetFloat("Effects", Mathf.Log10(NewVolume) * 20);
    }

    public void StartGame ()
    {
        MainMenu = false;
        CanvasGame.SetActive(true);
        PanelMainMenu.SetActive(false);
        CameraMenu.SetActive(false);
        if (MobileControl == true)
        {
            CanvasMobile.SetActive(true);
        }

#if UNITY_STANDALONE
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
#endif
    }

    public void PauseMenu ()
    {
        CanvasMobile.SetActive(false);
        CanvasGame.SetActive(false);
        PanelPause.SetActive(true);
        Time.timeScale = 0f;
        PauseMenuBool = true;
        PauseEvent.Invoke();
        // Находим все объекты в сцене с компонентом AudioInteraction
        AudioInteraction[] audioInteractions = Object.FindObjectsOfType<AudioInteraction>();
        // Перебираем все найденные объекты
        foreach (AudioInteraction audioInteraction in audioInteractions)
        {
            // Вызываем нужный метод
            audioInteraction.OnAudioPaused(true);
        }
    }

    public void ResumeMenu()
    {   
        CanvasGame.SetActive(true);
        PanelPause.SetActive(false);
        Time.timeScale = 1f;
        PauseMenuBool = false;
        if (MobileControl == true)
        {
            CanvasMobile.SetActive(true);
        }
        ResumeEvent.Invoke();
        // Находим все объекты в сцене с компонентом AudioInteraction
        AudioInteraction[] audioInteractions = Object.FindObjectsOfType<AudioInteraction>();
        // Перебираем все найденные объекты
        foreach (AudioInteraction audioInteraction in audioInteractions)
        {
            // Вызываем нужный метод
            audioInteraction.OnAudioPaused(false);
        }
    }

    public void PauseGame() // Остановка игры, например для открытия кодового замка
    {
        PauseGameBool = true;
        Time.timeScale = 0f;
        if (MobileControl == true)
        {
            CanvasMobile.SetActive(false);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ResumeGame() // Возобновление игры
    {
        PauseGameBool = false;
        Time.timeScale = 1f;
        if (MobileControl == true)
        {
            CanvasMobile.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ExitMenu()
    {
        Time.timeScale = 1f;
        MainMenu = false;
        SceneManager.LoadScene("Level_A");
    }

    public void SettingsOpen ()
    {
        PanelSetting.SetActive(true);

        if (MainMenu == true)
        {
            PanelMainMenu.SetActive(false);
        } else
        {
            PanelPause.SetActive(false);

        }
    }

    public void SettingsClose()
    {
        PanelSetting.SetActive(false);

        if (MainMenu == true)
        {
            PanelMainMenu.SetActive(true);
            
        }
        else
        {
            PanelPause.SetActive(true);
        }
    }

    public void EnableCursor ()
    {
        if (!MobileControl)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void DisableCursor()
    {
        if (!MobileControl)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ExitGame ()
    {
        Application.Quit();
    }

    public void EnableMobileCanvas ()
    {
        if(MobileControl)
        {
            CanvasMobile.SetActive(true);
        }
    }
}
