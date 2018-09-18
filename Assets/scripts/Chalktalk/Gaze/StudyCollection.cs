using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyCollection : MonoBehaviour {

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
}

