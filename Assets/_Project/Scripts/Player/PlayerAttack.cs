using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.started += Attack;
    }

    void OnDisable()
    {
        inputActions.Player.Attack.started -= Attack;
        inputActions.Player.Disable();
    }

    private void Attack(InputAction.CallbackContext context)
    {
        
    }
}
