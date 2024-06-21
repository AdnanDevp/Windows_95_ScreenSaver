using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PipeManager : MonoBehaviour
{
    public GameObject straightPipePrefab;
    public GameObject bentPipePrefab;
    public int maxSegments = 100;
    public float segmentLength = 1.0f;

    private List<Vector3> positions = new List<Vector3>();
    private Vector3 currentPos = Vector3.zero;
    private Vector3 currentDir = Vector3.forward;

    private Bounds bounds;

    void Start()
    {
        // Define bounds for the pipes (adjust as needed)
        bounds = new Bounds(Vector3.zero, new Vector3(10, 10, 10));
        positions.Add(currentPos);
        StartCoroutine(GeneratePipes());
    }

    IEnumerator GeneratePipes()
    {
        for (int i = 0; i < maxSegments; i++)
        {
            if (IsDeadEnd(currentPos))
            {
                currentPos = GetNewStartPosition();
                currentDir = GetRandomDirection();
            }

            CreatePipeSegment();
            yield return new WaitForSeconds(0.1f); // Delay to show gradual growth
        }
    }

    bool IsDeadEnd(Vector3 pos)
    {
        // Check if all possible directions are blocked
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };
        foreach (Vector3 dir in directions)
        {
            if (!positions.Contains(pos + dir * segmentLength) && bounds.Contains(pos + dir * segmentLength))
            {
                return false;
            }
        }
        return true;
    }

    Vector3 GetNewStartPosition()
    {
        // Find a new starting position from the existing positions list
        Vector3 newPos;
        int tries = 0;
        do
        {
            newPos = positions[Random.Range(0, positions.Count)];
            tries++;
            if (tries > 1000) // Safety break to avoid infinite loop
            {
                Debug.LogError("Couldn't find a new start position.");
                break;
            }
        } while (IsDeadEnd(newPos));
        return newPos;
    }

    Vector3 GetRandomDirection()
    {
        // Return a random direction (up, down, left, right, forward, back)
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };
        return directions[Random.Range(0, directions.Length)];
    }

    void CreatePipeSegment()
    {
        // Create a straight pipe segment
        GameObject newSegment = Instantiate(straightPipePrefab, currentPos, Quaternion.LookRotation(currentDir));
        newSegment.GetComponent<Renderer>().material.color = GetRandomColor();

        // Store the position
        positions.Add(currentPos);

        // Move the current position forward
        currentPos += currentDir * segmentLength;

        // Randomly decide to add a bend
        if (Random.value > 0.7f)
        {
            CreateBend();
        }
    }

    void CreateBend()
    {
        // Create a bend in the pipe
        GameObject newBend = Instantiate(bentPipePrefab, currentPos, Quaternion.identity);
        newBend.GetComponent<Renderer>().material.color = GetRandomColor();

        // Change direction
        currentDir = GetRandomDirection();

        // Ensure the new direction doesn't immediately cause an overlap or go out of bounds
        while (positions.Contains(currentPos + currentDir * segmentLength) || !bounds.Contains(currentPos + currentDir * segmentLength))
        {
            currentDir = GetRandomDirection();
        }

        // Store the bend position
        positions.Add(currentPos);

        // Move the current position forward
        currentPos += currentDir * segmentLength;
    }

    Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
