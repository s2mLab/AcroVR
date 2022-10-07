using UnityEngine;
using System;
using System.Linq;
using System.Text;
using Microsoft.Research.Oslo;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class DrawManager : MonoBehaviour
{
    public enum AvatarModel
    {
        SingleFemale = 0,
        DoubleFemale = 1,
        SingleMale = 2,
        DoubleMale = 3,
    }

    const string dllpath = "biorbd_c.dll";
    [DllImport(dllpath)] static extern IntPtr c_biorbdModel(StringBuilder pathToModel);
    [DllImport(dllpath)] static extern int c_nQ(IntPtr model);
    [DllImport(dllpath)] static extern void c_massMatrix(IntPtr model, IntPtr q, IntPtr massMatrix);
    [DllImport(dllpath)] static extern void c_inverseDynamics(IntPtr model, IntPtr q, IntPtr qdot, IntPtr qddot, IntPtr tau);
    [DllImport(dllpath)] static extern void c_solveLinearSystem(IntPtr matA, int nbCol, int nbLigne, IntPtr matB, IntPtr solX);

    protected StatManager statManager;
    protected SliderPlayAnimation sliderAnimation;
    protected DisplayResultGraphicS resultGraphics;

    public GameObject avatarSpawnpoint;
    public Vector3 avatarVector3;
    public GameObject girl1;
    public GameObject girl2;
    GameObject girl1Prefab;
    GameObject girl2Prefab;
    float InitialFeetHeight = (float)Double.NaN;

    // Hip
    private GameObject girl1LeftThigh;
    private ControlThigh girl1ThighControl;
    private GameObject girl1RightThigh;
    // Knee
    private GameObject girl1LeftLeg;
    private ControlShin girl1LegControl;
    private GameObject girl1RightLeg;
    // Shoulder
    private GameObject girl1LeftArm;
    private ControlLeftArmAbduction girl1LeftArmControlAbd;
    private ControlLeftArmFlexion girl1LeftArmControlFlex;
    private GameObject girl1RightArm;
    private ControlRightArmAbduction girl1RightArmControlAbd;
    private ControlRightArmFlexion girl1RightArmControlFlex;
    // Root
    public GameObject girl1Hip;

    // Hip
    private GameObject girl2LeftUp;
    private GameObject girl2RightUp;
    // Knee
    private GameObject girl2LeftLeg;
    private GameObject girl2RightLeg;
    // Shoulder
    private GameObject girl2LeftArm;
    private GameObject girl2RightArm;
    // Root
    private GameObject girl2Hip;

    private GameObject firstView;
    public bool canResumeAnimation { get; protected set; } = false;
    public void SetCanResumeAnimation(bool value) { canResumeAnimation = value; }

    float[,] allQ;
    float[,] q_girl2;
    public double[] qf { get; protected set; }
    double[] qf_girl2;
    public float frameRate { get; } = 0.02f;
    public int frameN { get; protected set; } = 0;
    public void SetFrameN(int value) { 
        frameN = value;
        if (sliderAnimation) sliderAnimation.SetSlider(frameN);
    }
    public float frameNtime { get { return frameN * frameRate; } }
    public int secondFrameN { get; protected set; } = 0;
    public void SetSecondFrameN(int value) { secondFrameN = value; }
    public float secondFrameNtime { get { return secondFrameN * frameRate; } }
    int firstFrame = 0;
    internal int numberFrames = 0;
    public float timeElapsed = 0;
    public float timeFrame = 0;
    float timeStarted = 0;
    float factorPlaySpeed = 1f;

    string playMode = MainParameters.Instance.languages.Used.animatorPlayModeSimulation;
    public int cntAvatar = 1;

    protected float[,] q1;
    float[,] q1_girl2;

    public bool isPaused { get; protected set; } = true;
    public void Pause() { isPaused = true; secondPaused = false; }
    public void Resume(){ isPaused = false; secondPaused = false; }
    public bool IsEditing { get; protected set; } = false;

    float pauseStart = 0;
    float pauseTime = 0;

    protected int CurrentVizualisationMode = 0;
    public bool IsSimulationMode { get { return CurrentVizualisationMode == 0; } }
    public void ActivateSimulationMode() { CurrentVizualisationMode = 0; }
    public bool IsGestureMode { get { return CurrentVizualisationMode == 1; } }
    public void ActivateGestureMode() { CurrentVizualisationMode = 1; }

    public AvatarModel CurrentAvatar { get; protected set; }
    public void SetAvatar(AvatarModel _avatar){
        CurrentAvatar = _avatar;
        PlayerPrefs.SetInt("AvatarModel", (int)CurrentAvatar);
    }

    public void SetGravity(bool value) { 
        MainParameters.Instance.joints.UseGravity = value; 
        ForceFullUpdate();
    }

    GameObject Ground;
    public void SetGround(GameObject _floor) { Ground = _floor; }
    public void SetStopOnGround(bool value) { 
        MainParameters.Instance.joints.StopOnGround = value;
        ForceFullUpdate();
        if (Ground != null)
            Ground.SetActive(MainParameters.Instance.joints.StopOnGround);
    }

    public AvatarSimulation secondParameters = new AvatarSimulation();

    public int secondNumberFrames = 0;
    public bool secondPaused = false;
    public List<string> secondResultMessages = new List<string>();

    public float secondTimeElapsed = 0;
    public float resultDistance = 0;


    void Start()
    {
        statManager = ToolBox.GetInstance().GetManager<StatManager>();
        SetAvatar((AvatarModel)PlayerPrefs.GetInt("AvatarModel", (int)AvatarModel.SingleFemale));
    }



    public void RegisterResultShow(DisplayResultGraphicS _newResultGraphics)
    {
        resultGraphics = _newResultGraphics;
    }

    public void UnregisterResultShow()
    {
        resultGraphics = null;
    }

    public void RegisterSliderAnimation(SliderPlayAnimation _newSliderAnimation)
    {
        sliderAnimation = _newSliderAnimation;
    }

    public void UnregisterSliderAnimation()
    {
        sliderAnimation = null;
    }

    void Update()
    {
        if (isPaused && pauseStart == 0) 
            pauseStart = Time.time;

        if (!isPaused && pauseStart != 0)
        {
            pauseTime = Time.time - pauseStart;
            pauseStart = 0;
        }

        if (frameN <= 0 && !isPaused) timeStarted = Time.time;
        if (Time.time - (timeStarted + pauseTime) >= (timeFrame * frameN) * factorPlaySpeed)
        {
            if(!isPaused)
                timeElapsed = Time.time - (timeStarted + pauseTime);

            if (ShouldContinuePlaying())
                PlayOneFrame();
            else
                PlayEnd();
        }

        if (Time.time - (timeStarted + pauseTime) >= (timeFrame * secondFrameN) * factorPlaySpeed)
        {
            if (!secondPaused)
                secondTimeElapsed = Time.time - (timeStarted + pauseTime);

            if (secondFrameN < secondNumberFrames-1)
            {
                PlayOneFrameForSecond();
            }
            else
                secondPaused = true;
        }
    }

    public bool ShouldContinuePlaying()
    {
        if (frameN >= numberFrames - 1) return false;

        if (
            frameN != 0 
            && MainParameters.Instance.joints.StopOnGround && 
            !IsGestureMode && 
            FeetHeight(qf) < InitialFeetHeight
        ) 
            return false;
        
        return true;
    }

    public void UpdateFullKinematics(bool restartToZero)
    {
        MakeSimulationFrame();
        Play_s(q1, 0, q1.GetUpperBound(1) + 1, restartToZero);   
    }

    public void MakeSimulationFrame()
    {
        if (MainParameters.Instance.joints.nodes == null) return;
        q1 = MakeSimulation();
        if(cntAvatar > 1)
        {
            q1_girl2 = MakeSimulationSecond();
            q_girl2 = MathFunc.MatrixCopy(q1_girl2);
        }
    }

    public void InitAvatar(AvatarModel mode)
    {
        Pause();
        canResumeAnimation = false;

        string namePrefab1 = "";
        switch (mode)
        {
            case AvatarModel.SingleFemale:
                cntAvatar = 1;
                namePrefab1 = "girl1_control";
                break;
            case AvatarModel.SingleMale:
                namePrefab1 = "man1_control";
                break;
        }

        if (girl1 == null)
        {
            LoadGirlPrefab(
                namePrefab1, ref girl1Prefab, ref girl1, 
                ref girl1LeftThigh, ref girl1RightThigh, ref girl1LeftLeg, ref girl1RightLeg, 
                ref girl1LeftArm, ref girl1RightArm, ref girl1Hip, ref firstView);
            LoadAvatarControls(girl1,
                ref girl1ThighControl, ref girl1LegControl,
                ref girl1LeftArmControlAbd, ref girl1RightArmControlAbd, ref girl1LeftArmControlFlex, ref girl1RightArmControlFlex);
        }
    }

    protected void LoadGirlPrefab(
        String namePrefab, 
        ref GameObject prefab,
        ref GameObject avatar, 
        ref GameObject leftThigh, 
        ref GameObject rightThigh,
        ref GameObject leftLeg,
        ref GameObject rightLeg,
        ref GameObject leftArm,
        ref GameObject rightArm,
        ref GameObject hip,
        ref GameObject view
    )
    {
        prefab = (GameObject)Resources.Load(namePrefab, typeof(GameObject));
        avatar = Instantiate(prefab);

        leftThigh = avatar.transform.Find("Petra.002/hips/zero_thigh.L/thigh.L").gameObject;
        rightThigh = avatar.transform.Find("Petra.002/hips/zero_thigh.R/thigh.R").gameObject;
        // Knee
        leftLeg = avatar.transform.Find("Petra.002/hips/zero_thigh.L/thigh.L/zero_shin.L/shin.L").gameObject;
        rightLeg = avatar.transform.Find("Petra.002/hips/zero_thigh.R/thigh.R/zero_shin.R/shin.R").gameObject;
        // Shoulder
        rightArm = avatar.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R").gameObject;
        leftArm = avatar.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L").gameObject;
        // Root
        hip = avatar.transform.Find("Petra.002/hips").gameObject;
        ///////////////////////////

        view = avatar.transform.Find("Petra.002/hips/FirstViewPoint").gameObject;

        // Zero position
        avatar.transform.position = Vector3.zero;
        hip.transform.position = Vector3.zero;
        CenterAvatar(avatar, hip);
        leftArm.transform.localRotation = Quaternion.identity;
        rightArm.transform.localRotation = Quaternion.identity;
    }

    protected void LoadAvatarControls(
        GameObject avatar, 
        ref ControlThigh thigh, 
        ref ControlShin shin, 
        ref ControlLeftArmAbduction leftArmAbd, 
        ref ControlRightArmAbduction rightArmAbd, 
        ref ControlLeftArmFlexion leftArmFlex, 
        ref ControlRightArmFlexion rightArmFlex
    )
    {
        thigh = avatar.transform.Find("Petra.002/hips/zero_thigh.L/thigh.L/ControlThigh").GetComponent<ControlThigh>();
        shin = avatar.transform.Find("Petra.002/hips/zero_thigh.L/thigh.L/zero_shin.L/shin.L/ControlShin").GetComponent<ControlShin>();
        leftArmAbd = avatar.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L/ControlLeftArm").GetComponent<ControlLeftArmAbduction>();
        rightArmAbd = avatar.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R/ControlRightArm").GetComponent<ControlRightArmAbduction>();
        leftArmFlex = avatar.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L/ControlLeftArm").GetComponent<ControlLeftArmFlexion>();
        rightArmFlex = avatar.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R/ControlRightArm").GetComponent<ControlRightArmFlexion>();
    }

    public bool LoadAvatar(AvatarModel mode)
    {
        Pause();

        string namePrefab1 = "";
        string namePrefab2 = "";
        switch (mode)
        {
            case AvatarModel.SingleFemale:
                cntAvatar = 1;
                namePrefab1 = "girl1_control";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(false);
                break;
            case AvatarModel.DoubleFemale:
                cntAvatar = 2;
                namePrefab1 = "girl1_control";
                namePrefab2 = "girl2";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(true);
                break;
            case AvatarModel.SingleMale:
                cntAvatar = 1;
                namePrefab1 = "man1_control";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(false);
                break;
            case AvatarModel.DoubleMale:
                cntAvatar = 2;
                namePrefab1 = "man1_control";
                namePrefab2 = "man2";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(true);
                break;
        }

        if (girl1 == null)
        {
            LoadGirlPrefab(
                namePrefab1, ref girl1Prefab, ref girl1, 
                ref girl1LeftThigh, ref girl1RightThigh, ref girl1LeftLeg, ref girl1RightLeg, 
                ref girl1RightArm, ref girl1LeftArm, ref girl1Hip, ref firstView
            );

            LoadAvatarControls(girl1, 
                ref girl1ThighControl, ref girl1LegControl, 
                ref girl1LeftArmControlAbd, ref girl1RightArmControlAbd, ref girl1LeftArmControlFlex, ref girl1RightArmControlFlex);
        }

        q1 = MakeSimulation();

        if (cntAvatar > 1)
        {
            if (girl2 == null)
            {

                LoadGirlPrefab(
                    namePrefab2, ref girl2Prefab, ref girl2,
                    ref girl2LeftUp, ref girl2RightUp, ref girl2LeftLeg, ref girl2RightLeg,
                    ref girl2RightArm, ref girl2LeftArm, ref girl2Hip, ref firstView
                );
            }

            q1_girl2 = MakeSimulationSecond();
            q_girl2 = MathFunc.MatrixCopy(q1_girl2);
        }

        return true;
    }

    public void CenterAvatar(GameObject avatar, GameObject _hip)
    {
        Vector3 _scaling = avatar.transform.localScale;
        var _hipTranslations = Double.IsNaN(InitialFeetHeight) ? new Vector3(0f, 0f, 0f) : new Vector3(0f, -InitialFeetHeight * _scaling.y, 0f);
        var _hipRotations = new Vector3(0f, 0f, 0f);
        if (IsSimulationMode && qf != null)
        {
            _hipTranslations += new Vector3((float)qf[6] * _scaling.x, (float)qf[8] * _scaling.y, (float)qf[7] * _scaling.z);
            _hipRotations += new Vector3((float)qf[9] * Mathf.Rad2Deg, (float)qf[10] * Mathf.Rad2Deg, (float)qf[11] * Mathf.Rad2Deg);
        }
        _hip.transform.localPosition = _hipTranslations;
        _hip.transform.localEulerAngles = _hipRotations;
    }

    public void ForceFullUpdate()
    {
        var _currentFrame = frameN;
        ShowAvatar();
        PlayOneFrame();
        SetFrameN(_currentFrame);
    }

    public void ShowAvatar()
    {
        MakeSimulationFrame();
        if (MainParameters.Instance.joints.nodes == null) return;
        CenterAvatar(girl1, girl1Hip);

        Play_s(q1, 0, q1.GetUpperBound(1) + 1, true);

        if (cntAvatar > 1)
        {
            girl2.transform.rotation = Quaternion.identity;
            girl2.transform.position = Vector3.zero;
            CenterAvatar(girl1, girl2Hip);

            secondNumberFrames = q1_girl2.GetUpperBound(1) + 1;
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        factorPlaySpeed = speed;
    }

    public GameObject GetFirstViewTransform()
    {
        return firstView;
    }

    public void PlayAvatar()
    {
        if (MainParameters.Instance.joints.nodes == null) return;
        ShowAvatar();
        Resume();
        canResumeAnimation = true;
    }

    public void PlayEnd()
    {
        Pause();
    }

    private void DisplayNewMessage(bool clear, bool display, string message)
    {
        if (clear) MainParameters.Instance.scrollViewMessages.Clear();
        MainParameters.Instance.scrollViewMessages.Add(message);
    }

    private void AddsecondMessage(bool clear, bool display, string message)
    {
        if (clear) secondResultMessages.Clear();
        secondResultMessages.Add(message);
    }

    public string DisplayMessage()
    {
        return string.Join(Environment.NewLine, MainParameters.Instance.scrollViewMessages.ToArray());
    }

    public string DisplayMessageSecond()
    {
        return string.Join(Environment.NewLine, secondResultMessages.ToArray());
    }

    protected void Play_s(float[,] qq, int frFrame, int nFrames, bool restartToZero)
    {
        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        allQ = MathFunc.MatrixCopy(qq);
        if (restartToZero)
            SetFrameN(0);
        firstFrame = frFrame;
        numberFrames = nFrames;

        timeElapsed = 0;

        pauseTime = 0;
        pauseStart = 0;

        if (nFrames > 1)
        {
            if (joints.tc > 0)                          // Il y a eu contact avec le sol, alors seulement une partie des données sont utilisé
                timeFrame = joints.tc / (numberFrames - 1);
            else                                        // Aucun contact avec le sol, alors toutes les données sont utilisé
                timeFrame = joints.Duration / (numberFrames - 1);
        }
        else
            timeFrame = 0;

        if (cntAvatar > 1)
        {
            secondFrameN = 0;
        }
    }

    private void Quintic_s(float t, float ti, float tj, float qi, float qj, out float p, out float v, out float a)
    {
        if (t < ti)
            t = ti;
        else if (t > tj)
            t = tj;
        float tp0 = tj - ti;
        float tp1 = t - ti;
        float tp2 = tp1 / tp0;
        float tp3 = tp2 * tp2;
        float tp4 = tp3 * tp2 * (6 * tp3 - 15 * tp2 + 10);
        float tp5 = qj - qi;
        float tp6 = tj - t;
        float tp7 = Mathf.Pow(tp0, 5);
        p = qi + tp5 * tp4;
        v = 30 * tp5 * tp1 * tp1 * tp6 * tp6 / tp7;
        a = 60 * tp5 * tp1 * tp6 * (tj + ti - 2 * t) / tp7;
    }

    public void Trajectory_s(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float t, int[] qi, out float[] qd, out float[] qdotd, out float[] qddotd)
    {
        qd = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        qdotd = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        qddotd = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            qdotd[i] = 0;
            qddotd[i] = 0;
        }

        int n = qi.Length;

        // n=6, 6Node (HipFlexion, KneeFlexion ...)
        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            int ii = qi[i] - lagrangianModel.q2[0];
            MainParameters.StrucNodes nodes = MainParameters.Instance.joints.nodes[ii];

            int j = 1;
            while (j < nodes.T.Length - 1 && t > nodes.T[j]) j++;
            Quintic_s(t, nodes.T[j - 1], nodes.T[j], nodes.Q[j - 1], nodes.Q[j], out qd[ii], out qdotd[ii], out qddotd[ii]);
        }
    }

    public void TrajectorySecond(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float t, int[] qi, out float[] qd, out float[] qdotd, out float[] qddotd)
    {
        qd = new float[secondParameters.joints.lagrangianModel.nDDL];
        qdotd = new float[secondParameters.joints.lagrangianModel.nDDL];
        qddotd = new float[secondParameters.joints.lagrangianModel.nDDL];
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            qdotd[i] = 0;
            qddotd[i] = 0;
        }

        int n = qi.Length;

        // n=6, 6Node (HipFlexion, KneeFlexion ...)
        for (int i = 0; i < secondParameters.joints.nodes.Length; i++)
        {
            int ii = qi[i] - lagrangianModel.q2[0];
            AvatarSimulation.StrucNodes nodes = secondParameters.joints.nodes[ii];
            int j = 1;
            while (j < nodes.T.Length - 1 && t > nodes.T[j]) j++;
            Quintic_s(t, nodes.T[j - 1], nodes.T[j], nodes.Q[j - 1], nodes.Q[j], out qd[ii], out qdotd[ii], out qddotd[ii]);
        }
    }

    private float[,] MakeSimulation()
    {
        if (MainParameters.Instance.joints.nodes == null) return new float[0,0];

        MainParameters.StrucJoints joints = MainParameters.Instance.joints;
        float[] q0 = new float[joints.lagrangianModel.nDDL];
        float[] q0dot = new float[joints.lagrangianModel.nDDL];
        if (Double.IsNaN(InitialFeetHeight)){
            InitialFeetHeight = FeetHeight(q0);
        }

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            MainParameters.StrucNodes nodes = MainParameters.Instance.joints.nodes[i];
            q0[i] = nodes.Q[0];
        }

        // Beginning Pose
        int[] rotation = new int[3] { joints.lagrangianModel.root_somersault, joints.lagrangianModel.root_tilt, joints.lagrangianModel.root_twist };
        int[] rotationSign = MathFunc.Sign(rotation);
        for (int i = 0; i < rotation.Length; i++) rotation[i] = Math.Abs(rotation[i]);

        int[] translation = new int[3] { joints.lagrangianModel.root_right, joints.lagrangianModel.root_foreward, joints.lagrangianModel.root_upward };
        int[] translationS = MathFunc.Sign(translation);
        for (int i = 0; i < translation.Length; i++) translation[i] = Math.Abs(translation[i]);

        float rotRadians = joints.takeOffParam.Somersault * (float)Math.PI / 180;

        float tilt = joints.takeOffParam.Tilt;
        if (tilt == 90)
            tilt = 90.001f;
        else if (tilt == -90)
            tilt = -90.01f;

        // q0[12]
        // q0[9] = somersault
        // q0[10] = tilt
        q0[Math.Abs(joints.lagrangianModel.root_tilt) - 1] = tilt * (float)Math.PI / 180; 
        q0[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = rotRadians; 

        //q0dot[12]
        //q0dot[7] = AnteroposteriorSpeed
        //q0dot[8] = verticalSpeed
        //q0dot[9] = somersaultSpeed
        //q0dot[11] = twistSpeed
        q0dot[Math.Abs(joints.lagrangianModel.root_foreward) - 1] = joints.takeOffParam.HorizontalSpeed;                       // m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_upward) - 1] = joints.takeOffParam.VerticalSpeed;                                // m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = joints.takeOffParam.SomersaultSpeed * 2 * (float)Math.PI;     // radians/s
        q0dot[Math.Abs(joints.lagrangianModel.root_twist) - 1] = joints.takeOffParam.TwistSpeed * 2 * (float)Math.PI;               // radians/s


        // q0[11] = twist
        // q0dot[10] = tiltSpeed
        q0[Math.Abs(joints.lagrangianModel.root_twist) - 1] = joints.takeOffParam.Twist * (float)Math.PI / 180;
        q0dot[Math.Abs(joints.lagrangianModel.root_tilt) - 1] = joints.takeOffParam.TiltSpeed * 2 * (float)Math.PI;


        double[] Q = new double[joints.lagrangianModel.nDDL];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
            Q[i] = q0[i];
        EvaluateTags_s(Q, out float[] tagX, out float[] tagY, out float[] tagZ);

        // Q[12]
        // tagX[26], tagY[26], tagZ[26]

        //the last one = Center of Mass
        float[] cg = new float[3];
        cg[0] = tagX[tagX.Length - 1];
        cg[1] = tagY[tagX.Length - 1];
        cg[2] = tagZ[tagX.Length - 1];

        float[] u1 = new float[3];
        float[,] rot = new float[3, 1];
        for (int i = 0; i < 3; i++)
        {
            u1[i] = cg[i] - q0[translation[i] - 1] * translationS[i];
            rot[i, 0] = q0dot[rotation[i] - 1] * rotationSign[i];
        }
        float[,] u = { { 0, -u1[2], u1[1] }, { u1[2], 0, -u1[0] }, { -u1[1], u1[0], 0 } };
        float[,] rotM = MathFunc.MatrixMultiply(u, rot);
        for (int i = 0; i < 3; i++)
        {
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i] + rotM[i, 0];
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i];
        }

        q0[Math.Abs(joints.lagrangianModel.root_foreward) - 1] += joints.takeOffParam.HorizontalPosition;
        q0[Math.Abs(joints.lagrangianModel.root_upward) - 1] += joints.takeOffParam.VerticalPosition;

        double[] x0 = new double[joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            x0[i] = q0[i];
            x0[joints.lagrangianModel.nDDL + i] = q0dot[i];
        }

        // x0[24]

        Options options = new Options();
        options.InitialStep = joints.lagrangianModel.dt;
        var sol = Ode.RK547M(0, joints.Duration + joints.lagrangianModel.dt, new Vector(x0), ShortDynamics_s, options);

        var points = sol.SolveFromToStep(0, joints.Duration + joints.lagrangianModel.dt, joints.lagrangianModel.dt).ToArray();

        // test0 = point[51]
        // test1 = point[251]
        double[] t = new double[points.GetUpperBound(0) + 1];
        double[,] q = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        double[,] qdot = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            for (int j = 0; j <= points.GetUpperBound(0); j++)
            {
                if (i <= 0)
                    t[j] = points[j].T;

                q[i, j] = points[j].X[i];
                qdot[i, j] = points[j].X[joints.lagrangianModel.nDDL + i];
            }
        }

        // test0 = t[51], q[12,51], qdot[12,51]
        // test1 = t[251], q[12,251], qdot[12,251]
        int tIndex = 0;
        MainParameters.Instance.joints.tc = 0;
        for (int i = 0; i <= q.GetUpperBound(1); i++)
        {
            tIndex++;
            double[] qq = new double[joints.lagrangianModel.nDDL];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
                qq[j] = q[j, i];
            EvaluateTags_s(qq, out tagX, out tagY, out tagZ);

            // Cut the trial when the feet crosses the ground (vertical axis = 0)
            if (
                  !IsGestureMode && i > 0 
                  && MainParameters.Instance.joints.StopOnGround 
                  && MainParameters.Instance.joints.UseGravity 
                  && tagZ.Min() < InitialFeetHeight
            )
            {
                MainParameters.Instance.joints.tc = (float)t[i];
                break;
            }
        }

        MainParameters.Instance.joints.t = new float[tIndex];
        float[,] qOut = new float[joints.lagrangianModel.nDDL, tIndex];
        float[,] qdot1 = new float[joints.lagrangianModel.nDDL, tIndex];
        for (int i = 0; i < tIndex; i++)
        {
            MainParameters.Instance.joints.t[i] = (float)t[i];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
            {
                qOut[j, i] = (float)q[j, i];
                qdot1[j, i] = (float)qdot[j, i];
            }
        }

        MainParameters.Instance.joints.rot = new float[tIndex, rotation.Length];
        MainParameters.Instance.joints.rotdot = new float[tIndex, rotation.Length];
        float[,] rotAbs = new float[tIndex, rotation.Length];
        for (int i = 0; i < rotation.Length; i++)
        {
            float[] rotCol = new float[tIndex];
            float[] rotdotCol = new float[tIndex];
            rotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qOut, rotation[i] - 1));
            rotdotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qdot1, rotation[i] - 1));
            for (int j = 0; j < tIndex; j++)
            {
                MainParameters.Instance.joints.rot[j, i] = rotCol[j] / (2 * (float)Math.PI);
                MainParameters.Instance.joints.rotdot[j, i] = rotdotCol[j] / (2 * (float)Math.PI);
                rotAbs[j, i] = Math.Abs(MainParameters.Instance.joints.rot[j, i]);
            }
        }

        float numSomersault = MathFunc.MatrixGetColumn(rotAbs, 0).Max() + MainParameters.Instance.joints.takeOffParam.Somersault / 360;
        DisplayNewMessage(true, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberSomersaults, numSomersault));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberTwists, MathFunc.MatrixGetColumn(rotAbs, 2).Max()));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgFinalTwist, MainParameters.Instance.joints.rot[tIndex - 1, 2]));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgMaxTilt, MathFunc.MatrixGetColumn(rotAbs, 1).Max() * 360));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgFinalTilt, MainParameters.Instance.joints.rot[tIndex - 1, 1] * 360));

        return qOut;
    }


    protected float FeetHeight(float[] q){
        double[] qDouble = new double[q.Length];
        for (int i = 0; i<q.Length; ++i)
            qDouble[i] = q[i];

        return (float)FeetHeight(qDouble);
    }
    protected double FeetHeight(double[] q)
    {
        float[] tagX;
        float[] tagY;
        float[] tagZ;
        EvaluateTags_s(q, out tagX, out tagY, out tagZ);
        return Math.Min(
            tagZ[MainParameters.Instance.joints.lagrangianModel.feet[0] - 1],
            tagZ[MainParameters.Instance.joints.lagrangianModel.feet[1] - 1]
        );
    }

    private float[,] MakeSimulationSecond()
    {
        if (secondParameters.joints.nodes == null) return new float[0, 0];

        AvatarSimulation.StrucJoints joints = secondParameters.joints;
        float[] q0 = new float[joints.lagrangianModel.nDDL];
        float[] q0dot = new float[joints.lagrangianModel.nDDL];

        for (int i = 0; i < secondParameters.joints.nodes.Length; i++)
        {
            AvatarSimulation.StrucNodes nodes = secondParameters.joints.nodes[i];
            q0[i] = nodes.Q[0];
        }

        int[] rotation = new int[3] { joints.lagrangianModel.root_somersault, joints.lagrangianModel.root_tilt, joints.lagrangianModel.root_twist };
        int[] rotationS = MathFunc.Sign(rotation);
        for (int i = 0; i < rotation.Length; i++) rotation[i] = Math.Abs(rotation[i]);

        int[] translation = new int[3] { joints.lagrangianModel.root_right, joints.lagrangianModel.root_foreward, joints.lagrangianModel.root_upward };
        int[] translationS = MathFunc.Sign(translation);
        for (int i = 0; i < translation.Length; i++) translation[i] = Math.Abs(translation[i]);

        float rotRadians = joints.takeOffParam.Somersault * (float)Math.PI / 180;

        float tilt = joints.takeOffParam.Tilt;
        if (tilt == 90)
            tilt = 90.001f;
        else if (tilt == -90)
            tilt = -90.01f;

        q0[Math.Abs(joints.lagrangianModel.root_tilt) - 1] = tilt * (float)Math.PI / 180;                                        // en radians
        q0[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = rotRadians;                                         // en radians

        q0dot[Math.Abs(joints.lagrangianModel.root_foreward) - 1] = joints.takeOffParam.HorizontalSpeed;                       // en m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_upward) - 1] = joints.takeOffParam.VerticalSpeed;                                // en m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = joints.takeOffParam.SomersaultSpeed * 2 * (float)Math.PI;     // en radians/s
        q0dot[Math.Abs(joints.lagrangianModel.root_twist) - 1] = joints.takeOffParam.TwistSpeed * 2 * (float)Math.PI;               // en radians/s

        double[] Q = new double[joints.lagrangianModel.nDDL];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
            Q[i] = q0[i];
        float[] tagX;
        float[] tagY;
        float[] tagZ;
        EvaluateTags_s(Q, out tagX, out tagY, out tagZ);

        float[] cg = new float[3];
        cg[0] = tagX[tagX.Length - 1];
        cg[1] = tagY[tagX.Length - 1];
        cg[2] = tagZ[tagX.Length - 1];

        float[] u1 = new float[3];
        float[,] rot = new float[3, 1];
        for (int i = 0; i < 3; i++)
        {
            u1[i] = cg[i] - q0[translation[i] - 1] * translationS[i];
            rot[i, 0] = q0dot[rotation[i] - 1] * rotationS[i];
        }
        float[,] u = { { 0, -u1[2], u1[1] }, { u1[2], 0, -u1[0] }, { -u1[1], u1[0], 0 } };
        float[,] rotM = MathFunc.MatrixMultiply(u, rot);
        for (int i = 0; i < 3; i++)
        {
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i] + rotM[i, 0];
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i];
        }

        float hFeet = Math.Min(tagZ[joints.lagrangianModel.feet[0] - 1], tagZ[joints.lagrangianModel.feet[1] - 1]);
        float hHand = Math.Min(tagZ[joints.lagrangianModel.hand[0] - 1], tagZ[joints.lagrangianModel.hand[1] - 1]);

        if (Math.Cos(rotRadians) > 0)
            q0[Math.Abs(joints.lagrangianModel.root_upward) - 1] += joints.lagrangianModel.hauteurs[joints.condition] - hFeet;
        else                                                            // bars, vault and tumbling from hands
            q0[Math.Abs(joints.lagrangianModel.root_upward) - 1] += joints.lagrangianModel.hauteurs[joints.condition] - hHand;

        double[] x0 = new double[joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            x0[i] = q0[i];
            x0[joints.lagrangianModel.nDDL + i] = q0dot[i];
        }

        Options options = new Options();
        options.InitialStep = joints.lagrangianModel.dt;

        var sol = Ode.RK547M(0, joints.Duration + joints.lagrangianModel.dt, new Vector(x0), ShortDynamicsSecond, options);
        var points = sol.SolveFromToStep(0, joints.Duration + joints.lagrangianModel.dt, joints.lagrangianModel.dt).ToArray();

        double[] t = new double[points.GetUpperBound(0) + 1];
        double[,] q = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        double[,] qdot = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            for (int j = 0; j <= points.GetUpperBound(0); j++)
            {
                if (i <= 0)
                    t[j] = points[j].T;

                q[i, j] = points[j].X[i];
                qdot[i, j] = points[j].X[joints.lagrangianModel.nDDL + i];
            }
        }

        int tIndex = 0;
        secondParameters.joints.tc = 0;
        for (int i = 0; i <= q.GetUpperBound(1); i++)
        {
            tIndex++;
            double[] qq = new double[joints.lagrangianModel.nDDL];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
                qq[j] = q[j, i];
            EvaluateTags_s(qq, out tagX, out tagY, out tagZ);
            if (MainParameters.Instance.joints.UseGravity && tagZ.Min() < -0.05f)
            {
                secondParameters.joints.tc = (float)t[i];
                break;
            }
        }

        secondParameters.joints.t = new float[tIndex];
        float[,] qOut = new float[joints.lagrangianModel.nDDL, tIndex];
        float[,] qdot1 = new float[joints.lagrangianModel.nDDL, tIndex];
        for (int i = 0; i < tIndex; i++)
        {
            secondParameters.joints.t[i] = (float)t[i];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
            {
                qOut[j, i] = (float)q[j, i];
                qdot1[j, i] = (float)qdot[j, i];
            }
        }

        secondParameters.joints.rot = new float[tIndex, rotation.Length];
        secondParameters.joints.rotdot = new float[tIndex, rotation.Length];
        float[,] rotAbs = new float[tIndex, rotation.Length];
        for (int i = 0; i < rotation.Length; i++)
        {
            float[] rotCol = new float[tIndex];
            float[] rotdotCol = new float[tIndex];
            rotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qOut, rotation[i] - 1));
            rotdotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qdot1, rotation[i] - 1));
            for (int j = 0; j < tIndex; j++)
            {
                secondParameters.joints.rot[j, i] = rotCol[j] / (2 * (float)Math.PI);
                secondParameters.joints.rotdot[j, i] = rotdotCol[j] / (2 * (float)Math.PI);
                rotAbs[j, i] = Math.Abs(secondParameters.joints.rot[j, i]);
            }
        }

        float numSomersault = MathFunc.MatrixGetColumn(rotAbs, 0).Max() + secondParameters.joints.takeOffParam.Somersault / 360;
        AddsecondMessage(true, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberSomersaults, numSomersault));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberTwists, MathFunc.MatrixGetColumn(rotAbs, 2).Max()));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgFinalTwist, secondParameters.joints.rot[tIndex - 1, 2]));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgMaxTilt, MathFunc.MatrixGetColumn(rotAbs, 1).Max() * 360));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgFinalTilt, secondParameters.joints.rot[tIndex - 1, 1] * 360));

        return qOut;
    }

    private void EvaluateTags_s(double[] q, out float[] tagX, out float[] tagY, out float[] tagZ)
    {
        // q[12]

        double[] tag1;
        TagsSimple tagsSimple = new TagsSimple();
        tag1 = tagsSimple.Tags(q);

        // tag1[78]

        int newTagLength = tag1.Length / 3;

        // newTagLength = 26;

        tagX = new float[newTagLength];
        tagY = new float[newTagLength];
        tagZ = new float[newTagLength];
        for (int i = 0; i < newTagLength; i++)
        {
            tagX[i] = (float)tag1[i];
            tagY[i] = (float)tag1[i + newTagLength];
            tagZ[i] = (float)tag1[i + newTagLength * 2];
        }
    }

    private Vector ShortDynamics_s(double t, Vector x)
    {
        int nDDL = MainParameters.Instance.joints.lagrangianModel.nDDL;

        double[] q = new double[nDDL];
        double[] qdot = new double[nDDL];
        for (int i = 0; i < nDDL; i++)
        {
            q[i] = x[i];
            qdot[i] = x[nDDL + i];
        }

        double[,] m12;
        double[] n1;
        Inertia11Simple inertia11Simple = new Inertia11Simple();
        double[,] m11 = inertia11Simple.Inertia11(q);

        Inertia12Simple inertia12Simple = new Inertia12Simple();
        m12 = inertia12Simple.Inertia12(q);
        NLEffects1Simple nlEffects1Simple = new NLEffects1Simple();
        n1 = nlEffects1Simple.NLEffects1(q, qdot);

        if (!MainParameters.Instance.joints.UseGravity)
        {
            double[] n1zero;
            n1zero = nlEffects1Simple.NLEffects1(q, new double[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            for (int i = 0; i < 6; i++)
                n1[i] = n1[i] - n1zero[i];
        }

        float kp = 10;
        float kv = 3;
        float[] qd = new float[nDDL];
        float[] qdotd = new float[nDDL];
        float[] qddotd = new float[nDDL];

        Trajectory_s(MainParameters.Instance.joints.lagrangianModel, (float)t, MainParameters.Instance.joints.lagrangianModel.q2, out qd, out qdotd, out qddotd);

        float[] qddot = new float[nDDL];
        for (int i = 0; i < nDDL; i++)
            qddot[i] = qddotd[i] + kp * (qd[i] - (float)q[i]) + kv * (qdotd[i] - (float)qdot[i]);

        double[,] mA = MatrixInverse.MtrxInverse(m11);

        double[] q2qddot = new double[MainParameters.Instance.joints.lagrangianModel.q2.Length];
        for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q2.Length; i++)
            q2qddot[i] = qddot[MainParameters.Instance.joints.lagrangianModel.q2[i] - 1];
        double[,] mB = MatrixInverse.MtrxProduct(m12, q2qddot);

        double[,] n1mB = new double[mB.GetUpperBound(0) + 1, mB.GetUpperBound(1) + 1];
        for (int i = 0; i <= mB.GetUpperBound(0); i++)
            for (int j = 0; j <= mB.GetUpperBound(1); j++)
                n1mB[i, j] = -n1[i] - mB[i, j];

        double[,] mC = MatrixInverse.MtrxProduct(mA, n1mB);

        for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
            qddot[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = (float)mC[i, 0];

        double[] xdot = new double[MainParameters.Instance.joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.nDDL; i++)
        {
            xdot[i] = qdot[i];
            xdot[MainParameters.Instance.joints.lagrangianModel.nDDL + i] = qddot[i];
        }

        //xdot[24]
        return new Vector(xdot);
    }

    private Vector ShortDynamicsSecond(double t, Vector x)
    {
        int nDDL = secondParameters.joints.lagrangianModel.nDDL;

        double[] q = new double[nDDL];
        double[] qdot = new double[nDDL];
        for (int i = 0; i < nDDL; i++)
        {
            q[i] = x[i];
            qdot[i] = x[nDDL + i];
        }

        double[,] m12;
        double[] n1;
        Inertia11Simple inertia11Simple = new Inertia11Simple();
        double[,] m11 = inertia11Simple.Inertia11(q);

        Inertia12Simple inertia12Simple = new Inertia12Simple();
        m12 = inertia12Simple.Inertia12(q);
        NLEffects1Simple nlEffects1Simple = new NLEffects1Simple();
        n1 = nlEffects1Simple.NLEffects1(q, qdot);
        if (!MainParameters.Instance.joints.UseGravity)
        {
            double[] n1zero;
            n1zero = nlEffects1Simple.NLEffects1(q, new double[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            for (int i = 0; i < 6; i++)
                n1[i] = n1[i] - n1zero[i];
        }

        float kp = 10;
        float kv = 3;
        float[] qd = new float[nDDL];
        float[] qdotd = new float[nDDL];
        float[] qddotd = new float[nDDL];

        TrajectorySecond(secondParameters.joints.lagrangianModel, (float)t, secondParameters.joints.lagrangianModel.q2, out qd, out qdotd, out qddotd);

        float[] qddot = new float[nDDL];
        for (int i = 0; i < nDDL; i++)
            qddot[i] = qddotd[i] + kp * (qd[i] - (float)q[i]) + kv * (qdotd[i] - (float)qdot[i]);

        double[,] mA = MatrixInverse.MtrxInverse(m11);

        double[] q2qddot = new double[secondParameters.joints.lagrangianModel.q2.Length];
        for (int i = 0; i < secondParameters.joints.lagrangianModel.q2.Length; i++)
            q2qddot[i] = qddot[secondParameters.joints.lagrangianModel.q2[i] - 1];
        double[,] mB = MatrixInverse.MtrxProduct(m12, q2qddot);

        double[,] n1mB = new double[mB.GetUpperBound(0) + 1, mB.GetUpperBound(1) + 1];
        for (int i = 0; i <= mB.GetUpperBound(0); i++)
            for (int j = 0; j <= mB.GetUpperBound(1); j++)
                n1mB[i, j] = -n1[i] - mB[i, j];

        double[,] mC = MatrixInverse.MtrxProduct(mA, n1mB);

        for (int i = 0; i < secondParameters.joints.lagrangianModel.q1.Length; i++)
            qddot[secondParameters.joints.lagrangianModel.q1[i] - 1] = (float)mC[i, 0];

        double[] xdot = new double[secondParameters.joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < secondParameters.joints.lagrangianModel.nDDL; i++)
        {
            xdot[i] = qdot[i];
            xdot[secondParameters.joints.lagrangianModel.nDDL + i] = qddot[i];
        }

        //xdot[24]
        return new Vector(xdot);
    }

    public void PlayOneFrameForSecond()
    {
        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        if (!IsEditing)
            if (q_girl2.GetUpperBound(1) >= secondFrameN)
            {
                qf_girl2 = MathFunc.MatrixGetColumnD(q_girl2, firstFrame + secondFrameN);
                if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
                    for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
                        qf_girl2[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = 0;
            }

        SetAllDof(qf_girl2);
        girl2Hip.transform.localRotation = Quaternion.AngleAxis((float)qf_girl2[9] * Mathf.Rad2Deg, Vector3.right) *
                                            Quaternion.AngleAxis((float)qf_girl2[10] * Mathf.Rad2Deg, Vector3.forward) *
                                            Quaternion.AngleAxis((float)qf_girl2[11] * Mathf.Rad2Deg, Vector3.up);

        girl2Hip.transform.position = new Vector3((float)qf_girl2[6], (float)qf_girl2[8], (float)qf_girl2[7]);

        if (!secondPaused) secondFrameN++;
    }

    public float CheckPositionAvatar()
    {
        if (IsGestureMode)
        {
            return 0;
        }

        if (allQ == null) return 0;
        if (allQ.GetUpperBound(1) == 0) return 0;

        float vertical = Mathf.Max((float)MathFunc.MatrixGetColumnD(allQ, 1)[8], (float)MathFunc.MatrixGetColumnD(allQ, numberFrames - 1)[8]);
        float horizontal = Mathf.Max((float)MathFunc.MatrixGetColumnD(allQ,1)[7], (float)MathFunc.MatrixGetColumnD(allQ, numberFrames - 1)[7]);

        float max = Mathf.Max(vertical, horizontal);

        resultDistance = Vector3.Distance(new Vector3((float)MathFunc.MatrixGetColumnD(allQ, 1)[6], (float)MathFunc.MatrixGetColumnD(allQ, 1)[8], (float)MathFunc.MatrixGetColumnD(allQ, 1)[7]),
            new Vector3((float)MathFunc.MatrixGetColumnD(allQ, numberFrames - 1)[6], (float)MathFunc.MatrixGetColumnD(allQ, numberFrames - 1)[8], (float)MathFunc.MatrixGetColumnD(allQ, numberFrames - 1)[7]));

        if (q_girl2 != null && cntAvatar > 1)
        {
            float vertical2 = Mathf.Max((float)MathFunc.MatrixGetColumnD(q_girl2, 1)[8], (float)MathFunc.MatrixGetColumnD(q_girl2, secondNumberFrames - 1)[8]);
            float horizontal2 = Mathf.Max((float)MathFunc.MatrixGetColumnD(q_girl2, 1)[7], (float)MathFunc.MatrixGetColumnD(q_girl2, secondNumberFrames - 1)[7]);

            float max2 = Mathf.Max(vertical2, horizontal2);

            if (max2 > max) return max2;
        }

        return max;
    }

    public void PlayOneFrame()
    {
        if (!IsEditing)
        {
            if (allQ.GetUpperBound(1) >= frameN)
            {
                qf = MathFunc.MatrixGetColumnD(allQ, firstFrame + frameN);
                if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
                    for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
                        qf[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = 0;
            }
            SetAllDof(qf);
            if (!isPaused) SetFrameN(frameN + 1);
        }
    }

    public void InitPoseAvatar()
    {
        qf = MathFunc.MatrixGetColumnD(allQ, 1);
        SetAllDof(qf);
    }


    public void StartEditing()
    {
        IsEditing = true;
        canResumeAnimation = false;
        if (sliderAnimation) sliderAnimation.DisableSlider();
    }

    public void StopEditing()
    {
        statManager.ResetTemporaries();
        UpdateFullKinematics(false);
        IsEditing = false;
        Pause();
        if (sliderAnimation) 
            sliderAnimation.EnableSlider();
    }

    public void SetAllDof(double[] _qf){
        CenterAvatar(girl1, girl1Hip);
        SetThigh(_qf);
        SetShin(_qf);
        SetRightArm(_qf);
        SetLeftArm(_qf);
    }

    public void SetQfThigh(float _value)
    {
        int ddl = girl1ThighControl.avatarIndex;
        qf[ddl] = _value;
    }

    public void SetThigh(double[] _qf){
        int ddl = girl1ThighControl.avatarIndex;
        girl1LeftThigh.transform.localEulerAngles = new Vector3(-(float)_qf[ddl], 0f, 0f) * Mathf.Rad2Deg;
        girl1RightThigh.transform.localEulerAngles = new Vector3(-(float)_qf[ddl], 0f, 0f) * Mathf.Rad2Deg;
    }

    public void ControlThigh(float _value)
    {
        SetQfThigh(_value);
        SetThigh(qf);
    }

    public void SetQfShin(float _value)
    {
        int ddl = girl1LegControl.avatarIndex;
        qf[ddl] = _value;
    }

    public void SetShin(double[] _qf)
    {
        int ddl = girl1LegControl.avatarIndex;
        girl1LeftLeg.transform.localEulerAngles = new Vector3((float)_qf[ddl], 0f, 0f) * Mathf.Rad2Deg;
        girl1RightLeg.transform.localEulerAngles = new Vector3((float)_qf[ddl], 0f, 0f) * Mathf.Rad2Deg;
    }

    public void ControlShin(float _value)
    {
        SetQfShin(_value);
        SetShin(qf);
    }

    public void SetQfRightArmAbduction(float _value)
    {
        int ddl = girl1RightArmControlAbd.avatarIndex;
        qf[ddl] = _value;
    }
    public void SetQfRightArmFlexion(float _value)
    {
        int ddl =girl1RightArmControlFlex.avatarIndex;
        qf[ddl] = _value;
    }

    protected void SetRightArm(double[] _qf)
    {
        int ddlAbduction = girl1RightArmControlAbd.avatarIndex;
        int ddlFlexion = girl1RightArmControlFlex.avatarIndex;
        girl1RightArm.transform.localEulerAngles = new Vector3((float)_qf[ddlFlexion], 0, (float)_qf[ddlAbduction]) * Mathf.Rad2Deg;
    }

    public void ControlRightArmAbduction(float _value)
    {
        SetQfRightArmAbduction(_value);
        SetRightArm(qf);
    }
    
    public void ControlRightArmFlexion(float _value)
    {
        SetQfRightArmFlexion(_value);
        SetRightArm(qf);
    }

    public void SetQfLeftArmAbduction(float _value)
    {
        int ddl = girl1LeftArmControlAbd.avatarIndex;
        qf[ddl] = _value;
    }
    public void SetQfLeftArmFlexion(float _value)
    {
        int ddl = girl1LeftArmControlFlex.avatarIndex;
        qf[4] = _value;
    }

    protected void SetLeftArm(double[] _qf)
    {
        int ddlAbduction = girl1LeftArmControlAbd.avatarIndex;
        int ddlFlexion = girl1LeftArmControlFlex.avatarIndex;
        girl1LeftArm.transform.localEulerAngles = new Vector3((float)_qf[ddlFlexion], 0, (float)_qf[ddlAbduction]) * Mathf.Rad2Deg;
    }

    public void ControlLeftArmAbduction(float _value)
    {
        SetQfLeftArmAbduction(_value);
        SetLeftArm(qf);
    }

    public void ControlLeftArmFlexion(float _value)
    {
        SetQfLeftArmFlexion(_value);
        SetLeftArm(qf);
    }

    public bool PauseAvatar()
    {
        if (MainParameters.Instance.joints.nodes == null || girl1 == null || !girl1.activeSelf) 
            return false;

        girl1.transform.rotation = Quaternion.identity;
        isPaused = !isPaused;

        if (cntAvatar > 1)
            secondPaused = !secondPaused;
        return true;
    }

    public void ResetFrame()
    {
        canResumeAnimation = false;
        SetFrameN(0);
        firstFrame = 0;
        numberFrames = 0;
        timeElapsed = 0;

        pauseTime = 0;
        pauseStart = 0;

        secondFrameN = 0;
        cntAvatar = 1;
    }
}
