using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private bool moveKeyHeld;

    private void Awake() => controls = new Controls();

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Movement.started += OnMovement;
        controls.Player.Movement.canceled += OnMovement;
        controls.Player.Exit.performed += OnExit;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Movement.started -= OnMovement;
        controls.Player.Movement.canceled -= OnMovement;
        controls.Player.Exit.performed -= OnExit;
    }

    private void OnExit(InputAction.CallbackContext context)
    {
        Debug.Log("EXIT");
    }

    private void OnMovement(InputAction.CallbackContext context)
    {
        if (context.started)
            moveKeyHeld = true;
        else if (context.canceled)
            moveKeyHeld = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.IsPlayerTurn && moveKeyHeld)
            MovePlayer();
    }

    private void MovePlayer()
    {
        transform.position += (Vector3)controls.Player.Movement.ReadValue<Vector2>();
        GameManager.instance.EndTurn();
    }
}
