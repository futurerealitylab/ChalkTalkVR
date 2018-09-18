﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gaze_Type
{
    NONE          = 0,
    AUDIENCE_A    = 1,
    AUDIENCE_B    = 2,
    AUDIENCE_BOTH = 3,
    PRESENTER     = 4,
    BOARD         = 5,
    ENUM_COUNT
}



public struct GazeData
{
    public Gaze_Type typeParticipant;
    public Gaze_Type typeBoard;
    public Vector3 focusPointParticipantA;
    public Vector3 focusPointParticipantB;
    public Vector3 focusPointBoard;

    public override string ToString()
    {
        return "[[" + (int)typeParticipant + ", " + focusPointParticipantA.ToString() + ", " + focusPointParticipantB.ToString() + "][" + (int)typeBoard + ", " + focusPointBoard.ToString() + "]]";
    }
}

public class FocusDataPoint
{
    public float time;
    public int frame;
    public GazeData presenter;
    public GazeData audienceA;
    public GazeData audienceB;

    public FocusDataPoint() {
        presenter.typeParticipant = Gaze_Type.NONE;
        presenter.typeBoard = Gaze_Type.BOARD;
    }

    public override string ToString()
    {
        return "[" + time + ", " + frame + ", " + presenter.ToString() + audienceA.ToString() + audienceB.ToString() + "]";
    }
}

public enum Eye_Contact_Type
{
    NONE                = 0,
    PRESENTER_WITH_A    = 1,
    PRESENTER_WITH_B    = 2,
    PRESENTER_WITH_BOTH = 3,
    ENUM_COUNT
}

public class EyeContactDataPoint
{
    public Eye_Contact_Type type; 
    public float timeStart;
    public int frameStart;
    public float duration;

    public EyeContactDataPoint()
    {
        this.type = Eye_Contact_Type.NONE;
    }

    public override string ToString()
    {
        return "[" + (int)type + ", " + this.timeStart + ", " + this.frameStart + ", " + this.duration + "]";
    }
}

