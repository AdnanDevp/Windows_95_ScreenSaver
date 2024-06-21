using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeLong : MonoBehaviour
{
    public GameObject longPipePrefab;
    public GameObject bentPipePrefab;
    public int maxSegmentsPerPipe = 50;
    public float delayBetweenSegments = 0.1f;
    public float timeBeforeBend = 5.0f; // Time before a bend appears initially
    public float bendInterval = 10.0f;  // Interval between bend appearances
    public float bendDuration = 3.0f;   // Duration of the bend segment

    private Transform currentEndMarker;
    private bool isBending = false;
    private bool hasBentAppeared = false; // Flag to track if bend has appeared
    private Vector3 currentDirection = Vector3.forward; // Initial direction

    void Start()
    {
        StartCoroutine(GeneratePipes());
    }

    IEnumerator GeneratePipes()
    {
        GameObject initialSegment = Instantiate(longPipePrefab, transform.position, Quaternion.LookRotation(currentDirection));
        currentEndMarker = initialSegment.transform.Find("PipeEnd");

        for (int i = 0; i < maxSegmentsPerPipe; i++)
        {
            if (!hasBentAppeared && Time.timeSinceLevelLoad > timeBeforeBend)
            {
                hasBentAppeared = true;
                StartCoroutine(GenerateBend());
            }

            GameObject newSegment;
            if (isBending)
            {
                newSegment = Instantiate(bentPipePrefab, currentEndMarker.position, Quaternion.LookRotation(currentDirection));
                currentEndMarker = newSegment.transform.Find("PipeEnd"); // Update end marker to new segment's end
                yield return new WaitForSeconds(bendDuration);
                isBending = false; // After bending, switch back to elongating straight

                // Change to a new random direction
                currentDirection = RandomDirection(currentDirection);
            }
            else
            {
                newSegment = Instantiate(longPipePrefab, currentEndMarker.position, Quaternion.LookRotation(currentDirection));
                currentEndMarker = newSegment.transform.Find("PipeEnd"); // Update end marker to new segment's end
                yield return new WaitForSeconds(delayBetweenSegments);
            }
        }
    }

    IEnumerator GenerateBend()
    {
        while (true)
        {
            isBending = true; // Start bending mode

            // Wait for bend duration
            yield return new WaitForSeconds(bendDuration);

            isBending = false; // After bend, switch back to elongating straight

            // Wait for bend interval before the next bend
            yield return new WaitForSeconds(bendInterval);
        }
    }

    Vector3 RandomDirection(Vector3 previousDirection)
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

        // Remove the previous direction and its opposite to ensure a change in direction
        directions.Remove(previousDirection);
        directions.Remove(-previousDirection);

        return directions[Random.Range(0, directions.Count)];
    }
}
