using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private float musicVolume, sfxVolume, ResolutionIndex;
    [SerializeField] bool gamePaused = false, pauseableFrame = true;
    private int[] ScreenWidth = {800, 1024, 1152, 1280, 1280, 1440, 1600, 1920, 2560}, ScreenHeight = {600, 768, 864, 720, 1024, 900, 1200, 1080, 1440}; // [0, 8]
    [SerializeField] Slider MusicSlider, SfxSlider, ResolutionSlider;
    GameObject[] MusicComponent, SFXComponent;
    [SerializeField] GameObject HUD, Main, Setting, SaveMenu;
    [SerializeField] Text MusicDisplay, SFX_Display, ResolutionDisplay;
    [SerializeField] Animator ScreenFader;
    [SerializeField] GameObject resumeButton, playButton, loadButton; // load button is the level select button

    [SerializeField] playerMovement movementScript;
    [SerializeField] SaveAndLoad SaveDataScript;

    void Awake()
    {
        Application.targetFrameRate = 65;

        if (SceneManager.GetActiveScene().buildIndex > 0 && SceneManager.GetActiveScene().buildIndex < 8)
        {
            ResumeGame();
            ApplyPlayerSettings();
        } else
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    movementScript.transform.gameObject.SetActive(false);
                    BackToMainCanvas();
                }

                if (SceneManager.GetActiveScene().buildIndex == 8)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
            }
    }

    void Start()
    {
        MusicComponent = GameObject.FindGameObjectsWithTag("Music");
        SFXComponent = GameObject.FindGameObjectsWithTag("SFX");

        StartFadingIn();
    }

    public void ConfigForNewLevel()
    {
        Application.targetFrameRate = 65;
        
        if (Setting != null)
        {
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                ResumeGame();
            } else
                {
                    BackToMainCanvas();
                }
    
            // Get all objects in the scene that have a script for editing
            MusicComponent = GameObject.FindGameObjectsWithTag("Music");
            SFXComponent = GameObject.FindGameObjectsWithTag("SFX");
    
            ApplyPlayerSettings();
        }
    }

    public void PauseBuffer(bool value)
    {
        pauseableFrame = value;
        // print(pauseableFrame);
    }

    public void ExternalPause()
    {
        GoToSettings();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0 && SceneManager.GetActiveScene().buildIndex < 8)
        {            
            loadButton.SetActive(false);
            playButton.SetActive(false);

            resumeButton.SetActive(true);

            if (Input.GetKeyDown(KeyCode. Tab) && pauseableFrame) // player isnt on the desktop
            {
                if (!gamePaused)
                {
                    GoToSettings();
                    Time.timeScale = 0f;
                } else
                    {
                        ResumeGame();
                    }
            }
        } else
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    loadButton.SetActive(true);
                    playButton.SetActive(true);
    
                    resumeButton.SetActive(false);
                }
            }

        if (SceneManager.GetActiveScene().buildIndex != 8)
        {
            if (Setting.activeSelf && Setting != null)
            {
                ResolutionIndex = ResolutionSlider.value;
    
                MusicDisplay.text = "Music: " + MusicSlider.value * 10 + " %";
                SFX_Display.text = "SFX: " + SfxSlider.value * 10 + " %";
                ResolutionDisplay.text = ScreenWidth[(int)ResolutionIndex] + "x" + ScreenHeight[(int)ResolutionIndex];
    
                SetAudioSettings();
            }
        }
    }

