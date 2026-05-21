using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Hero trainingHero;
    [SerializeField] private Enemy trainingDummy;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject destination;
    
    private float inpTimer;
    private float inputDelay = 0.5f;
    
    private string[] tutorialSteps =
    {
        "Welcome to BG4!\nPress [Space] to continue!", // 36
        // Space
        "Lets start with the basics.\n Select a character with [Left Click].", // 24
        // Character Selected
        "Great! Now let's move around.", // 24
        // Space/Movement input
        "[Right click] to move your selected character.\n Try navigating to the highlighted area.", // 20
        // Reach trigger
        "Awesome! Now let's try attacking.", // 24
        //
        "Target an enemy by using [Right Click] on them.\n Your hero will do the rest!", //20
        //
        "Next up, abilities!", // 24
        //
        "Each hero has unique abilities to help them out during combat.\n Use [Q] and [W] to activate your selected hero's abilities." // 20
        // 
    };

    private int currentStep = 0;
    void Start()
    {
        ProgressTutorial(currentStep);
        Debug.Log("Hero: "+ trainingHero.AttackDamage);
        Debug.Log("Enemy MaxHP: "+ trainingDummy.CurrentHealth);
    }
    void Update()
    {
        inpTimer += Time.deltaTime;
        CheckStepCondition();
    }

    void ProgressTutorial(int stepIndex)
    {
        switch (stepIndex)
        {
            case 0: setTutorialText(tutorialSteps[0], 36); break;
            case 1: setTutorialText(tutorialSteps[1], 24); break;
            case 2: setTutorialText(tutorialSteps[2], 20); break;
            // etc
            default:
                break;
        }
    }
    void CheckStepCondition()
    {
        switch (currentStep)
        {
            case 0:
                if (FetchInput()) { NextStep(); }
                break;
            case 1:
                if (SelectionManager.Instance.currentlySelected)
                { NextStep(); }
                break;
            case 2:
                if (FetchInput())
                {
                    NextStep();
                }
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            default:
                break;
        }
    }
    
    private bool FetchInput()
    {
        if (Keyboard.current != null && inpTimer >= inputDelay)
        {
            inpTimer = 0f;
            return true;
        } return false;
    }
    private void NextStep()
    {
        currentStep++;
        if (currentStep >= tutorialSteps.Length)
        {
            Debug.Log("Tutorial complete!");
            SceneManager.LoadScene("Armory");
            return;
        }
        ProgressTutorial(currentStep);
    }
    private void setTutorialText(string text, int fontSize)
    {
        tutorialText.text = text;
        tutorialText.fontSize = fontSize;
    }
    private void setTutorialText(string text)
    {
        tutorialText.text = text;
    }

    void OnTriggerEnter(Collider other)
    {
        return;
    }
}
