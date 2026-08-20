using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    public Vector2 moveInput { get; private set; }
    public bool abilityInput { get; private set; }
    public bool pauseInput { get; private set; }

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction abilityAction;
    private InputAction pauseAction;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        setupInputActions();
    }

    void Update()
    {
        updateInputs();
    }

    private void setupInputActions()
    {
        moveAction = playerInput.actions["Move"];
        abilityAction = playerInput.actions["Ability"];
        pauseAction = playerInput.actions["Pause"];
    }

    private void updateInputs()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        abilityInput = abilityAction.WasPressedThisFrame();
        pauseInput = pauseAction.WasPressedThisFrame();
    }
}
