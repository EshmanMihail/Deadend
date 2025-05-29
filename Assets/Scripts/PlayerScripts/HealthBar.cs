using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

public class HealthBar : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] float hp;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damageHighlightDuration = 0.4f;

    private SpriteRenderer characterSprite;
    private int damageShaderID = Shader.PropertyToID("_DissolveAmount");
    private int damageShaderNoiseID = Shader.PropertyToID("_NoiseAmount");
    private Material damageMaterial;

    private float shaderAmount = 0.85f;
    private Image healthBar;
    private Color damageColor = Color.red;
    [HideInInspector] public bool isCharacterDead;

    private CameraFollowScript cameraFollowScript;
    private Character character;
    private FlashLightBarController flashLightBarController;
    private InventoryController inventoryController;

    private void Awake()
    {
        hp = maxHealth;
    }

    void Start()
    {
        healthBar = UIManager.Instance.GetFillingBar();
        characterSprite = GetComponent<SpriteRenderer>();
        cameraFollowScript = GetComponent<CameraFollowScript>();
        character = GetComponent<Character>();
        flashLightBarController = GetComponent<FlashLightBarController>();
        inventoryController = GetComponent<InventoryController>();

        damageMaterial = characterSprite.material;

        if (isServer)
        {
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
            int noiseType = rand.Next(8, 70);
            //CmdGetNoiseNumber(noiseType);
        }

        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            LevelManager.Instance.RegisterPlayer(this);
        }
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            LevelManager.Instance.UnregisterPlayer(this);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            CmdTakeDamage(20);
        }

        if (hp <= 0 && !isCharacterDead)
        {
            Die();
        }
        if (healthBar != null)
        {
            healthBar.fillAmount = hp / maxHealth;
        }
    }

    #region Get Noise Number
    [Command]
    private void CmdGetNoiseNumber(int randNumber)
    {
        RpcGetNoiseNumber(randNumber);
    }

    [ClientRpc]
    private void RpcGetNoiseNumber(int randNumber)
    {
        damageMaterial.SetFloat(damageShaderNoiseID, randNumber);
    }
    #endregion

    #region damage
    [Command]
    public void CmdTakeDamage(float damage)
    {
        if (hp > 0)
        {
            if (hp - damage < 0) damage = hp;

            hp -= damage;

            shaderAmount -= damage / 100 * 0.35f;

            RpcShowDamageHighlight(shaderAmount);

            if (hp <= 0)
            {
                hp = 0;
                Die();
            }
        }
    }

    [ClientRpc]
    private void RpcShowDamageHighlight(float shaderAmount)
    {
        damageMaterial.SetFloat(damageShaderID, shaderAmount);
        StartCoroutine(ShowDamageHighlight());
    }

    private IEnumerator ShowDamageHighlight()
    {
        if (characterSprite != null)
        {
            characterSprite.color = damageColor;

            yield return new WaitForSeconds(damageHighlightDuration);

            characterSprite.color = Color.white;
        }
    }
    #endregion

    private void Die()
    {
        if (isLocalPlayer) UIManager.Instance.HideTextHint();

        isCharacterDead = true;
        
        character.enabled = false;
        flashLightBarController.enabled = false;

        cameraFollowScript.OnPlayerDied(this);
        if (isServer)
        {
            LevelManager.Instance.CheckAllPlayersDead();
        }
    }
}
