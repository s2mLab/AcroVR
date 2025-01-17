using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;


public class BaseProfile : LevelBase
{
    protected AvatarManager avatarManager;
    protected LevelManager levelManager;
    protected GameManager gameManager;
    protected DrawManager drawManager;
    protected AniGraphManager aniGraphManager;
    protected UIManager uiManager;
    protected MissionManager missionManager;

    public GameObject Avatar { get => avatarManager.LoadedModels[0].gameObject; }
    public GameObject SaveLoadCompareMenu;
    public Dropdown dropDownCondition;
    public Dropdown dropDownDDLNames;
    public Camera AvatarCamera;

    public Dropdown dropDownPlaySpeed;

    public GameObject ErrorObject;
    public GameObject TutorialObject;
    public MissionBanner missionBanner;
    public GameObject NodeNameObject;
    protected GameObject CurrentTabContent;
    public Toggle ToggleSimulationButton;
    public Toggle ToggleGestureButton;
    public Button CompareButton;
    public Button StopCompareButton;

    public Text ConditionName;
    
    public UserUIInputs userUiInputs;
    protected UserUIInputsValues userUiInputsDefaultValues = new UserUIInputsValues();

    public Text endFrameText;

    public GameObject[] cameraList;

    public SliderPlayAnimation sliderPlay;

    public Fireworks fireworks;

    public GameObject Floor;

    public Text fileName;

    public enum CameraView
    {
        FirstPOV,
        FrontPOV,
    }

    public CameraView camView;

    public override void SetPrefab(GameObject _prefab)
    {
    }

    public override void CreateLevel()
    {
    }

    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        levelManager = ToolBox.GetInstance().GetManager<LevelManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        aniGraphManager = ToolBox.GetInstance().GetManager<AniGraphManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();

