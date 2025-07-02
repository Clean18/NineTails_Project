using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompEffectAction : MonoBehaviour
{
    [SerializeField] private Animator StompAnimator;

    private void Start()
    {
        StompAnimator.Play("Stomp_Ani");

        
    }
}
