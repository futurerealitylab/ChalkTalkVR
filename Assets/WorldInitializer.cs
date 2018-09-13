using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInitializer : MonoBehaviour {
    public bool setAngle;
    public float angle;

    private void Awake()
    {
        if (setAngle) {
            this.gameObject.transform.Rotate(new Vector3(0.0f, angle, 0.0f));
        }
    }
}
