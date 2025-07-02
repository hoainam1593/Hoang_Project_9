using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;

public class PopupVictory : BasePopup
{
    [SerializeField] private TMPro.TextMeshProUGUI textGoldBonus;
    [SerializeField] private List<Image> starImages;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonNextLevel;
    [SerializeField] private TMPro.TextMeshProUGUI textNextLevel; // Text component for button text

    protected override void Start()
    {
        base.Start();
        OnStart();
    }

    private void OnStart()
    {
        // Debug.Log("OnStart");
        buttonExit.onClick.AddListener(() =>
        {
            GameManager.instance?.ExitGame().Forget();
            ClosePopup();
        });

        buttonNextLevel.onClick.AddListener(() =>
        {
            GameManager.instance?.NextLevel().Forget();
            ClosePopup();
        });
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);

        //UnSubscribes Event
        buttonExit.onClick.RemoveAllListeners();
        buttonNextLevel.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Initialize the victory popup view
    /// </summary>
    /// <param name="star">Number of stars earned (1-3)</param>
    /// <param name="goldBonus">Gold bonus amount</param>
    /// <param name="isMaxLevel">Whether this is the maximum level available</param>
    public void InitView(int star, int goldBonus, bool isMaxLevel = false)
    {
        textGoldBonus.text = goldBonus.ToString();
        FillStar(star);
        
        // Handle max level UI changes
        if (isMaxLevel)
        {
            // Disable the next level button
            buttonNextLevel.interactable = false;
            
            // Change button text to "Max Level"
            if (textNextLevel != null)
            {
                textNextLevel.text = "Max Level";
            }
        }
        else
        {
            // Ensure button is enabled for normal levels
            buttonNextLevel.interactable = true;
            
            // Set normal button text
            if (textNextLevel != null)
            {
                textNextLevel.text = "Next Level";
            }
        }
    }

    /// <summary>
    /// Overload for backward compatibility
    /// </summary>
    /// <param name="star">Number of stars earned (1-3)</param>
    /// <param name="goldBonus">Gold bonus amount</param>
    public void InitView(int star, int goldBonus)
    {
        InitView(star, goldBonus, false);
    }

    private void FillStar(int star)
    {
        for (int i = 0; i < starImages.Count; i++)
        {
            if (i < star)
            {
                starImages[i].gameObject.SetActive(true);
            }
            else
            {
                starImages[i].gameObject.SetActive(false);
            }
        }
    }
}

