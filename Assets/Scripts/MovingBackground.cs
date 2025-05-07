using System;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [SerializeField] private float speedX; // right -48 left -12
    [SerializeField] private float speedY; // bottom 43 top -12

    private float directionY = 1;

    private Vector2 startVec = new Vector2(-48, 10);
    private Vector2 endVec = new Vector2(188, 10);

    [SerializeField] private float endx;

    [SerializeField] private float timerToChangeDirectionY;
    private float timer;

    private System.Random rand;

    void Start()
    {
        rand = new System.Random(Guid.NewGuid().GetHashCode());
        //if (ship == null) ship = GameObject.FindGameObjectWithTag("Ship");
    }

    private void FixedUpdate()
    {
        transform.localPosition += new Vector3(speedX, speedY * directionY, 0);
        if (transform.localPosition.y >= 41)
        {
            directionY *= -1;
        }
        else if (transform.localPosition.y <= -12)
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
}
