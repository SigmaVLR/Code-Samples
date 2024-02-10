using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ChallengeState { player, challenger }

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    public ChallengerAI AIScript;
    public ChallengeState CurrentState;
    public ChallengeState PreviousState;
    public bool Challenged;
    public List<ChallengerAI> AIList;
    public GameObject prefab;
    [SerializeField] GameObject Challenger;

    void Awake()
    {
        Instance = this;

        AIScript = prefab.GetComponent<ChallengerAI>();
        SetState(ChallengeState.player);
    }

    public void AddToList(ChallengerAI AI)
    {
        AIList.Add(AI);
        AI.enabled = true;
    }

    public void CreateChallenger()
    {
        Debug.Log("Creating challenger!");

        Challenged = true;
        Challenger = Instantiate(prefab, PlayManager.Instance.AimingController.DefaultPosition, Quaternion.identity);
        AIScript.SetState(AIState.holeStart);
    }

    public void SetState(ChallengeState newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;

        if (Challenged == true)
        {
            switch (CurrentState)
            {
                case ChallengeState.player:
                    Debug.Log("AIManager: changing to " + newState + ".");

                    if (ScoreManager.Instance.Scored == true && ScoreManager.Instance.ScoredAI == false)
                    {
                        PlayManager.Instance.SetState(PlayState.idle);
                        
                        SetState(ChallengeState.challenger);
                    }
                    else
                    {
                        /*
                        if (PlayManager.Instance.golfBallScript.DetectedTerrainTypeIndex != 0)
                        {
                            Debug.Log("Warning: AI needs to go to the next state.");

                            // PlayManager.Instance.SetState(PlayState.aim);
                        }
                        else
                        {
                            Debug.Log("Warning: AI needs to go to the next state.");

                            // PlayManager.Instance.SetState(PlayState.putting);
                        }
                        */
                    }
                    break;

                case ChallengeState.challenger:
                    Debug.Log("AIManager: changing to " + newState + ".");

                    if (ScoreManager.Instance.Scored == false && ScoreManager.Instance.ScoredAI == true)
                    {
                        ChallengerAI.Instance.SetState(AIState.turnEnd);
                        ChallengerAI.Instance.SetState(AIState.idle);
                        
                        SetState(ChallengeState.player);
                    }
                    else
                    {
                        AIScript.SetState(AIState.turnStart);
                    }
                    break;
            }
        }
    }

    public int GetRandomQTEResult()
    {
        int randomValue = Random.Range(1, 11);

        if (randomValue <= 2) return 0;
        if (randomValue <= 4) return 1;
        if (randomValue <= 7) return 2;

        return 3;
    }

    public int GetRandomQTESide()
    {
        return ((Random.Range(0, 2) == 0 ? -1 : 1));
    }
}
