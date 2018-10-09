using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FishCounter : MonoBehaviour {

    public GameObject Image;
    public GameObject Text;

    public string Type { get; set; }

    private int _amount;
    public int Amount
    {
        get
        {
            return _amount;
        }
        set
        {
            _amount = value;
            Text.GetComponent<Text>().text = "x" + Amount;
        }
    }

    private void Update()
    {
        var textRect = Text.GetComponent<RectTransform>();
        var imageRect = Image.GetComponent<RectTransform>();
        imageRect.localPosition = new Vector3(-textRect.rect.width - 25, imageRect.localPosition.y, imageRect.localPosition.z);
    }

}