        // Fill some important informations
        if (cameraList.Length != 0){  // cameraList.Length is 0 if we are in the menu
            userUiInputsDefaultValues.SetAll(userUiInputs);
            avatarManager.LoadAvatar(0);
            drawManager.Pause();
                
            // Give some handler to relevant scripts
            drawManager.SetGround(Floor);
            PrepareMissionManager();
            if (SaveLoadCompareMenu != null)
                SaveLoadCompareMenu.SetActive(!missionManager.HasActiveMission);
            
            FrontCameraPOV(0);
            gameManager.UpdateDropDownNames();
        }
    }

    protected void PrepareMissionManager()
    {
        uiManager.SetUserInputs(userUiInputs, userUiInputsDefaultValues);
        missionManager.SetupFireworks(fireworks);
        missionManager.SetInformationBanner(missionBanner);
        missionManager.SetAndShowCurrentMission();
    }

    public void CheckMissionResult()
    {
        missionManager.CheckMissionResult();
    }

    private void Update()
    {
        if (aniGraphManager.isTutorial == 1)
        {
            if (Input.anyKeyDown)
            {
                aniGraphManager.isTutorial++;
                TutorialObject.GetComponent<Animator>().Play("Panel Out");
            }
        }
    }

    public void ToggleCameraFirstOrThird(){
        if (camView != CameraView.FirstPOV){
            FirstPOVCamera();
        } else {
            FrontCameraPOV(drawManager.CheckPositionAvatar(0));
        }

    }

    public void SwitchCameraView()
    {
        if (Avatar == null)
        {
            string errorMessage = MainParameters.Instance.languages.current == Language.English 
                ? "Please load files first" 
                : "SVP charger d'abord les fichiers";
            ErrorMessage(errorMessage);
            return;
        }

        if (camView == CameraView.FirstPOV)
            FirstPOVCamera();
        else
            FrontCameraPOV(drawManager.CheckPositionAvatar(0));
    }

    public void FrontCameraPOV(float _v)
    {
        if (cameraList == null || cameraList.Length < 16) return;

        if(cameraList[15] == null)
            cameraList[15] = drawManager.GetFirstViewTransform();

        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if(cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if(Avatar != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = Avatar.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }


        if(_v >= 0 && _v < 2)
            cameraList[0].gameObject.SetActive(true);
        else if(_v >= 2 && _v < 4)
            cameraList[1].gameObject.SetActive(true);
        else if(_v >= 4 && _v < 6)
            cameraList[2].gameObject.SetActive(true);
        else if (_v >= 6 && _v < 8)
            cameraList[3].gameObject.SetActive(true);
        else
            cameraList[4].gameObject.SetActive(true);

        camView = CameraView.FrontPOV;
    }

    public void FirstPOVCamera()
    {
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if (cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if (Avatar != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = Avatar.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }

        cameraList[15].gameObject.SetActive(true);

        camView = CameraView.FirstPOV;
    }

    public void BackToMenu()
    {
        gameManager.InitAnimationInfo();
        drawManager.StopEditing();
        aniGraphManager.cntAvatar = 0;
        drawManager.Pause();
        drawManager.ResetFrame();
        aniGraphManager.isTutorial = 0;
        missionManager.UnloadMission();

        levelManager.GotoScreen("MainMenu");
    }

    public void ToProfile()
    {
        levelManager.GotoScreen("Profile");
    }

    public void ToTraining()
    {
        levelManager.GotoScreen("Training");
    }

    public void ToNextLevel()
    {
        levelManager.NextLevel();
    }

    public void ToQuit()
    {
        if (Application.isEditor)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Application.Quit();
        }
    }

    public void ToBaseLevel1()
    {
        levelManager.GotoScreen("BaseLevel1");
    }

    public void AnimationLoad()
    {
        int _avatarIndex = 0;
        int ret = gameManager.AnimationLoad(_avatarIndex);

        if (ret < 0)
        {
            string errorMessage;
            if (ret == -1)
                errorMessage = MainParameters.Instance.languages.current == Language.English
                    ? "Please load files first"
                    : "SVP charger d'abord les fichiers";
            else
                errorMessage = MainParameters.Instance.languages.current == Language.English
                    ? "Loaded incorrect Simulation files:  " + ret.ToString()
                    : "Fichiers de simulation incorrects chargés:  " + ret.ToString();
            ErrorMessage(errorMessage);
            return;
        }

        fileName.text = Path.GetFileName(avatarManager.LoadedModels[_avatarIndex].Joints.fileName);
        StartCoroutine(WaitThenForceUpdate());
    }

    public void CompareLoad()
    {
        int _avatarIndex = 1;

        avatarManager.LoadAvatar(_avatarIndex);
        SwitchCameraView();  // For some reason, the camera moves to first person. So we reset the settings to whatever it was already
        int ret = gameManager.AnimationLoad(_avatarIndex);

        if (ret < 0)
        {
            avatarManager.DestroyAvatar(_avatarIndex);
            string errorMessage;
            if (ret == -1)
                errorMessage = MainParameters.Instance.languages.current == Language.English
                    ? "Please load files first" 
                    : "SVP charger d'abord les fichiers";
            else
                errorMessage = MainParameters.Instance.languages.current == Language.English
                    ? "Loaded incorrect Simulation files:  " + ret.ToString()
                    : "Fichiers de simulation incorrects chargés:  " + ret.ToString();
            ErrorMessage(errorMessage);
            return;
        }
        drawManager.Pause();
        drawManager.MakeSimulationFrame(_avatarIndex);
        drawManager.PlayOneFrame(_avatarIndex);  // Force the avatar to conform to its first frame

        aniGraphManager.cntAvatar = 1;

        CompareButton.gameObject.SetActive(false);
        StopCompareButton.gameObject.SetActive(true);
        StartCoroutine(WaitThenForceUpdate());
    }

    public void RemoveCompareAvatar(){
        int _avatarIndex = 1;
        avatarManager.DestroyAvatar(_avatarIndex);
        aniGraphManager.cntAvatar = 0;

        CompareButton.gameObject.SetActive(true);
        StopCompareButton.gameObject.SetActive(false);

        StartCoroutine(WaitThenForceUpdate());
    }

    IEnumerator WaitThenForceUpdate(){
        // Wait a full frame to make sure everything that should be clean is actually cleaned
        yield return null;
        drawManager.ForceFullUpdate();
        drawManager.ForceResultShowUpdate();
    }

    public void SaveFile()
    {
        gameManager.SaveFile();
    }

    public void TakeOffOn()
    {
        aniGraphManager.GraphOn();
    }

    public void TakeOffOff()
    {
        aniGraphManager.TaskOffGraphOff();
    }

    public void InitDropdownDDLNames(int ddl)
    {
        if (uiManager.GetCurrentTab() != 2) return;

        if (Avatar == null)
        {
            if (MainParameters.Instance.languages.current == Language.English)
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        List<string> dropDownOptions = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            if (i == 0) dropDownOptions.Add("Hanche_Flexion");
            else if (i == 1) dropDownOptions.Add("Genou_Flexion");
            else if (i == 2) dropDownOptions.Add("Bras_Droit_Flexion");
            else if (i == 3) dropDownOptions.Add("Bras_Droit_Abduction");
            else if (i == 4) dropDownOptions.Add("Bras_Gauche_Flexion");
            else if (i == 5) dropDownOptions.Add("Bras_Gauche_Abduction");
        }
        dropDownDDLNames.ClearOptions();
        dropDownDDLNames.AddOptions(dropDownOptions);
        if (ddl >= 0)
        {
            dropDownDDLNames.value = ddl;
        }
    }

    public void DisplayDDL(int ddl, bool axisRange)
    {
        if (avatarManager.LoadedModels[0].Joints.nodes == null) return;

        if (ddl >= 0)
        {
            aniGraphManager.DisplayCurveAndNodes(0, ddl, axisRange);
            if (avatarManager.LoadedModels[0].Joints.nodes[ddl].ddlOppositeSide >= 0)
            {
                aniGraphManager.DisplayCurveAndNodes(1, avatarManager.LoadedModels[0].Joints.nodes[ddl].ddlOppositeSide, axisRange);
            }
        }
    }

    public void DropDownDDLNamesOnValueChanged(int value)
    {
        DisplayDDL(value, true);
    }


    public void ThirdPOVCamera()
    {
        if (Avatar == null)
        {
            if (MainParameters.Instance.languages.current == Language.English)
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        FrontCameraPOV(drawManager.CheckPositionAvatar(0));
    }

    public void PlayAvatar()
    {
        if (Avatar == null)
        {
            if (MainParameters.Instance.languages.current == Language.English)
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        drawManager.ShowAvatar(0);
    }

    public void PlayAvatarButton()
    {
        if (drawManager.canResumeAnimation)
        {
            PauseAvatarButton();
            return;
        }

        drawManager.Resume();

        if (Avatar == null || !Avatar.activeSelf)
        {
            return;
        }

        string playSpeed = dropDownPlaySpeed.captionText.text;
        if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow3)
            drawManager.SetAnimationSpeed(10);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow2)
            drawManager.SetAnimationSpeed(3);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow1)
            drawManager.SetAnimationSpeed(1.5f);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedNormal)
            drawManager.SetAnimationSpeed(1);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedFast)
            drawManager.SetAnimationSpeed(0.8f);

        drawManager.StopEditing();
        drawManager.PlayAvatar(0);

        SwitchCameraView();

        TakeOffOn();

        sliderPlay.ShowPauseButton();
    }

    public void PauseAvatarButton()
    {
        if (!drawManager.PauseAvatar(0)) 
            return;
        if (drawManager.IsPaused)
        {
            sliderPlay.ShowPlayButton();
        } 
        else
        {
            sliderPlay.ShowPauseButton();
        }
    }

    public void SetTab(TabProperties _properties)
    {
        uiManager.SetCurrentTab(_properties.TabIndex);
        
        if (CurrentTabContent != null)
            CurrentTabContent.SetActive(false);
        CurrentTabContent = _properties.Content;
        CurrentTabContent.SetActive(true);
        
        _properties.BackgroundImage.sprite = _properties.BackgroundSprite;
        if (_properties.IsAGestureMode){
            // We have to trigger the button that calls themselves the method SetGestureMode
            // Otherwise the button won't update. If you put this isOn inside the method, then
            // we get a circular call
            ToggleSimulationButton.isOn = false;
            ToggleGestureButton.isOn = true;
        }
        else {
            // We have to trigger the button that calls themselves the method SetSimulationMode
            // Otherwise the button won't update. If you put this isOn inside the method, then
            // we get a circular call
            ToggleGestureButton.isOn = false;
            ToggleSimulationButton.isOn = true;
        }

        drawManager.ForceFullUpdate();
        SwitchCameraView();

        if (_properties.HasTutorial)
            TutorialMessage();

    }

    public void SetFrench()
    {
        MainParameters.Instance.SetLanguage(Language.French);
    }

    public void SetEnglish()
    {
        MainParameters.Instance.SetLanguage(Language.English);
    }

    public void SetTooltip(bool _flag)
    {
        uiManager.SetTooltip(_flag);
    }

    public void SetMaleAvatar()
    {
        avatarManager.SelectAvatar(AvatarManager.Model.SingleMale);
    }

    public void SetFemaleAvatar()
    {
        avatarManager.SelectAvatar(AvatarManager.Model.SingleFemale);
    }

    public void SetSimulationMode()
    {
        drawManager.ActivateSimulationMode();
    }

    public void SetGestureMode()
    {
        drawManager.ActivateGestureMode();
    }

    public void ErrorMessage(string _msg)
    {
        ErrorObject.GetComponent<Animator>().Play("Panel In");
        ErrorObject.GetComponentInChildren<Text>().text = _msg;
        Invoke("CloseMsg", 0.5f);
    }

    private void CloseMsg()
    {
        ErrorObject.GetComponent<Animator>().Play("Panel Out");
    }

    public void NodeName(string _msg)
    {
        NodeNameObject.GetComponent<Animator>().Play("Panel In");
        NodeNameObject.GetComponentInChildren<Text>().text = _msg;
        Invoke("CloseNodeName", 0.5f);
    }

    private void CloseNodeName()
    {
        NodeNameObject.GetComponent<Animator>().Play("Panel Out");
    }

    public void TutorialMessage()
    {
        if (Avatar == null) return;

        if(aniGraphManager.isTutorial == 0)
        {
            int _avatarIndex = 0;
            TakeOffOn();
            InitDropdownDDLNames(_avatarIndex);
            gameManager.InterpolationDDL(_avatarIndex);
            gameManager.DisplayDDL(0, true);

            aniGraphManager.isTutorial++;
            TutorialObject.GetComponent<Animator>().Play("Panel In");

            if (MainParameters.Instance.languages.current == Language.English)
            {
                TutorialObject.GetComponentInChildren<Text>().text = "1. Mouse Right Button: On/Off Rotation Modifier\n" +
                    "2. Each shoulder can be clicked twice(x and y)\n" +
                    "3. Shift + Mouse Left Button: Rotate 3D avatar\n" +
                    "4. Mouse Drag: Change Node value";
            }
            else
            {
                TutorialObject.GetComponentInChildren<Text>().text = "1. Bouton droit de la souris: modificateur de rotation On/Off\n" +
                    "2. Chaque épaule peut être cliquée deux fois (x et y)\n" +
                    "3. Shift + Bouton gauche de la souris: faire pivoter l'avatar 3D\n" +
                    "4. Glisser la souris: modifier la valeur du noeud";
            }
        }
    }

    public void ToggleGravity()
    {
        uiManager.ToggleGravity();
    }

    public void ToggleStopAtGround()
    {
        uiManager.ToggleStopOnGround();
    }

    public void SelectPresetCondition(){
        uiManager.SetDropDownPresetCondition(uiManager.userInputs.PresetConditions.value);
        uiManager.UpdateAllPropertiesFromDropdown();
    }

    public void AddPresetCondition(Text name)
    {
        if (name.text != "")
        {
            gameManager.SaveCondition(name.text);
        }
    }

    public void DeletePresetCondition()
    {
        gameManager.RemoveCondition(uiManager.userInputs.PresetConditions.value);
    }

    public void NamePresetCondition()
    {
        ConditionName.text = gameManager.PresetConditions.conditions[uiManager.userInputs.PresetConditions.value].name;
    }
}