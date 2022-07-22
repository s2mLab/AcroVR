﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlThigh : ControlSegmentGeneric
{
    public override string dofName { get { return "HipFlexion"; } }
    public override int avatarIndex { get { return 0; } }
    public override int qIndex { get { return 1; } }
    protected override DrawingCallback drawingCallback { get {return drawManager.ControlThigh;} }
    protected override Vector3 arrowOrientation { get {return new Vector3();} }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(90, 0, 0); } }
    public override int direction { get { return 1; } }
}
