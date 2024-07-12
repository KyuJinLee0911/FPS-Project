using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class ClassManager : MonoBehaviour
{
    
    public PlayerClassData[] playerClassDatas;
    public PlayerClassData currentClass;
    public Dictionary<string, Ability> activatedAbilityDict = new Dictionary<string, Ability>();
    public GameObject abilityUI;
    
    // 어빌리티 UI를 열 때
    // 현재 클래스에 맞는 어빌리티 데이터 중 중복없이 3개를 골라
    // 어빌리티 선택 버튼에 할당해주고
    // 어빌리티 등록 버튼에 해당 어빌리티 데이터의 설명을 설정해줌
    public void Init()
    {
        currentClass = playerClassDatas[0];
    }
    public void OpenSelectAbilityUI()
    {
        // 총 어빌리티 개수에서 현재 활성화된 어빌리티 개수를 뺀 값이 3보다 작으면
        // 선택할 어빌리티가 충분하지 않으므로
        // 더 이상 어빌리티를 선택할 수 없게 함
        // 그렇지 않으면 아래에서 무한반복에 걸려서 게임이 터짐
        if (currentClass.AllAbilities.Count - activatedAbilityDict.Count < 3)
        {
            Debug.Log("Not enough abilities left");
            return;
        }

        abilityUI.SetActive(true);
        int cnt = 0;

        while (cnt < 3)
        {
            AbilityUI temp = abilityUI.transform.GetChild(cnt).GetComponent<AbilityUI>();

            // 중복체크
            // 랜덤으로 숫자를 하나 뽑아 해당 숫자를 인덱스로 가지는 어빌리티를 가져옴
            // 해당 어빌리티가 현재 활성화된 어빌리티 딕셔너리에 있는지 체크
            // 현재 활성화된 어빌리티 딕셔너리에 존재한다면 반복문 다시 처음부터
            int randomIndex = UnityEngine.Random.Range(0, currentClass.AllAbilities.Count);
            Ability newAbility = currentClass.AllAbilities[randomIndex];

            if (activatedAbilityDict.ContainsValue(newAbility))
                continue;

            // 중복되지 않았다면
            // 버튼에 ability를 먼저 할당해준 후
            // 버튼 오브젝트 활성화
            // - 그렇지 않으면 ability가 할당되기전에 버튼이 활성화 되면서
            // - ability data를 정상적으로 받아오지 못하는 버그가 있음
            temp.ability = newAbility;
            temp.gameObject.SetActive(true);

            // 카운트 증가
            // 카운트가 3이 되기 전, 즉 0, 1, 2 총 세번 반복하여 중복 없이 등록
            cnt++;
        }
    }
}
