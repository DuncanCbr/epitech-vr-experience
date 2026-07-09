using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerVR : MonoBehaviour
{
    public static GameManagerVR Instance;

    [Header("Parcours")]
    public List<Step> steps = new();

    [Header("UI")]
    public Text objectiveText;
    public Text progressText;

    [Header("Fin")]
    public GameObject badgeFinal;

    public int CurrentStep => currentStep;

    private int currentStep;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentStep = 0;

        if (badgeFinal != null)
            badgeFinal.SetActive(false);

        RefreshUI();
    }

    public bool CompleteStep(GameObject sender)
    {
        if (currentStep >= steps.Count)
            return false;

        if (steps[currentStep].interactionObject != sender)
            return false;

        currentStep++;

        if (currentStep >= steps.Count)
        {
            FinishExperience();
        }
        else
        {
            RefreshUI();
        }

        return true;
    }

    void RefreshUI()
    {
        objectiveText.text = steps[currentStep].objective;
        progressText.text = $"{currentStep + 1}/{steps.Count}";
    }

    void FinishExperience()
    {
        objectiveText.text = "Bravo ! Badge étudiant débloqué.";

        progressText.text = $"{steps.Count}/{steps.Count}";

        if (badgeFinal != null)
            badgeFinal.SetActive(true);
    }
}