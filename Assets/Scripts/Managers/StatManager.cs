﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public struct InputData
{
    public float Velocity;
    public float Distance;
    public float Duration;
}

[System.Serializable]
public struct PlayerInfo
{
    public string Name;
    public string Id;
    public float Score;
    public InputData input;
}

[System.Serializable]
public struct PlayerReplayInfo
{
    public PlayerInfo player;
    public AnimationInfo replay;
}

public class StatManager : MonoBehaviour
{
    public PlayerInfo info;
    protected GameObject selectedJoint;
    protected ControlSegmentGeneric currentControlSegment;
    protected bool isRotating = false;
    protected Vector3 initPosition;
    public int currentJointSubIdx;
    RaycastHit hit;

    public string dofName;

    Camera _avatarCameraInternal;
    Ray mouseRay { get {
            if (_avatarCameraInternal == null)
            {
                var tp = GameObject.Find("AvatarCamera"); 
                if (tp == null) return new Ray();
                _avatarCameraInternal = tp.GetComponent<Camera>();
            }
            return _avatarCameraInternal.ScreenPointToRay(Input.mousePosition);
    }}
    Color colorBlue;
    Color colorWhite;

    GameObject arrow;

    //    GameObject errorPrefab;
    //    Text errorMessage;

    // Caching
    GameManager gameManager;
    DrawManager drawManager;
    LevelManager levelManager;
    UIManager uiManager;
    GameObject error;
    BaseProfile _baseProfileInternal;
    BaseProfile baseProfile { get {
        if (error == null) error = GameObject.Find("Training");
        if (_baseProfileInternal == null) _baseProfileInternal = error.GetComponent<BaseProfile>();
        return _baseProfileInternal;}
    }


    void Start()
    {
        gameManager = transform.parent.GetComponentInChildren<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        levelManager = transform.parent.GetComponentInChildren<LevelManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();
        error = GameObject.Find("Training");

        colorBlue = Color.blue;
        colorBlue.a = 0.5f;

        colorWhite = Color.white;
        colorWhite.a = 0.5f;

    }

    public void ProfileLoad(string fileName)
    {
        ReadDataFromJSON(fileName);
    }

    public void ProfileSave()
    {
        if (info.Name != null)
        {
            WriteDataToJSON(info.Name);
        }
    }


    public void ProfileReplaySave(string fileName)
    {
        PlayerReplayInfo replayInfo = new PlayerReplayInfo();

        replayInfo.player = info;

        replayInfo.replay.Objective = "default";
        replayInfo.replay.Duration = MainParameters.Instance.joints.duration;
        replayInfo.replay.Somersault = MainParameters.Instance.joints.takeOffParam.Somersault;
        replayInfo.replay.Tilt = MainParameters.Instance.joints.takeOffParam.Tilt;
        replayInfo.replay.Twist = MainParameters.Instance.joints.takeOffParam.Twist;
        replayInfo.replay.HorizontalPosition = MainParameters.Instance.joints.takeOffParam.HorizontalPosition;
        replayInfo.replay.VerticalPosition = MainParameters.Instance.joints.takeOffParam.VerticalPosition;
        replayInfo.replay.SomersaultSpeed = MainParameters.Instance.joints.takeOffParam.SomersaultSpeed;
        replayInfo.replay.TiltSpeed = MainParameters.Instance.joints.takeOffParam.TiltSpeed;
        replayInfo.replay.TwistSpeed = MainParameters.Instance.joints.takeOffParam.TwistSpeed;
        replayInfo.replay.HorizontalSpeed = MainParameters.Instance.joints.takeOffParam.HorizontalSpeed;
        replayInfo.replay.VerticalSpeed = MainParameters.Instance.joints.takeOffParam.VerticalSpeed;

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            Nodes n = new Nodes();
            n.Name = MainParameters.Instance.joints.nodes[i].name;
            n.T = MainParameters.Instance.joints.nodes[i].T;
            n.Q = MainParameters.Instance.joints.nodes[i].Q;

            replayInfo.replay.nodes.Add(n);
        }

