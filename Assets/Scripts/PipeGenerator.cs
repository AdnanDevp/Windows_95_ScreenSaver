using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public GameObject pipeSegmentPrefab;
    public GameObject pipeBendPrefab;
    public GameObject pipeStartMarker; // Reference to the starting point marker
    public GameObject pipeEndMarker;   // Reference to the ending point marker
    public int maxSegmentsPerPipe = 50;
    public float segmentLength = 1.0f;
    public float delayBetweenSegments = 0.1f;
    public Color[] segmentColors;      // Array of colors for segments

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private Vector3 currentDirection;
    private Quaternion currentRotation;
    private Transform currentEndMarker;

    void Start()
    {
        currentEndMarker = pipeStartMarker.transform; // Start at the beginning marker
        StartCoroutine(GeneratePipes());
    }

    IEnumerator GeneratePipes()
    {
        for (int i = 0; i < maxSegmentsPerPipe; i++)
        {
            if (!GeneratePipeSegment())
            {
                break;
            }

            yield return new WaitForSeconds(delayBetweenSegments);

            currentEndMarker = pipeEndMarker.transform; // Move to the next end marker
        }
    }

   bool GeneratePipeSegment()
{
    Vector3 nextPosition = currentEndMarker.position + currentDirection * segmentLength;

    if (occupiedPositions.Contains(nextPosition) || !IsWithinBounds(nextPosition))
    {
        return false;
    }

    GameObject pipeSegment = Instantiate(pipeSegmentPrefab, currentEndMarker.position, currentRotation);

    // Ensure segmentColors array has elements before accessing
    if (segmentColors != null && segmentColors.Length > 0)
    {
        int colorIndex = Random.Range(0, segmentColors.Length);
        pipeSegment.GetComponent<Renderer>().material.color = segmentColors[colorIndex];
    }
    else
    {
        Debug.LogError("segmentColors array is not initialized or empty!");
    }

    occupiedPositions.Add(currentEndMarker.position);

    currentDirection = RandomDirection();
    currentRotation = Quaternion.LookRotation(currentDirection);

    if (Random.value < 0.2f) // 20% chance to create a bend
    {
        CreateBend(currentEndMarker.position, currentRotation);
    }

    currentEndMarker.position = nextPosition;

    return true;
}

   void CreateBend(Vector3 position, Quaternion rotation)
{
    // Ensure segmentColors array has elements before accessing
    if (segmentColors != null && segmentColors.Length > 0)
    {
        int colorIndex = Random.Range(0, segmentColors.Length);
        GameObject bend = Instantiate(pipeBendPrefab, position, rotation);
        bend.GetComponent<Renderer>().material.color = segmentColors[colorIndex];
    }
    else
    {
        Debug.LogError("segmentColors array is not initialized or empty!");
    }
}

    Vector3 RandomDirection()
    {
        List<Vector3> directions = new List<Vector3>
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            Vector3.up,
            Vector3.down
        };

        return directions[Random.Range(0, directions.Count)];
    }

    bool IsWithinBounds(Vector3 position)
    {
        // Adjust bounds checking logic as per your scene setup
        return true; // Placeholder logic
    }
}
