using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    private bool[] uniqueInputs = new bool[10];
    private float timer = 0f;
    private float timeout = 5f;
    private CanvasGroup canvasGroup;
    private bool tutorialDone = true;

    private void Awake()
    {
        for (int i = 0; i < uniqueInputs.Length; i++)
        {
            uniqueInputs[i] = false;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        FindFirstObjectByType<UIManager>().OnUITypeChangeAddListener(TriggerTutorial);
    }


    private void Update()
    {
        if (tutorialDone) return;

        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W)) uniqueInputs[0] = true;
        if (Input.GetKeyDown(KeyCode.A)) uniqueInputs[1] = true;
        if (Input.GetKeyDown(KeyCode.S)) uniqueInputs[2] = true;
        if (Input.GetKeyDown(KeyCode.D)) uniqueInputs[3] = true;
        if (Input.GetKeyDown(KeyCode.Q)) uniqueInputs[4] = true;
        if (Input.GetKeyDown(KeyCode.E)) uniqueInputs[5] = true;
        if (Input.GetKeyDown(KeyCode.UpArrow)) uniqueInputs[6] = true;
        if (Input.GetKeyDown(KeyCode.DownArrow)) uniqueInputs[7] = true;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) uniqueInputs[8] = true;
        if (Input.GetKeyDown(KeyCode.RightArrow)) uniqueInputs[9] = true;

        int numberOfUniqueInputs = 0;
        foreach (bool input in uniqueInputs)
        {
            if (input == true) numberOfUniqueInputs++;
        }

        if (numberOfUniqueInputs >= 2 || timer >= timeout)
        {
            canvasGroup.alpha -= Time.deltaTime;
            if (canvasGroup.alpha <= 0f)
            {
                tutorialDone = true;
                ProgressGame.Progress();
                canvasGroup.ToggleCanvasGroup(false);
            }
        }
    }

    private void TriggerTutorial(TypeOfUIWindow typeOfUIWindow)
    {
        if(typeOfUIWindow == TypeOfUIWindow.TutorialMenu)
        {
            ResetValues();
        }
    }

    private void ResetValues()
    {
        tutorialDone = false;
        timer = 0;
        canvasGroup.ToggleCanvasGroup(true);

        for (int i = 0; i < uniqueInputs.Length; i++)
        {
            uniqueInputs[i] = false;
        }
    }
}