//========================== GAME SETTINGS =====================================
    public void ApplyPlayerSettings()
    {
        // Screen.SetResolution((int) PlayerPrefs.GetFloat("ScreenWidth"), (int) PlayerPrefs.GetFloat("ScreenHeight"), true);

        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol") * 10;
        SfxSlider.value = PlayerPrefs.GetFloat("SFXVol") * 10;
        ResolutionSlider.value = PlayerPrefs.GetFloat("ResSlideVal");
        ResolutionIndex = PlayerPrefs.GetFloat("ResSlideVal");

        SetResolutionSettings();
        SetAudioSettings();

        Debug.Log("[Apply] => " + (int) PlayerPrefs.GetFloat("ScreenWidth") + " : " + (int) PlayerPrefs.GetFloat("ScreenHeight"));
    }

    public void SetResolutionSettings()
    {
        if (!ResolutionDisplay.gameObject.activeSelf) // webgl build doesnt use a resolution slider
        {
            ResolutionIndex = 4; // 1440:900
        }

        Screen.SetResolution(ScreenWidth[(int)ResolutionIndex], ScreenHeight[(int)ResolutionIndex], true);

        PlayerPrefs.SetFloat("ScreenWidth", ScreenWidth[(int)ResolutionIndex]);
        PlayerPrefs.SetFloat("ScreenHeight", ScreenHeight[(int)ResolutionIndex]);
        PlayerPrefs.SetFloat("ResSlideVal", ResolutionSlider.value);

        SaveDataScript.SaveNewData();
    }

    private void SetAudioSettings()
    {
        MusicComponent = GameObject.FindGameObjectsWithTag("Music");
        SFXComponent = GameObject.FindGameObjectsWithTag("SFX");

        musicVolume = MusicSlider.value / 10;
        sfxVolume = SfxSlider.value / 10;

        PlayerPrefs.SetFloat("MusicVol", musicVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);

        PlayerPrefs.SetFloat("MusicSlideVal", MusicSlider.value);
        PlayerPrefs.SetFloat("SFXSlideVal", SfxSlider.value);

        if (MusicComponent != null && SFXComponent != null)
        {
            foreach (GameObject Sound in MusicComponent)
            {
                Sound.GetComponent<AudioSource>().volume = musicVolume;
            }
    
            foreach (GameObject Sound in SFXComponent)
            {
                Sound.GetComponent<AudioSource>().volume = sfxVolume;
            }
        }
    }

    public void StartFadingIn()
    {
        if (MusicComponent != null && SFXComponent != null)
        {
            foreach (GameObject Sound in MusicComponent)
            {
                StartCoroutine(FadeIn(Sound.GetComponent<AudioSource>(), 0.25f));
            }
    
            foreach (GameObject Sound in SFXComponent)
            {
                StartCoroutine(FadeIn(Sound.GetComponent<AudioSource>(), 0.25f));
            }
        }
    }

    public void StartFadingOut()
    {
        if (MusicComponent != null && SFXComponent != null)
        {
            foreach (GameObject Sound in MusicComponent)
            {
                StartCoroutine(FadeOut(Sound.GetComponent<AudioSource>(), 0.25f));
            }
    
            foreach (GameObject Sound in SFXComponent)
            {
                StartCoroutine(FadeOut(Sound.GetComponent<AudioSource>(), 0.25f));
            }
        }
    }

    IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = PlayerPrefs.GetFloat("MusicVol");
 
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }

        audioSource.volume = 0;
    }
 
    IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.1f;
 
        audioSource.volume = 0;
 
        while (audioSource.volume < PlayerPrefs.GetFloat("MusicVol"))
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.volume = PlayerPrefs.GetFloat("MusicVol");
    }
    
//======================= SCENE CHANGES ==========================

    public void EndLevelFade()
    {
        ScreenFader.Play("FadeIn");
    }

    public void StartTheGame() // transtion
    {
        ScreenFader.Play("FadeIn");
        PlayerPrefs.SetInt("ManualLoading", 0);
        StartFadingOut();
        Invoke("LoadGameScene", 1.65f);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("furthestLevelReached"));
    }

    public void ReturnToMainMenuScene() // transition
    {
        ScreenFader.Play("FadeIn");

        Invoke("GoToMain", 1f);
    }

    void GoToMain() // Main Menu Scene
    {
        SceneManager.LoadScene(0);
    }

    public void CloseApplication()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // on main menu scene
        {
            Application.Quit();
        } else
            {
                ResumeGame();
                ReturnToMainMenuScene();
            }
    }

//======================= SCREEN CHANGES ==========================

    public void GoToPauseMenu() // InGame Scenes
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        GoToSettings();
    }

    public void GoToSettings()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        HUD.SetActive(false);
        Main.SetActive(false);
        Setting.SetActive(true);
        SaveMenu.SetActive(false);

        gamePaused = true;

        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
    }

    public void LoadSave() // open menu for level select
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        HUD.SetActive(false);
        Main.SetActive(false);
        Setting.SetActive(false);
        SaveMenu.SetActive(true); // level select menu

        SaveDataScript.GetAvaliableLevels();
    }

    public void BackToMainCanvas()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        HUD.SetActive(false);
        Main.SetActive(true);
        Setting.SetActive(false);
        SaveMenu.SetActive(false);
    }

    public void ResumeGame() // InGame Scenes
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        HUD.SetActive(true);
        Main.SetActive(false);
        Setting.SetActive(false);
        SaveMenu.SetActive(false);

        gamePaused = false;

        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        Time.timeScale = 1;
    }

//==================================== Level Select ================================================

    public void SelectLevelOne()
    {
        StartCoroutine(loadSelectedLevel(1));
    }

    public void SelectLevelTwo()
    {
        StartCoroutine(loadSelectedLevel(2));
    }

    public void SelectLevelThree()
    {
        StartCoroutine(loadSelectedLevel(3));
    }

    public void SelectLevelFour()
    {
        StartCoroutine(loadSelectedLevel(4));
    }

    public void SelectLevelFive()
    {
        StartCoroutine(loadSelectedLevel(5));
    }

    public void SelectLevelSix()
    {
        StartCoroutine(loadSelectedLevel(6));
    }

    public void SelectLevelSeven()
    {
        StartCoroutine(loadSelectedLevel(7));
    }

    IEnumerator loadSelectedLevel(int level)
    {
        ScreenFader.Play("FadeIn");
        StartFadingOut();

        if (level == PlayerPrefs.GetInt("furthestLevelReached")) // selected the level their currently on
        {
            PlayerPrefs.SetInt("ManualLoading", 0);
        } else
            {
                PlayerPrefs.SetInt("ManualLoading", 1);
            }

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(level);
    }

}//EndScript