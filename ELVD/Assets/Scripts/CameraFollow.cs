using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float edgeBuffer = 1.0f; // Adjust this value based on your level design

    private bool canFollow = true;

    void Start()
    {

    }

    void Update()
    {
        if (canFollow)
        {
            Vector3 targetPosition = PlayerController.Instance.transform.position + offset;

            // Check if the player is near the edge of the level
            if (IsPlayerNearEdge(targetPosition))
            {
                canFollow = false;
            }

            // Update the camera position using Lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        }
        else
        {
            // Check if the player has moved away from the edge, then allow camera following again
            if (!IsPlayerNearEdge(PlayerController.Instance.transform.position + offset))
            {
                canFollow = true;
            }
        }
    }

    bool IsPlayerNearEdge(Vector3 position)
    {
        float minX = LevelManager.Instance.MinX + edgeBuffer;
        float minY = LevelManager.Instance.MinY + edgeBuffer;
        float maxX = LevelManager.Instance.MaxX - edgeBuffer;
        float maxY = LevelManager.Instance.MaxY - edgeBuffer;

        // Check if the player is outside the defined boundaries
        return position.x < minX || position.x > maxX || position.y < minY || position.y > maxY;
    }
}