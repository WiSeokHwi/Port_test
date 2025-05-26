using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{

    private Animator animator;
    private List<int> currentWeapone;

    private int weaponLayerIndex;
    int attackTriggerHash = Animator.StringToHash("Attack");
    
    
    private Coroutine layerBlendCoroutine;
    private Transform cameraTransform;
    

    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        animator = player._animator;
        currentWeapone = player.CurrentWeapon.comboData.GetStateHashes();
        weaponLayerIndex = animator.GetLayerIndex(player.CurrentWeapon.comboData.animationLayerName);
        player.HeadOverray.weight = 0f;
        player.movePosition = Vector3.zero;
        animator.applyRootMotion = true;
        cameraTransform = player.cameraTransform;
        
        //layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 0f));
        
        
        animator.SetTrigger(attackTriggerHash);
    }
    public override void Update()
    {
        if (animator.IsInTransition(weaponLayerIndex)) return;
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(weaponLayerIndex);

        if (currentWeapone.Contains(stateInfo.fullPathHash))
        {
            Debug.Log("콤보 메서드 진입");
            // 마우스 클릭, 애니메이션 재생이 70% 전일때 
            if (PlayerInput.AttackPressed && stateInfo.normalizedTime <= 0.7f)
            {
                Debug.Log("콤보 진입");
                Attack();
            }
            else if (stateInfo.normalizedTime >= 0.95f || !currentWeapone.Contains(stateInfo.fullPathHash))
            {
                Player.ChangeState(new PlayerIdleState());
            }
        }
        if (PlayerInput.rollTriggerPressed)
        {
            // 구르기 실행
            Player.ChangeState(new PlayerRollState());
        }
    }

    public override void Exit()
    {
        //layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 1f));
        animator.applyRootMotion = false;
        Player.HeadOverray.weight = 1f;
        animator.ResetTrigger(attackTriggerHash);
    }
    //private IEnumerator SetLayerWeightSmooth(string layerName, float targetWeight, float duration = 0.1f)
    //{
    //    int layerIndex = animator.GetLayerIndex(layerName);
    //    float currentWeight = animator.GetLayerWeight(layerIndex);
    //    float time = 0f;

    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float weight = Mathf.Lerp(currentWeight, targetWeight, time / duration);
    //        animator.SetLayerWeight(layerIndex, weight);
    //        yield return null;
    //    }

    //    animator.SetLayerWeight(layerIndex, targetWeight);
    //}

    private void Attack()
    {
        if (animator.IsInTransition(weaponLayerIndex)) return;
        
        
        animator.SetTrigger(attackTriggerHash);
    }
}
