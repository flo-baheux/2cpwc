using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    
    private Rigidbody2D _rigidbody;
    private float _movementX;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(_movementX, 0f, 0f);
        _rigidbody.AddForce(movement * movementSpeed);
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
