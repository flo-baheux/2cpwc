using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 5f;
    
    private Rigidbody2D _rigidbody;
    private float _movementX;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(_movementX, 0f);
        _rigidbody.AddForce(movement * acceleration);

        // Limits the player's movement speed in X-axis
        if (Mathf.Abs(_rigidbody.velocity.x) > 5f)
        {
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            _rigidbody.velocity = velocity;
        }
    }
    
    // Horizontal Movement P1(A and D) P2(Left and Right Arrow)
    private void OnMove(InputValue movementValue)
    {
        float movementVector = movementValue.Get<float>();
        _movementX = movementVector;
    }

    // Jump P1(W) P2(Up Arrow)
    private void OnJump()
    {
        Debug.Log("Jump");
    }

    // Dropping down through platforms P1(S) P2(Down Arrow)
    private void OnDropDown()
    {
        Debug.Log("Drop Down");
    }

    // Crouch P1(Left Ctrl) P2(Right Ctrl)
    private void OnCrouch()
    {
        Debug.Log("Crouch");
    }
}
