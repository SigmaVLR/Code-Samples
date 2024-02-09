using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState { idle, walkToQueue, holeStart, walkToBall, aim, inFlight, turnStart, turnEnd, holeEnd, }

public class ChallengerAI : MonoBehaviour
{
    public static ChallengerAI Instance;

    public AIState CurrentState;
    public AIState PreviousState;
    public AimingController AimingController;
    public Transform goal;
    [SerializeField]Vector3 startpoint;
    public GameObject AIBall;
    public GolfBall golfBallScript;
    public NavMeshAgent agent;
    public int CurrentMarker;
    [SerializeField] float CurrentDistance;
    [SerializeField] float goalDistance;
    [SerializeField] float RandomPoint;
    [SerializeField] bool playing;
    [SerializeField] bool aiming;
    [SerializeField] bool scored;

    private void Awake()
    {
        Instance = this;

        CurrentMarker = -1;
        AIBall = Instantiate(AimingController.GolfBall);
        AIBall.name = "AIBall";
        golfBallScript = AIBall.GetComponent<GolfBall>();
        AIBall.transform.position = startpoint;

        goal = PlayManager.Instance.Holes[PlayManager.Instance.CurrentHole].HoleTransform;
        AIManager.Instance.AddToList(this);
        RandomizePoint();

        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        AimingController.ResetGolf(AIBall);
        SetState(AIState.idle);
    }

    void Start()
    {
        // controller.SetTargetActive(true);
    }

    void Update()
    {
        CurrentDistance = Vector3.Distance(agent.transform.position, startpoint);
        goalDistance = agent.remainingDistance;

        if (AIManager.Instance.CurrentState != ChallengeState.challenger)
        {
            SetState(AIState.idle);
            aiming = true;
        }
        else if ((AIManager.Instance.CurrentState == ChallengeState.challenger))
        {
            if (aiming == true)
            {
                SetState(AIState.aim);
            }

            if (agent.remainingDistance <= 0 && playing == false)
            {
                SetState(AIState.inFlight);

            }

        }
    }

    public void AimPoint()
    {
        //Debug.Log("Hitting the ball");
        // PhysicsManager.Instance.ApplyInfluences();
        AimingController.UpdateStartPoint(AIBall);
        AimingController.UpdateEndPoint(agent.transform.position, false);
        AimingController.HitBall(AIBall);
        playing = true;
    }

    public void RandomizePoint()
    {
        RandomPoint = Random.Range(AimingController.MaxDistance - 20, AimingController.MaxDistance);
    }

    public void SetState(AIState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case AIState.idle:
                agent.isStopped = true;
                break;

            case AIState.holeStart:
                ScoreManager.Instance.ScoredAI = false;
                CurrentMarker = -1;
                // ClubManager.Instance.SetClub(0);
                Debug.Log("Changing state: Hole Start");
                AimingController.SetTargetActive(true);
                AIBall.transform.position = PlayManager.Instance.Holes[PlayManager.Instance.CurrentHole].GolfBallStart.position;
                startpoint = AIBall.transform.position;
                golfBallScript.BallPosition = startpoint;
                aiming = false;
                PreviousState = CurrentState;
                CurrentState = newState;
                break;

            case AIState.inFlight:
                Debug.Log("Changing state: In Flight");
                agent.isStopped = true;
                AimPoint();
                PreviousState = CurrentState;
                CurrentState = newState;
                break;

            case AIState.aim:
                Debug.Log("Changing state: Aim");
                CameraManager.Instance.SetAutoCameraFollow(AIBall.transform);
                // TerrainPhysics.Instance.SetType(golfBallScript.DetectedTerrainTypeIndex);
                StartCoroutine(SetAIposition());

                if (CurrentMarker <= 0)
                {
                    // TerrainPhysics.Instance.SetType(-1);
                    // ClubManager.Instance.SetClub(0);
                }
                else if (TerrainPhysics.Instance.CurrentTerrainType == TerrainType.fairway)
                {
                    // ClubManager.Instance.SetClub(1);
                }
                else
                {
                    // ClubManager.Instance.AutoSelectClub();
                }
                break;

            case AIState.holeEnd:
                goal.position = PlayManager.Instance.Holes[PlayManager.Instance.CurrentHole].Waymarks[CurrentMarker].transform.position;
                agent.destination = goal.position;
                Debug.Log("Changing state: Hole End");
                PreviousState = CurrentState;
                CurrentState = newState;
                break;

            case AIState.turnEnd:
                CameraManager.Instance.SetAutoCameraFollow(PlayManager.Instance.PlayerBall.transform);
                startpoint = golfBallScript.BallPosition;   
                break;

            case AIState.turnStart:
                Debug.Log("Starting Turn");

                PreviousState = CurrentState;
                CurrentState = newState;
                break;
        }
    }

    public IEnumerator SetAIposition()
    {
        Debug.Log("coroutine is still running");
        PlayManager.Instance.CurrentPlayerMatch.GoToNextMarker(CurrentMarker + 1);
        agent.transform.position = startpoint;
        RandomizePoint();
        agent.isStopped = false;
        aiming = false;
        yield return new WaitForSeconds(1f);
        playing = false;
    }
}