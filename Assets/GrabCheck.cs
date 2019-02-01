using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class GrabCheck : MonoBehaviour {
    
    public bool IsLeftHand;

    MLHand activeHand;

    List<MLHandKeyPose> PosesToCheck;
    
    bool debugView = false;

    private LineRenderer lineRenderer;

    private Vector3 offset;

    public float ObjectSpacingMultiplier;

    public float GrabDistanceMultiplier;

    void Awake()
    {
        offset = Camera.main.transform.position;
        PosesToCheck = new List<MLHandKeyPose>() { MLHandKeyPose.C, MLHandKeyPose.Pinch, MLHandKeyPose.NoPose };

        if(debugView)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.red;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!MLHands.IsStarted)
        {
            return;
        }
        activeHand = IsLeftHand ? MLHands.Left : MLHands.Right;
        var PoseResult = GetPose(activeHand);
        if (PoseResult.confidence > 0.75f)
        {
            if (PoseResult.pose == MLHandKeyPose.Pinch)
            {
                var viewport = Camera.main.transform.position - offset;
                var indexPoint = activeHand.Index.Tip.Position;
                var thumbPoint = activeHand.Thumb.Tip.Position;
                var pinchPoint = (indexPoint + thumbPoint) / 2;
                var direction = pinchPoint - viewport;
                RaycastHit hitInfo;
                Physics.Raycast(viewport, direction, out hitInfo);
                var hits = Physics.RaycastAll(viewport, direction, GrabDistanceMultiplier);

                if(debugView)
                {
                    lineRenderer.SetPositions(new Vector3[] { viewport, viewport + direction.normalized * ObjectSpacingMultiplier });
                }
                foreach (var hit in hits)
                {
                    if (hitInfo.collider.tag == "Photocube")
                    {
                        Debug.Log("photocube hit");
                        var script = hitInfo.collider.gameObject.GetComponent<Fakenado>();
                        script.handPoint = viewport + direction * 5;
                    }
                }

            }
        }

    }

    private PoseWithConfidence GetPose(MLHand hand)
    {
        MLHandKeyPose mostConfidentPose = MLHandKeyPose.NoHand;
        float highestConfidence = 0f;
        foreach (var pose in PosesToCheck)
        {
            if (hand != null)
            {
                if (hand.KeyPose == pose)
                {
                    float confidence = hand.KeyPoseConfidence;
                    if (confidence > highestConfidence)
                    {
                        mostConfidentPose = pose;
                        highestConfidence = confidence;
                    }
                }
            }
        }
        return new PoseWithConfidence(mostConfidentPose, highestConfidence);
    }
    
    struct PoseWithConfidence
    {
        public MLHandKeyPose pose;
        public float confidence;

        public PoseWithConfidence(MLHandKeyPose pose, float confidence)
        {
            this.pose = pose;
            this.confidence = confidence;
        }
    }
}
