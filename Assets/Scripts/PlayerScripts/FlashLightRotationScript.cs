using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightRotationScript : NetworkBehaviour
{
    [SerializeField] private GameObject flashlighObject;
    private Light2D pointLight;
    public float rotationSpeed = 10f;

    [SyncVar] private Quaternion flashlightRotation;
    private bool isHaveFlashlight = true;
    private Camera mainCamera;
    private Transform target;

    private void Awake()
    {
        pointLight = flashlighObject.GetComponent<Light2D>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        target = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || !isHaveFlashlight) return;

        RotateFlashlight();
        CmdSyncFlashlightRotation(pointLight.transform.rotation);
    }

    private void RotateFlashlight()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.transform.position.z));

        Vector3 direction = worldMousePosition - target.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        pointLight.transform.rotation = Quaternion.Slerp(pointLight.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    [Command]
    private void CmdSyncFlashlightRotation(Quaternion rotation)
    {
        flashlightRotation = rotation;
        RpcSyncFlashlightRotation(rotation);
    }

    [ClientRpc]
    private void RpcSyncFlashlightRotation(Quaternion rotation)
    {
        if (!isLocalPlayer)
        {
            pointLight.transform.rotation = rotation;
        }
    }
}