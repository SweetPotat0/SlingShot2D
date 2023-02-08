using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotPlayer : MonoBehaviour
{
    public enum PlayerMode
    {
        MidAir,
        Grounded
    }
    public enum AboutTo
    {
        Nothing,
        Bounce,
        Stick
    }

    public PlayerMode playerMode = SlingShotPlayer.PlayerMode.Grounded;

    [HideInInspector]
    public AboutTo aboutTo = AboutTo.Nothing;

    public float distToGround;

    private GameManager gameManager;
    private Rigidbody2D rigidbody;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.player = this;
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        distToGround = collider.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameStatus != GameManager.GameStatus.Running) { return; }
        if (IsGrounded())
        {
            if (playerMode != PlayerMode.Grounded)
            {
                playerMode = PlayerMode.Grounded;
            }
        }
        else
        {
            if (playerMode != PlayerMode.MidAir)
            {
                playerMode = PlayerMode.MidAir;
            }

            //Check if player is falling to infinity
            if (transform.position.y < gameManager.FloorYLocation)
            {
                gameManager.FinishGame(new GameManager.GameFinishArgs { Win = false });
            }
        }
    }

    public void Jump(Vector2 force)
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        audioSource.Play();
        rigidbody.AddForce(force);
    }

    public bool isStuck = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (aboutTo == AboutTo.Stick)
        {
            isStuck = true;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.useFullKinematicContacts = true;
            rigidbody.velocity = Vector2.zero;
            aboutTo = AboutTo.Nothing;
        }
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    bool isGrounded = false;

    bool IsGrounded()
    {
        //Debug.DrawLine(transform.position, (Vector2)transform.position - (distToGround + 0.1f) * Vector2.up);
        //RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.1f);
        //return raycastHit2D.collider != null;
        return isGrounded;
    }
}
