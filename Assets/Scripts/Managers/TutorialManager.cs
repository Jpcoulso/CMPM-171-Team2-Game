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
        // 0- input
        "Lets start with the basics.\n Select a character with [Left Click].", // 24
        // 1- Character Selected
        "Great! Now let's try moving around.", // 24
        // 2- input
        "[Right click] to move your selected character.\n Try navigating to the highlighted area.", // 20
        // 3- Reach trigger
        "Awesome! Now let's try attacking.", // 24
        // 4- input
        "Target an enemy by using [Right Click] on them.\n Your hero will do the rest!", //20
        // 5- target dummy
        "Next up, abilities!", // 24
        // 6- input
        "Each hero has unique abilities to help them out during combat.\n Use [Q] and [W] to activate your selected hero's abilities.", // 20
        // 7- ability check, maybe check for usage of each?
        "ABILITY_STEP_PLACEHOLDER"
        // 8- 
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
        destination.SetActive(false);
        switch (stepIndex)
        {
            case 0: setTutorialText(tutorialSteps[0], 36); break;
            case 1: setTutorialText(tutorialSteps[1], 24); break;
            case 2: setTutorialText(tutorialSteps[2], 24); break;
            case 3: setTutorialText(tutorialSteps[3], 20);
                destination.SetActive(true);
                break;
            case 4: setTutorialText(tutorialSteps[4], 24); break;
            case 5: setTutorialText(tutorialSteps[5], 20);
                trainingDummy.gameObject.SetActive(true);
                break;
            case 6: setTutorialText(tutorialSteps[6], 24); break;
            case 7: setTutorialText(tutorialSteps[7], 24); break;
                // Ability usage checkboxes?
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
                if (FetchInput()) { NextStep(); }
                break;
            case 3:
                if (HeroInZone()) NextStep();
                break;
            case 4:
                if (FetchInput()) { NextStep(); }
                break;
            case 5:
                if (trainingDummy.CurrentHealth < trainingDummy.MaxHealth) { NextStep(); }
                break;
            case 6:
                if (FetchInput()) { NextStep(); }
                break;
            case 7:
                // abiity checks
                break;
            default:
                break;
        }
    }
    
    private bool FetchInput()
    {
        if ((Keyboard.current.spaceKey.isPressed || Mouse.current.rightButton.wasPressedThisFrame
        || Mouse.current.leftButton.wasPressedThisFrame) && inpTimer >= inputDelay)
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

    private bool HeroInZone()
    {
        if (destination == null || trainingHero == null) return false;
        return Vector2.Distance(trainingHero.transform.position, destination.transform.position) < 1f;
    }
}
