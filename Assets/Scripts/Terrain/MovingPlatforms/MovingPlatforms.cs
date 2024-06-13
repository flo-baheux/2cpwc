using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] private List<Transform> targetLocations;
    [SerializeField] private float moveSpeed = 0.01f;
    
    
    private Transform previousLocation;
    private Transform targetLocation;
    private int targetIndex = 1;
    private BoxCollider2D _collider;

    private void Awake()
    {
        transform.position = targetLocations[0].position;
        previousLocation = targetLocations[0];
        targetLocation = targetLocations[1];
        StartCoroutine(MovePlatform());
    }

    // Goes through the list of target locations
    private void ChangeTargetLocation()
    {
        previousLocation = targetLocation;
        targetIndex++;

        if (targetIndex == targetLocations.Count)
        {
            targetIndex = 0;
        }

        targetLocation = targetLocations[targetIndex];
    }
    
    IEnumerator MovePlatform()
    {
        float timer = 0f;

        // Since the platform will always be moving, there's no reason to break the loop
        while (true)
        {
            timer += moveSpeed;
            transform.position = Vector3.Lerp(previousLocation.position, targetLocation.position, timer);
            
            if (timer >= 1f)
            {
                timer -= 1f;
                ChangeTargetLocation();
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}
