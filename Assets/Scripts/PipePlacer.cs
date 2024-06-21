using System.Collections;
using UnityEngine;

public class PipePlacer : MonoBehaviour
{
    public GameObject straightPipePrefab;
    public GameObject elbowPipePrefab;
    public int numberOfPipes = 50;
    public float placementDelay = 1f;

    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Color currentColor;
    private System.Random random = new System.Random();

    void Start()
    {
        currentPosition = transform.position;
        currentRotation = transform.rotation;
        currentColor = new Color(Random.value, Random.value, Random.value); // Initialize with a random color

        StartCoroutine(PlacePipes());
    }

    IEnumerator PlacePipes()
    {
        for (int i = 0; i < numberOfPipes; i++)
        {
            PlaceRandomPipe();
            yield return new WaitForSeconds(placementDelay);
        }
    }

    void PlaceRandomPipe()
    {
        GameObject pipePrefab;
        if (random.NextDouble() > 0.5)
        {
            pipePrefab = straightPipePrefab;
        }
        else
        {
            pipePrefab = elbowPipePrefab;
            // Randomly decide between 90-degree rotations on different axes
            int axis = random.Next(3);
            if (axis == 0) currentRotation *= Quaternion.Euler(90, 0, 0); // X axis
            else if (axis == 1) currentRotation *= Quaternion.Euler(0, 90, 0); // Y axis
            else currentRotation *= Quaternion.Euler(0, 0, 90); // Z axis

            // Change the color when the pipe takes a turn
            currentColor = new Color(Random.value, Random.value, Random.value);
        }

        GameObject pipe = Instantiate(pipePrefab, currentPosition, currentRotation);
        ApplyColor(pipe, currentColor);

        // Find the EndPoint of the newly placed pipe
        Transform endPoint = pipe.transform.Find("EndPoint");
        if (endPoint != null)
        {
            // Set the currentPosition to the EndPoint of the newly placed pipe
            currentPosition = endPoint.position;

            // Align the next pipe's StartPoint with the current pipe's EndPoint
            Transform startPoint = pipe.transform.Find("StartPoint");
            if (startPoint != null)
            {
                Vector3 offset = endPoint.position - startPoint.position;
                currentPosition += offset;
            }
        }
    }

    void ApplyColor(GameObject pipe, Color color)
    {
        Renderer renderer = pipe.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}