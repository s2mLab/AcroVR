﻿using System.Collections;
using System.Collections.Generic;

public class AvatarSimulation
{
    public enum InterpolationType { Quintic, CubicSpline };

    public struct StrucInterpolation
    {
        public InterpolationType type;
        public int numIntervals;
        public float[] slope;
    }

    public struct StrucNodes
    {
        public int ddl;
        public string name;
        public float[] T;
        public float[] Q;
        public StrucInterpolation interpolation;
        public int ddlOppositeSide;
    }

    public struct StrucTakeOffParam
    {
        public float verticalSpeed;
        public float anteroposteriorSpeed;
        public float somersaultSpeed;
        public float twistSpeed;
        public float tilt;
        public float rotation;
    }

    public enum DataType { Simulation };
    public enum LagrangianModelNames { Simple, Sasha23ddl };

    public struct StrucJoints
    {
        public string fileName;
        public StrucNodes[] nodes;
        public float[] t0;
        public float[,] q0;
        public float duration;
        public StrucTakeOffParam takeOffParam;
        public int condition;
        public DataType dataType;
        public LagrangianModelNames lagrangianModelName;
        public LagrangianModelManager.StrucLagrangianModel lagrangianModel;
        public float tc;
        public float[] t;
        public float[,] rot;
        public float[,] rotdot;
    }

    public StrucJoints joints;
    public StrucInterpolation interpolationDefault;
    public StrucTakeOffParam takeOffParamDefault;
    public float durationDefault;
    public int conditionDefault;

    public AvatarSimulation()
    {
        interpolationDefault.type = InterpolationType.Quintic;
        interpolationDefault.numIntervals = 0;
        interpolationDefault.slope = new float[] { 0, 0 };
        takeOffParamDefault.verticalSpeed = 0;
        takeOffParamDefault.anteroposteriorSpeed = 0;
        takeOffParamDefault.somersaultSpeed = 0;
        takeOffParamDefault.twistSpeed = 0;
        takeOffParamDefault.tilt = 0;
        takeOffParamDefault.rotation = 0;
        durationDefault = 0;
        conditionDefault = 0;

        joints.fileName = "";
        joints.nodes = null;
        joints.t0 = null;
        joints.q0 = null;
        joints.duration = durationDefault;
        joints.takeOffParam = takeOffParamDefault;
        joints.condition = conditionDefault;
        joints.dataType = DataType.Simulation;
        joints.lagrangianModelName = LagrangianModelNames.Simple;
        joints.lagrangianModel = new LagrangianModelManager.StrucLagrangianModel();
        joints.tc = 0;
        joints.t = null;
        joints.rot = null;
        joints.rotdot = null;
    }
}