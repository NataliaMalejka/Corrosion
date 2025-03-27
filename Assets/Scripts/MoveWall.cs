using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class MoveWall : MonoBehaviour
{
    private Animator animator;
    //[SerializeField] AnimationEventBroadcaster animationEventBroadcaster;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        //animationEventBroadcaster.OnAnimationEndEvent.AddListener(OnAnimationEnded);

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
        if (animator != null)
        {
            SoundsManager.Instance.PlaySounds(Sounds.WallMove);
            animator.SetTrigger("Click");
        }
    }

    //public void OnAnimationEnded()
    //{
    //    if(GameManager.Instance.State == GameState.PlayerTurn)
    //    {
    //        GetComponent<PlayerInput>().ActivateInput();
    //        GameManager.Instance.UpdateState(GameState.Busy);
    //    }
    //}
}
