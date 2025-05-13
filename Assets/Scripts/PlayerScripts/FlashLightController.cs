using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class FlashLightBarController : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flashlightClip;
    [SerializeField] private GameObject flashlighObject;
    [SerializeField] private float maxEnergy;

    private SoundManager soundManager;
    private Light2D flashlight;

    private bool isWorking = false;
    private bool isOnTerminal = false;
    private bool isHaveFlashlight = true;
    private Image flashlightBar;

    void Start()
    {
        flashlight = flashlighObject.GetComponent<Light2D>();
        soundManager = GetComponent<SoundManager>();
        flashlightBar = UIManager.Instance.GetFlashlightFillingBar();
        flashlight.intensity = 0;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.T) && !isOnTerminal && isHaveFlashlight)
        {
            CmdToggleFlashlight();
        }

        if (isWorking)
        {
            WasteOfEnergy();
        }
    }

    [Command]
    private void CmdToggleFlashlight()
    {
        isWorking = !isWorking;
        RpcUpdateFlashlightState(isWorking);
    }

    [ClientRpc]
    private void RpcUpdateFlashlightState(bool state)
    {
        audioSource.clip = flashlightClip;
        audioSource.Play();

        isWorking = state;
        flashlight.intensity = state ? 1 : 0;
    }

    private void WasteOfEnergy()
    {
        if (flashlightBar.fillAmount > 0)
        {
            flashlightBar.fillAmount -= Time.deltaTime / 100;

            if (flashlightBar.fillAmount == 0)
            {
                flashlight.intensity = 0;
                CmdToggleFlashlight();
            }
        }
    }

    public void RestoreEnergy()
    {
        if (flashlightBar.fillAmount < 1) soundManager.CmdPlayAudioClip(1, 1);
        flashlightBar.fillAmount = 1;
    }

    public void OnTerminal(bool f)
    {
        if (!isLocalPlayer) return;
        isOnTerminal = f;
    }
}