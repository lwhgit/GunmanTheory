using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextView : MonoBehaviour {

    private RectTransform rectTransform = null;
    private Text text = null;

    private float originX = 0;
    private float originY = 0;

    void Awake() {
        rectTransform = gameObject.GetComponent<RectTransform>();
        text = gameObject.GetComponent<Text>();

        originX = rectTransform.anchoredPosition.x;
        originY = rectTransform.anchoredPosition.y;


    }


    void Update() {

    }

    private IEnumerator ShakeCorutine() {
        float dx = 0;

        for (float p = 0;p < 20;p += 1f) {
            dx = Mathf.Sin(p) / p * 100;
            rectTransform.anchoredPosition = new Vector2(originX + dx, originY);
            yield return new WaitForSeconds(0.01f);
        }
        
        yield return null;
    }

    public void ShakeText() {
        StartCoroutine(ShakeCorutine());
    }

    public void SetText(string str) {
        text.text = str;
    }
    
}
