using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum PlatformType
    {
        Jumpy,
        Glass,
        Ice,
        Finish,
        Regular
    }

    [SerializeField]
    public PlatformType platformType;

    private GameManager GameManager;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        GameManager.platforms.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")) { return; }
        if (GameManager.gameStatus != GameManager.GameStatus.Running) { return; }


        if(platformType == PlatformType.Finish)
        {
            GameManager.FinishGame(new GameManager.GameFinishArgs { Win = true });
            return;
        }
        if (GameManager.player.aboutTo == SlingShotPlayer.AboutTo.Bounce)
        {
            GameManager.player.aboutTo = SlingShotPlayer.AboutTo.Nothing;
            GameManager.player.isStuck = false;
            GameManager.player.Jump(new Vector2(0, 500));
            return;
        }
        switch (platformType)
        {
            case PlatformType.Finish:
                
                break;
            case PlatformType.Jumpy:
                if (GameManager.player.aboutTo != SlingShotPlayer.AboutTo.Stick && !GameManager.player.isStuck)
                {
                    //Vector2 direction =  col.GetContact(0).point - (Vector2)GameManager.player.transform.position;
                    Debug.Log($"Normal is: {col.GetContact(0).normal}");
                    GameManager.player.Jump(new Vector2(0, 500));
                }
                break;
            case PlatformType.Glass:
                Debug.Log("Lost by glass");
                GameManager.FinishGame(new GameManager.GameFinishArgs { Win = false, DelayAudioClip = 35000 });
                GameManager.player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                GetComponent<AudioSource>().Play();
                GameObject LegRight = gameObject.transform.Find("Leg_Right").gameObject;
                GameObject LegLeft = gameObject.transform.Find("Leg_Left").gameObject;
                GameObject TopLeft = gameObject.transform.Find("Top").gameObject.transform.Find("Top_Left").gameObject;
                GameObject TopCenter = gameObject.transform.Find("Top").gameObject.transform.Find("Top_Center").gameObject;
                GameObject TopRight = gameObject.transform.Find("Top").gameObject.transform.Find("Top_Right").gameObject;
                LegRight.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                LegLeft.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                TopCenter.AddComponent<Rigidbody2D>().mass = 0.5f;
                TopCenter.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -15));
                TopLeft.AddComponent<Rigidbody2D>().mass = 0.5f;
                TopLeft.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -6));
                TopRight.AddComponent<Rigidbody2D>().mass = 0.5f;
                Destroy(gameObject.GetComponent<BoxCollider2D>());
                break;
            case PlatformType.Regular:
                break;
        }
        if (GameManager.player.playerMode == SlingShotPlayer.PlayerMode.MidAir)
        {
            GameManager.player.playerMode = SlingShotPlayer.PlayerMode.Grounded;
        }
    }
}
