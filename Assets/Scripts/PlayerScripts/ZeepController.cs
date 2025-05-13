using Mirror;
using UnityEngine;

public class ZeepController : NetworkBehaviour
{
    private GameObject zeep;

    private FlashLightBarController flashLightBarController;

    void Start()
    {
        flashLightBarController = GetComponent<FlashLightBarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (zeep != null)
            {
                flashLightBarController.RestoreEnergy();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zeep"))
        {
            if (isLocalPlayer)
            {
                UIManager.Instance.ShowTextHint(collision.gameObject.transform.position, "Нажмите Е для зарядки фонарика");
            }
            zeep = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zeep"))
        {
            if (isLocalPlayer)
            {
                UIManager.Instance.HideTextHint();
            }
            zeep = null;
        }
    }
}
