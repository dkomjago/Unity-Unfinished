using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ContainerItem : MonoBehaviour
{
    public Info_Item item;

    void Start()
    {
        var parentSize = transform.parent.GetComponent<RectTransform>().sizeDelta;
        transform.parent.GetComponent<RectTransform>().sizeDelta = parentSize + new Vector2(0, gameObject.GetComponent<RectTransform>().sizeDelta.y);
    }

    private void OnDestroy()
    {
        var parentSize = transform.parent.GetComponent<RectTransform>().sizeDelta;
        transform.parent.GetComponent<RectTransform>().sizeDelta = parentSize - new Vector2(0, gameObject.GetComponent<RectTransform>().sizeDelta.y);
    }
}
