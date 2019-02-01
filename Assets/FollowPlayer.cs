using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    Vector3 offset;
	// Use this for initialization
	void Start () {
        offset = Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.transform.position - offset;
        transform.Translate(new Vector3(0, 0, -1f));
	}
}
