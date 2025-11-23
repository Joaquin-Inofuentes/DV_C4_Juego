using UnityEngine;
using UnityEngine.Advertisements;

public enum AdsResult
{
    Started,
    Clicked,
    Completed,
    Skipped,
    Failed
}

public class AM2_P2_AdsManager : MonoBehaviour,
    IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    public static AM2_P2_AdsManager Instance;

    public System.Action<string, AdsResult> OnAdResult;

    [Header("GAME IDs")]
#if UNITY_ANDROID
    [SerializeField] string gameID = "5975125";
#elif UNITY_IOS
    [SerializeField] string gameID = "5990830";
#endif

    [Header("PLACEMENTS")]
    [SerializeField] public string bannerID = "Banner_Android";
    [SerializeField] public string interstitialID = "Interstitial_Android";
    [SerializeField] public string rewardedID = "Rewarded_Android";

    bool isInitialized = false;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;

        InitializeAds();
    }

    // ============================================================
    // INITIALIZATION
    // ============================================================
    void InitializeAds()
    {
        Advertisement.Initialize(gameID, false, this);
    }

    public void OnInitializationComplete()
    {
        isInitialized = true;
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"INIT ERROR: {error} — {message}");
    }

    // ============================================================
    // LOAD CALLBACKS
    // ============================================================
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementId, this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"LOAD ERROR {placementId}: {error}");
        OnAdResult?.Invoke(placementId, AdsResult.Failed);
    }

    // ============================================================
    // SHOW CALLBACKS
    // ============================================================
    public void OnUnityAdsShowStart(string placementId)
    {
        OnAdResult?.Invoke(placementId, AdsResult.Started);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        OnAdResult?.Invoke(placementId, AdsResult.Clicked);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"SHOW ERROR {placementId}: {error}");
        OnAdResult?.Invoke(placementId, AdsResult.Failed);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        if (state == UnityAdsShowCompletionState.COMPLETED)
            OnAdResult?.Invoke(placementId, AdsResult.Completed);
        else
            OnAdResult?.Invoke(placementId, AdsResult.Skipped);
    }

    // ============================================================
    // PUBLIC API
    // ============================================================
    public void ShowRewarded()
    {
        if (!isInitialized)
        {
            OnAdResult?.Invoke(rewardedID, AdsResult.Failed);
            return;
        }

        Advertisement.Load(rewardedID, this);
    }

    public void ShowInterstitial()
    {
        if (!isInitialized)
        {
            OnAdResult?.Invoke(interstitialID, AdsResult.Failed);
            return;
        }

        Advertisement.Load(interstitialID, this);
    }

    public void ShowBanner()
    {
        if (!isInitialized)
        {
            OnAdResult?.Invoke(bannerID, AdsResult.Failed);
            return;
        }

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerID);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }



    // =========================================================
    // MÉTODOS PARA USAR DESDE BOTONES
    // =========================================================

    public void Btn_ShowRewarded()
    {
        ShowRewarded(result =>
        {
            if (result == AdsResult.Completed)
                Debug.Log("Rewarded: COMPLETED ✔");
            else if (result == AdsResult.Skipped)
                Debug.Log("Rewarded: SKIPPED ⏭");
            else if (result == AdsResult.Failed)
                Debug.Log("Rewarded: FAILED ❌");
            else if (result == AdsResult.Started)
                Debug.Log("Rewarded: STARTED ▶");
            else if (result == AdsResult.Clicked)
                Debug.Log("Rewarded: CLICKED 👆");
        });
    }

    public void Btn_ShowInterstitial()
    {
        ShowInterstitial(result =>
        {
            if (result == AdsResult.Completed)
                Debug.Log("Interstitial: COMPLETED ✔");
            else if (result == AdsResult.Skipped)
                Debug.Log("Interstitial: SKIPPED ⏭");
            else if (result == AdsResult.Failed)
                Debug.Log("Interstitial: FAILED ❌");
            else if (result == AdsResult.Started)
                Debug.Log("Interstitial: STARTED ▶");
            else if (result == AdsResult.Clicked)
                Debug.Log("Interstitial: CLICKED 👆");
        });
    }

    public void Btn_ShowBanner()
    {
        ShowBanner(result =>
        {
            if (result == AdsResult.Completed)
                Debug.Log("Banner: COMPLETED ✔");
            else if (result == AdsResult.Skipped)
                Debug.Log("Banner: SKIPPED ⏭");
            else if (result == AdsResult.Failed)
                Debug.Log("Banner: FAILED ❌");
            else if (result == AdsResult.Started)
                Debug.Log("Banner: STARTED ▶");
            else if (result == AdsResult.Clicked)
                Debug.Log("Banner: CLICKED 👆");
        });
    }

    // =========================================================
    // API PROXY ORIGINAL
    // =========================================================

    public void ShowRewarded(System.Action<AdsResult> callback)
    {
        Instance.OnAdResult = (id, result) =>
        {
            if (id == Instance.rewardedID)
                callback(result);
        };

        Instance.ShowRewarded();
    }

    public void ShowInterstitial(System.Action<AdsResult> callback)
    {
        Instance.OnAdResult = (id, result) =>
        {
            if (id == Instance.interstitialID)
                callback(result);
        };

        Instance.ShowInterstitial();
    }

    public void ShowBanner(System.Action<AdsResult> callback)
    {
        Instance.OnAdResult = (id, result) =>
        {
            if (id == Instance.bannerID)
                callback(result);
        };

        Instance.ShowBanner();
    }
}
