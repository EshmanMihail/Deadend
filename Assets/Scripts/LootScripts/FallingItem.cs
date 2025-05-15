using UnityEngine;
using Mirror;

public class FallingItem : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private LootProperty lootProperty;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        lootProperty = GetComponent<LootProperty>();
    }

    [Command(requiresAuthority = false)]
    public void CmdActivatePhysics()
    {
        RpcActivatePhysics();
    }

    [ClientRpc]
    private void RpcActivatePhysics()
    {
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 10f;
        rb.mass = lootProperty.weight;
        rb.drag = 0.5f;
        rb.angularDrag = 0.05f;

        if (col != null)
        {
            col.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SpaceToDelete"))
        {
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
