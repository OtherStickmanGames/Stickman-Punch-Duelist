using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif

public class Advertising : MonoBehaviour
#if UNITY_ANDROID
    , IRewardedVideoAdListener 
#endif
{
    public System.Action onVideoClosed;


    public static int countSession;

    void Start()
    {
#if UNITY_ANDROID

        string appKey = "a7041cfd22630fb14fb42041f782a6edf100613649b020a6";
        Appodeal.disableLocationPermissionCheck();
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO);
#endif
    }


    public static void ShowAds()
    {
#if UNITY_ANDROID

        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
        {
            Appodeal.show(Appodeal.REWARDED_VIDEO);
        }
        else if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
        {
            Appodeal.show(Appodeal.INTERSTITIAL);
        }
        else
        {
            Appodeal.show(Appodeal.REWARDED_VIDEO);
        }
        countSession = 0;

        Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, false);
        //Appodeal.initialize(Advertising.appKey, Appodeal.NON_SKIPPABLE_VIDEO |  Appodeal.REWARDED_VIDEO);
        Appodeal.cache(Appodeal.REWARDED_VIDEO);
#endif
    }

    public void onRewardedVideoLoaded(bool precache)
    {
        throw new System.NotImplementedException();
    }

    public void onRewardedVideoFailedToLoad()
    {
        throw new System.NotImplementedException();
    }

    public void onRewardedVideoShowFailed()
    {
        throw new System.NotImplementedException();
    }

    public void onRewardedVideoShown()
    {
        throw new System.NotImplementedException();
    }

    public void onRewardedVideoFinished(double amount, string name)
    {
        throw new System.NotImplementedException();
    }

    public void onRewardedVideoClosed(bool finished)
    {
        onVideoClosed?.Invoke();
    }

    public System.Action xyeta;

    public void onRewardedVideoExpired()
    {
        xyeta?.Invoke();
    }

    public void onRewardedVideoClicked()
    {
        throw new System.NotImplementedException();
    }
}
