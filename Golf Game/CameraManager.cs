using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public int CurrentVirtualCamera;
    public CinemachineVirtualCamera[] VirtualCamera;
    public CinemachineVirtualCamera AutoCamera;
    public GolfBallFollower GolfBallFollower;
    public CinemachineTargetGroup CameraTargetGroup;
    [SerializeField] private List<Transform> CameraTarget;

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < CameraTargetGroup.m_Targets.Length; i++)
        {
            CameraTarget.Add(CameraTargetGroup.m_Targets[i].target.transform);
        }

        SetTargetGroupRotation();

        for (int i = 0; i < VirtualCamera.Length; i++)
        {
            VirtualCamera[i].Priority = (i == CurrentVirtualCamera ? 10 : 0);
        }
    }

    void Update()
    {
        SetTargetGroupRotation();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetVirtualCamera(CurrentVirtualCamera + 1);
        }
    }

    public void AddCameraTarget(Transform target, float weight, float radius)
    {
        if (weight == -1) weight = 1.0f;
        if (radius == -1) radius = 3.0f;

        if (CameraTargetGroup.FindMember(target) == -1)
        {
            CameraTargetGroup.AddMember(target, weight, radius);
            CameraTarget.Add(target);
        }

        SetTargetGroupRotation();
    }

    public void RemoveAllCameraTargets()
    {
        int numberOfTargets = CameraTargetGroup.m_Targets.Length;

        for (int i = 0; i < numberOfTargets; i++)
        {
            // Debug.Log("About to remove " + 0 + ": " + CameraTargetGroup.m_Targets[0].target.name + " from Camera Target Group.");
            RemoveCameraTarget(CameraTargetGroup.m_Targets[0].target);
        }

        SetTargetGroupRotation();
    }

    public void RemoveCameraTarget(Transform target)
    {
        if (CameraTargetGroup.FindMember(target) > -1)
        {
            int index = CameraTarget.IndexOf(target);

            CameraTargetGroup.RemoveMember(target);

            // Debug.Log("Just removed " + target.name + " from Camera Target Group.");

            if (index > -1)
            {
                // CameraTarget.RemoveAt(index);
            }

            CameraTarget.Remove(target);
        }
        else
        {
            Debug.Log("Warning: " + target.name + " not found in Camera Target Group.");
        }

        SetTargetGroupRotation();
    }

    public void SetTargetGroupRotation()
    {
        if (CameraTarget.Count > 0)
        {
            CameraTargetGroup.transform.LookAt(CameraTarget[0]);
            Vector3 angles = CameraTargetGroup.transform.localEulerAngles;
            angles.x = 0;
            angles += new Vector3(0, 90.0f, 0);
            CameraTargetGroup.transform.localEulerAngles = angles;
        }
    }

    public void SetVirtualCamera(int index)
    {
        VirtualCamera[CurrentVirtualCamera].Priority = 0;

        CurrentVirtualCamera = index;

        if (CurrentVirtualCamera > (VirtualCamera.Length - 1)) CurrentVirtualCamera = 0;

        VirtualCamera[CurrentVirtualCamera].Priority = 10;
    }

    public void SetAutoCameraLookAt(Transform lookAtObject)
    {
        GolfBallFollower.LookAtObject = lookAtObject;
        // AutoCamera.LookAt = lookAtObject;
    }

    public void SetAutoCameraFollow(Transform FollowObject)
    {
        GolfBallFollower.FollowObject = FollowObject;
    }
}
