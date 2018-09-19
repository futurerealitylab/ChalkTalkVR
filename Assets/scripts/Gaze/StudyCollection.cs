//#define OLD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

public class StudyCollection : MonoBehaviour {
#if OLD
    public List<FocusDataPoint> focus;
    public List<EyeContactDataPoint> contact;

    public GameObject presenter;
    public GameObject audienceA;
    public GameObject audienceB;
    public GameObject board;

    public LayerMask layerP;
    public LayerMask layerA;
    public LayerMask layerB;
    public LayerMask layerNone;
    public LayerMask layerBoard;

    void Awake() {
        this.focus = new List<FocusDataPoint>();
        this.contact = new List<EyeContactDataPoint>();
        debugMarkers = new GameObject[3];

        Color[] c = { Color.red, Color.green, Color.blue };

        for (int i = 0; i < 3; i += 1)
        {
            debugMarkers[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugMarkers[i].GetComponent<Renderer>().material.color = c[i];
            debugMarkers[i].transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        }

        this.focus.Add(new FocusDataPoint());

        EyeContactDataPoint ec = new EyeContactDataPoint();
        ec.type = Eye_Contact_Type.NONE;
        this.contact.Add(ec);
    }

    GameObject[] debugMarkers;


    void LateUpdate() {


        //Debug.DrawLine(presenter.transform.position, presenter.transform.forward * 80.0f, Color.red);
        //Debug.DrawLine(audienceA.transform.position, audienceA.transform.forward * 80.0f, Color.blue);

        // Raycast from presenter
        //raycasthit hit;
        //if (physics.raycast(this.presenter.transform.position, this.presenter.transform.forward, out hit, mathf.infinity, ))
        //{
        //    debug.drawline(this.transform.position, hit.point, color.blue);
        //    go.transform.position = hit.point;

        //    debugmarks
        //}
        Debug.DrawLine(presenter.transform.position, presenter.transform.forward * 80.0f, Color.red);
        Debug.DrawLine(audienceA.transform.position, audienceA.transform.forward * 80.0f, Color.blue);
        Debug.DrawLine(audienceB.transform.position, audienceB.transform.forward * 80.0f, Color.blue);




        FocusDataPoint F = new FocusDataPoint();
        F.time = Time.time;
        F.frame = Time.frameCount;
        

        EyeContactDataPoint EC = new EyeContactDataPoint();

       // Debug.Log(contact[contact.Count - 1]);

        // from presenter (check audience A)
        RaycastHit hitPAA;
        if (Physics.Raycast(this.presenter.transform.position, this.presenter.transform.forward, out hitPAA, Mathf.Infinity, this.layerA))
        {
            //if (hitPAA.collider.gameObject == audienceA)
            {
                //Debug.Log("PRESENTER LOOKS AT AUDIENCE A");
                F.presenter.typeParticipant = Gaze_Type.AUDIENCE_A;
                F.presenter.focusPointParticipantA = hitPAA.point;
            }
        }
        // from presenter (check audience B)
        RaycastHit hitPAB;
        if (Physics.Raycast(this.presenter.transform.position, this.presenter.transform.forward, out hitPAB, Mathf.Infinity, this.layerB))
        {
            //if (hitPAB.collider.gameObject == audienceB)
            {
                F.presenter.typeParticipant |= Gaze_Type.AUDIENCE_B;
                F.presenter.focusPointParticipantB = hitPAB.point;

                Debug.Log("PRESENTER LOOKS AT AUDIENCE B");
            }
        }
        // from presenter (check board)
        RaycastHit hitPB;
        if (Physics.Raycast(this.presenter.transform.position, this.presenter.transform.forward, out hitPB, Mathf.Infinity, this.layerBoard))
        {
            if (hitPB.collider.gameObject == board)
            {
                F.presenter.typeBoard = Gaze_Type.BOARD;
                F.presenter.focusPointBoard = hitPB.point;

                //Debug.Log("PRESENTER LOOKS AT BOARD");
            }
        }

        // from audience a (check presenter)
        RaycastHit hitAP;
        if (Physics.Raycast(this.audienceA.transform.position, this.audienceA.transform.forward, out hitAP, Mathf.Infinity, this.layerP))
        {
            if (hitAP.collider.gameObject == presenter)
            {
                F.audienceA.typeParticipant = Gaze_Type.PRESENTER;
                F.audienceA.focusPointParticipantA = hitAP.point;

                //Debug.Log("AUDIENCE A LOOKS AT PRESENTER");
            }
        }
        // from audience b (check presenter)
        RaycastHit hitBP;
        if (Physics.Raycast(this.audienceB.transform.position, this.audienceB.transform.forward, out hitBP, Mathf.Infinity, this.layerP))
        {
            if (hitBP.collider.gameObject == presenter)
            {
                F.audienceB.typeParticipant = Gaze_Type.PRESENTER;
                F.audienceB.focusPointParticipantA = hitBP.point;
                Debug.Log("AUDIENCE B LOOKS AT PRESENTER");
            }
        }

        // from audience a (check board)
        RaycastHit hitAB;
        if (Physics.Raycast(this.audienceA.transform.position, this.audienceA.transform.forward, out hitAB, Mathf.Infinity, this.layerBoard))
        {
            if (hitAB.collider.gameObject == board)
            {
                F.audienceA.typeBoard = Gaze_Type.BOARD;
                F.audienceA.focusPointBoard = hitAB.point;

                //Debug.Log("AUDIENCE A LOOKS AT BOARD");
            }
        }
        // from audience b (check board)
        RaycastHit hitBB;
        if (Physics.Raycast(this.audienceB.transform.position, this.audienceB.transform.forward, out hitBB, Mathf.Infinity, this.layerBoard))
        {
            if (hitBB.collider.gameObject == board)
            {
                F.audienceB.typeBoard = Gaze_Type.BOARD;
                F.audienceB.focusPointBoard = hitBB.point;

                //Debug.Log("AUDIENCE B LOOKS AT BOARD");
            }
        }



        EyeContactDataPoint prev = this.contact[this.contact.Count - 1];
        
        bool updatedEyeContact = false;
        if (F.presenter.typeParticipant == Gaze_Type.AUDIENCE_BOTH && F.audienceA.typeParticipant == Gaze_Type.PRESENTER && F.audienceB.typeParticipant == Gaze_Type.PRESENTER)
        {
            if (prev.type != Eye_Contact_Type.PRESENTER_WITH_BOTH)
            {
                EC.type = Eye_Contact_Type.PRESENTER_WITH_BOTH;
                EC.timeStart = Time.time;
                EC.frameStart = Time.frameCount;
                prev.duration = EC.timeStart - prev.timeStart;
                updatedEyeContact = true;

                Debug.Log("P W BOTH");
            }

            Debug.DrawLine(presenter.transform.position, audienceA.transform.position, Color.green);
            Debug.DrawLine(presenter.transform.position, audienceB.transform.position, Color.green);

        }
        else
        {
            if (F.presenter.typeParticipant == Gaze_Type.AUDIENCE_A && F.audienceA.typeParticipant == Gaze_Type.PRESENTER)
            {
                if (prev.type != Eye_Contact_Type.PRESENTER_WITH_A)
                {
                    EC.type = Eye_Contact_Type.PRESENTER_WITH_A;
                    EC.timeStart = Time.time;
                    EC.frameStart = Time.frameCount;
                    prev.duration = EC.timeStart - prev.timeStart;
                    updatedEyeContact = true;

                }
                Debug.Log("P W A");

                Debug.DrawLine(presenter.transform.position, audienceA.transform.position, Color.green);

            }
            else if (F.presenter.typeParticipant == Gaze_Type.AUDIENCE_B && F.audienceB.typeParticipant == Gaze_Type.PRESENTER)
            {
                if (prev.type != Eye_Contact_Type.PRESENTER_WITH_B)
                {
                    EC.type = Eye_Contact_Type.PRESENTER_WITH_B;
                    EC.timeStart = Time.time;
                    EC.frameStart = Time.frameCount;
                    prev.duration = EC.timeStart - prev.timeStart;
                    updatedEyeContact = true;
                }
                Debug.Log("P W B");

                Debug.DrawLine(presenter.transform.position, audienceB.transform.position, Color.green);

            }
            else if (F.presenter.typeParticipant == Gaze_Type.NONE || F.audienceA.typeParticipant != Gaze_Type.PRESENTER || F.audienceB.typeParticipant != Gaze_Type.PRESENTER) // TODO fix bug by checking whether any audience members are not looking by adjusting this if statement
            {
                if (prev.type != Eye_Contact_Type.NONE)
                {
                    EC.type = Eye_Contact_Type.NONE;
                    EC.timeStart = Time.time;
                    EC.frameStart = Time.frameCount;
                    prev.duration = EC.timeStart - prev.timeStart;
                    updatedEyeContact = true;

                }
                Debug.Log("P W NONE");

            }
        }

        if (updatedEyeContact)
        {
            contact.Add(EC);
            //Debug.Log(EC.ToString());
            //Debug.Log("ELAPSED: " + prev.duration);

            Debug.Log("UPDATE");
            Debug.Log(contact.Count);
        }

    }
#else

    [SerializeField]
    GameObject presenter;
    public GameObject[] audiences;

    public GameObject board;

    public LayerMask layerP;
    public LayerMask layerA;
    public LayerMask layerB;
    public LayerMask layerNone;
    public LayerMask layerBoard;

    public List<GazeData> gazePresA;
    public List<GazeData> gazePresB;
    public List<GazeData> gazePresBoard;

    public List<GazeData> gazeAPres;
    public List<GazeData> gazeABoard;

    public List<GazeData> gazeBPres;
    public List<GazeData> gazeBBoard;

    public List<EyeContactData> eyePresA;
    public List<EyeContactData> eyePresB;

    public string namePresenter;
    public string nameAudienceA;
    public string nameAudienceB;


    public bool writeAll = false;

    void WriteList<T>(List<T> list, string filePath)
    {
        StreamWriter sw = File.CreateText(filePath);
        for (int i = 0; i < list.Count; i += 1)
        {
            sw.WriteLine(list[i]);
        }
        sw.Close();
    }

    void WriteToFiles()
    {
        int count = 0;
        string directory = "Assets/Resources/" + namePresenter + "_" + nameAudienceA + "_" + nameAudienceB + "_";

        while (Directory.Exists(directory + count))
        {
            count += 1;
        }
        try
        {
            directory = directory + count;
            Directory.CreateDirectory(directory);     
        }
        catch (IOException ex)
        {
            Debug.LogError("ERROR: FILE WRITE FAILED");
            return;
        } 

        { // presenter
            WriteList(gazePresA, directory + "/gazePresA.csv");
            WriteList(gazePresB, directory + "/gazePresB.csv");
            WriteList(gazePresBoard, directory + "/gazePresBoard.csv");
        }
        { // A
            WriteList(gazeAPres, directory + "/gazeAPres.csv");
            WriteList(gazeABoard, directory + "/gazeABoard.csv");
        }
        { // B
            WriteList(gazeBPres, directory + "/gazeBPres.csv");
            WriteList(gazeBBoard, directory + "/gazeBBoard.csv");
        }
        { // eye contact
            WriteList(eyePresA, directory + "/eyePresA.csv");
            WriteList(eyePresB, directory + "/eyePresB.csv");
        }
    }


    public void Awake()
    {
        this.gazeAPres = new List<GazeData>();
        this.gazeABoard = new List<GazeData>();
        this.gazeBPres = new List<GazeData>();
        this.gazeBBoard = new List<GazeData>();
        this.gazePresA = new List<GazeData>();
        this.gazePresB = new List<GazeData>();
        this.gazePresBoard = new List<GazeData>();

        this.eyePresA = new List<EyeContactData>();
        this.eyePresB = new List<EyeContactData>();

        // sentinel data
        this.gazeAPres.Add(new GazeData());
        this.gazeABoard.Add(new GazeData());
        this.gazeBPres.Add(new GazeData());
        this.gazeBBoard.Add(new GazeData());
        this.gazePresA.Add(new GazeData());
        this.gazePresB.Add(new GazeData());
        this.gazePresBoard.Add(new GazeData());

        this.eyePresA.Add(new EyeContactData());
        this.eyePresB.Add(new EyeContactData());
    }

    bool isInit = false;
    void initialize()
    {
        if (isInit)
            return;

        presenter = Camera.main.gameObject;
        // assign audience A and B
        GameObject[] gos = GameObject.FindGameObjectsWithTag("emptyhead");
        audiences = new GameObject[gos.Length];
        for(int i = 0; i < gos.Length; i++) {
            audiences[i] = gos[i];
            audiences[i].layer = 8 + i;
            isInit = true;
        }
        
    }

        public void Update()
    {
        initialize();

        Debug.DrawLine(presenter.transform.position, presenter.transform.forward * 80.0f, Color.red);
        foreach(GameObject audience in audiences) {
            Debug.DrawLine(audience.transform.position, audience.transform.forward * 80.0f, Color.blue);
        }
        

        float timeNow = Time.time;
        int frameCount = Time.frameCount;
        int[] eyeContact = {0, 0};

        {
            GameObject src = this.presenter;

            { // Presenter to A
                RaycastHit hit;
                if (Physics.Raycast(src.transform.position, src.transform.forward, out hit, Mathf.Infinity, this.layerA))
                {
                    GazeData g = new GazeData();
                    g.timeStamp = timeNow;
                    g.frameCount = frameCount;
                    g.positionTarget = hit.point;
                    this.gazePresA.Add(g);

                    eyeContact[0] += 1;
                    Debug.Log("presenter watching A: " + eyeContact[0]);
                }
            }
            { // Presenter to B
                RaycastHit hit;
                if (Physics.Raycast(src.transform.position, src.transform.forward, out hit, Mathf.Infinity, this.layerB))
                {
                    GazeData g = new GazeData();
                    g.timeStamp = timeNow;
                    g.frameCount = frameCount;
                    g.positionTarget = hit.point;
                    this.gazePresB.Add(g);

                    eyeContact[1] += 1;
                }

            }
            { // Presenter to board
                RaycastHit hit;
                if (Physics.Raycast(src.transform.position, src.transform.forward, out hit, Mathf.Infinity, this.layerBoard))
                {
                    GazeData g = new GazeData();
                    g.timeStamp = timeNow;
                    g.frameCount = frameCount;
                    g.positionTarget = hit.point;
                    this.gazePresBoard.Add(g);
                }
            }
        }
        {
            foreach(GameObject audience in audiences) {
                GameObject src = audience;
                { // A to presenter
                    RaycastHit hit;
                    if (Physics.Raycast(src.transform.position, src.transform.forward, out hit, Mathf.Infinity, this.layerP)) {
                        GazeData g = new GazeData();
                        g.timeStamp = timeNow;
                        g.frameCount = frameCount;
                        g.positionTarget = hit.point;
                        if((1 << audience.layer) == layerA) {
                            this.gazeAPres.Add(g);
                            eyeContact[0] += 1;
                        }                            
                        else if((1 << audience.layer) == layerB) {
                            this.gazeBPres.Add(g);
                            eyeContact[1] += 1;
                        }
                        Debug.Log("audience watching presenter " + eyeContact[0]);
                    }
                }
                { // A to board
                    RaycastHit hit;
                    if (Physics.Raycast(src.transform.position, src.transform.forward, out hit, Mathf.Infinity, this.layerBoard)) {
                        GazeData g = new GazeData();
                        g.timeStamp = timeNow;
                        g.frameCount = frameCount;
                        g.positionTarget = hit.point;
                        if ((1 << audience.layer) == layerA) {
                            this.gazeABoard.Add(g);
                        }
                        else if((1 << audience.layer)  == layerB) {
                            this.gazeBBoard.Add(g);
                        }
                        Debug.DrawLine(src.transform.position, hit.point, Color.magenta);
                    }
                }
            }
            
        }
        {

            // eye contact checks

            if (eyeContact[0] == 2)
            {
                if (eyePresA[eyePresA.Count - 1].type != Target_Type.A)
                {
                    eyePresA[eyePresA.Count - 1].setElapsed(timeNow - eyePresA[eyePresA.Count - 1].timeStart);

                    EyeContactData eye = new EyeContactData();
                    eye.timeStart = timeNow;
                    eye.frameStart = frameCount;
                    eye.type = Target_Type.A;
                    eyePresA.Add(eye);

                    Debug.Log(eyePresA[eyePresA.Count - 1]);
                }
            }
            else if (eyePresA[eyePresA.Count - 1].type != Target_Type.NONE)
            {
                eyePresA[eyePresA.Count - 1].setElapsed(timeNow - eyePresA[eyePresA.Count - 1].timeStart);

                EyeContactData eye = new EyeContactData();
                eye.timeStart = timeNow;
                eye.frameStart = frameCount;
                eye.type = Target_Type.NONE;
                eyePresA.Add(eye);

                Debug.Log(eyePresA[eyePresA.Count - 1]);
            }

            if (eyeContact[1] == 2)
            {
                if (eyePresB[eyePresB.Count - 1].type != Target_Type.B)
                {
                    eyePresB[eyePresB.Count - 1].setElapsed(timeNow - eyePresB[eyePresB.Count - 1].timeStart);

                    EyeContactData eye = new EyeContactData();
                    eye.timeStart = timeNow;
                    eye.frameStart = frameCount;
                    eye.type = Target_Type.B;
                    eyePresB.Add(eye);

                    Debug.Log(eyePresB[eyePresB.Count - 1]);
                }
            }
            else if (eyePresB[eyePresB.Count - 1].type != Target_Type.NONE)
            {
                eyePresB[eyePresB.Count - 1].setElapsed(timeNow - eyePresB[eyePresB.Count - 1].timeStart);

                EyeContactData eye = new EyeContactData();
                eye.timeStart = timeNow;
                eye.frameStart = frameCount;
                eye.type = Target_Type.NONE;
                eyePresB.Add(eye);

                Debug.Log(eyePresB[eyePresB.Count - 1]);
            }

            if (eyePresA[eyePresA.Count - 1].type != Target_Type.NONE)
            {
                Debug.DrawLine(presenter.transform.position, audiences[0].transform.position, Color.green);
            }
            if (eyePresB[eyePresB.Count - 1].type != Target_Type.NONE)
            {
                if(audiences.Length > 1)
                    Debug.DrawLine(presenter.transform.position, audiences[1].transform.position, Color.green);
            }


            if (writeAll)
            {
                writeAll = false;
                WriteToFiles();
            }
        }
    }

    void OnApplicationQuit()
    {
        WriteToFiles();
    } 

#endif
}

