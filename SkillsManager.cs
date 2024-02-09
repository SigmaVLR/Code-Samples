using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat { Power, Accuracy, Fade, Draw, Recovery, Focus }

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;
    public int SkillPoints;
    public int PointsPerClick = 25;

    public int WoodPower;
    public int WoodAccuracy;
    public int WoodFade;
    public int WoodDraw;
    public int WoodRecovery;
    public int WoodFocus;

    public int IronPower;
    public int IronAccuracy;
    public int IronFade;
    public int IronDraw;
    public int IronRecovery;
    public int IronFocus;

    public int PuttPower;
    public int PuttAccuracy;
    public int PuttFade;
    public int PuttDraw;
    public int PuttRecovery;
    public int PuttFocus;

    StatText statText;

    void Awake()
    {
        Instance = this;

        statText = gameObject.GetComponent<StatText>();
    }

    public void Increase(Stats increaseStats)
    {
        switch (increaseStats)
        {
#region "Wood Switch"
            case Stats.WoodPower:
                WoodPower = SpendPoints(WoodPower);
                break;
            case Stats.WoodAccuracy:
                WoodAccuracy = SpendPoints(WoodAccuracy);
                break;
            case Stats.WoodFade:
                WoodFade = SpendPoints(WoodFade);
                break;
            case Stats.WoodDraw:
                WoodDraw = SpendPoints(WoodDraw);
                break;
            case Stats.WoodRecovery:
                WoodRecovery = SpendPoints(WoodRecovery);
                break;
            case Stats.WoodFocus:
                WoodFocus = SpendPoints(WoodFocus);
                break;
#endregion

#region "Iron Switch"
            case Stats.IronPower:
                IronPower = SpendPoints(IronPower);
                break;
            case Stats.IronAccuracy:
                IronAccuracy = SpendPoints(IronAccuracy);
                break;
            case Stats.IronFade:
                IronFade = SpendPoints(IronFade);
                break;
            case Stats.IronDraw:
                IronDraw = SpendPoints(IronDraw);
                break;
            case Stats.IronRecovery:
                IronRecovery = SpendPoints(IronRecovery);
                break;
            case Stats.IronFocus:
                IronFocus = SpendPoints(IronFocus);
                break;
            #endregion

#region "Putter Switch"
            case Stats.PuttPower:
                PuttPower = SpendPoints(PuttPower);
                break;
            case Stats.PuttAccuracy:
                PuttAccuracy = SpendPoints(PuttAccuracy);
                break;
            case Stats.PuttFade:
                PuttFade = SpendPoints(PuttFade);
                break;
            case Stats.PuttDraw:
                PuttDraw = SpendPoints(PuttDraw);
                break;
            case Stats.PuttRecovery:
                PuttRecovery = SpendPoints(PuttRecovery);
                break;
            case Stats.PuttFocus:
                PuttFocus = SpendPoints(PuttFocus);
                break;
#endregion
        }

        statText.SetSkillPointTexts();
    }

    public void Decrease(Stats decreaseStats)
    {
        switch (decreaseStats)
        {
            #region "Wood Switch"
            case Stats.WoodPower:
                WoodPower = RestorePoints(WoodPower);
                break;
            case Stats.WoodAccuracy:
                WoodAccuracy = RestorePoints(WoodAccuracy);
                break;
            case Stats.WoodFade:
                WoodFade = RestorePoints(WoodFade);
                break;
            case Stats.WoodDraw:
                WoodDraw = RestorePoints(WoodDraw);
                break;
            case Stats.WoodRecovery:
                WoodRecovery = RestorePoints(WoodRecovery);
                break;
            case Stats.WoodFocus:
                WoodFocus = RestorePoints(WoodFocus);
                break;
            #endregion

            #region "Iron Switch"
            case Stats.IronPower:
                IronPower = RestorePoints(IronPower);
                break;
            case Stats.IronAccuracy:
                IronAccuracy = RestorePoints(IronAccuracy);
                break;
            case Stats.IronFade:
                IronFade = RestorePoints(IronFade);
                break;
            case Stats.IronDraw:
                IronDraw = RestorePoints(IronDraw);
                break;
            case Stats.IronRecovery:
                IronRecovery = RestorePoints(IronRecovery);
                break;
            case Stats.IronFocus:
                IronFocus = RestorePoints(IronFocus);
                break;
            #endregion

            #region "Putter Switch"
            case Stats.PuttPower:
                PuttPower = RestorePoints(PuttPower);
                break;
            case Stats.PuttAccuracy:
                PuttAccuracy = RestorePoints(PuttAccuracy);
                break;
            case Stats.PuttFade:
                PuttFade = RestorePoints(PuttFade);
                break;
            case Stats.PuttDraw:
                PuttDraw = RestorePoints(PuttDraw);
                break;
            case Stats.PuttRecovery:
                PuttRecovery = RestorePoints(PuttRecovery);
                break;
            case Stats.PuttFocus:
                PuttFocus = RestorePoints(PuttFocus);
                break;
                #endregion
        }

        statText.SetSkillPointTexts();
    }

    int SpendPoints(int statValue)
    {
        if (SkillPoints >= PointsPerClick && statValue <= (100 - PointsPerClick))
        {
            SkillPoints -= PointsPerClick;
            statValue += PointsPerClick;
        }

        return statValue;
    }

    int RestorePoints(int statValue)
    {
        if (statValue >= PointsPerClick)
        {
            statValue -= PointsPerClick;
            SkillPoints += PointsPerClick;
        }

        return statValue;
    }

    void DecreaseSkillPoints()
    {
        SkillPoints -= PointsPerClick;
    }

    public void AddItemIncrease(Stats ItemIncreaseStats, Items item)
    {
        switch (ItemIncreaseStats)
        {
            #region "Wood Switch"
            case Stats.WoodPower:
                WoodPower += item.IncreaseValue;
                break;
            case Stats.WoodAccuracy:
                WoodAccuracy += item.IncreaseValue;
                break;
            case Stats.WoodFade:
                WoodFade += item.IncreaseValue;
                break;
            case Stats.WoodDraw:
                WoodDraw += item.IncreaseValue;
                break;
            case Stats.WoodRecovery:
                WoodRecovery += item.IncreaseValue;
                break;
            case Stats.WoodFocus:
                WoodFocus += item.IncreaseValue;
                break;
            #endregion

            #region "Iron Switch"
            case Stats.IronPower:
                IronPower += item.IncreaseValue;
                break;
            case Stats.IronAccuracy:
                IronAccuracy += item.IncreaseValue;
                break;
            case Stats.IronFade:
                IronFade += item.IncreaseValue;
                break;
            case Stats.IronDraw:
                IronDraw += item.IncreaseValue;
                break;
            case Stats.IronRecovery:
                IronRecovery += item.IncreaseValue;
                break;
            case Stats.IronFocus:
                IronFocus += item.IncreaseValue;
                break;
            #endregion

            #region "Putter Switch"
            case Stats.PuttPower:
                PuttPower += item.IncreaseValue;
                break;
            case Stats.PuttAccuracy:
                PuttAccuracy += item.IncreaseValue;
                break;
            case Stats.PuttFade:
                PuttFade += item.IncreaseValue;
                break;
            case Stats.PuttDraw:
                PuttDraw += item.IncreaseValue;
                break;
            case Stats.PuttRecovery:
                PuttRecovery += item.IncreaseValue;
                break;
            case Stats.PuttFocus:
                PuttFocus += item.IncreaseValue;
                break;
                #endregion
        }

        statText.SetSkillPointTexts();
    }

    public void RemoveItemIncrease(Stats ItemIncreaseStats, Items item)
    {
        switch (ItemIncreaseStats)
        {
            #region "Wood Switch"
            case Stats.WoodPower:
                WoodPower -= item.IncreaseValue;
                break;
            case Stats.WoodAccuracy:
                WoodAccuracy -= item.IncreaseValue;
                break;
            case Stats.WoodFade:
                WoodFade -= item.IncreaseValue;
                break;
            case Stats.WoodDraw:
                WoodDraw -= item.IncreaseValue;
                break;
            case Stats.WoodRecovery:
                WoodRecovery -= item.IncreaseValue;
                break;
            case Stats.WoodFocus:
                WoodFocus -= item.IncreaseValue;
                break;
            #endregion

            #region "Iron Switch"
            case Stats.IronPower:
                IronPower -= item.IncreaseValue;
                break;
            case Stats.IronAccuracy:
                IronAccuracy -= item.IncreaseValue;
                break;
            case Stats.IronFade:
                IronFade -= item.IncreaseValue;
                break;
            case Stats.IronDraw:
                IronDraw -= item.IncreaseValue;
                break;
            case Stats.IronRecovery:
                IronRecovery -= item.IncreaseValue;
                break;
            case Stats.IronFocus:
                IronFocus -= item.IncreaseValue;
                break;
            #endregion

            #region "Putter Switch"
            case Stats.PuttPower:
                PuttPower -= item.IncreaseValue;
                break;
            case Stats.PuttAccuracy:
                PuttAccuracy -= item.IncreaseValue;
                break;
            case Stats.PuttFade:
                PuttFade -= item.IncreaseValue;
                break;
            case Stats.PuttDraw:
                PuttDraw -= item.IncreaseValue;
                break;
            case Stats.PuttRecovery:
                PuttRecovery -= item.IncreaseValue;
                break;
            case Stats.PuttFocus:
                PuttFocus -= item.IncreaseValue;
                break;
                #endregion
        }

        statText.SetSkillPointTexts();
    }

    public int GetStat(Stat stat, Character character)
    {
        int statValue = 0;

        ClubCategory clubCategory = character.CurrentClubCategory;

        switch(stat)
        {
            case Stat.Power:
                if (clubCategory == ClubCategory.wood) statValue = WoodPower;
                if (clubCategory == ClubCategory.iron) statValue = IronPower;
                if (clubCategory == ClubCategory.putter) statValue = PuttPower;
                break;

            case Stat.Accuracy:
                if (clubCategory == ClubCategory.wood) statValue = WoodAccuracy;
                if (clubCategory == ClubCategory.iron) statValue = IronAccuracy;
                if (clubCategory == ClubCategory.putter) statValue = PuttAccuracy;
                break;
        }

        if (character.Type == PlayerType.ai)
        {
            statValue += 50;

            if (statValue > 100) statValue = 100;

            // Debug.Log("Getting " + stat.ToString() + " stat for " + character.gameObject.name + ", which is an AI. Adding +50% -> " + statValue + ".");
        }

        return statValue;
    }
}