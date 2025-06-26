using System.Collections;
using UnityEngine;

public abstract class SkillLogic : MonoBehaviour
{
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public bool IsCooldown { get; set; } = false;
    
    private WaitForSeconds _wait ;

    private void Start()
    {
        _wait = new WaitForSeconds(Cooldown);
    }

    protected virtual bool TryUseSkill(MonoBehaviour component)
    {
        if (IsCooldown)
        {
            Debug.Log($"현재 쿨타임 중");
            return false;
        }

        Debug.Log($"스킬 쿨타임 시작");
        StartCooldown(component);
        return true;
    }

    public void StartCooldown(MonoBehaviour component)
    {
        if (IsCooldown) return;
        component.StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        IsCooldown = true;
        yield return _wait;
        IsCooldown = false;
    }
}
