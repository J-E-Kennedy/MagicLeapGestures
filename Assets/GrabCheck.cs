using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class GrabCheck : MonoBehaviour {


    private SpriteRenderer _spriteRenderer;

    public bool IsLeftHand;

    MLHand activeHand;

    List<MLHandKeyPose> PosesToCheck;

    Dictionary<MLHandKeyPose, Color> PoseColors;

    //private Camera _camera;

    //private Canvas _canvas;

    bool debugView = false;

    private LineRenderer lineRenderer;

    private Vector3 offset;

    void Awake()
    {
        offset = Camera.main.transform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //_canvas = GetComponent<Canvas>();
        //_camera = _canvas.worldCamera;
        PosesToCheck = new List<MLHandKeyPose>() { MLHandKeyPose.C, MLHandKeyPose.Pinch, MLHandKeyPose.NoPose };
        PoseColors = new Dictionary<MLHandKeyPose, Color>()
        {
            { MLHandKeyPose.NoPose, Color.blue },
            { MLHandKeyPose.C, Color.cyan },
            { MLHandKeyPose.Pinch, Color.green }
        };

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


    // Use this for initialization
    //void Start ()
    //{
    //    MLResult result = MLHands.Start();
    //    if (!result.IsOk)
    //    {
    //        Debug.LogErrorFormat("Error: HandTrackingVisualizer failed starting MLHands, disabling script. Reason: {0}", result);
    //        enabled = false;
    //        return;
    //    }
    //    activeHand = IsLeftHand ? MLHands.Left : MLHands.Right;
    //    _spriteRenderer = GetComponent<SpriteRenderer>();
    //    PosesToCheck = new List<MLHandKeyPose>() { MLHandKeyPose.C, MLHandKeyPose.Pinch, MLHandKeyPose.NoPose };
    //    PoseColors = new Dictionary<MLHandKeyPose, Color>()
    //    {
    //        { MLHandKeyPose.NoPose, Color.blue },
    //        { MLHandKeyPose.C, Color.cyan },
    //        { MLHandKeyPose.Pinch, Color.green }
    //    };
    //}

    // Update is called once per frame
    void Update()
    {
        //Physics.Raycast()
        if (!MLHands.IsStarted)
        {
            _spriteRenderer.material.color = Color.red;
            return;
        }
        activeHand = IsLeftHand ? MLHands.Left : MLHands.Right;
        _spriteRenderer.material.color = Color.white;
        var PoseResult = GetPose(activeHand);
        if (PoseResult.confidence > 0.75f)
        {
            _spriteRenderer.material.color = PoseColors[PoseResult.pose];
            if (PoseResult.pose == MLHandKeyPose.Pinch)
            {
                var viewport = Camera.main.transform.position - offset;
                var indexPoint = activeHand.Index.Tip.Position;
                var thumbPoint = activeHand.Thumb.Tip.Position;
                var pinchPoint = (indexPoint + thumbPoint) / 2;
                var direction = pinchPoint - viewport;
                RaycastHit hitInfo;
                Physics.Raycast(viewport, direction, out hitInfo);
                var hits = Physics.RaycastAll(viewport, direction, 10f);
                //Debug.Log("You: " + viewport + ", Pinch: " + pinchPoint);

                if(debugView)
                {
                    lineRenderer.SetPositions(new Vector3[] { viewport, viewport + direction.normalized * 10 });
                }
                foreach (var hit in hits)
                {
                    if (hitInfo.collider.tag == "Photocube")
                    {
                        Debug.Log("photocube hit");
                        var script = hitInfo.collider.gameObject.GetComponent<Fakenado>();
                        script.handPoint = viewport + direction * 2;
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
