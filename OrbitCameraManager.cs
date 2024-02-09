using UnityEngine;
using System.Collections;
using Cinemachine;
public class OrbitCameraManager : MonoBehaviour
{
    public GameObject CameraParentObject;
    public GameObject CameraObject;
    public Transform FocusTransform;
    public Collider BoundsCollider;
    public float DragTime = 1.0f;
    public bool GotValidStartPosition;
    public Vector3 dragStartMouseWorldPosition;
    public Vector3 currentMouseWorldPosition;
    public Vector3 newPosition;
    public Vector3 deltaPosition;
    public Vector3 deltaPositionThisFrame;
    public Vector3 dragStartCameraPosition;
    public Vector3 offsetPosition;
    Vector3 targetCameraParentPosition;
    public Vector3 targetCameraParentEulerAngles;
    Vector3 previousMouseScreenPosition;
    Vector3 currentMouseScreenPosition;
    float distance;
    public float CameraDistance = 23.0f;
    bool moving;
    public float MoveSpeed = 2.0f;
    public AnimationCurve MoveCurve;
    public float MinZoom = 5.0f;
    public float MaxZoom = 200.0f;
    public float ZoomSpeed = 10.0f;

    void Awake()
    {
        targetCameraParentEulerAngles = CameraParentObject.transform.localEulerAngles;
        CameraDistance = -CameraObject.transform.localPosition.z;

        newPosition = CameraParentObject.transform.position;
    }

    public void Init()
    {
        targetCameraParentPosition = FocusTransform.localPosition;
        targetCameraParentEulerAngles = CameraParentObject.transform.localEulerAngles;
        CameraDistance = -CameraObject.transform.localPosition.z;
    }

    void LateUpdate()
    {
        if (CameraManager.Instance.CurrentVirtualCamera == 0)
        {
            #region CameraControls
            if (new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
            {
                #region Move
                if (Input.GetMouseButtonDown(2))
                {
                    Plane plane = new Plane(Vector3.up, Vector3.zero);

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    float entry;

                    if (plane.Raycast(ray, out entry))
                    {
                        if (BoundsCollider.bounds.Contains(ray.GetPoint(entry)))
                        {
                            GotValidStartPosition = true;
                            dragStartMouseWorldPosition = ray.GetPoint(entry);
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButton(2) && GotValidStartPosition)
                    {
                        Plane plane = new Plane(Vector3.up, Vector3.zero);

                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        float entry;

                        if (plane.Raycast(ray, out entry))
                        {
                            currentMouseWorldPosition = ray.GetPoint(entry);
                        }

                        Vector3 tempPosition = CameraParentObject.transform.position + (dragStartMouseWorldPosition - currentMouseWorldPosition);

                        if (BoundsCollider.bounds.Contains(tempPosition))
                        {
                            newPosition = tempPosition;
                        }
                    }

                    if (Input.GetMouseButtonUp(2))
                    {
                        // Debug.Log("Releasing middle mouse button.");

                        GotValidStartPosition = false;
                    }
                }
                #endregion

                #region Rotate
                if (BuildManager.Instance.InteractionMode == BuildInteractionMode.idle)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        currentMouseScreenPosition = Input.mousePosition;
                        previousMouseScreenPosition = currentMouseScreenPosition;
                        targetCameraParentEulerAngles = CameraParentObject.transform.localEulerAngles;
                    }

                    if (targetCameraParentEulerAngles.y < 0 && CameraParentObject.transform.localEulerAngles.y >= 180.0f) targetCameraParentEulerAngles.y += 360.0f;
                    if (targetCameraParentEulerAngles.y >= 360.0f && CameraParentObject.transform.localEulerAngles.y <= 180.0f) targetCameraParentEulerAngles.y -= 360.0f;

                    if (Input.GetMouseButton(1))
                    {
                        previousMouseScreenPosition = currentMouseScreenPosition;
                        currentMouseScreenPosition = Input.mousePosition;
                        deltaPosition = currentMouseScreenPosition - previousMouseScreenPosition;

                        float deltaX = (deltaPosition.x / Screen.width) * 360.0f;
                        targetCameraParentEulerAngles.y += deltaX;

                        float deltaY = (deltaPosition.y / Screen.height) * 65.0f;
                        targetCameraParentEulerAngles.x -= deltaY;

                        targetCameraParentEulerAngles.x = Mathf.Clamp(targetCameraParentEulerAngles.x, 5.0f, 89.9999f);
                    }
                }
                #endregion

                #region Zoom
                if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) >= 0.01f)
                {
                    CameraDistance -= ZoomSpeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel");
                    if (CameraDistance < MinZoom) CameraDistance = MinZoom;
                    if (CameraDistance > MaxZoom) CameraDistance = MaxZoom;
                }
                #endregion
            }
            #endregion

            if (moving)
            {
                if (Vector3.Distance(transform.position, targetCameraParentPosition) <= 0.001f)
                {
                    moving = false;
                }
            }

            Vector3 keyboardMovementVector = GetBaseInput();

            if (keyboardMovementVector.sqrMagnitude > 0)
            {
                keyboardMovementVector *= MoveSpeed;

                if (Input.GetKey(KeyCode.LeftShift)) keyboardMovementVector *= 2.0f;

                /*
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MoveSpeed += Time.deltaTime;
                    keyboardMovementVector *= (totalRun * 250.0f);
                    keyboardMovementVector.x = Mathf.Clamp(keyboardMovementVector.x, -maxShift, maxShift);
                    keyboardMovementVector.y = Mathf.Clamp(keyboardMovementVector.y, -maxShift, maxShift);
                    keyboardMovementVector.z = Mathf.Clamp(keyboardMovementVector.z, -maxShift, maxShift);
                }
                else
                {
                    MoveSpeed = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                    keyboardMovementVector *= 100;
                }
                */

                Vector3 planarForwardVector = CameraParentObject.transform.forward;
                planarForwardVector.y = 0;
                planarForwardVector = planarForwardVector.normalized;

                Vector3 planarRightVector = CameraParentObject.transform.right;
                planarRightVector.y = 0;
                planarRightVector = planarRightVector.normalized;

                newPosition = CameraParentObject.transform.position + (planarForwardVector * keyboardMovementVector.z) + (planarRightVector * keyboardMovementVector.x);
            }
        }

        CameraParentObject.transform.position = Vector3.Lerp(CameraParentObject.transform.position, newPosition, DragTime * Time.deltaTime);
        CameraParentObject.transform.localEulerAngles = Vector3.Lerp(CameraParentObject.transform.localEulerAngles, targetCameraParentEulerAngles, 10.0f * Time.deltaTime);
        CameraObject.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(CameraObject.transform.localPosition.z, -CameraDistance, 5.0f * Time.deltaTime));
    }

    public void MoveCamera(Vector3 targetPos)
    {
        CameraParentObject.transform.position = targetPos;
        // CameraParentObject.transform.position = Vector3.MoveTowards(CameraParentObject.transform.position, targetPos, 1000);
    }

    private Vector3 GetBaseInput()
    {
        Vector3 keyboardMovementVector = new Vector3();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            keyboardMovementVector += new Vector3(0, 0, 1);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            keyboardMovementVector += new Vector3(0, 0, -1);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            keyboardMovementVector += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            keyboardMovementVector += new Vector3(1, 0, 0);
        }

        return keyboardMovementVector.normalized;
    }
}