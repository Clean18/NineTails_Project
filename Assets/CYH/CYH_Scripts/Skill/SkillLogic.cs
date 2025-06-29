using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillLogic : MonoBehaviour
{
    [SerializeField] protected int _skillLevel = 0;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected List<GameObject> _hitMonsters = new List<GameObject>();

    [SerializeField] protected bool _isSkillUsed = false;
    protected virtual void Damage() { }
    protected virtual void Damage(GameObject monsters) { }
}