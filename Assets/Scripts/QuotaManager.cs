using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuotaManager : NetworkBehaviour
{
    [SerializeField] private Text daysLeftText;
    [SerializeField] private Text quotaText;
    [SerializeField] private Text newQuotaText;

    [SerializeField] private Text quotaReachedText;
    [SerializeField] private Text quotaNotReachedText;

    [SerializeField] private GameObject defeatObj;

    private int numberOfDaysToReachTheQuota = 3;

    void Start()
    {
        QuotaSettings.numberOfDay++;

        int daysLeft = numberOfDaysToReachTheQuota - QuotaSettings.numberOfDay;
        if (daysLeft < 0) daysLeft = 0;

        daysLeftText.text = "ƒней осталось: " + (daysLeft).ToString();

        if (QuotaSettings.numberOfDay - 1 == numberOfDaysToReachTheQuota)
        {
            if (QuotaSettings.isQuotaReached)
            {
                StartCoroutine(QuotaReached());
            }
            else
            {
                StartCoroutine(QuotaNotReached());
            }
        }
    }

    private IEnumerator QuotaReached()
    {
        quotaReachedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        quotaReachedText.gameObject.SetActive(false);

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

        defeatObj.gameObject.SetActive(true);
    }
}
