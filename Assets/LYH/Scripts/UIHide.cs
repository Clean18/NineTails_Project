using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHide : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image bgHide;
    // Start is called before the first frame update
    private void OnEnable()
    {
        bgHide.sprite = bgImage.sprite;
    }
}
