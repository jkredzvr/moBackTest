using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjs : MonoBehaviour {
    public Rigidbody rb;
    public float angleDiffUp;
    public float angleThres;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        angleDiffUp = Vector3.Angle(transform.up, Vector3.up);

        if (angleDiffUp > angleThres) {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        else
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
	}
}
