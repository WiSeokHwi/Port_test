using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController player;
    private Animator animator;
    private List<int> currentWeapone;

    private int weaponLayerIndex;
    
    
    
    private Coroutine layerBlendCoroutine;
    private Transform cameraTransform;

    public void Enter(PlayerController player)
    {
        this.player = player;
        this.animator = player._animator;
        currentWeapone = this.player.CurrentWeapon.comboData.GetStateHashes();
        weaponLayerIndex = animator.GetLayerIndex(this.player.CurrentWeapon.comboData.animationLayerName);
 
        animator.applyRootMotion = true;
        cameraTransform = player.cameraTransform;
        
        //layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 0f));
        
        animator.SetTrigger("Attack");
    }

    
    public void InputHandler()
    {

        if (animator.IsInTransition(weaponLayerIndex)) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(weaponLayerIndex);

        if (currentWeapone.Contains(stateInfo.fullPathHash))
        {
            Debug.Log("콤보 메서드 진입");
            // 마우스 클릭, 애니메이션 재생이 70% 전일때 
            if (Input.GetMouseButtonDown(0) && stateInfo.normalizedTime <= 0.7f)
            {
                Debug.Log("콤보 진입");
                Attack();
            }
            else if (stateInfo.normalizedTime >= 0.95f)
            {
                player.ChangeState(new PlayerIdleState());
            }
        }
        
    }

    public void Update()
    {

    }
    public void PhysicsUpdate() { }

    public void Exit()
    {
        //layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 1f));
        animator.applyRootMotion = false;
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
        
        
        animator.SetTrigger("Attack");
    }
}
