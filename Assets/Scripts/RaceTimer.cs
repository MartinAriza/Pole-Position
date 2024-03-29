﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class RaceTimer : NetworkBehaviour
{
    PlayerInfo playerInfo;
    UIManager uIManager;

    //Si el timer debe contar el tiempo o no
    bool timerRunning = true;

    public static event Action<PlayerInfo, float> OnTotalTime;
    public static event Action<PlayerInfo, float> OnLapTime;

    private void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
        uIManager = FindObjectOfType<UIManager>();

        StartCoroutine(UpdateUI());
    }

    void FixedUpdate()
    {
        if(timerRunning)
        {
            playerInfo.totalTime += Time.fixedDeltaTime;
            playerInfo.currentLapTime += Time.fixedDeltaTime;
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetLapTime()
    {
        playerInfo.currentLapTime = 0f;
    }

    //Se actualiza la interfaz que muestra el tiempo 10 veces por segundo 
    //No es necesario actualizar más amenudo y así no se mandan mensajes innecesarios
    IEnumerator UpdateUI()
    {
        while(timerRunning && isLocalPlayer)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            uIManager.UpdateTime(playerInfo.currentLapTime, playerInfo.totalTime);
        }

        StopCoroutine(UpdateUI());
    }

    //Se sincroniza el tiempo total de cada jugador en todas las instancias del juego
    public void GetFinalTimes()
    {
        CmdChangeTotalTime(playerInfo.totalTime);
    }

    //Command y Rpc para cambiar totalTime
    [Command]
    public void CmdChangeTotalTime(float totalTime)
    {
        RpcChangeTotalTime(totalTime);
    }

    [ClientRpc]
    public void RpcChangeTotalTime(float totalTime)
    {
        OnTotalTime?.Invoke(playerInfo, totalTime);
    }

    //Command y Rpc para cambiar LapTime
    [Command]
    public void CmdChangeLapTime(float lapTime)
    {
        RpcChangeLapTime(lapTime);
    }

    [ClientRpc]
    public void RpcChangeLapTime(float lapTime)
    {
        OnLapTime?.Invoke(playerInfo, lapTime);
    }
}
