using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

/// <summary>
///	 
/// </summary>

public class SliderPlayAnimation : MonoBehaviour
{
    protected AvatarManager avatarManager;
    protected DrawManager drawManager;
    protected UIManager uiManager;
    public Slider slider;

    public Text textChrono;
    public Button playButton;
    public Button pauseButton;

    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();
        slider.minValue = 0f;

        drawManager.RegisterSliderAnimation(this);
    }

    void Destroy()
    {
        drawManager.UnregisterSliderAnimation();
    }

    void Update()
    {
        if (drawManager.timeElapsed > 0 && !drawManager.IsPaused)
            textChrono.text = drawManager.CurrentTime + " s";
    }

    public void SetSlider(int value)
    {
        slider.value = value;
    }

    public void ShowPlayButton()
    {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void ShowPauseButton()
    {
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }
    
    public void OnPlayAnimationSlider()
    {
        if (drawManager.IsEditing) return;
        
        slider.maxValue = (float)drawManager.NumberFrames - 1;
        var _currentFrame = (int)slider.value;
        for (int i = 0; i < avatarManager.NumberOfLoadedAvatars; i++)
            drawManager.SetCurrrentFrame(0, _currentFrame);
        textChrono.text = drawManager.CurrentTime + " s";

        if (!drawManager.IsPaused && ((int)slider.value == 0 || (int)slider.value == (int)slider.maxValue || !drawManager.ShouldContinuePlaying(0)) )
        {
            drawManager.SetCanResumeAnimation(false);
            ShowPlayButton();
        }

        if (Input.GetMouseButton(0))
        {
            ShowPlayButton();
            drawManager.Pause();
            for (int i = 0; i < avatarManager.NumberOfLoadedAvatars; i++)
                drawManager.PlayOneFrame(i);
        }
    }

    public void EnableSlider()
    {
        slider.interactable = true;
        playButton.interactable = true;
        pauseButton.interactable = true;
    }

    public void DisableSlider()
    {
        slider.interactable = false;
        playButton.interactable = false;
        pauseButton.interactable = false;
    }

}