        string jsonData = JsonUtility.ToJson(replayInfo, true);
        File.WriteAllText(fileName, jsonData);
    }

    private void WriteDataToJSON(string fileName)
    {
        string jsonData = JsonUtility.ToJson(info, true);
        File.WriteAllText(fileName, jsonData);
    }

    private void ReadDataFromJSON(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);
        info = JsonUtility.FromJson<PlayerInfo>(dataAsJson);
    }

    void Update()
    {
        if (levelManager.currentState != SceneState.Training) return;

        bool hasHit = Physics.Raycast(mouseRay, out hit);

        if (Input.GetMouseButton(0))
        {
            if (isRotating)
            {
                Vector3 newPosition = Input.mousePosition;
                Vector3 mouseDistance = newPosition - initPosition;
                drawManager.girl1.transform.Rotate(Vector3.up * -mouseDistance.x / 5f);
                initPosition = newPosition;
            }
            if (drawManager.girl1 != null && !drawManager.IsEditing && !isRotating && hasHit)
            {
                initPosition = Input.mousePosition;
                isRotating = true;
            }

        }
        else
        {
            isRotating = false;
        }

        if (Input.GetMouseButtonDown(1) && hasHit)
        {
            HandleJointClick();
        }
    }

    public void ResetTemporaries(){
        if (!drawManager.IsEditing) return;

        if (currentControlSegment)
            currentControlSegment.DestroyCircle();
        currentControlSegment = null;
        selectedJoint = null;
        currentJointSubIdx = -1;
    }

    void HandleJointClick() {
        if (!uiManager.IsInEditingTab){
            // Prevent from changing avantar position if not in modification tab
            return;
        }
        
        var _previousTp = selectedJoint;
        var _nextJointSubIdx = currentJointSubIdx + 1;  // Assume for now same joint
        ResetTemporaries();

        selectedJoint = hit.collider.gameObject;
        ControlSegmentGeneric[] _controlSegment = selectedJoint.GetComponents<ControlSegmentGeneric>();
        if (selectedJoint != _previousTp){
            // If not the same joint, reset to 0
            _nextJointSubIdx = 0;
        } else {
            // If we reached the end, joint is unselected
            if (_nextJointSubIdx >= _controlSegment.Length){
                drawManager.StopEditing();
                return;
            }
        }
        drawManager.StartEditing();

        currentJointSubIdx = _nextJointSubIdx;
        currentControlSegment = _controlSegment[currentJointSubIdx];
        currentControlSegment.Init(AddNode);

        baseProfile.InitDropdownDDLNames(currentControlSegment.avatarIndex);
        baseProfile.NodeName(currentControlSegment.dofName);
    }

    public int FindPreviousNode(int _dof)
    {
        int last = MainParameters.Instance.joints.nodes[_dof].T.Length - 1;
        if (drawManager.frameN == 0) 
            return 0;
        else if (drawManager.frameNtime == MainParameters.Instance.joints.nodes[_dof].T[last]) 
            return last;

        int i = 0;
        while (
                i < MainParameters.Instance.joints.nodes[_dof].T.Length
                && drawManager.frameNtime >= MainParameters.Instance.joints.nodes[_dof].T[i]
            )
        {
            i++;
        }
        return i - 1;
    }

    public int AddNode(int _dof)
    {
        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(_dof, true);

        int node = FindPreviousNode(_dof);
        if (MainParameters.Instance.joints.nodes[_dof].T[node] == drawManager.frameNtime)
            return node;


        float[] T = new float[MainParameters.Instance.joints.nodes[_dof].T.Length + 1];
        float[] Q = new float[MainParameters.Instance.joints.nodes[_dof].Q.Length + 1];

        for (int i = 0; i <= node; i++)
        {
            T[i] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }

        T[node + 1] = drawManager.frameNtime;
        Q[node + 1] = currentControlSegment.angle;

        for (int i = node + 1; i < MainParameters.Instance.joints.nodes[_dof].T.Length; i++)
        {
            T[i + 1] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i + 1] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }
        MainParameters.Instance.joints.nodes[_dof].T = MathFunc.MatrixCopy(T);
        MainParameters.Instance.joints.nodes[_dof].Q = MathFunc.MatrixCopy(Q);

        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(_dof, true);

        return node + 1;
    }
}
