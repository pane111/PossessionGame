using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Deathtext : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.expertMode)
        {
            GetComponent<TextMeshProUGUI>().text = "THE BLADE CLAIMS A PIECE OF YOUR SOUL";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
