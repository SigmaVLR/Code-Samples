using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEUpdated : MonoBehaviour
{
    public QTEBarType QTEBarType;
    public string Title;
    public TextMeshProUGUI TitleText;
    public int direction;
    public Transform MarkerParent;
    public Transform PowerMarkerParent;
    public Transform AccuracyMarkerParent;

    float moveSpeed = .5f;
    public float CurrentAngle;
    Vector3 markerParentAngles;
    float distanceToTarget;
    public int PowerQTEResult;
    public int AccuracyQTEResult;
    public string[] QTEResultText;
    public string[] QTEExtraPowerResultText;
    [SerializeField] int PowerQTESide = 1;
    [SerializeField] int AccuracyQTESide = 1;
    public bool Clicked;
    public bool TurnFinished;
    float time;
    Coroutine continueTimer;

    void OnEnable()
    {
        time = 0;
        QTEBarType = QTEBarType.power;

        if (continueTimer != null)
        {
            StopCoroutine(continueTimer);
            continueTimer = null;
        }

        direction = 1;
        PowerQTESide = 1;
        AccuracyQTESide = 1;
        PowerQTEResult = 0;
        AccuracyQTEResult = 0;
        Clicked = false;
        TurnFinished = false;

        PowerMarkerParent.gameObject.SetActive(false);
        AccuracyMarkerParent.gameObject.SetActive(false);

        if (TitleText != null) TitleText.text = Title;

        if (QTEBarType == QTEBarType.accuracy && PlayManager.Instance.CurrentPlayerMatch.CurrentState != MatchState.accuracy)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if ((QTEBarType == QTEBarType.power && PlayManager.Instance.CurrentPlayerMatch.CurrentState == MatchState.power) || (QTEBarType == QTEBarType.accuracy && PlayManager.Instance.CurrentPlayerMatch.CurrentState == MatchState.accuracy))
        {
            if (!Clicked)
            {
                // Code to move the bar back and forth.

                time += Time.deltaTime * moveSpeed;

                if (time >= 1.0f) time = 1.0f;

                if (direction == 1)
                {
                    CurrentAngle = Mathf.Lerp(0, 270.0f, time);
                }

                if (direction == -1)
                {
                    CurrentAngle = Mathf.Lerp(270.0f, 0, time);
                }

                markerParentAngles.z = -CurrentAngle;
                MarkerParent.localEulerAngles = markerParentAngles;

                // Code for click handling.
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    switch (QTEBarType)
                    {
                        case QTEBarType.power:
                            if (CurrentAngle >= 55.0f)
                            {
                                PowerMarkerParent.localRotation = MarkerParent.localRotation;
                                PowerMarkerParent.gameObject.SetActive(true);

                                PowerQTESide = (CurrentAngle < 235 ? -1 : 1);
                                PowerQTEResult = GetPowerQTEResult();

                                // Debug.Log("QTEUpdated: PowerQTESide == " + PowerQTESide.ToString() + ", PowerQTEResult == " + PowerQTEResult.ToString() + ".");

                                ApplyQTEInfluences();

                                direction = -1;
                                time = (1.0f - time);

                                QTEBarType = QTEBarType.accuracy;
                            }
                            break;

                        case QTEBarType.accuracy:
                            if (CurrentAngle <= 55.0f)
                            {
                                AccuracyMarkerParent.localRotation = MarkerParent.localRotation;
                                AccuracyMarkerParent.gameObject.SetActive(true);

                                AccuracyQTESide = (CurrentAngle <= 27.5 ? 1 : -1);
                                AccuracyQTEResult = GetAccuracyQTEResult();
                                ApplyQTEInfluences();
                                Clicked = true;

                                TurnFinished = true;
                            }
                            break;
                    }

                    /*
                    if (QTESide == 1)
                    {   
                        TitleText.text += ": " + QTEExtraPowerResultText[PQTEResult];
                        //Debug.Log("Extra Power");
                    }
                    else
                    {
                        TitleText.text += ": " + QTEResultText[PQTEResult];
                        //Debug.Log("Regular Power");
                    }
                    */

                }

                // Out of time.
                if (time == 1.0f)
                {
                    switch (QTEBarType)
                    {
                        case QTEBarType.power:
                            // Debug.Log("Failed Power QTE");
                            markerParentAngles.z = -55.0f;
                            PowerMarkerParent.localEulerAngles = markerParentAngles;
                            PowerMarkerParent.gameObject.SetActive(true);

                            PowerQTESide = -1;
                            PowerQTEResult = 3;
                            ApplyQTEInfluences();

                            time = 0;
                            direction = -1;

                            QTEBarType = QTEBarType.accuracy;
                            break;

                        case QTEBarType.accuracy:
                            // Debug.Log("Failed Accuracy QTE");
                            AccuracyQTESide = (Random.Range(0, 2) == 0 ? -1 : 1);
                            AccuracyQTEResult = 3;
                            ApplyQTEInfluences();

                            markerParentAngles.z = (AccuracyQTESide == -1 ? -55.0f : 0);
                            AccuracyMarkerParent.localEulerAngles = markerParentAngles;
                            AccuracyMarkerParent.gameObject.SetActive(true);

                            TurnFinished = true;
                            break;
                    }
                }
            }
        }
    }

    int GetPowerQTEResult()
    {
        switch (PowerQTESide)
        {
            case -1:
                if (CurrentAngle >= 207)
                {
                    return 0;
                }
                else
                {
                    if (CurrentAngle >= 153)
                    {
                        return 1;
                    }
                    else
                    {
                        if (CurrentAngle >= 99)
                        {
                            return 2;
                        }
                    }
                }

                return 3;

            case 1:
                if (CurrentAngle >= 262)
                {
                    return 3;
                }
                else
                {
                    if (CurrentAngle >= 253)
                    {
                        return 2;
                    }
                    else
                    {
                        if (CurrentAngle >= 244)
                        {
                            return 1;
                        }
                    }
                }

                return 0;
        }

        return (PowerQTESide == -1 ? 3 : 0);
    }

    int GetAccuracyQTEResult()
    {
        // Debug.Log("CurrentAngle: " + CurrentAngle.ToString() + ".");

        switch (AccuracyQTESide)
        {
            case -1:
                if (CurrentAngle <= 31.35f)
                {
                    return 0;
                }
                else
                {
                    if (CurrentAngle <= 39.25f)
                    {
                        return 1;
                    }
                    else
                    {
                        if (CurrentAngle <= 47.1f)
                        {
                            return 2;
                        }
                    }
                }

                return 3;

            case 1:
                if (CurrentAngle >= 23.5f)
                {
                    return 0;
                }
                else
                {
                    if (CurrentAngle >= 15.7f)
                    {
                        return 1;
                    }
                    else
                    {
                        if (CurrentAngle >= 7.85f)
                        {
                            return 2;
                        }
                    }
                }

                return 3;
        }

        return 3;
    }

    public void ApplyQTEInfluences()
    {
        // Debug.Log("Power Result: " + PowerQTEResult + ", Side: " + PowerQTESide);
        // Debug.Log("Accuracy Result: " + AccuracyQTEResult + ", Side: " + AccuracyQTESide);

        switch (QTEBarType)
        {
            case QTEBarType.power:
                PlayManager.Instance.GetCurrentPlayerMatchPlayer().PowerQTEResult = PowerQTEResult;
                PlayManager.Instance.GetCurrentPlayerMatchPlayer().PowerQTESide = PowerQTESide;
                PhysicsManager.Instance.SetPowerInfluence(PlayManager.Instance.GetCurrentPlayerMatchPlayer());
                PlayManager.Instance.CurrentPlayerMatch.SetState(MatchState.accuracy);
                break;

            case QTEBarType.accuracy:
                PlayManager.Instance.GetCurrentPlayerMatchPlayer().AccuracyQTEResult = AccuracyQTEResult;
                PlayManager.Instance.GetCurrentPlayerMatchPlayer().AccuracyQTESide = AccuracyQTESide;
                PhysicsManager.Instance.SetAccuracyInfluence(PlayManager.Instance.GetCurrentPlayerMatchPlayer());
                break;
        }

        if (QTEBarType == QTEBarType.accuracy)
        {
            PlayManager.Instance.CurrentPlayerMatch.SetState(MatchState.inFlight);
        }
    }
}