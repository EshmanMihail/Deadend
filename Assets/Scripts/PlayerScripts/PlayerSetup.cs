using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    private void Start()
    {
        if (isLocalPlayer)
        {
            if (GetComponent<AudioListener>() == null)
            {
                gameObject.AddComponent<AudioListener>();
            }
        }
        else
        {
            var audioListener = GetComponent<AudioListener>();
            if (audioListener != null)
            {
                Destroy(audioListener);
            }
        }
    }
}