using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{

    private bool isReleased = false;
    private Rigidbody2D rb;
    private Vector3 movement = Vector3.right;
    private bool won = false;
    public GameManager gameManager;
    private int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        level = gameManager.GetLevel();
        gameObject.SetActive(true);
        if(level == 2) {
            Collider2D collider2D = gameObject.GetComponent<Collider2D>();
            collider2D.sharedMaterial.bounciness = 0.1f;
            collider2D.enabled = false;
            collider2D.enabled = true;

        }
        if(level == 3) {
            gameObject.transform.localScale = new Vector3(0.125f, 0.125f, 1);
        }
        rb = GetComponent<Rigidbody2D>();
        if(isReleased == false) {
            rb.gravityScale = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(won) {
            return;
        }
        if(!isReleased) {
            if(transform.position.x >= 2f) {
                movement = Vector3.left;
            }
            if(transform.position.x <= -2f) {
                movement = Vector3.right;
            }
            transform.Translate(movement * Time.deltaTime * 3);
            return;
        }
        float speed = rb.velocity.magnitude;
        if(speed < 0.1) {
            if(gameObject.transform.position.y > GameObject.Find("WinningLine").transform.position.y) {
                won = true;
                gameManager.Win();
            }
        }
    }

    public void Release() {
        isReleased = true;
        rb.gravityScale = 1;
        rb.velocity = new Vector3(0, -0.5f, 0);
    }
}
