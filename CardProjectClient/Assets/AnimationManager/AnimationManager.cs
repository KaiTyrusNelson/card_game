using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> The animation manager is used to ensure all animations are played within perfect order <summary>
public class AnimationManager : MonoBehaviour
{
    public Queue<IEnumerator> animationOrder = new Queue<IEnumerator>();

    public static AnimationManager Singleton;
    public void Start()
    {
        Singleton = this;
        StartCoroutine(ANIMATION_GO());
    }
    public void ADD_ANIMATION(IEnumerator x)
    {
        animationOrder.Enqueue(x);
    }
    public IEnumerator ANIMATION_GO()
    {
        while (true)
        {
            if (animationOrder.TryDequeue(out IEnumerator x))
            {
                yield return StartCoroutine(x);
            }
            yield return null;
        }
    }
}