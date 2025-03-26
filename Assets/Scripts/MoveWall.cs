using UnityEngine;
using UnityEngine.InputSystem;

public class MoveWall : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        animator.SetFloat("Speed", 1);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("click");

        if (GameManager.Instance.State != GameState.PlayerTurn || !context.performed)
            return;

        GetComponent<PlayerInput>().DeactivateInput();

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                MoveWall clickedWall = hit.collider.GetComponent<MoveWall>();
                if (clickedWall != null)
                {
                    clickedWall.TriggerAnimation();
                }
            }
        }

        GetComponent<PlayerInput>().ActivateInput();
    }

    public void TriggerAnimation()
    {
        Debug.Log("animacja");

        if (animator != null)
        {
            animator.SetTrigger("Click");
        }
    }
}
