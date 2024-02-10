using PathCreation;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AimingController : MonoBehaviour
{
    // public static AimingController Instance;

    public Match Match;
    public bool IsActive;
    public bool BallIsFlying;
    public bool IsHit;
    public bool BallIsStopped;
    public GameObject Character;
    public int Texture;
    public bool Finished = false;

    public Vector3 StartPlaneNormal = Vector3.up;
    public Vector3 StartPoint;
    public Vector3 StartPointAI;
    public Vector3 DefaultPosition;

    public float MaxDistance = 10;
    public float CurrentDistance;
    public float BallTimer = 0.0f;
    public float BallTimeInTheAir = 5.0f;

    public GameObject GolfBall;
    public GolfBall currentGolfBallScript;
    public Transform Arrow;

    private Plane _startPlane;
    [SerializeField] LayerMask AimLayerMask;
    private BezierPath _curve;
    private BezierPath _line;
    [SerializeField] LineRenderer _lineRender;
    public AnimationCurve FlightCurve;
    public Coroutine EndPointCoroutine = null;

    [SerializeField] Transform _curveStartPoint;
    [SerializeField] Transform _curveMidPoint;
    [SerializeField] Transform _curveEndPoint;
    Vector3 endPointNormal;
    [SerializeField] Transform GolfBallStartpoint;
    [SerializeField] Transform GolfBallMidpoint;
    public Transform GolfBallTarget;
    [SerializeField] DecalProjector GolfBallTargetProjector;
    public Transform TargetAreaTransform;
    public TargetAreaManager TargetArea;
    public DecalProjector TargetAreaDecalProjector;
    public Transform TargetAreaCenter;
    bool targetVisible;
    bool targetAreaVisible;

    // to-tweak: the ratio the mid-high curve point should be when compared to max dist
    [SerializeField] private float _heightRatio = 0.5f;

    private void Awake()
    {
        // Instance = this;

        if (GolfBall != null) currentGolfBallScript = GolfBall.GetComponent<GolfBall>();

        SetLineRendererActive(IsActive);
        SetTargetActive(false);
        SetTargetAreaActive(false);
        SetArrowActive(false);

        StartPoint = transform.position;
        DefaultPosition = StartPoint;

        _startPlane = new Plane(StartPlaneNormal, StartPoint);

        _curveStartPoint.position = StartPoint;
        _curveEndPoint.position = StartPoint + (transform.forward * MaxDistance);

        float distance = (_curveEndPoint.position - _curveStartPoint.position).magnitude;

        _curveMidPoint.position = StartPoint + (transform.forward * (distance / 2)) + new Vector3(0, (distance * _heightRatio), 0);

        UpdateCurve();
        DrawCurve();
    }

    public void SetMatch(Match match)
    {
        Match = match;

        currentGolfBallScript = match.Players[0].GolfBall;
        GolfBall = currentGolfBallScript.gameObject;
    }

    public void StartAiming(Character character)
    {
        // Debug.Log(character.name + ": start aiming.");

        IsActive = true;

        SetTargetActive(character.Type == PlayerType.human);
        SetArrowActive(character.Type == PlayerType.human);

        MaxDistance = (character.CurrentCarry + character.CurrentCarryOffset);

        float terrainDebuffModifier = TerrainPhysics.Instance.GetTerrainTypeRecoveryModifier(RecoveryInfluenceType.MaxCarry, character, character.CurrentClubIndex);

        if (PhysicsManager.Instance.UseTerrainDebuffs)
        {
            if (ClubManager.Instance.AllowAllClubs && terrainDebuffModifier < 0) terrainDebuffModifier = 1.0f;

            // Debug.Log("MaxDistance: " + MaxDistance + ". With terrainDebuffModifier " + terrainDebuffModifier + " -> " + (MaxDistance * terrainDebuffModifier) + ".");

            MaxDistance *= terrainDebuffModifier;
        }
    }

    private void Update()
    {
        if (Match == null || Match.Type != MatchType.aiVSai)
        {
            if (IsActive && PlayManager.Instance.CurrentPlayerMatch.IsCurrentPlayerHuman())
            {
                if (!UIManager.Instance.CursorOverUI)
                {
                    if (!targetVisible) SetTargetActive(true);

                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(mouseRay, out hit, 1000.0f, AimLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        // Debug.Log("Raycast target: " + hit.collider.gameObject.name + ".");

                        endPointNormal = hit.normal;

                        UpdateEndPoint(hit.point, true);
                        UpdateCurve();
                    }

                    UpdateCurrentDistance();

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlayManager.Instance.CurrentPlayerMatch.SetState(MatchState.power);
                    }
                }
                else
                {
                    if (targetVisible) SetTargetActive(false);
                }
            }
        }
    }

    public void UpdateStartPoint(GameObject currentGolfBall)
    {
        currentGolfBallScript = currentGolfBall.GetComponent<GolfBall>();

        StartPoint = currentGolfBallScript.BallPosition;
        // Debug.Log("Updating Start Point to " + currentGolfBall + " position: " + StartPoint.ToString() + ".");

        _curveStartPoint.position = StartPoint;
        if (GolfBallStartpoint != null) GolfBallStartpoint.transform.position = StartPoint;

        if (Arrow != null) Arrow.position = StartPoint;
    }

    public void UpdateEndPoint(Vector3 targetPosition, bool useLimits)
    {
        if (!IsHit)
        {
            // To do: add Power stat influence, based on club type.

            Vector3 directionVector = (targetPosition - _curveStartPoint.position).normalized;
            var ratio = (_curveEndPoint.position - _curveStartPoint.position).sqrMagnitude / (MaxDistance * MaxDistance);

            float actualDistance = (targetPosition - _curveStartPoint.position).magnitude;

            _curveEndPoint.position = targetPosition;

            if (useLimits && actualDistance > MaxDistance)
            {
                _curveEndPoint.position = _curveStartPoint.position + (directionVector * MaxDistance);

                RaycastHit hit;

                if (Physics.Raycast((_curveEndPoint.position + new Vector3(0, 100.0f, 0)), Vector3.down, out hit, 1000.0f, AimLayerMask, QueryTriggerInteraction.Ignore))
                {
                    _curveEndPoint.position = hit.point;
                    endPointNormal = hit.normal;
                    UpdateCurve();
                }
            }

            // Debug.Log("Updating end position: " + _curveEndPoint.position.ToString() + ".");
            // Debug.Log("curveStartposition: " + _curveStartPoint.position.ToString() + ".");

            _curveMidPoint.position = StartPoint + ((_curveEndPoint.position - _curveStartPoint.position) / 2) + (MaxDistance * ratio * _heightRatio * _startPlane.normal);

            if (GolfBallMidpoint != null) GolfBallMidpoint.transform.position = _curveMidPoint.position;

            if (GolfBallTarget != null)
            {
                GolfBallTarget.position = _curveEndPoint.position;
                GolfBallTarget.rotation = Quaternion.FromToRotation(Vector3.up, endPointNormal);
            }

            SetBallCurvePositions();

            if (Arrow != null)
            {
                Arrow.LookAt(GolfBallTarget.transform.position);
                Vector3 tempVector = Arrow.eulerAngles;
                tempVector.x = -Match.Players[Match.CurrentPlayerIndex].CurrentLoft;
                Arrow.eulerAngles = tempVector;
            }
        }
        else
        {
            Debug.Log("Already hit!");

            return;
        }
    }

    public void UpdateCurrentDistance()
    {
        CurrentDistance = (_curveEndPoint.position - _curveStartPoint.position).magnitude;
    }

    public void AutoTargetHole()
    {
        UpdateEndPoint(PlayManager.Instance.GetHolePosition(Match), false);
        SetTargetActive(true);
    }

    public void UpdateCurve()
    {
        Vector3[] curvePoints = new[] { _curveStartPoint.position, _curveMidPoint.position, _curveEndPoint.position };
        _curve = new BezierPath(curvePoints);
    }

    public void DrawCurve()
    {
        VertexPath path = new VertexPath(_curve, gameObject.transform);
        _lineRender.positionCount = path.NumPoints;
        _lineRender.SetPositions(path.localPoints);
    }

    public void SetLineRendererActive(bool active)
    {
        // _lineRender.gameObject.SetActive(active);

        _lineRender.gameObject.SetActive(false);
    }

    public void SetTargetActive(bool active)
    {
        if (GolfBallTarget != null) GolfBallTarget.gameObject.SetActive(active);
        targetVisible = active;
    }

    public void SetTargetAreaActive(bool active)
    {
        if (active)
        {
            UpdateTargetArea();
        }

        if (TargetAreaTransform != null) TargetAreaTransform.gameObject.SetActive(active);
        targetAreaVisible = active;

        if (active)
        {
            if (TargetAreaDecalProjector != null) TargetAreaDecalProjector.enabled = false;
            if (GolfBallTargetProjector != null)
            {
                GolfBallTargetProjector.enabled = false;
                // Debug.Log("GolfBallTargetProjector: character type = " + Match.Players[Match.CurrentPlayerIndex].Type.ToString() + ".");

                if (Match.Players[Match.CurrentPlayerIndex].Type == PlayerType.human)
                {
                    GolfBallTargetProjector.enabled = true;
                }
            }
            // TargetAreaDecalProjector.enabled = true;
        }
    }

    public void UpdateTargetArea()
    {
        if (TargetAreaTransform != null)
        {
            TargetAreaTransform.position = GolfBallTarget.position;
            TargetAreaTransform.rotation = GolfBallTarget.rotation;

            Vector3 planarGolfBallStartpoint = GolfBallStartpoint.position;
            planarGolfBallStartpoint.y = TargetAreaTransform.position.y;
            TargetAreaCenter.LookAt(planarGolfBallStartpoint);
            float yAngle = TargetAreaCenter.eulerAngles.y + 180.0f;
            if (yAngle >= 360.0f) yAngle -= 360.0f;
            TargetAreaCenter.localEulerAngles = new Vector3(0, yAngle, 0);
            Vector3 TargetAreaDecalProjectorAngles = TargetAreaDecalProjector.transform.localEulerAngles;
            TargetAreaDecalProjectorAngles.y = yAngle;
            TargetAreaDecalProjector.transform.localEulerAngles = TargetAreaDecalProjectorAngles;
        }
    }

    public void SetTargetAreaProjectionRadius(float powerRadius, float accuracyRadius)
    {
        powerRadius *= 2.0f;
        accuracyRadius *= 2.0f;
        Vector3 originalSize = TargetAreaDecalProjector.size;
        TargetAreaDecalProjector.size = new Vector3(accuracyRadius, accuracyRadius, originalSize.z);
    }

    public void SetArrowActive(bool active)
    {
        if (Arrow != null) Arrow.gameObject.SetActive(active);
    }

    void SetBallCurvePositions()
    {
        currentGolfBallScript.StartPosition = _curveStartPoint.position;
        currentGolfBallScript.MidPosition = _curveMidPoint.position;
        currentGolfBallScript.TargetPosition = _curveEndPoint.position;
    }

    public void HitBall(GameObject TargetBall)
    {
        currentGolfBallScript = TargetBall.GetComponent<GolfBall>();

        SetLineRendererActive(false);
        SetTargetActive(false);

        BallTimer = 0.0f;
        BallIsFlying = true;

        TargetBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        TargetBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        SetBallCurvePositions();

        Vector3 tempEnd = _curveEndPoint.position;
        tempEnd.y = _curveStartPoint.position.y;

        // Debug.Log("Distance = " + (tempEnd - _curveStartPoint.position).magnitude);

        currentGolfBallScript.StartFlight();
    }

    public void StopBall(GameObject TargetBall)
    {
        Debug.Log("Stopping ball 2.");

        UpdateStartPoint(TargetBall);
        BallIsStopped = currentGolfBallScript.BallStopActivated;
        TerrainDetector.Instance.GetDominantTextureIndexAt(currentGolfBallScript.BallPosition);
        Texture = TerrainDetector.Instance.mostDominantTextureIndex;
    }

    public void ResetGolf(GameObject TargetBall)
    {
        StartPoint = DefaultPosition;
        _curveStartPoint.position = DefaultPosition;
        if (Arrow != null) Arrow.position = DefaultPosition;
        _heightRatio = 0.5f;

        if (TargetBall != null)
        {
            // Debug.Log("Reset golf." + TargetBall);
            currentGolfBallScript = TargetBall.GetComponent<GolfBall>();
            currentGolfBallScript.BallPosition = DefaultPosition;
            currentGolfBallScript.ResetGolfBallPhysics();
            currentGolfBallScript.SetGolfBallVisibility(true);
            currentGolfBallScript.ResetGolfBallTrail();
            currentGolfBallScript.scored = false;

            Character character = currentGolfBallScript.Owner;

            TerrainPhysics.Instance.SetType(character, - 1);
            ClubManager.Instance.SetClub(character, 0);
            ClubManager.Instance.AutoSelectClub(character);
            character.ShotWillMiss = false;
        }
    }

    public void StopAiming()
    {
        // Debug.Log("Stop aiming.");

        IsActive = false;
        SetArrowActive(false);
    }

    public void StopAimingCompletely()
    {
        SetTargetActive(false);
        SetTargetAreaActive(false);
        PhysicsManager.Instance.SetPowerOffsetObjectVisibility(TargetArea, false);
        PhysicsManager.Instance.SetPowerQTEOffsetObjectVisibility(TargetArea, false);
        PhysicsManager.Instance.SetAccuracyOffsetObjectVisibility(TargetArea, false);
        PhysicsManager.Instance.SetAccuracyQTEOffsetObjectVisibility(TargetArea, false);
        PhysicsManager.Instance.SetPuttingOffsetObjectVisibility(TargetArea, false);
    }
}
