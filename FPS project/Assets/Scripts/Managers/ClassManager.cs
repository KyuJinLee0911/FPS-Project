using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public enum ClassType
{
    CT_GUARDIAN = 0,
    CT_COMMANDO = 1,
    CT_RANGER = 2,
    CT_REACON = 3

}

public class ClassManager : MonoBehaviour
{
    public PlayerClassData[] playerClassDatas;
    public List<PlayerClassData> unlockedClasses;
    public PlayerClassData currentClass;
    public List<Ability> activatedAbility = new List<Ability>();

    [Header("UI")]
    public GameObject abilityUI;
    public GameObject unlockUI;
    public Text unlockTxt;


    // 어빌리티 UI를 열 때
    // 현재 클래스에 맞는 어빌리티 데이터 중 중복없이 3개를 골라
    // 어빌리티 선택 버튼에 할당해주고
    // 어빌리티 등록 버튼에 해당 어빌리티 데이터의 설명을 설정해줌
    public void Init()
    {
        // currentClass = playerClassDatas[(int)GameManager.Instance.player.classData.classType];
    }
    public void OpenSelectAbilityUI()
    {
        // 총 어빌리티 개수에서 현재 활성화된 어빌리티 개수를 뺀 값이 3보다 작으면
        // 선택할 어빌리티가 충분하지 않으므로
        // 더 이상 어빌리티를 선택할 수 없게 함
        // 그렇지 않으면 아래에서 무한반복에 걸려서 게임이 터짐
        if (currentClass.AllAbilities.Count - activatedAbility.Count < 3)
        {
            Debug.Log("Not enough abilities left");
            return;
        }

        abilityUI.SetActive(true);
        int cnt = 0;
        int[] idxs = { -1, -1, -1 };

        while (cnt < 3)
        {
            AbilityUI temp = abilityUI.transform.GetChild(cnt).GetComponent<AbilityUI>();

            // 중복체크
            // 랜덤으로 숫자를 하나 뽑아 해당 숫자를 인덱스로 가지는 어빌리티를 가져옴
            // 해당 어빌리티가 현재 활성화된 어빌리티 딕셔너리에 있는지 체크
            // 현재 활성화된 어빌리티 딕셔너리에 존재한다면 반복문 다시 처음부터
            // 랜덤으로 뽑은 어빌리티의 인덱스가 이미 뽑았던 인덱스라면 처음부터 다시
            int randomIndex = UnityEngine.Random.Range(0, currentClass.AllAbilities.Count);
            Ability newAbility = currentClass.AllAbilities[randomIndex];

            if (activatedAbility.Contains(newAbility))
                continue;

            if (Array.IndexOf(idxs, randomIndex) > -1)
                continue;

            // 중복되지 않았다면
            // 랜덤으로 뽑은 인덱스를 어레이에 추가
            // 버튼에 ability를 먼저 할당해준 후
            // 버튼 오브젝트 활성화
            // - 그렇지 않으면 ability가 할당되기전에 버튼이 활성화 되면서
            // - ability data를 정상적으로 받아오지 못하는 버그가 있음
            idxs[cnt] = randomIndex;
            temp.ability = newAbility;
            temp.gameObject.SetActive(true);
            GameManager.Instance.SetCursorState(true, CursorLockMode.None);
            // 어빌리티 선택시 일시정지 해제 및 커서 숨기고, 버튼 다시 비활성화
            temp.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.AdjustTimeScale(1));
            temp.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SetCursorState(false, CursorLockMode.Locked));
            
            // 카운트 증가
            // 카운트가 3이 되기 전, 즉 0, 1, 2 총 세번 반복하여 중복 없이 등록
            cnt++;

            if (cnt == 3)
            {
                for (int i = 0; i < idxs.Length; i++)
                {
                    idxs[i] = -1;
                }
            }
        }
        // 어빌리티 창 켜질때 일시정지
        GameManager.Instance.AdjustTimeScale(0);
    }

    public void RemoveEveryAbility()
    {
        if(activatedAbility.Count == 0) return;
        foreach(var abil in activatedAbility)
        {
            abil.RemoveAbility();
        }

        activatedAbility.Clear();
    }

    public void UnlockClass(string className)
    {
        foreach (var playerClass in playerClassDatas)
        {
            if (playerClass.className == className)
            {
                unlockedClasses.Add(playerClass);
                break;
            }
            else
                continue;
        }

        StartCoroutine(EnableUnlockClassUI(className));
    }

    IEnumerator EnableUnlockClassUI(string className)
    {
        if (!unlockUI.activeSelf)
            unlockUI.SetActive(true);
        unlockTxt.text = $"{className}을(를) 잠금해제 했습니다";
        yield return new WaitForSeconds(3);

        unlockUI.SetActive(false);
    }
}
