using Mirror;
using UnityEngine;

public class MainDoorScript : NetworkBehaviour
{
    [SerializeField] private GameObject buildingOutsideDoor;
    [SerializeField] private GameObject buildingInsideDoor;
    [SerializeField] private AudioClip getInSound;
    private AudioSource audioSource;

    private GameObject player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = getInSound;
    }

    void Update()
    {
        if (player != null && Input.GetKeyDown(KeyCode.E))
        {
            CmdTeleportPlayer(player.GetComponent<NetworkIdentity>().netId);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            player = null;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdTeleportPlayer(uint playerNetId)
    {
        GameObject targetPlayer = NetworkServer.spawned[playerNetId].gameObject;

        if (targetPlayer == null) return;

        Vector2 targetPosition;

        if (buildingOutsideDoor == null && buildingInsideDoor != null)
        {
            targetPosition = (Vector2)buildingInsideDoor.transform.position + new Vector2(1, 0); // Телепортация к внутренней двери
        }
        else if (buildingInsideDoor == null && buildingOutsideDoor != null)
        {
            targetPosition = (Vector2)buildingOutsideDoor.transform.position + new Vector2(-1, 0); // Телепортация к внешней двери
        }
        else
        {
            Debug.LogWarning("One of the doors is not set up properly!");
            return;
        }

        RpcTeleportPlayer(playerNetId, targetPosition);
    }

    [ClientRpc]
    private void RpcTeleportPlayer(uint playerNetId, Vector2 targetPosition)
    {
        if (NetworkClient.spawned.TryGetValue(playerNetId, out NetworkIdentity targetIdentity))
        {
            GameObject targetPlayer = targetIdentity.gameObject;

            audioSource.Play();

            if (buildingOutsideDoor == null && buildingInsideDoor != null)
            {
                buildingInsideDoor.GetComponent<AudioSource>().Play();
            }
            else if (buildingInsideDoor == null && buildingOutsideDoor != null)
            {
                buildingOutsideDoor.GetComponent<AudioSource>().Play();
            }

            if (targetPlayer != null)
            {
                targetPlayer.transform.position = targetPosition;
            }
        }
    }
}