using Mirror;
using UnityEngine;

public class BuildingDoorController : NetworkBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask doorLayer;          

    private void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.E))
        {
            BuildingDoorScript door = GetDoorInFront();
            if (door != null)
            {
                door.ChangeDoorState();
            }
        }
    }

    private BuildingDoorScript GetDoorInFront()
    {
        Vector2[] directions = { transform.right, -transform.right };

        foreach (Vector2 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, interactionDistance, doorLayer);

            Vector2 vec = new Vector2(transform.right.x, transform.right.y);
            Debug.DrawRay(transform.position, direction * interactionDistance, direction == vec ? Color.red : Color.blue, 0.1f);

            if (hit.collider != null)
            {
                return hit.collider.GetComponent<BuildingDoorScript>();
            }
        }
        return null;
    }
}