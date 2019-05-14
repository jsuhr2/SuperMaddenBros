using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float gravity;
    public float bounceVelocity;
    public float jumpVelocity;
    public Vector2 velocity;
    private bool walk, walkLeft, walkRight, jump;
    public LayerMask wallMask;
    public LayerMask floorMask;

    public enum PlayerState
    {
        jumping,
        idle,
        walking,
        bouncing
    }
    private PlayerState playerState = PlayerState.idle;
    private bool grounded = false;
    private bool bounce = false;

    // Start is called before the first frame update
    void Start()
    {
        fall();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInput();
        UpdatePlayerPosition();
        UpdateAnimationStates();
    }

    void UpdatePlayerPosition()
    {
        Vector3 position = transform.localPosition;
        Vector3 scale = transform.localScale;
        if (walk)
        {
            if (walkLeft)
            {
                position.x -= velocity.x * Time.deltaTime;
                scale.x = -1;
            }
            else if (walkRight)
            {
                position.x += velocity.x * Time.deltaTime;
                scale.x = 1;
            }

            position = CheckWallRays(position, scale.x);
        }
        if (jump && playerState != PlayerState.jumping)
        {
            playerState = PlayerState.jumping;
            velocity = new Vector2(velocity.x, jumpVelocity);
        }
        if(playerState == PlayerState.jumping)
        {
            position.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }
        if(bounce && playerState != PlayerState.bouncing)
        {
            playerState = PlayerState.bouncing;
            velocity = new Vector2(velocity.x, bounceVelocity);
        }
        if(playerState == PlayerState.bouncing)
        {
            position.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }

        if(velocity.y <= 0)
        {
            position = CheckFloorRays(position);
        }

        if(velocity.y >= 0)
        {
            position = CheckCeilingRays(position);
        }

        transform.localPosition = position;
        transform.localScale = scale;
    }

    void UpdateAnimationStates()
    {
        if(grounded && !walk && !bounce)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
        }

        if(grounded && walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
        }

        if (playerState == PlayerState.jumping)
        {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    void CheckPlayerInput()
    {
        bool inputLeft = Input.GetKey(KeyCode.LeftArrow);
        bool inputRight = Input.GetKey(KeyCode.RightArrow);
        bool inputSpace = Input.GetKeyDown(KeyCode.Space);

        walk = inputLeft || inputRight;
        walkLeft = inputLeft && !inputRight;
        walkRight = !inputLeft && inputRight;
        jump = inputSpace;

    }

    Vector3 CheckWallRays(Vector3 position, float direction)
    {
        Vector2 originTop = new Vector2(position.x + direction * 0.4f, position.y + 1f - 0.2f);
        Vector2 originMiddle = new Vector2(position.x + direction * 0.4f, position.y);
        Vector2 originBottom = new Vector2(position.x + direction * 0.4f, position.y - 1f + 0.2f);
        RaycastHit2D wallTop = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null)
        {
            position.x -= velocity.x * Time.deltaTime * direction;
        }

        return position;
    }

    Vector3 CheckFloorRays(Vector3 position)
    {
        Vector2 originLeft = new Vector2(position.x - 0.5f + 0.2f, position.y - 1f);
        Vector2 originMiddle = new Vector2(position.x, position.y - 1f);
        Vector2 originRight = new Vector2(position.x + 0.5f - 0.2f, position.y - 1f);
        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if (floorLeft.collider != null || floorMiddle.collider != null || floorRight.collider != null)
        {
            RaycastHit2D hitRay = floorRight;
            if (floorLeft)
            {
                hitRay = floorLeft;
            }
            else if(floorMiddle)
            {
                hitRay = floorMiddle;
            } else if (floorRight)
            {
                hitRay = floorRight;
            }

            if (hitRay.collider.tag == "Enemy")
            {
                hitRay.collider.GetComponent<EnemyAI>().Crush();
                bounce = true;
            }

            playerState = PlayerState.idle;
            grounded = true;
            velocity.y = 0;
            position.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1;
        }
        else
        {
            if(playerState != PlayerState.jumping)
            {
                fall();
            }
        }

        return position;
    }

    Vector3 CheckCeilingRays(Vector3 position)
    {
        Vector2 originLeft = new Vector2(position.x - 0.5f + 0.2f, position.y + 1f);
        Vector2 originMiddle = new Vector2(position.x, position.y + 1f);
        Vector2 originRight = new Vector2(position.x + 0.5f - 0.2f, position.y + 1f);
        RaycastHit2D ceilingLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilingMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilingRight = Physics2D.Raycast(originRight, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        if (ceilingLeft.collider != null || ceilingMiddle.collider != null || ceilingRight.collider != null) {
            RaycastHit2D hitRay = ceilingLeft;
            if (ceilingLeft)
            {
                hitRay = ceilingLeft;
            }
            else if (ceilingMiddle)
            {
                hitRay = ceilingMiddle;
            }
            else if (ceilingRight)
            {
                hitRay = ceilingRight;
            }

            position.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - 1;
            fall();
        }

        return position;
    }

    void fall()
    {
        velocity.y = 0;
        playerState = PlayerState.jumping;
        grounded = false;
        bounce = false;
    }
}
