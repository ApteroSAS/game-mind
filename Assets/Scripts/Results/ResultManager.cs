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

    [SerializeField] private GameObject question1Result;
    [SerializeField] private GameObject question1EasyMiniLabyrinth;
    [SerializeField] private GameObject question1HardMiniLabyrinth;

    [SerializeField] private GameObject question2Result;

    [SerializeField] private GameObject question3Result;
    [SerializeField] private GameObject question3Podest;

    [SerializeField] private GameObject question4Result;


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

    public void SpawnResults(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        CalculateQuestions(hostAnswers, guestAnswers);

        SpawnQuestion1(hostAnswers, guestAnswers);
        SpawnQuestion3(hostAnswers, guestAnswers);
        SpawnQuestion4(hostAnswers, guestAnswers);
    }

    private void SpawnQuestion1(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        float offSetX = 1.5f;

        var question1Instance = Instantiate(question1Result);
        Vector3 q1pos = ApplyOffsetToVector3(question1Explanation.transform.position, offSetX);
        question1Instance.GetComponent<NetworkObject>().Spawn();
        question1Instance.GetComponent<Q1_Results>().OnSpawnClientRpc(q1pos, hostAnswers.NetworkQ1Answer.Value);

        question1Instance = Instantiate(question1Result);
        q1pos = ApplyOffsetToVector3(question1Explanation.transform.position, -offSetX);
        question1Instance.GetComponent<NetworkObject>().Spawn();
        question1Instance.GetComponent<Q1_Results>().OnSpawnClientRpc(q1pos, guestAnswers.NetworkQ1Answer.Value);

    }

    private void SpawnQuestion3(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        Debug.Log("doing question 3");
        float offSetX = 2.5f;

        var question3PodestInstance = Instantiate(question3Podest);
        Vector3 podestPos = ApplyOffsetToVector3(question3Explanation.transform.position, offSetX);
        podestPos.z += 1;
        question3PodestInstance.GetComponent<NetworkObject>().Spawn();
        question3PodestInstance.GetComponent<TeleportOnSpawn>().MoveOnSpawnClientRpc(podestPos);
        for (int i = 0; i < hostAnswers.NetworkQ3Blocks.Count; i++)
        {
            var question3BlockInstance = Instantiate(question3Result);
            Vector3 blockPos = ApplyOffsetToVector3(hostAnswers.NetworkQ3Blocks[i].PositionData, offSetX);
            blockPos.z = podestPos.z;
            question3BlockInstance.GetComponent<NetworkObject>().Spawn();
            question3BlockInstance.GetComponent<Q3_Results>().OnSpawnClientRpc(hostAnswers.NetworkQ3Blocks[i].SymbolData, blockPos);
        }

        question3PodestInstance = Instantiate(question3Podest);
        podestPos = ApplyOffsetToVector3(question3Explanation.transform.position, -offSetX);
        podestPos.z += 1;
        question3PodestInstance.GetComponent<NetworkObject>().Spawn();
        question3PodestInstance.GetComponent<TeleportOnSpawn>().MoveOnSpawnClientRpc(podestPos);
        for (int i = 0; i < guestAnswers.NetworkQ3Blocks.Count; i++)
        {
            var question3BlockInstance = Instantiate(question3Result);
            Vector3 blockPos = ApplyOffsetToVector3(guestAnswers.NetworkQ3Blocks[i].PositionData, -offSetX);
            blockPos.z = podestPos.z;
            question3BlockInstance.GetComponent<NetworkObject>().Spawn();
            question3BlockInstance.GetComponent<Q3_Results>().OnSpawnClientRpc(guestAnswers.NetworkQ3Blocks[i].SymbolData, blockPos);
        }
        Debug.Log("end of spawnign question3");
    }

    private void SpawnQuestion4(PlayerAnswers hostAnswers, PlayerAnswers questAnswers)
    {
        float offSetX = 1f;

        var question4Instance = Instantiate(question4Result);
        Vector3 cauldronPos = ApplyOffsetToVector3(question4Explanation.transform.position, offSetX);
        question4Instance.GetComponent<NetworkObject>().Spawn();
        question4Instance.GetComponent<Q4_Results>().OnSpawnClientRpc(hostAnswers.NetworkSexMeter.Value, true, cauldronPos);

        question4Instance = Instantiate(question4Result);
        cauldronPos = ApplyOffsetToVector3(question4Explanation.transform.position, -offSetX);
        question4Instance.GetComponent<NetworkObject>().Spawn();
        question4Instance.GetComponent<Q4_Results>().OnSpawnClientRpc(hostAnswers.NetworkSexMeter.Value, false, cauldronPos);
    }

    private Vector3 ApplyOffsetToVector3(Vector3 origin, float offSetX)
    {
        Vector3 newPos = origin;
        newPos.x += offSetX;
        return newPos;
    }


    public void CalculateQuestions(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        CalculateQuestion1(hostAnswers, guestAnswers);
        //CalculateQuestion2(hostAnswers, guestAnswers);
        CalculateQuestion3(hostAnswers, guestAnswers);
        CalculateQuestion4(hostAnswers, guestAnswers);
    }

    private void CalculateQuestion1(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
    {
        var q1InfoInstance = Instantiate(question1Explanation);
        q1InfoInstance.GetComponent<NetworkObject>().Spawn();

        Vector3 posQ1Info = q1InfoInstance.transform.position;
        posQ1Info.y += 2f;

        if (hostAnswers.NetworkQ1Answer.Value == guestAnswers.NetworkQ1Answer.Value)
        {
            UpdateResultInvokeClientRpc(question1MaxValue);
            q1InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(true);

            var miniLabyrinth = Instantiate(question1EasyMiniLabyrinth);
            miniLabyrinth.GetComponent<NetworkObject>().Spawn();
            miniLabyrinth.GetComponent<TeleportOnSpawn>().MoveOnSpawnClientRpc(posQ1Info);
        }
        else
        {
            UpdateResultInvokeClientRpc(-question1MaxValue);
            q1InfoInstance.GetComponent<Q_Info>().SetFeedBackClientRpc(false);

            var miniLabyrinth = Instantiate(question1HardMiniLabyrinth);
            miniLabyrinth.GetComponent<NetworkObject>().Spawn();
            miniLabyrinth.GetComponent<TeleportOnSpawn>().MoveOnSpawnClientRpc(posQ1Info);
        }
    }

    private void CalculateQuestion3(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
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

    private void CalculateQuestion4(PlayerAnswers hostAnswers, PlayerAnswers guestAnswers)
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
