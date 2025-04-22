using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController _player;
    private Animator animator;
    private List<int> currentWeapone;
    private PlayerInputCommend _input;
    private int weaponLayerIndex;
    int attackTriggerHash = Animator.StringToHash("Attack");
    
    private float lastShiftTapTime = -1f;
    private float doubleTapThreshold = 0.5f; // 0.3초 이내에 두번 누르면 구르기
    
    
    private Coroutine layerBlendCoroutine;
    private Transform cameraTransform;

    
    public void Enter(PlayerController player)
    {
        _player = player;
        this.animator = player._animator;
        currentWeapone = _player.CurrentWeapon.comboData.GetStateHashes();
        weaponLayerIndex = animator.GetLayerIndex(_player.CurrentWeapon.comboData.animationLayerName);
        _player.HeadOverray.weight = 0f;
        animator.applyRootMotion = true;
        cameraTransform = player.cameraTransform;
        
        //layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 0f));
        
        
        animator.SetTrigger(attackTriggerHash);
    }


    public void HandleInput(PlayerInputCommend input)
    {

        _input = input;
        
    }

    public void Update()
    {
        if (animator.IsInTransition(weaponLayerIndex)) return;
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(weaponLayerIndex);

        if (currentWeapone.Contains(stateInfo.fullPathHash))
        {
            Debug.Log("콤보 메서드 진입");
            // 마우스 클릭, 애니메이션 재생이 70% 전일때 
            if (_input.AttackPressed && stateInfo.normalizedTime <= 0.7f)
            {
                Debug.Log("콤보 진입");
                Attack();
            }
            else if (stateInfo.normalizedTime >= 0.95f || !currentWeapone.Contains(stateInfo.fullPathHash))
            {
                _player.ChangeState(new PlayerIdleState());
            }
        }
        if (_input.ShiftTap)
        {
            if (Time.time - lastShiftTapTime < doubleTapThreshold)
            {
               
                // 구르기 실행
                _player.ChangeState(new PlayerRollState());
                lastShiftTapTime = -1f; // 초기화
            }
            else
            {
                lastShiftTapTime = Time.time; // 첫 번째 탭 기록
            }
        }
    }
    public void PhysicsUpdate() { }

    public void Exit()
    {
        //layerBlendCoroutine = player.StartCoroutine(SetLayerWeightSmooth("Upper Mask", 1f));
        animator.applyRootMotion = false;
        _player.HeadOverray.weight = 1f;
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
