using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private const float minimalAcceptableVelocity = 0.01f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [Space(10)]
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [Space(5)]
    [SerializeField, Range(0, 1f)] private float inertia;
    [SerializeField] private float friction;
    [SerializeField] private int walkEventThreshold;
    [Header("Imports")]
    [SerializeField] private KeyCode runKey;
    [SerializeField] private Rigidbody2D physicalBody;

    [SerializeField] private UnityEvent onNewGridCellSteppedEvent;
    [SerializeField] private UnityEvent onWalkedEvent;
    [SerializeField] private UnityEvent onChangedDirectionEvent;

    private Vector2 movementDirection, frictionAmount;
    [HideInInspector] public Vector2 lastMovementDirection;
    private Vector2 targetSpeed, actualForce, delta;
    private bool isControlled = true;
    private Vector2Int currentGridPosition, previousGridPosition;
    int gridStepCounter = 0;

    public void SetControl(bool value) {
        isControlled = value;
    }

    private void Start()
    {
        currentGridPosition = TileManager.instance.GetTiles().GetCellAtWorld(transform.position);
    }

    void Update()
    {
        if (!isControlled) return;
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (lastMovementDirection != movementDirection && movementDirection.sqrMagnitude != 0) { onChangedDirectionEvent.Invoke(); }
        lastMovementDirection = movementDirection.sqrMagnitude == 0 ? lastMovementDirection : movementDirection;
        targetSpeed = movementDirection * (Input.GetKey(runKey) ? runSpeed : walkSpeed);
        #region Acceleration&Deceleration
        delta = targetSpeed - physicalBody.velocity;
        Vector2 accelerationRate = new Vector2(
            Mathf.Abs(targetSpeed.x) >= minimalAcceptableVelocity ? acceleration : deceleration,
            Mathf.Abs(targetSpeed.y) >= minimalAcceptableVelocity ? acceleration : deceleration);
        actualForce = new Vector2(
            Mathf.Pow(Mathf.Abs(delta.x) * accelerationRate.x, inertia) * Mathf.Sign(delta.x),
            Mathf.Pow(Mathf.Abs(delta.y) * accelerationRate.y, inertia) * Mathf.Sign(delta.y));
        #endregion
        #region Friction
        float frictionX = 0, frictionY = 0;
        if (Mathf.Abs(movementDirection.x) <= minimalAcceptableVelocity)
        {
            frictionX = Mathf.Min(Mathf.Abs(physicalBody.velocity.x), Mathf.Abs(friction));
        }
        if (Mathf.Abs(movementDirection.y) <= minimalAcceptableVelocity)
        {
            frictionY = Mathf.Min(Mathf.Abs(physicalBody.velocity.y), Mathf.Abs(friction));
        }
        frictionAmount = new Vector2(frictionX, frictionY) * physicalBody.velocity.normalized;
        #endregion

        #region Grid step check
        currentGridPosition = TileManager.instance.GetTiles().GetCellAtWorld(transform.position);
        if (currentGridPosition != previousGridPosition)
        {
            onNewGridCellSteppedEvent.Invoke();
            gridStepCounter++;
            if (gridStepCounter >= walkEventThreshold)
            {
                onWalkedEvent.Invoke();
                gridStepCounter = 0;
            }
        }
        previousGridPosition = currentGridPosition;
        #endregion

        Player.instance.playerAnimator.AnimateWalk(movementDirection);
    }

    void FixedUpdate() {
        physicalBody.AddForce(actualForce, ForceMode2D.Force);
        physicalBody.AddForce(-frictionAmount, ForceMode2D.Impulse);
    }
}
