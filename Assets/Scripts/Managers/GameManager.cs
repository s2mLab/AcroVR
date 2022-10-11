using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Crosstales.FB;
using System.Globalization;


[System.Serializable]
public struct AllMissionNodes
{
    public MissionNodes HipFlexion;
    public MissionNodes KneeFlexion;
    public MissionNodes RightArmFlexion;
    public MissionNodes RightArmAbduction;
    public MissionNodes LeftArmFlexion;
    public MissionNodes LeftArmAbduction;
}

[System.Serializable]
public struct MissionSolution
{
    // For each of the item, if it is null, then this item is ignored.
    // If the array is one element, then answer must equal the solution's value. 
    // If the array is two elements, then answer must be comprised between solution[0] and solution[1].

    // Solution on general parameters
    public float[] Duration;
    public bool[] UseGravity;
    public bool[] StopOnGround;

    // Solution on take off parameters
    public float[] Somersault;
    public float[] Tilt;
    public float[] Twist;
    public float[] HorizontalPosition;
    public float[] VerticalPosition;
    public float[] SomersaultSpeed;
    public float[] TiltSpeed;
    public float[] TwistSpeed;
    public float[] HorizontalSpeed;
    public float[] VerticalSpeed;
    
    // Solution on resulting computation
    public float[] TravelDistance; 
    public float[] HorizontalTravelDistance; 
    public float[] VerticalTravelDistance; 

    // Solution on angle nodes
    public AllMissionNodes Nodes;
}

[System.Serializable]
public struct MissionInfo
{
    public string Name;
    public int Level;

    public int PresetCondition;
    public UserUIInputsIsActive EnabledInputs;
    public AllMissionNodes StartingPositions;

    public MissionSolution Solution;
    public string Hint;

    public string ToHash(){
        return Hash128.Compute(Name).ToString();
    }
}

[System.Serializable]
public class MissionList
{
    public int count;
    public List<MissionInfo> missions = new List<MissionInfo>();
}

[System.Serializable]
public struct MissionNodes
{
    public float[] T;
    public float[,] Q;
}

[System.Serializable]
public struct Nodes
{
    public string Name;
    public float[] T;
    public float[] Q;
}

[System.Serializable]
public class AnimationInfo
{
    public string Objective;
    public float Duration;
    public bool UseGravity;
    public bool StopOnGround;
    public float Somersault;
    public float Tilt;
    public float Twist;
    public float HorizontalPosition;
    public float VerticalPosition;
    public float SomersaultSpeed;
    public float TiltSpeed;
    public float TwistSpeed;
    public float HorizontalSpeed;
    public float VerticalSpeed;
    public int Condition;


    public List<Nodes> nodes = new List<Nodes>();
}

[System.Serializable]
public class ConditionList
{
    public int count;
    public List<ConditionInfo> conditions = new List<ConditionInfo>();
}

[System.Serializable]
public class ConditionInfo
{
    public string name;
    public UserUIInputsValues userInputsValues = new UserUIInputsValues();
}

public class GameManager : MonoBehaviour
{
    protected BaseProfile profile;
    protected DrawManager drawManager;
    protected MissionManager missionManager;
    protected UIManager uiManager;
    public MissionInfo mission;

	public string pathDataFiles;
	public string pathUserDocumentsFiles;
	public string pathUserSystemFiles;

    public ConditionList listCondition;

	string conditionJsonFileName;				// Répertoire et nom du fichier des conditions

	private void Start()
    {
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        System.Globalization.CultureInfo ci = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        ci.NumberFormat = nfi;

		GetPathForDataFiles();

		// Copier le fichier des conditions dans un répertoire que l'utilisateur peut accéder en mode "Write".
		// Si utilisez via un fichier d'installation, alors le logiciel va s'installer dans un répertoire où l'utilisateur n'aura probablement pas d'accès "Write" (C:\Program Files).

		conditionJsonFileName = string.Format(@"{0}/Conditions.json", pathUserSystemFiles);
		try
		{
			File.Copy(string.Format(@"{0}/ConditionJson/Conditions.json", pathDataFiles), conditionJsonFileName, false);
		}
		catch (IOException e)
		{
			if (!e.Message.Contains("already exists"))
				Debug.Log("Erreur GameManager(Start): " + e.Message);
		}
		LoadConditions(conditionJsonFileName);
        LoadMissions(string.Format(@"{0}/MissionJson/Missions.json", pathDataFiles));
	}

    public int MissionLoad()
    {
        ExtensionFilter[] extensions = new[]
        {
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileAllFiles, "*" ),
        };

		string dirSimulationFiles = string.Format(@"{0}/SimulationJson", pathDataFiles);

		string fileName = FileBrowser.OpenSingleFile(MainParameters.Instance.languages.Used.movementLoadDataFileTitle, dirSimulationFiles, extensions);
        if (fileName.Length <= 0)
        {
            WriteToLogFile("fileName.Length false: " + fileName.Length.ToString());
            return -1;
        }

        WriteToLogFile(fileName);
        WriteToLogFile("CultureInfo.CurrentCulture.Name: " + CultureInfo.CurrentCulture.Name);

        string extension = GetSimpleExtension(fileName);
        if (extension == "txt")
        {
            if(!ReadDataFiles_s(fileName))
                return -2;
        }
        else
        {
            if (!ReadAniFromJson(fileName))
                return -3;
        }

