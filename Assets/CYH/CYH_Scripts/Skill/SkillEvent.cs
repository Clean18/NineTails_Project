using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEvent : MonoBehaviour
{
    public static event Action OnAAnimationEnd;
    public void AnimationEndEvent()
    {
        OnAAnimationEnd?.Invoke();
        Debug.Log("애니메이션 재생 끝");
    }
}
