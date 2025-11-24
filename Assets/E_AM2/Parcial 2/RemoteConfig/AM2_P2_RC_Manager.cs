using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;

public class AM2_P2_RC_Manager : MonoBehaviour
{
    public static AM2_P2_RC_Manager I;

    public struct userAttributes { }
    public struct appAttributes { }

    bool isReady = false;

    // Cola cuando I aún no existe
    static Queue<Action> pendingInstanceQueue = new Queue<Action>();

    // Cola cuando Init aún no terminó
    Queue<Action> pendingInitQueue = new Queue<Action>();


    void OnEnable()
    {
        I = this;

        // Procesamos todas las acciones que dependían de I
        while (pendingInstanceQueue.Count > 0)
            pendingInstanceQueue.Dequeue().Invoke();

        _ = Init();
    }

    async Task Init()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        isReady = true;

        // Ejecutamos todo lo que esperaba el Init
        while (pendingInitQueue.Count > 0)
            pendingInitQueue.Dequeue().Invoke();
    }

    // -----------------------------------------------------------
    //  ETAPA 1: Ver si existe I (instancia)
    // -----------------------------------------------------------
    static void ExecuteOrQueueInstance(Action action)
    {
        if (I != null)
            action.Invoke();
        else
            pendingInstanceQueue.Enqueue(action);
    }

    // -----------------------------------------------------------
    //  ETAPA 2: Ver si Init terminó
    // -----------------------------------------------------------
    void ExecuteOrQueueInit(Action action)
    {
        if (isReady)
            action.Invoke();
        else
            pendingInitQueue.Enqueue(action);
    }


    // -----------------------------------------------------------
    //  GET INT
    // -----------------------------------------------------------
    public static void GetInt(string key, Action<int> callback)
    {
        ExecuteOrQueueInstance(() =>
        {
            I.ExecuteOrQueueInit(() =>
            {
                RemoteConfigService.Instance.FetchCompleted += OnDone;
                RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());

                void OnDone(ConfigResponse r)
                {
                    RemoteConfigService.Instance.FetchCompleted -= OnDone;

                    var cfg = RemoteConfigService.Instance.appConfig;
                    int value = cfg.HasKey(key) ? cfg.GetInt(key) : 0;

                    callback(value);
                }
            });
        });
    }

    // -----------------------------------------------------------
    //  GET FLOAT
    // -----------------------------------------------------------
    public static void GetFloat(string key, Action<float> callback)
    {
        ExecuteOrQueueInstance(() =>
        {
            I.ExecuteOrQueueInit(() =>
            {
                RemoteConfigService.Instance.FetchCompleted += OnDone;
                RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());

                void OnDone(ConfigResponse r)
                {
                    RemoteConfigService.Instance.FetchCompleted -= OnDone;

                    var cfg = RemoteConfigService.Instance.appConfig;
                    float value = cfg.HasKey(key) ? cfg.GetFloat(key) : 0;
                    callback(value);
                }
            });
        });
    }

    // -----------------------------------------------------------
    //  GET STRING
    // -----------------------------------------------------------
    public static void GetString(string key, Action<string> callback)
    {
        ExecuteOrQueueInstance(() =>
        {
            I.ExecuteOrQueueInit(() =>
            {
                RemoteConfigService.Instance.FetchCompleted += OnDone;
                RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());

                void OnDone(ConfigResponse r)
                {
                    RemoteConfigService.Instance.FetchCompleted -= OnDone;

                    var cfg = RemoteConfigService.Instance.appConfig;
                    string value = cfg.HasKey(key) ? cfg.GetString(key) : "";
                    callback(value);
                }
            });
        });
    }
}