        return 1;
    }

    public int LoadSimulationSecond()
    {
        ExtensionFilter[] extensions = new[]
        {
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileTxtFile, "*"),
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileAllFiles, "*" ),
        };

		string dirSimulationFiles = string.Format(@"{0}\SimulationJson", pathDataFiles);

		string fileName = FileBrowser.OpenSingleFile(MainParameters.Instance.languages.Used.movementLoadDataFileTitle, dirSimulationFiles, extensions);
        if (fileName.Length <= 0)
        {
            WriteToLogFile("fileName.Length false: " + fileName.Length.ToString());
            return -1;
        }

        WriteToLogFile(fileName);

        WriteToLogFile("CultureInfo.CurrentCulture.Name: " + CultureInfo.CurrentCulture.Name);

        string extension = GetSimpleExtension(fileName);
        if (extension == "txt")
        {
            if (!ReadDataFileSecond(fileName))
                return -2;
        }
        else
        {
            if (!ReadAniFromJsonSecond(fileName))
                return -3;
        }

        return 1;
    }

    private string GetSimpleExtension(string fileName)
    {
        return Path.GetExtension(fileName).Replace(".", "");
    }

    public void ReadDataFromJSON(string fileName)
    {
        WriteToLogFile("ReadDataFromJSON()");

        string dataAsJson = File.ReadAllText(fileName);
        mission = JsonUtility.FromJson<MissionInfo>(dataAsJson);
    }

    public void InitAnimationInfo()
    {
        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = null;
        jointsTemp.nodes = null;
        jointsTemp.Duration = 1;
        jointsTemp.UseGravity = false;
        jointsTemp.StopOnGround = true;
        jointsTemp.condition = 0;
        jointsTemp.takeOffParam.Somersault = 0;
        jointsTemp.takeOffParam.Tilt = 0;
        jointsTemp.takeOffParam.Twist = 0;
        jointsTemp.takeOffParam.HorizontalPosition = 0;
        jointsTemp.takeOffParam.VerticalPosition = 0;
        jointsTemp.takeOffParam.SomersaultSpeed = 0;
        jointsTemp.takeOffParam.TiltSpeed = 0;
        jointsTemp.takeOffParam.TwistSpeed = 0;
        jointsTemp.takeOffParam.HorizontalSpeed = 0;
        jointsTemp.takeOffParam.VerticalSpeed = 0;

        jointsTemp.nodes = new MainParameters.StrucNodes[6];

        for (int i = 0; i < 6; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;

            if (i == 0) jointsTemp.nodes[i].name = "Hip_Flexion";
            else if (i == 1) jointsTemp.nodes[i].name = "Knee_Flexion";
            else if (i == 2) jointsTemp.nodes[i].name = "Right_Arm_Flexion";
            else if (i == 3) jointsTemp.nodes[i].name = "Right_Arm_Abduction";
            else if (i == 4) jointsTemp.nodes[i].name = "Left_Arm_Flexion";
            else if (i == 5) jointsTemp.nodes[i].name = "Left_Arm_Abduction";


            jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
            jointsTemp.nodes[i].T = new float[] { 0, 1.000000f };
            jointsTemp.nodes[i].Q = new float[] { 0, 0.0f};
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        MainParameters.Instance.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;
    }

    private bool ReadAniFromJson(string fileName)
    {
        WriteToLogFile("ReadAniFromJSON()");

        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        AnimationInfo info = JsonUtility.FromJson<AnimationInfo>(dataAsJson);

        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.Duration = info.Duration;
        jointsTemp.UseGravity = info.UseGravity;
        jointsTemp.StopOnGround = info.StopOnGround;
        jointsTemp.condition = info.Condition;
        jointsTemp.takeOffParam.Somersault = info.Somersault;
        jointsTemp.takeOffParam.Tilt = info.Tilt;
        jointsTemp.takeOffParam.Twist = info.Twist;
        jointsTemp.takeOffParam.HorizontalPosition = info.HorizontalPosition;
        jointsTemp.takeOffParam.VerticalPosition = info.VerticalPosition;
        jointsTemp.takeOffParam.SomersaultSpeed = info.SomersaultSpeed;
        jointsTemp.takeOffParam.TiltSpeed = info.TiltSpeed;
        jointsTemp.takeOffParam.TwistSpeed = info.TwistSpeed;
        jointsTemp.takeOffParam.HorizontalSpeed = info.HorizontalSpeed;
        jointsTemp.takeOffParam.VerticalSpeed = info.VerticalSpeed;

        jointsTemp.nodes = new MainParameters.StrucNodes[info.nodes.Count];

        WriteToLogFile("For() Start info.nodes.Count: " + info.nodes.Count.ToString());

        for (int i = 0; i < info.nodes.Count; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;
            jointsTemp.nodes[i].name = info.nodes[i].Name;
            jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
            jointsTemp.nodes[i].T = info.nodes[i].T;
            jointsTemp.nodes[i].Q = info.nodes[i].Q;
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        MainParameters.Instance.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        return true;
    }

    private bool ReadAniFromJsonSecond(string fileName)
    {
        WriteToLogFile("ReadAniFromJsonSecond()");

        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        AnimationInfo info = JsonUtility.FromJson<AnimationInfo>(dataAsJson);

        AvatarSimulation.StrucJoints jointsTemp = new AvatarSimulation.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.Duration = info.Duration;
        jointsTemp.UseGravity = info.UseGravity;
        jointsTemp.StopOnGround = info.StopOnGround;
        jointsTemp.condition = info.Condition;
        jointsTemp.takeOffParam.Somersault = info.Somersault;
        jointsTemp.takeOffParam.Twist = info.Twist;
        jointsTemp.takeOffParam.Tilt = info.Tilt;
        jointsTemp.takeOffParam.HorizontalPosition = info.HorizontalPosition;
        jointsTemp.takeOffParam.VerticalPosition = info.VerticalPosition;
        jointsTemp.takeOffParam.SomersaultSpeed = info.SomersaultSpeed;
        jointsTemp.takeOffParam.TwistSpeed = info.TwistSpeed;
        jointsTemp.takeOffParam.TiltSpeed = info.TiltSpeed;
        jointsTemp.takeOffParam.HorizontalSpeed = info.HorizontalSpeed;
        jointsTemp.takeOffParam.VerticalSpeed = info.VerticalSpeed;

        jointsTemp.nodes = new AvatarSimulation.StrucNodes[info.nodes.Count];

        WriteToLogFile("For() Start info.nodes.Count: " + info.nodes.Count.ToString());

        for (int i = 0; i < info.nodes.Count; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;
            jointsTemp.nodes[i].name = info.nodes[i].Name;
            jointsTemp.nodes[i].interpolation = drawManager.secondParameters.interpolationDefault;
            jointsTemp.nodes[i].T = info.nodes[i].T;
            jointsTemp.nodes[i].Q = info.nodes[i].Q;
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        drawManager.secondParameters.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        drawManager.secondParameters.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        return true;
    }

    public void SaveCondition(string name)
    {
        ConditionInfo n = new ConditionInfo();
        n.name = name;
        n.userInputsValues.SetAll(uiManager.userInputs);

        listCondition.conditions.Add(n);
        listCondition.count++;

        string jsonData = JsonUtility.ToJson(listCondition, true);
        File.WriteAllText(conditionJsonFileName, jsonData);
    }

    public void RemoveCondition(int index)
    {
        listCondition.conditions.RemoveAt(index);
        listCondition.count--;

        string jsonData = JsonUtility.ToJson(listCondition, true);
        File.WriteAllText(conditionJsonFileName, jsonData);
    }

    public bool LoadConditions(string fileName)
    {
		string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        listCondition = JsonUtility.FromJson<ConditionList>(dataAsJson);
        return true;
    }

    public bool LoadMissions(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        missionManager.SetMissions(dataAsJson);

        return true;
    }

    private void WriteDataToJSON(string fileName)
    {
        AnimationInfo info = new AnimationInfo();

        info.Objective = "default";
        info.Duration = MainParameters.Instance.joints.Duration;
        info.UseGravity = MainParameters.Instance.joints.UseGravity;
        info.StopOnGround = MainParameters.Instance.joints.StopOnGround;
        info.Condition = MainParameters.Instance.joints.condition;
        info.Somersault = MainParameters.Instance.joints.takeOffParam.Somersault;
        info.Tilt = MainParameters.Instance.joints.takeOffParam.Tilt;
        info.Twist = MainParameters.Instance.joints.takeOffParam.Twist;
        info.HorizontalPosition = MainParameters.Instance.joints.takeOffParam.HorizontalPosition;
        info.VerticalPosition = MainParameters.Instance.joints.takeOffParam.VerticalPosition;
        info.SomersaultSpeed = MainParameters.Instance.joints.takeOffParam.SomersaultSpeed;
        info.TiltSpeed = MainParameters.Instance.joints.takeOffParam.TiltSpeed;
        info.TwistSpeed = MainParameters.Instance.joints.takeOffParam.TwistSpeed;
        info.HorizontalSpeed = MainParameters.Instance.joints.takeOffParam.HorizontalSpeed;
        info.VerticalSpeed = MainParameters.Instance.joints.takeOffParam.VerticalSpeed;

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            Nodes n = new Nodes();
            n.Name = MainParameters.Instance.joints.nodes[i].name;
            n.T = MainParameters.Instance.joints.nodes[i].T;
            n.Q = MainParameters.Instance.joints.nodes[i].Q;

            info.nodes.Add(n);
        }

        string jsonData = JsonUtility.ToJson(info, true);
        File.WriteAllText(fileName, jsonData);
    }

    public void WriteDataFiles_s(string fileName)
    {
        string fileLines = string.Format(
            "Duration: {0}{1}Condition: {2}{3}VerticalSpeed: {4:0.000}{5}AnteroposteriorSpeed: {6:0.000}{7}SomersaultSpeed: {8:0.000}{9}TwistSpeed: {10:0.000}{11}Tilt: {12:0.000}{13}Rotation: {14:0.000}{15}{16}",
            MainParameters.Instance.joints.Duration, System.Environment.NewLine,
            MainParameters.Instance.joints.condition, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.Somersault, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.Tilt, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.Twist, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.HorizontalPosition, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.VerticalPosition, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.SomersaultSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.TiltSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.TwistSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.HorizontalSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.VerticalSpeed, System.Environment.NewLine
        );

        fileLines = string.Format("{0}Nodes{1}DDL, name, interpolation (type, numIntervals, slopes), T, Q{2}", fileLines, System.Environment.NewLine, System.Environment.NewLine);

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            fileLines = string.Format("{0}{1}:{2}:{3},{4},{5:0.000000},{6:0.000000}:", fileLines, i + 1, MainParameters.Instance.joints.nodes[i].name, MainParameters.Instance.joints.nodes[i].interpolation.type,
                MainParameters.Instance.joints.nodes[i].interpolation.numIntervals, MainParameters.Instance.joints.nodes[i].interpolation.slope[0], MainParameters.Instance.joints.nodes[i].interpolation.slope[1]);
            for (int j = 0; j < MainParameters.Instance.joints.nodes[i].T.Length; j++)
            {
                if (j < MainParameters.Instance.joints.nodes[i].T.Length - 1)
                    fileLines = string.Format("{0}{1:0.000000},", fileLines, MainParameters.Instance.joints.nodes[i].T[j]);
                else
                    fileLines = string.Format("{0}{1:0.000000}:", fileLines, MainParameters.Instance.joints.nodes[i].T[j]);
            }
            for (int j = 0; j < MainParameters.Instance.joints.nodes[i].Q.Length; j++)
            {
                if (j < MainParameters.Instance.joints.nodes[i].Q.Length - 1)
                    fileLines = string.Format("{0}{1:0.000000},", fileLines, MainParameters.Instance.joints.nodes[i].Q[j]);
                else
                    fileLines = string.Format("{0}{1:0.000000}:{2}", fileLines, MainParameters.Instance.joints.nodes[i].Q[j], System.Environment.NewLine);
            }
        }

        System.IO.File.WriteAllText(fileName, fileLines);
    }

    private bool ReadDataFiles_s(string fileName)
    {
        WriteToLogFile("ReadDataFilesTxT()");

        string[] fileLines = System.IO.File.ReadAllLines(fileName);

        if(fileLines[0][0] == '{')
        {
            WriteToLogFile("Parse Error [0]: " + fileLines[0][0]);
            return false;
        }

        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.Duration = 0;
        jointsTemp.UseGravity = false;
        jointsTemp.StopOnGround = true;
        jointsTemp.condition = 0;
        jointsTemp.takeOffParam.Somersault = 0;
        jointsTemp.takeOffParam.Tilt = 0;
        jointsTemp.takeOffParam.Twist = 0;
        jointsTemp.takeOffParam.HorizontalPosition = 0;
        jointsTemp.takeOffParam.VerticalPosition = 0;
        jointsTemp.takeOffParam.SomersaultSpeed = 0;
        jointsTemp.takeOffParam.TiltSpeed = 0;
        jointsTemp.takeOffParam.TwistSpeed = 0;
        jointsTemp.takeOffParam.HorizontalSpeed = 0;
        jointsTemp.takeOffParam.VerticalSpeed = 0;

        string[] values;
        int ddlNum = -1;

        WriteToLogFile("For() Start fileLines.Length: " + fileLines.Length.ToString());

        for (int i = 0; i < fileLines.Length; i++)
        {
            values = Regex.Split(fileLines[i], ":");

            WriteToLogFile("Regex.Split values: " + values[0]);

            if (values[0].Contains("Duration"))
            {
                WriteToLogFile("In Duration");

                jointsTemp.Duration = Utils.ToFloat(values[1]);
                if (jointsTemp.Duration == -999)
                    jointsTemp.Duration = MainParameters.Instance.DurationDefault;

                WriteToLogFile("jointsTemp.Duration: " + jointsTemp.Duration.ToString());
            }
            else if (values[0].Contains("UseGravity"))
            {
                WriteToLogFile("In UseGravity");
                jointsTemp.UseGravity = Utils.ToBool(values[1]);
                WriteToLogFile("jointsTemp.UseGravity " + jointsTemp.UseGravity.ToString());
            }
            else if (values[0].Contains("StopOnGround"))
            {
                WriteToLogFile("In StopOnGround");
                jointsTemp.StopOnGround = Utils.ToBool(values[1]);
                WriteToLogFile("jointsTemp.StopOnGround " + jointsTemp.StopOnGround.ToString());
            }
            else if (values[0].Contains("Condition"))
            {
                WriteToLogFile("In Condition");

                jointsTemp.condition = int.Parse(values[1], CultureInfo.InvariantCulture);
                if (jointsTemp.condition == -999)
                    jointsTemp.condition = MainParameters.Instance.conditionDefault;

                WriteToLogFile("jointsTemp.condition: " + jointsTemp.condition.ToString());
            }
            else if (values[0].Contains("VerticalSpeed"))
            {
                WriteToLogFile("In VerticalSpeed");

                jointsTemp.takeOffParam.VerticalSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.VerticalSpeed == -999)
                    jointsTemp.takeOffParam.VerticalSpeed = MainParameters.Instance.takeOffParamDefault.VerticalSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.VerticalSpeed: " + jointsTemp.takeOffParam.VerticalSpeed.ToString());
            }
            else if (values[0].Contains("HorizontalSpeed"))
            {
                WriteToLogFile("In HorizontalSpeed");

                jointsTemp.takeOffParam.HorizontalSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.HorizontalSpeed == -999)
                    jointsTemp.takeOffParam.HorizontalSpeed = MainParameters.Instance.takeOffParamDefault.HorizontalSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.HorizontalSpeed: " + jointsTemp.takeOffParam.HorizontalSpeed.ToString());
            }
            else if (values[0].Contains("SomersaultSpeed"))
            {
                WriteToLogFile("In SomersaultSpeed");

                jointsTemp.takeOffParam.SomersaultSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.SomersaultSpeed == -999)
                    jointsTemp.takeOffParam.SomersaultSpeed = MainParameters.Instance.takeOffParamDefault.SomersaultSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.SomersaultSpeed: " + jointsTemp.takeOffParam.SomersaultSpeed.ToString());
            }
            else if (values[0].Contains("TwistSpeed"))
            {
                WriteToLogFile("In TwistSpeed");

                jointsTemp.takeOffParam.TwistSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.TwistSpeed == -999)
                    jointsTemp.takeOffParam.TwistSpeed = MainParameters.Instance.takeOffParamDefault.TwistSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.TwistSpeed: " + jointsTemp.takeOffParam.TwistSpeed.ToString());
            }
            else if (values[0].Contains("Tilt"))
            {
                WriteToLogFile("In Tilt");

                jointsTemp.takeOffParam.Tilt = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.Tilt == -999)
                    jointsTemp.takeOffParam.Tilt = MainParameters.Instance.takeOffParamDefault.Tilt;

                WriteToLogFile("jointsTemp.takeOffParam.Tilt: " + jointsTemp.takeOffParam.Tilt.ToString());
            }
            else if (values[0].Contains("Somersault"))
            {
                WriteToLogFile("In Somerssault");

                jointsTemp.takeOffParam.Somersault = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.Somersault == -999)
                    jointsTemp.takeOffParam.Somersault = MainParameters.Instance.takeOffParamDefault.Somersault;

                WriteToLogFile("jointsTemp.takeOffParam.Somersault: " + jointsTemp.takeOffParam.Somersault.ToString());
            }
            else if (values[0].Contains("DDL"))
            {
                WriteToLogFile("In DDL");

                jointsTemp.nodes = new MainParameters.StrucNodes[fileLines.Length - i - 1];
                ddlNum = 0;

                int temp = fileLines.Length - i - 1;

                WriteToLogFile("jointsTemp.nodes: " + temp.ToString());
            }
            else if (ddlNum >= 0)
            {
                WriteToLogFile("In ddlNum: " + ddlNum.ToString());

                jointsTemp.nodes[ddlNum].ddl = int.Parse(values[0], CultureInfo.InvariantCulture);

                WriteToLogFile("jointsTemp.nodes[ddlNum].ddl: " + jointsTemp.nodes[ddlNum].ddl.ToString());

                jointsTemp.nodes[ddlNum].name = values[1];

                WriteToLogFile("jointsTemp.nodes[ddlNum].name: " + jointsTemp.nodes[ddlNum].name);

                jointsTemp.nodes[ddlNum].interpolation = MainParameters.Instance.interpolationDefault;

                WriteToLogFile("jointsTemp.nodes[ddlNum].interpolation: " + jointsTemp.nodes[ddlNum].interpolation.type.ToString());

                int indexTQ = 2;

                WriteToLogFile("values.Length: " + values.Length.ToString());

                if (values.Length > 5)
                {
                    WriteToLogFile("In values.Length > 5");

                    string[] subValues;
                    subValues = Regex.Split(values[2], ",");
                    if (subValues[0].Contains(MainParameters.InterpolationType.CubicSpline.ToString()))
                        jointsTemp.nodes[ddlNum].interpolation.type = MainParameters.InterpolationType.CubicSpline;
                    else
                        jointsTemp.nodes[ddlNum].interpolation.type = MainParameters.InterpolationType.Quintic;
                    jointsTemp.nodes[ddlNum].interpolation.numIntervals = int.Parse(subValues[1], CultureInfo.InvariantCulture);
                    jointsTemp.nodes[ddlNum].interpolation.slope[0] = Utils.ToFloat(subValues[2]);
                    jointsTemp.nodes[ddlNum].interpolation.slope[1] = Utils.ToFloat(subValues[3]);
                    indexTQ++;
                }
                jointsTemp.nodes[ddlNum].T = ExtractDataTQ(values[indexTQ]);

                foreach(float a in jointsTemp.nodes[ddlNum].T)
                    WriteToLogFile("jointsTemp.nodes[ddlNum].T: " + a.ToString());

                jointsTemp.nodes[ddlNum].Q = ExtractDataTQ(values[indexTQ + 1]);

                foreach (float b in jointsTemp.nodes[ddlNum].Q)
                    WriteToLogFile("jointsTemp.nodes[ddlNum].Q: " + b.ToString());

                jointsTemp.nodes[ddlNum].ddlOppositeSide = -1;

                WriteToLogFile("jointsTemp.nodes[ddlNum].ddlOppositeSide: " + jointsTemp.nodes[ddlNum].ddlOppositeSide.ToString());

                ddlNum++;
            }
        }

        MainParameters.Instance.joints = jointsTemp;

        WriteToLogFile("Assigned MainParameters.Instance.joints");

        //        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        //        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        if (MainParameters.Instance.joints.lagrangianModelName == MainParameters.LagrangianModelNames.Sasha23ddl)
        {
            WriteToLogFile("LagrangianModelSasha23ddl()");

            LagrangianModelSasha23ddl lagrangianModelSasha23ddl = new LagrangianModelSasha23ddl();
            MainParameters.Instance.joints.lagrangianModel = lagrangianModelSasha23ddl.GetParameters;
        }
        else
        {
            WriteToLogFile("LagrangianModelSimple()");

            LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
            MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;
        }

        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        int nDDL = 0;
        MainParameters.StrucNodes[] nodes = new MainParameters.StrucNodes[joints.lagrangianModel.q2.Length];
        int nNodes = joints.nodes.Length;
        MainParameters.StrucInterpolation interpolation = joints.nodes[0].interpolation;

        WriteToLogFile("For() Start joints.lagrangianModel.q2.Length: " + joints.lagrangianModel.q2.Length.ToString());

        foreach (int i in joints.lagrangianModel.q2)
        {
            int j = 0;
            string ddlname = joints.lagrangianModel.ddlName[i - 1].ToLower();
            while (j < nNodes && !ddlname.Contains(joints.nodes[j].name.ToLower()))
                j++;
            if (j < nNodes)                                 // Articulations défini dans le fichier de données, le conserver
            {
                nodes[nDDL] = joints.nodes[j];
                nodes[nDDL].ddl = i;
            }
            else                                            // Articulations non défini dans le fichier de données, alors utilisé la définition de défaut selon le modèle Lagrangien
            {
                nodes[nDDL].ddl = i;
                nodes[nDDL].name = joints.lagrangianModel.ddlName[i - 1];
                nodes[nDDL].T = new float[3] { joints.Duration * 0.25f, joints.Duration * 0.5f, joints.Duration * 0.75f };
                nodes[nDDL].Q = new float[3] { 0, 0, 0 };
                nodes[nDDL].interpolation = interpolation;
                nodes[nDDL].ddlOppositeSide = -1;
            }
            nDDL++;
        }

        WriteToLogFile("For() Start nodes.Length: " + nodes.Length.ToString());

        for (int i = 0; i < nodes.Length; i++)
        {
            string nameOppSide = "";
            string name = nodes[i].name.ToLower();
            if (name.Contains("left") || name.Contains("right"))
            {
                if (name.Contains("left"))
                    nameOppSide = "right" + name.Substring(name.IndexOf("left") + 4);
                else
                    nameOppSide = "left" + name.Substring(name.IndexOf("right") + 5);
                for (int j = 0; j < nodes.Length; j++)
                {
                    name = nodes[j].name.ToLower();
                    if (name.Contains(nameOppSide))
                        nodes[i].ddlOppositeSide = j;
                }
            }
        }

        MainParameters.Instance.joints.nodes = nodes;

        return true;
    }

    private bool ReadDataFileSecond(string fileName)
    {
        WriteToLogFile("ReadDataFileSecondTxt()");

        string[] fileLines = System.IO.File.ReadAllLines(fileName);

        if (fileLines[0][0] == '{')
        {
            WriteToLogFile("Parse Error [0]: " + fileLines[0][0]);
            return false;
        }

        AvatarSimulation.StrucJoints jointsTemp = new AvatarSimulation.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.Duration = 0;
        jointsTemp.UseGravity = false;
        jointsTemp.StopOnGround = true;
        jointsTemp.condition = 0;
        jointsTemp.takeOffParam.Somersault = 0;
        jointsTemp.takeOffParam.Tilt = 0;
        jointsTemp.takeOffParam.Twist = 0;
        jointsTemp.takeOffParam.HorizontalPosition = 0;
        jointsTemp.takeOffParam.VerticalPosition = 0;
        jointsTemp.takeOffParam.SomersaultSpeed = 0;
        jointsTemp.takeOffParam.TiltSpeed = 0;
        jointsTemp.takeOffParam.TwistSpeed = 0;
        jointsTemp.takeOffParam.HorizontalSpeed = 0;
        jointsTemp.takeOffParam.VerticalSpeed = 0;

        string[] values;
        int ddlNum = -1;

        WriteToLogFile("For() Start fileLines.Length: " + fileLines.Length.ToString());

        for (int i = 0; i < fileLines.Length; i++)
        {
            values = Regex.Split(fileLines[i], ":");

            WriteToLogFile("Regex.Split values: " + values[0]);

            if (values[0].Contains("Duration"))
            {
                WriteToLogFile("In Duration");

                jointsTemp.Duration = Utils.ToFloat(values[1]);
                if (jointsTemp.Duration == -999)
                    jointsTemp.Duration = MainParameters.Instance.DurationDefault;

                WriteToLogFile("jointsTemp.Duration: " + jointsTemp.Duration.ToString());
            }
            else if (values[0].Contains("UseGravity"))
            {
                WriteToLogFile("In UseGravity");

                jointsTemp.UseGravity = Utils.ToBool(values[1]);
                WriteToLogFile("jointsTemp.UseGravity: " + jointsTemp.UseGravity.ToString());
            }
            else if (values[0].Contains("StopOnGround"))
            {
                WriteToLogFile("In StopOnGround");

                jointsTemp.StopOnGround = Utils.ToBool(values[1]);
                WriteToLogFile("jointsTemp.StopOnGround: " + jointsTemp.StopOnGround.ToString());
            }
            else if (values[0].Contains("Condition"))
            {
                WriteToLogFile("In Condition");

                jointsTemp.condition = int.Parse(values[1], CultureInfo.InvariantCulture);
                if (jointsTemp.condition == -999)
                    jointsTemp.condition = MainParameters.Instance.conditionDefault;

                WriteToLogFile("jointsTemp.condition: " + jointsTemp.condition.ToString());
            }
            else if (values[0].Contains("VerticalSpeed"))
            {
                WriteToLogFile("In VerticalSpeed");

                jointsTemp.takeOffParam.VerticalSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.VerticalSpeed == -999)
                    jointsTemp.takeOffParam.VerticalSpeed = MainParameters.Instance.takeOffParamDefault.VerticalSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.VerticalSpeed: " + jointsTemp.takeOffParam.VerticalSpeed.ToString());
            }
            else if (values[0].Contains("HorizontalSpeed"))
            {
                WriteToLogFile("In HorizontalSpeed");

                jointsTemp.takeOffParam.HorizontalSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.HorizontalSpeed == -999)
                    jointsTemp.takeOffParam.HorizontalSpeed = MainParameters.Instance.takeOffParamDefault.HorizontalSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.HorizontalSpeed: " + jointsTemp.takeOffParam.HorizontalSpeed.ToString());
            }
            else if (values[0].Contains("SomersaultSpeed"))
            {
                WriteToLogFile("In SomersaultSpeed");

                jointsTemp.takeOffParam.SomersaultSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.SomersaultSpeed == -999)
                    jointsTemp.takeOffParam.SomersaultSpeed = MainParameters.Instance.takeOffParamDefault.SomersaultSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.SomersaultSpeed: " + jointsTemp.takeOffParam.SomersaultSpeed.ToString());
            }
            else if (values[0].Contains("TwistSpeed"))
            {
                WriteToLogFile("In TwistSpeed");

                jointsTemp.takeOffParam.TwistSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.TwistSpeed == -999)
                    jointsTemp.takeOffParam.TwistSpeed = MainParameters.Instance.takeOffParamDefault.TwistSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.TwistSpeed: " + jointsTemp.takeOffParam.TwistSpeed.ToString());
            }
            else if (values[0].Contains("Tilt"))
            {
                WriteToLogFile("In Tilt");

                jointsTemp.takeOffParam.Tilt = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.Tilt == -999)
                    jointsTemp.takeOffParam.Tilt = MainParameters.Instance.takeOffParamDefault.Tilt;

                WriteToLogFile("jointsTemp.takeOffParam.Tilt: " + jointsTemp.takeOffParam.Tilt.ToString());
            }
            else if (values[0].Contains("Somersault"))
            {
                WriteToLogFile("In Somersault");

                jointsTemp.takeOffParam.Somersault = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.Somersault == -999)
                    jointsTemp.takeOffParam.Somersault = MainParameters.Instance.takeOffParamDefault.Somersault;

                WriteToLogFile("jointsTemp.takeOffParam.Somersault: " + jointsTemp.takeOffParam.Somersault.ToString());
            }
            else if (values[0].Contains("DDL"))
            {
                WriteToLogFile("In DDL");

                jointsTemp.nodes = new AvatarSimulation.StrucNodes[fileLines.Length - i - 1];
                ddlNum = 0;

                int temp = fileLines.Length - i - 1;

                WriteToLogFile("jointsTemp.nodes: " + temp.ToString());
            }
            else if (ddlNum >= 0)
            {
                WriteToLogFile("In ddlNum: " + ddlNum.ToString());

                jointsTemp.nodes[ddlNum].ddl = int.Parse(values[0], CultureInfo.InvariantCulture);

                WriteToLogFile("jointsTemp.nodes[ddlNum].ddl: " + jointsTemp.nodes[ddlNum].ddl.ToString());

                jointsTemp.nodes[ddlNum].name = values[1];

                WriteToLogFile("jointsTemp.nodes[ddlNum].name: " + jointsTemp.nodes[ddlNum].name);

                jointsTemp.nodes[ddlNum].interpolation = drawManager.secondParameters.interpolationDefault;

                WriteToLogFile("jointsTemp.nodes[ddlNum].interpolation: " + jointsTemp.nodes[ddlNum].interpolation.type.ToString());

                int indexTQ = 2;

                WriteToLogFile("values.Length: " + values.Length.ToString());

                jointsTemp.nodes[ddlNum].T = ExtractDataTQ(values[indexTQ]);
                jointsTemp.nodes[ddlNum].Q = ExtractDataTQ(values[indexTQ + 1]);
                jointsTemp.nodes[ddlNum].ddlOppositeSide = -1;
                ddlNum++;
            }
        }

        drawManager.secondParameters.joints = jointsTemp;

        WriteToLogFile("Assigned MainParameters.Instance.joints");

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        drawManager.secondParameters.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        return true;
    }

    public void SaveFile()
    {
		string fileName = FileBrowser.SaveFile(MainParameters.Instance.languages.Used.movementSaveDataFileTitle, pathUserDocumentsFiles, "DefaultFile", "json");
        if (fileName.Length <= 0)
            return;

        WriteDataToJSON(fileName);
    }

    private float[] ExtractDataTQ(string values)
    {
        string[] subValues = Regex.Split(values, ",");
        float[] data = new float[subValues.Length];
        for (int i = 0; i < subValues.Length; i++)
            data[i] = Utils.ToFloat(subValues[i]);
        return data;
    }

    public void InterpolationDDL()
    {
        int n = (int)(MainParameters.Instance.joints.Duration / MainParameters.Instance.joints.lagrangianModel.dt)+1;
        float[] t0 = new float[n];
        float[,] q0 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, n];

        GenerateQ0_s(MainParameters.Instance.joints.lagrangianModel, MainParameters.Instance.joints.Duration, 0, out t0, out q0);

        MainParameters.Instance.joints.t0 = MathFunc.MatrixCopy(t0);
        MainParameters.Instance.joints.q0 = MathFunc.MatrixCopy(q0);
    }

    private void GenerateQ0_s(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float tf, int qi, out float[] t0, out float[,] q0)
    {
        int[] ni;
        if (qi > 0)
            ni = new int[1] { qi };
        else
            ni = lagrangianModel.q2;

        float[] qd;
        int n = (int)(tf / lagrangianModel.dt)+1;
        t0 = new float[n];
        q0 = new float[lagrangianModel.nDDL, n];

        int i = 0;
        for (float interval = 0; interval < tf; interval += lagrangianModel.dt)
        {
            t0[i] = interval;
            Trajectory_ss(lagrangianModel, interval, ni, out qd);
            //            Trajectory trajectory = new Trajectory(lagrangianModel, interval, ni, out qd);
            //            trajectory.ToString();                  // Pour enlever un warning lors de la compilation
            for (int ddl = 0; ddl < qd.Length; ddl++)
                q0[ddl, i] = qd[ddl];
            i++;

            if (i >= n) break;
        }
    }

    private void Trajectory_ss(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float t, int[] qi, out float[] qd)
    {
        float[] qdotd;
        float[] qddotd;
        drawManager.Trajectory_s(lagrangianModel, t, qi, out qd, out qdotd, out qddotd);
    }

    public void DisplayDDL(int ddl, bool axisRange)
    {
        if (ddl >= 0)
        {
            transform.parent.GetComponentInChildren<AniGraphManager>().DisplayCurveAndNodes(0, ddl, axisRange);
            if (MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide >= 0)
            {
                transform.parent.GetComponentInChildren<AniGraphManager>().DisplayCurveAndNodes(1, MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide, true);
            }
        }
    }

    public void WriteToLogFile(string msg)
    {
#if UNITY_EDITOR 
        using (System.IO.StreamWriter logFile = new System.IO.StreamWriter(@".\LogFile.txt", true))
        {
            System.DateTime dt = System.DateTime.Now;
            logFile.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss") +": " + msg);
        }
#endif
    }

	// =================================================================================================================================================================
	/// <summary> Configuration des répertoires utilisés pour accéder aux différents fichiers de données, selon que la plateforme d'Unity utilisée (OSX, Windows, Editor). </summary>

	void GetPathForDataFiles()
	{
#if UNITY_STANDALONE_OSX                       // À modifier quand la plateforme OSX sera configuré, pas fait pour le moment
		//string dirSimulationFiles;
		//int n = Application.dataPath.IndexOf("/AcroVR.app");
		//if (n > 0)
		//	dirSimulationFiles = string.Format("{0}/SimulationFiles", Application.dataPath.Substring(0, n));
		//else
		//	dirSimulationFiles = string.Format("{0}/Documents", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		//Debug.Log(string.Format("Mac: dirSimulationFiles = {0}", dirSimulationFiles));

		//string dirCheckFileName = string.Format("{0}/Documents/AcroVR/Lib", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		//string checkFileName = string.Format("{0}/AcroVR.dll", dirCheckFileName);

		//string dirSimulationFiles = string.Format("{0}/Documents/AcroVR", Environment.GetFolderPath(Environment.SpecialFolder.Personal));

#elif UNITY_EDITOR

		pathDataFiles = string.Format(@"{0}/DataFiles", Application.dataPath);
		int i = pathDataFiles.IndexOf("/Assets");
		if (i >= 0)
			pathDataFiles = pathDataFiles.Remove(i, 7);
		if (!System.IO.Directory.Exists(pathDataFiles))
		{
			pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles));
			if (!System.IO.Directory.Exists(pathDataFiles))
			{
				pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86));
				if (!System.IO.Directory.Exists(pathDataFiles))
					pathDataFiles = "";
			}
		}

		pathUserSystemFiles = Application.persistentDataPath;
		pathUserDocumentsFiles = System.Environment.ExpandEnvironmentVariables(@"%UserProfile%\Documents\AcroVR");
#else
		pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
		if (!System.IO.Directory.Exists(pathDataFiles))
		{
			pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
			if (!System.IO.Directory.Exists(pathDataFiles))
			{
				int i = Application.dataPath.IndexOf("/Build");
				if (i >= 0)
				{
					pathDataFiles = string.Format(@"{0}/DataFiles", Application.dataPath.Remove(i));
					if (!System.IO.Directory.Exists(pathDataFiles))
						pathDataFiles = "";
				}
				else
					pathDataFiles = "";
			}
		}
		pathUserSystemFiles = Application.persistentDataPath;
		pathUserDocumentsFiles = System.Environment.ExpandEnvironmentVariables(@"%UserProfile%\Documents\AcroVR");
#endif
	}
}
