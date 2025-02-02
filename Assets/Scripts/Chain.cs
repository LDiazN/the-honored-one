using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Chain : MonoBehaviour
{
    [Header("Game Objects")] [SerializeField]
    private Transform toji;

    [SerializeField]
    private Transform chainHandle;
    
    private LineRenderer _lineRenderer;

    public GameObject toFollow;
    

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        UpdatePositions();
    }

    private void Update()
    {
        UpdatePositions();
        UpdateFollow();
    }

    private void UpdatePositions()
    {
        _lineRenderer.SetPosition(0, chainHandle.position);
        _lineRenderer.SetPosition(1, toji.position);
    }

    private void UpdateFollow()
    {
        if (toFollow == null)
            return;
        
        transform.LookAt(toFollow.transform.position);
        var toKnife = transform.position - toFollow.transform.position;
        toKnife.Normalize();
        toKnife = 0.1f * toKnife;
        
        transform.position = toFollow.transform.position + toKnife;
    }
}

