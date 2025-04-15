using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController player;
    private Animator animator;

    private List<int> currentComboHashes;
    private WeaponComboData comboData;

    
    private int currentComboIndex = 0;
    private bool inputQueued = false;
    
    
    private Coroutine layerBlendCoroutine;

    public void Enter(PlayerController player)
    {
        this.player = player;
        this.animator = player._animator;
        comboData = player.CurrentWeapon.comboData;
        currentComboHashes = comboData.GetStateHashes("Base Layer"); // 또는 "MoveLayer" 등 실제 레이어명
    
        currentComboIndex = 0;
        inputQueued = false;

        layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 0f));
        animator.SetTrigger("Attack");
    }

    
    public void InputHandler()
    {
        
        if (animator.IsInTransition(0)) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentComboHashes.Contains(stateInfo.fullPathHash))
        {
            if (Input.GetMouseButtonDown(0))
                inputQueued = true;

            if (stateInfo.normalizedTime <= 0.95f && inputQueued)
            {
                if (inputQueued && currentComboIndex < comboData.maxComboCount - 1)
                {
                    currentComboIndex++;
                    animator.SetTrigger("Attack");
                    inputQueued = false;
                }
                else if (stateInfo.normalizedTime >= 1f)
                {
                    player.ChangeState(new PlayerIdleState());
                }
            }
        }
        else
        {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Update()
    { }
    public void PhysicsUpdate() { }

    public void Exit()
    {
        layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 1f));
    }
    private IEnumerator SetLayerWeightSmooth(string layerName, float targetWeight, float duration = 0.1f)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        float currentWeight = animator.GetLayerWeight(layerIndex);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float weight = Mathf.Lerp(currentWeight, targetWeight, time / duration);
            animator.SetLayerWeight(layerIndex, weight);
            yield return null;
        }

        animator.SetLayerWeight(layerIndex, targetWeight);
    }
}
