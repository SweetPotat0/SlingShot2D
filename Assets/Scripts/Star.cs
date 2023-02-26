using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public enum StarType
    {
        Time,
        Sticky,
        Bouncy
    }

    [SerializeField]
    public StarType starType;

    private GameManager GameManager;
    private bool isEnabled = true;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        GameManager.stars.Add(this);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) { return; }
        if (GameManager.gameStatus != GameManager.GameStatus.Running) { return; }
        GameManager.PickedStar(starType);
        if (isEnabled)
        {
            isEnabled = false;
            switch (starType)
            {
                case StarType.Time:
                    GameManager.Seconds += 10;
                    break;
                case StarType.Bouncy:
                    GameManager.player.aboutTo = SlingShotPlayer.AboutTo.Bounce;
                    break;
                case StarType.Sticky:
                    GameManager.player.aboutTo = SlingShotPlayer.AboutTo.Stick;
                    break;
            }
            animator.SetTrigger("DestroyTrigger");
            Destroy(gameObject, 400);
        }
    }
}
