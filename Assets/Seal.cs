using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seal : MonoBehaviour
{
    public List<DemonHeart> hearts;
    bool broken=false;
    public ParticleSystem burst;
    public GameObject fDoor;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            int counter = 0;
            foreach (DemonHeart demonHeart in hearts)
            {
                if (demonHeart.dead)
                {
                    counter++;
                }
            }
            if (counter >= 4)
            {
                broken = true;
                if (PlayerPrefs.HasKey("EMBeaten") && !PlayerPrefs.HasKey("FinalSecret") && GameManager.Instance.player.npckills >= 5)
                {
                    GameManager.Instance.SendNotification("!EntrDoorOp?nNotif!");
                    GameManager.Instance.displayTutorials = true;
                    GameManager.Instance.PopupTutorial("Tutorial.EntranceDoorOpen\nYou have chosen the path of blood. A door has ope?ed.=??#", null);
                    fDoor.SetActive(true);
                }
                burst.Play();
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        
    }
}
