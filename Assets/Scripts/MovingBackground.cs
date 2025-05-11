using System;
using System.Collections;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [SerializeField] private float speedX; // right -48 left -12
    [SerializeField] private float speedY; // bottom 43 top -12

    [SerializeField] private float maxSpeedX = 6f;
    [SerializeField] private float speedIncreaseDuration = 5f;

    private float directionY = 1;

    private Vector2 startVec = new Vector2(-30, 80);
    private Vector2 endVec = new Vector2(188, 10);

    [SerializeField] private float endx;

    [SerializeField] private float timerToChangeDirectionY;
    private float timer;

    private System.Random rand;

    void Start()
    {
        rand = new System.Random(Guid.NewGuid().GetHashCode());
    }

    private void FixedUpdate()
    {
        transform.localPosition += new Vector3(speedX, speedY * directionY, 0);
        if (transform.localPosition.y >= transform.localPosition.y + 41)
        {
            directionY *= -1;
        }
        else if (transform.localPosition.y <= transform.localPosition.y - 12)
        {
            directionY *= -1;
        }

        if (timer > 0) timer -= Time.fixedDeltaTime;
        else
        {
            directionY *= -1;
            timer = rand.Next(20, 40);
        }

        if (transform.localPosition.x >= endx) transform.localPosition = startVec;
    }

    public void StartSpeedingX()
    {
        StartCoroutine(IncreaseSpeedXOverTime());
    }

    private IEnumerator IncreaseSpeedXOverTime()
    {
        float elapsedTime = 0f;
        float initialSpeedX = speedX;

        while (elapsedTime < speedIncreaseDuration)
        {
            speedX = Mathf.Lerp(initialSpeedX, maxSpeedX, elapsedTime / speedIncreaseDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        speedX = maxSpeedX;
    }
}
