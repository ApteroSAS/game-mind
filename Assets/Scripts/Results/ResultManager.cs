using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ResultManager : MonoBehaviour
{
    private int result = 0;

    [SerializeField] private int question1MaxValue;
    [SerializeField] private int question2MaxValue;
    [SerializeField] private int question3MaxValue;
    [SerializeField] private int question4MaxValue;

    [SerializeField] private GameObject question1Explanation;
    [SerializeField] private GameObject question2Explanation;
    [SerializeField] private GameObject question3Explanation;
    [SerializeField] private GameObject question4Explanation;

    [SerializeField] private GameObject finalSay;

    public delegate void UpdateResult(int value);
    private UpdateResult updateResult;

    public void UpdateResultAddListener(UpdateResult listener)
    {
        updateResult += listener;
    }

    [ClientRpc]
    public void UpdateResultInvokeClientRpc(int value)
    {
        updateResult.Invoke(value);
    }

    private void Awake()
    {
        UpdateResultAddListener(AddToResult);
    }

    private void AddToResult(int value)
    {
        result += value;
        Debug.Log("Result value: " + result);
    }

    [ClientRpc]
    private void SetSameAnswerClientRpc(bool sameAnswer)
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerAnswers>().SetSameAnswer(sameAnswer);
    }

    public void CalculateQuestion1(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        var q1InfoInstance = Instantiate(question1Explanation);
        q1InfoInstance.GetComponent<NetworkObject>().Spawn();
        if (hostAnswers.NetworkQ1Answer.Value == guestAnswers.NetworkQ1Answer.Value)
        {
            UpdateResultInvokeClientRpc(question1MaxValue);
            SetSameAnswerClientRpc(true);
            q1InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(true);
        }
        else
        {
            UpdateResultInvokeClientRpc(-question1MaxValue);
            q1InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(false);
        }


    }

    public void CalculateQuestion3(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        var q3InfoInstance = Instantiate(question3Explanation);
        q3InfoInstance.GetComponent<NetworkObject>().Spawn();

        Symbol[] top3SymbolsHost = new Symbol[3];
        Symbol[] top3SymbolsGuest = new Symbol[3];

        List<Q3_HoldData> q3datas = new();

        foreach (var item in hostAnswers.NetworkQ3Blocks)
        {
            q3datas.Add(item);
        }
        q3datas.Sort((a, b) => b.PositionData.y.CompareTo(a.PositionData.y));
        for (int i = 0; i < top3SymbolsHost.Length; i++)
        {
            top3SymbolsHost[i] = (q3datas[i].SymbolData);
        }

        q3datas.Clear();
        foreach (var item in guestAnswers.NetworkQ3Blocks)
        {
            q3datas.Add(item);
        }
        q3datas.Sort((a, b) => b.PositionData.y.CompareTo(a.PositionData.y));
        for (int i = 0; i < top3SymbolsGuest.Length; i++)
        {
            top3SymbolsGuest[i] = (q3datas[i].SymbolData);
        }

        int matchCount = 0;
        foreach(var symbolHost in top3SymbolsHost)
        {
            foreach (var symbolGuest in top3SymbolsGuest)
            {
                if (symbolHost == symbolGuest) matchCount++;
                if (matchCount >= 2)
                {
                    UpdateResultInvokeClientRpc(question3MaxValue);
                    q3InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(true);
                    return;
                }
            }
        }
        UpdateResultInvokeClientRpc(-question3MaxValue);
        q3InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(false);
    }

    public void CalculateQuestion4(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        var q4InfoInstance = Instantiate(question4Explanation);
        q4InfoInstance.GetComponent<NetworkObject>().Spawn();

        float hostMeter = hostAnswers.NetworkSexMeter.Value;
        float guestMeter = guestAnswers.NetworkSexMeter.Value;
        float difference = Mathf.Abs(hostMeter - guestMeter);
        if (difference >= 0.25f)
        {
            UpdateResultInvokeClientRpc(-question4MaxValue);
            q4InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(false);
        }
        else
        {
            UpdateResultInvokeClientRpc(question4MaxValue);
            q4InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(true);
        }
    }

}
