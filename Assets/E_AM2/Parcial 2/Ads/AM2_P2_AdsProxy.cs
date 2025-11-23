using UnityEngine;

public class AM2_P2_AdsProxy : MonoBehaviour
{
    public static AM2_P2_AdsProxy Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    // =============================================
    // API PROXY
    // =============================================
    public void ShowRewarded(System.Action<AdsResult> callback)
    {
        AM2_P2_AdsManager.Instance.OnAdResult = (id, result) =>
        {
            if (id == AM2_P2_AdsManager.Instance.rewardedID)
                callback(result);
        };

        AM2_P2_AdsManager.Instance.ShowRewarded();
    }

    public void ShowInterstitial(System.Action<AdsResult> callback)
    {
        AM2_P2_AdsManager.Instance.OnAdResult = (id, result) =>
        {
            if (id == AM2_P2_AdsManager.Instance.interstitialID)
                callback(result);
        };

        AM2_P2_AdsManager.Instance.ShowInterstitial();
    }

    public void ShowBanner(System.Action<AdsResult> callback)
    {
        AM2_P2_AdsManager.Instance.OnAdResult = (id, result) =>
        {
            if (id == AM2_P2_AdsManager.Instance.bannerID)
                callback(result);
        };

        AM2_P2_AdsManager.Instance.ShowBanner();
    }




}
