using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity;
    public bool isWalkingLeft = true;

    private bool grounded = false;

    private enum EnemyState
    {
        walking,
        falling,
        dead
    }

    private EnemyState state = EnemyState.falling;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        fall();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyPosition();
    }

    void UpdateEnemyPosition()
    {
        if(state != EnemyState.dead)
        {
            Vector3 position = transform.localPosition;
            Vector3 scale = transform.localScale;

            if(state == EnemyState.falling)
            {
                position.y += velocity.y * Time.deltaTime;
                velocity.y -= gravity * Time.deltaTime;
            }
            if(state == EnemyState.walking)
            {

            }
        }
    }

    private void OnBecameInvisible()
    {
        enabled = true;
    }

    void fall()
    {
        velocity.y = 0;
        state = EnemyState.falling;
        grounded = false;
    }
}
