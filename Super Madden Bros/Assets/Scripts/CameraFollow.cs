using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform leftBound;
    public Transform rightBound;

    public float smoothDampTime = 0.15f;
    private Vector3 smoothDampVelocity = Vector3.zero;

    private float cameraWidth, cameraHeight, levelMinX, levelMaxX;
    // Start is called before the first frame update
    void Start()
    {
      cameraHeight = Camera.main.orthographicSize * 2;
      cameraWidth = cameraHeight * Camera.main.aspect;

      float leftBoundWidth = leftBound.GetComponentInChildren<SpriteRenderer> ().bounds.size.x / 2;
      float rightBoundWidth = rightBound.GetComponentInChildren<SpriteRenderer> ().bounds.size.x / 2;

      levelMinX = leftBound.position.x + leftBoundWidth + (cameraWidth / 2);
      levelMaxX = rightBound.position.x - rightBoundWidth - (cameraWidth / 2);
    }

    // Update is called once per frame
    void Update()
    {
      if(target){
        float targetX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, target.position.x));
        float x = Mathf.SmoothDamp(transform.position.x, targetX, ref smoothDampVelocity.x, smoothDampTime);
        transform.position = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
      }

    }
}
