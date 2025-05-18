using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuotaManager : NetworkBehaviour
{
    [SerializeField] private Text daysLeftText;
    [SerializeField] private Text quotaText;
    [SerializeField] private Text playersMoney;
    [SerializeField] private Text newQuotaText;

    [SerializeField] private Text quotaReachedText;
    [SerializeField] private Text quotaNotReachedText;

    [SerializeField] private GameObject defeatObj;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip oneDayLeft;
    [SerializeField] private AudioClip zeroDayLeft;
    [SerializeField] private AudioClip DefaulDayLeft;
    [SerializeField] private AudioClip quotaReachedSound;
    [SerializeField] private AudioClip newQuotaSound;

    private int numberOfDaysToReachTheQuota = 3;

    void Start()
    {
        SetTextValues();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            QuotaSettings.numberOfDay++;
            SetDayNumberText();
            PlayDayLeftSounds();

            if (QuotaSettings.numberOfDay - 1 == numberOfDaysToReachTheQuota || QuotaSettings.isQuotaReached)
            {
                if (QuotaSettings.isQuotaReached)
                {
                    QuotaSettings.numberOfDay = 0;
                    SetDayNumberText();

                    StartCoroutine(QuotaReached());

                    QuotaSettings.isQuotaReached = false;
                    QuotaSettings.soldLootAmount = 0;
                }
                else
                {
                    StartCoroutine(QuotaNotReached());
                }
            }
        }
    }

    private void SetTextValues()
    {
        playersMoney.text = QuotaSettings.money.ToString() + '$';

        SetDayNumberText();

        quotaText.text = " вота: " + QuotaSettings.quota.ToString();
    }

    private void SetDayNumberText()
    {
        int daysLeft = numberOfDaysToReachTheQuota - QuotaSettings.numberOfDay;
        if (daysLeft < 0) daysLeft = 0;
        daysLeftText.text = "ƒней осталось: " + (daysLeft).ToString();
    }

    private void PlayDayLeftSounds()
    {
        int daysLeft = numberOfDaysToReachTheQuota - QuotaSettings.numberOfDay;
        if (daysLeft == 1)
        {
            audioSource.PlayOneShot(oneDayLeft);
        }
        else if (daysLeft == 0)
        {
            audioSource.PlayOneShot(zeroDayLeft);
        }
        else
        {
            audioSource.PlayOneShot(DefaulDayLeft);
        }
    }

    private IEnumerator QuotaReached()
    {
        audioSource.PlayOneShot(quotaReachedSound);
        quotaReachedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        quotaReachedText.gameObject.SetActive(false);

        SetNewQuota();
    }

    [Server]
    private void SetNewQuota()
    {
        float randFloat = Random.Range(0.5f, 0.8f);
        int newQuota = (int)(QuotaSettings.quota * randFloat);
        RpcSetNewQuota(newQuota);
    }

    [ClientRpc]
    private void RpcSetNewQuota(int newQuota)
    {
        QuotaSettings.quota += newQuota;
        StartCoroutine(ShowNewQoutaText());
    }

    private IEnumerator ShowNewQoutaText()
    {
        newQuotaText.gameObject.SetActive(true);

        int targetQuota = QuotaSettings.quota;

        float animationDuration = Mathf.Clamp(targetQuota * 0.01f, 1f, 15f);

        int currentQuota = 0;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            currentQuota = Mathf.FloorToInt(Mathf.Lerp(0, targetQuota, elapsedTime / animationDuration));

            newQuotaText.text = currentQuota.ToString();

            yield return null;
        }

        audioSource.PlayOneShot(newQuotaSound);
        newQuotaText.text = targetQuota.ToString();
        yield return new WaitForSeconds(5f);
        newQuotaText.gameObject.SetActive(false);

        quotaText.text = " вота: " + targetQuota.ToString();
    }

    private IEnumerator QuotaNotReached()
    {
        quotaNotReachedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        quotaNotReachedText.gameObject.SetActive(false);

        ClearGameSettings();
        defeatObj.gameObject.SetActive(true);
    }

    private void ClearGameSettings()
    {
        MissionSettings.lootInShip.Clear();
        MissionSettings.countOfCollectedLoot = 0;
        MissionSettings.countOfDeath = 0;

        QuotaSettings.numberOfDay = -1;
        QuotaSettings.quota = 80;
        QuotaSettings.isQuotaReached = false;
        QuotaSettings.money = 60;
        QuotaSettings.soldLootAmount = 0;
    }
}
