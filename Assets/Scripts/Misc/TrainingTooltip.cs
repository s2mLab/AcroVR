using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingTooltip : MonoBehaviour
{
    public struct StrucLanguages
    {
        public StrucMessageLists french;
        public StrucMessageLists english;
    }

    public struct StrucMessageLists
    {
        public string Tab1Title;
        public string Tab2Title;
        public string Tab4Title;
        public string loadButton;
        public string saveButton;
        public string compareButton;
        public string Back;
        public string PerspectiveView;
        public string ConditionPreset;
        public string SimulationTime;
        public string Duration;

        public string TakeOffSomersault;
        public string TakeOffTilt;
        public string TakeOffTwist;
        public string TakeOffHorizontal;
        public string TakeOffVertical;

        public string Gravity;
        public string StopOnGround;
    }

    public StrucLanguages languages;

    public Text Tab1Title;
    public Text Tab2Title;
    public Text Tab4Title;

    public Text loadButton;
    public Text saveButton;
    public Text compareButton;

    public Text textBack;
    public Text PerspectiveView;
    public Text SimulationTime;
    public Text Duration;

    public Text textTakeOffSomersault;
    public Text textTakeOffTilt;
    public Text textTakeOffTwist;
    public Text textTakeOffHorizontal;
    public Text textTakeOffVertical;
 
    public Dropdown dropDownAnimationSpeed;
   
    public Text ConditionPreset;

    public Text TextGravity;
    public Text TextIsStopAtGround;

    public BaseProfile bp;

    private double mousePosX;
    private double mousePosY;
    protected GameObject panelToolTipPrefab;
    public GameObject panelAddRemoveNode;

    protected AniGraphManager aniGraphManager;
    protected AvatarManager avatarManager;
    protected DrawManager drawManager;
    protected GameManager gameManager;
    protected UIManager uiManager;

    private void Start()
    {
        aniGraphManager = ToolBox.GetInstance().GetManager<AniGraphManager>();
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        gameManager.InitAnimationInfo();

        languages.french.Tab1Title = "1. Décollage";
        languages.english.Tab1Title = "1. Take Off";
        languages.french.Tab2Title = "2. Mouvement";
        languages.english.Tab2Title = "2. Movement";
        languages.french.Tab4Title = "3. Résultat";
        languages.english.Tab4Title = "3. Result";

        languages.french.loadButton = "Charger";
        languages.english.loadButton = "Load";
        languages.french.saveButton = "Enregistrer";
        languages.english.saveButton = "Save";
        languages.french.compareButton = "Comparer";
        languages.english.compareButton = "Compare";

        languages.french.Back = "Retour au menu";
        languages.english.Back = "Back to menu";
        languages.french.PerspectiveView = "Vue de perspective";
        languages.english.PerspectiveView = "Perspective View";

        languages.english.SimulationTime = "Simulation Time";
        languages.french.SimulationTime = "Temps de Simulation";
        languages.english.Duration = "Duration:";
        languages.french.Duration = "Durée:";

        languages.english.TakeOffSomersault = "Somersault";
        languages.french.TakeOffSomersault = "Salto";
        languages.english.TakeOffTilt = "Tilt";
        languages.french.TakeOffTilt = "Inclinaison";
        languages.english.TakeOffTwist = "Twist";
        languages.french.TakeOffTwist = "Vrille";
        languages.english.TakeOffHorizontal= "Horizontal";
        languages.french.TakeOffHorizontal = "Horizontale";
        languages.english.TakeOffVertical = "Vertical";
        languages.french.TakeOffVertical = "Verticale";

        languages.english.ConditionPreset = "Condition preset";
        languages.french.ConditionPreset = "Condition préréglage";

        languages.french.Gravity = "Gravité";
        languages.english.Gravity = "Gravity";
        languages.french.StopOnGround = "Arrêt au sol";
        languages.english.StopOnGround = "Stop on ground";

        ChangedLanguage();
        SetTooltip();
    }

    private void ChangedLanguage()
    {
        MainParameters.StrucMessageLists languagesUsed = MainParameters.Instance.languages.Used;

        List<string> dropDownOptions = new List<string>();
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedFast);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedNormal);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedSlow1);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedSlow2);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedSlow3);
        dropDownAnimationSpeed.ClearOptions();
        dropDownAnimationSpeed.AddOptions(dropDownOptions);

        if (languagesUsed.toolTipButtonQuit == "Quit")
        {
            textTakeOffSomersault.text = languages.english.TakeOffSomersault;
            textTakeOffTilt.text = languages.english.TakeOffTilt;
            textTakeOffTwist.text = languages.english.TakeOffTwist;
            textTakeOffHorizontal.text = languages.english.TakeOffHorizontal;
            textTakeOffVertical.text = languages.english.TakeOffVertical;
            ConditionPreset.text = languages.english.ConditionPreset;
            TextGravity.text = languages.english.Gravity;
            TextIsStopAtGround.text = languages.english.StopOnGround;

            Tab1Title.text = languages.english.Tab1Title;
            Tab2Title.text = languages.english.Tab2Title;
            Tab4Title.text = languages.english.Tab4Title;
            loadButton.text = languages.english.loadButton;
            saveButton.text = languages.english.saveButton;
            compareButton.text = languages.english.compareButton;
            textBack.text = languages.english.Back;
            PerspectiveView.text = languages.english.PerspectiveView;

            SimulationTime.text = languages.english.SimulationTime;
            Duration.text = languages.english.Duration;
        }
        else
        {
            textTakeOffTilt.text = languages.french.TakeOffTilt;
            textTakeOffTwist.text = languages.french.TakeOffTwist;
            textTakeOffHorizontal.text = languages.french.TakeOffHorizontal;
            textTakeOffVertical.text = languages.french.TakeOffVertical;
            ConditionPreset.text = languages.french.ConditionPreset;
            TextGravity.text = languages.french.Gravity;
            TextIsStopAtGround.text = languages.french.StopOnGround;

            Tab1Title.text = languages.french.Tab1Title;
            Tab2Title.text = languages.french.Tab2Title;
            Tab4Title.text = languages.french.Tab4Title;
            textBack.text = languages.french.Back;
            PerspectiveView.text = languages.french.PerspectiveView;

            SimulationTime.text = languages.french.SimulationTime;
            Duration.text = languages.french.Duration;
        }
    }

    private void Update()
    {
        if(uiManager.GetCurrentTab() == 1 && uiManager.tooltipOn)
        {
            uiManager.ShowToolTip(1, textTakeOffHorizontal.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffHorizontal);
            uiManager.ShowToolTip(2, textTakeOffVertical.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffVertical);
            uiManager.ShowToolTip(3, textTakeOffSomersault.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffSomersaultPosition);
            uiManager.ShowToolTip(5, textTakeOffSomersault.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffSomersaultSpeed);
            uiManager.ShowToolTip(6, textTakeOffTilt.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffTilt);
            uiManager.ShowToolTip(7, textTakeOffTwist.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffTwist);
        }

        if (aniGraphManager.graph != null)
            aniGraphManager.graph.MouseToClient(out mousePosX, out mousePosY);
        else
            return;

        if (Input.GetMouseButtonDown(1) && uiManager.IsOnGameObject(aniGraphManager.graph.gameObject))
        {
            if (aniGraphManager.mouseRightButtonON)
            {
                aniGraphManager.mouseRightButtonON = false;
                aniGraphManager.mouseLeftButtonON = false;
                panelAddRemoveNode.SetActive(false);
            }
            else
            {
                aniGraphManager.mouseRightButtonON = true;
                aniGraphManager.mousePosSaveX = (float)mousePosX;
                aniGraphManager.mousePosSaveY = (float)mousePosY;
                DisplayContextMenu(panelAddRemoveNode);
            }
        }
    }

    public void SetTooltip()
    {
        panelToolTipPrefab = (GameObject)Resources.Load("ToolTipCanvas", typeof(GameObject));
        panelToolTipPrefab = Instantiate(panelToolTipPrefab);
        uiManager.SetPanelToolTip(panelToolTipPrefab);
    }

    void DisplayContextMenu(GameObject panel)
    {
        Vector3 mousePosWorldSpace;
        Vector3[] menuPos = new Vector3[4];
        Vector3[] graphPos = new Vector3[4];
        aniGraphManager.graph.PointToWorldSpace(out mousePosWorldSpace, mousePosX, mousePosY);
        panel.GetComponent<RectTransform>().GetWorldCorners(menuPos);
        aniGraphManager.graph.GetComponent<RectTransform>().GetWorldCorners(graphPos);
        float width = menuPos[2].x - menuPos[1].x;
        panel.transform.position = mousePosWorldSpace + new Vector3(width / 2, 0, 0);
        panel.SetActive(true);
    }

    public int FindPreviousNode(int _ddlUsed, float _mousePosSaveX)
    {
        int i = 0;
        while (i < avatarManager.LoadedModels[0].Joints.nodes[_ddlUsed].T.Length && _mousePosSaveX > avatarManager.LoadedModels[0].Joints.nodes[_ddlUsed].T[i])
            i++;
        return i - 1;
    }

    public void AddNode()
    {
        // TODO: This should be moved to AniGraphManager
        int _avatarIndex = 0;
        int ddl = aniGraphManager.ddlUsed;
        int node = FindPreviousNode(ddl, aniGraphManager.mousePosSaveX);

        gameManager.InterpolationDDL(_avatarIndex);
        gameManager.DisplayDDL(ddl, true);

        float[] T = new float[avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length + 1];
        float[] Q = new float[avatarManager.LoadedModels[0].Joints.nodes[ddl].Q.Length + 1];
        for (int i = 0; i <= node; i++)
        {
            T[i] = avatarManager.LoadedModels[0].Joints.nodes[ddl].T[i];
            Q[i] = avatarManager.LoadedModels[0].Joints.nodes[ddl].Q[i];
        }
        T[node + 1] = aniGraphManager.mousePosSaveX;
        Q[node + 1] = aniGraphManager.mousePosSaveY * Mathf.PI / 180;

        for (int i = node + 1; i < avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length; i++)
        {
            T[i + 1] = avatarManager.LoadedModels[0].Joints.nodes[ddl].T[i];
            Q[i + 1] = avatarManager.LoadedModels[0].Joints.nodes[ddl].Q[i];
        }
        avatarManager.LoadedModels[0].Joints.nodes[ddl].T = MathFunc.MatrixCopy(T);
        avatarManager.LoadedModels[0].Joints.nodes[ddl].Q = MathFunc.MatrixCopy(Q);

        gameManager.InterpolationDDL(_avatarIndex);
        gameManager.DisplayDDL(ddl, false);

        aniGraphManager.mouseRightButtonON = false;
    }

    public void RemoveNode()
    {
        // TODO: This should be moved to AniGraphManager
        int _avatarIndex = 0;
        int ddl = aniGraphManager.ddlUsed;

        if (avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length < 3 || avatarManager.LoadedModels[0].Joints.nodes[ddl].Q.Length < 3)
        {
            aniGraphManager.mouseLeftButtonON = false;
            aniGraphManager.mouseRightButtonON = false;
            return;
        }


        int node = aniGraphManager.FindNearestNode();

        float[] T = new float[avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length - 1];
        float[] Q = new float[avatarManager.LoadedModels[0].Joints.nodes[ddl].Q.Length - 1];
        for (int i = 0; i < node; i++)
        {
            T[i] = avatarManager.LoadedModels[0].Joints.nodes[ddl].T[i];
            Q[i] = avatarManager.LoadedModels[0].Joints.nodes[ddl].Q[i];
        }
        for (int i = node + 1; i < avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length; i++)
        {
            T[i - 1] = avatarManager.LoadedModels[0].Joints.nodes[ddl].T[i];
            Q[i - 1] = avatarManager.LoadedModels[0].Joints.nodes[ddl].Q[i];
        }
        avatarManager.LoadedModels[0].Joints.nodes[ddl].T = MathFunc.MatrixCopy(T);
        avatarManager.LoadedModels[0].Joints.nodes[ddl].Q = MathFunc.MatrixCopy(Q);

        gameManager.InterpolationDDL(_avatarIndex);
        gameManager.DisplayDDL(ddl, false);

        aniGraphManager.mouseRightButtonON = false;
    }
}