using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [Tooltip("To add more target locations, duplicate one of the existing ones and add it to this list.")]
    [SerializeField] private List<Transform> targetLocations;
    [SerializeField][Range(0.0001f, 0.1f)] private float moveSpeed = 0.01f;
    
    private Transform targetLocation;
    private int targetIndex = 1;
    private BoxCollider2D _collider;

    private void Awake()
    {
        transform.position = targetLocations[0].position;
        targetLocation = targetLocations[1];
    }

    private void Update()
    {
        MovePlatform();
    }
    
    // Goes through the list of target locations and wraps around when it gets to the end of the list
    private void ChangeTargetLocation()
    {
        targetIndex++;

        if (targetIndex == targetLocations.Count)
        {
            targetIndex = 0;
        }

        targetLocation = targetLocations[targetIndex];
    }

    // Moves the platform between each point and switches target when it reaches the destination
    private void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, moveSpeed);

        if (Vector3.Distance(transform.position, targetLocation.position) < 0.001f)
        {
            ChangeTargetLocation();
        }
    }
}
