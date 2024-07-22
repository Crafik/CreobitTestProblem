using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header ("===== Components =====")]
    [SerializeField] private Rigidbody m_Rigidbody;

    [Space (10)]
    [Header ("===== Variables =====")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnRate;

    private Controls m_controls;

    private float moveDir;
    private float turnDir;

    private bool isActive;

    void Awake(){
        m_controls = new Controls();

        isActive = false;
    }

    void OnEnable(){
        m_controls.Enable();

        m_controls.Player.Move.performed += MoveAction;
        m_controls.Player.Move.canceled += MoveAction;
        m_controls.Player.Turn.performed += TurnAction;
        m_controls.Player.Turn.canceled += TurnAction;
    }

    void OnDisable(){
        m_controls.Disable();

        m_controls.Player.Move.performed -= MoveAction;
        m_controls.Player.Move.canceled -= MoveAction;
        m_controls.Player.Turn.performed -= TurnAction;
        m_controls.Player.Turn.canceled -= TurnAction;
    }

    void MoveAction(InputAction.CallbackContext ctx){
        moveDir = ctx.ReadValue<float>();
    }

    void TurnAction(InputAction.CallbackContext ctx){
        turnDir = ctx.ReadValue<float>();
    }

    public void SetPlayerActive(bool State = true){
        if (State){
            isActive = true;
        }
        else{
            isActive = false;
            m_Rigidbody.velocity = Vector3.zero;
        }
    }

    public void SetPlayerPosition(Vector3 pos){
        m_Rigidbody.position = pos;
    }

    void FixedUpdate(){
        if (isActive){
            m_Rigidbody.velocity = moveDir * moveSpeed * transform.forward;
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * Quaternion.Euler(0f, turnDir * turnRate * Time.fixedDeltaTime, 0f));
        }
        else{
            if (!RunnerGameManagerSingleton.instance.isStarted){
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * Quaternion.Euler(0f, 180f * Time.fixedDeltaTime, 0f));
            }
        }
    }
}
