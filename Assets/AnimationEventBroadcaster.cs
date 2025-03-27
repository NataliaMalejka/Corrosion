using UnityEngine;
using UnityEngine.Events;

public class AnimationEventBroadcaster : MonoBehaviour
{
    public UnityEvent OnAnimationEndEvent;

    public void OnAnimationEnd()
    {
        OnAnimationEndEvent.Invoke();
    }
}
