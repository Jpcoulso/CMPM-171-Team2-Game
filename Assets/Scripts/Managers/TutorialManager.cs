using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    private string[] tutorialSteps =
    {
        "Welcome to BG4!\nPress [Space] to continue!",
        "Lets start with the basics.\n Select a character with [Left Click].", // 24
        "Great! Now let's move around.", // 24
        "[Right click] to move your selected character.\n Try navigating to the highlighted area.", // 20
        "Awesome! Now let's try attacking.", // 24
        "Target an enemy by using [Right Click] on them.\n Your hero will do the rest!", //20
        "Next up, abilities!", // 24
        "Each hero has unique abilities to help them out during combat.\n Use [Q] and [W] to activate your selected hero's abilities." // 20
    };

    private int currentStep = 0;
    void Start()
    {
        ProgressTutorial(currentStep);
    }
    void Update()
    {
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
                Debug.Log("Tutorial complete!");
                SceneManager.LoadScene("Armory");
                break;
        }
    }
    void CheckStepCondition(int stepIndex = 0)
    {
        switch (currentStep)
        {
            case 0:
                if (FetchInput()) NextStep();
                break;
            default:
                break;
        }
    }
    
    private bool FetchInput()
    {
        return Keyboard.current.spaceKey.wasPressedThisFrame;
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
}
