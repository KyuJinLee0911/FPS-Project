using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ItemManager : MonoBehaviour
{
    public List<Item> activatedItem = new List<Item>();
    public GameObject itemInfoPrefab;
    public Transform itemInfoParent;
    public int batteries = 0;
    public int gainedBatteriesInGame = 0;
    [SerializeField] List<float> itemDropRate; // 처음부터 순서대로 아무것도 나오지 않을 확률, 아이템이 나올 확률, 무기가 나올 확률
    [SerializeField] List<float> basicItemDropRate; // 기본 아이템 (전지, 힐링 아이템)은 위의 아이템들과 별개 확률로 드랍
    [SerializeField] List<ItemData> everyItems;
    [SerializeField] List<ItemData> everyWeapons;
    [SerializeField] List<ItemData> basicItems;
    private void LoadItemData()
    {
        var loadedJson = Resources.Load<TextAsset>("JSON/ItemDropTable");
    }

    public void RemoveEveryItem()
    {

    }

    public void Init()
    {

    }

    private void Awake()
    {

    }


    private void Start()
    {
    }

    public void DropItem(Transform locationTransform)
    {
        ItemData data = DecideItemType(PickRandomIndex(itemDropRate));
        ItemData basicData = DecideBasicType(PickRandomIndex(basicItemDropRate));
        if (data != null)
        {
            Item newItem = new Item();
            newItem.itemData = data;

            GameObject newItemObj = Instantiate(newItem.itemData.prefab);
            newItemObj.transform.position = locationTransform.position + Vector3.up * 0.7f;
            newItemObj.transform.LookAt(GameManager.Instance.player.transform.position);
        }

        if (basicData != null)
        {
            Item newBasicItem = new Item();
            newBasicItem.itemData = basicData;

            GameObject newBasicItemObj = Instantiate(newBasicItem.itemData.prefab);
            newBasicItemObj.transform.position = locationTransform.position + Vector3.up * 0.3f;
            newBasicItemObj.transform.LookAt(GameManager.Instance.player.transform.position);
        }
    }

    public void AddBattery()
    {
        gainedBatteriesInGame++;
        batteries++;
    }

    // 인게임중에 이 함수가 호출 -> 리스타트
    // 인게임에서 얻은 전지는 다시 반환
    public void RemoveBatteies(GameState state)
    {
        if (state == GameState.GS_INGAME)
            batteries -= gainedBatteriesInGame;
        gainedBatteriesInGame = 0;
    }

    // 리스트에서 랜덤 확률로 하나의 인덱스를 뽑는 함수
    int PickRandomIndex(List<float> rates)
    {
        if (rates.Count == 0) return -1;
        float total = 0;

        foreach (float n in rates)
        {
            total += n;
        }

        float randPoint = Random.value * total;
        for (int i = 0; i < rates.Count; i++)
        {
            if (randPoint >= rates[i])
            {
                randPoint -= rates[i];
            }
            else
            {
                return i;
            }
        }
        return rates.Count - 1;
    }

    // 무기를 떨어뜨릴 것인지, 아이템을 떨어뜨릴 것인지, 아무것도 안떨어트릴 것인지 결정하는 함수
    ItemData DecideItemType(int index)
    {
        switch (index)
        {
            case 0:
                return null;

            case 1:
                return PickItem(everyItems);

            case 2:
                return PickItem(everyWeapons);
        }

        return null;
    }

    // 기본 아이템 드랍 테이블 = { 체력 아이템 확률, 전지 확률, 아무것도 안나올 확률 }
    // 아무것도 안나올 확률에 걸린다면, index는 기본 아이템 드랍 테이블의 가장 마지막 원소의 index와 같다
    ItemData DecideBasicType(int index)
    {
        if (index >= basicItems.Count || index == -1) return null;

        return basicItems[index];
    }

    // 결정된 아이템 종류를 가지고 전체 아이템들 중 어떤 아이템을 떨어뜨릴 것인지 결정해 반환하는 함수
    ItemData PickItem(List<ItemData> datas)
    {
        // 아무런 데이터도 없으면 null 반환
        if (datas.Count == 0) return null;
        List<float> weights = new List<float>();
        foreach (var data in datas)
        {
            weights.Add(data.Weight);
        }

        int index = PickRandomIndex(weights);

        return datas[index];
    }
}

public class ItemDropData
{
    string name;
    int type;
    float rate;
}