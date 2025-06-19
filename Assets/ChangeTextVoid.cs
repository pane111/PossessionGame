using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeTextVoid : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void ChangeText(string _text)
    {
        text.text = _text;
    }
}
