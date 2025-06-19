using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name=="Player")
        {
            GetComponent<Animator>().SetTrigger("Player");
            Camera.main.transform.parent = transform;
            Destroy(collision.gameObject);
        }
    }
    public void GoToFight()
    {
        SceneManager.LoadScene("UltraSecretFight");
    }
}
