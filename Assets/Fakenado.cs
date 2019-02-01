using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fakenado : MonoBehaviour {

    [HideInInspector]
    public Vector3 handPoint;
    public float maxSpeed;
    private Renderer renderer;
	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
        handPoint = new Vector3(0,0,0);
    }
	
	// Update is called once per frame
	void Update () {

        transform.LookAt(Camera.main.transform);
        transform.Rotate(90, 0, 0);

        if(handPoint.magnitude > 0)
        {
            transform.position = Vector3.Slerp(transform.position, handPoint, maxSpeed * Time.deltaTime);
            renderer.material.color = Color.green;
            if(transform.position == handPoint)
            {
                renderer.material.color = Color.white;
                handPoint = Vector3.zero;
            }
        }
	}
        
}
