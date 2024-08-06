using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    Vector3 startPos;
    RectTransform rect;
    // Start is called before the first frame update
    private void OnEnable()
    {
        
        rect = GetComponent<RectTransform>();
        startPos = rect.position;
        StartCoroutine(MoveUp());
    }

    IEnumerator MoveUp()
    {
        float elapsedTime = 0f;
        float unitDistanceY = Time.deltaTime / GameManager.Instance.player.loopTime * 120;
        while (elapsedTime < GameManager.Instance.player.loopTime)
        {
            elapsedTime += Time.deltaTime;
            rect.position += Vector3.up * unitDistanceY;

            yield return null;
        }
        yield return null;

    }

    private void OnDisable()
    {
        rect.position = startPos;
    }
}
