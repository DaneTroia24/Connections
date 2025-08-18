using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/*
*   Class to handle the shuffle mechanic of buttons
*   - Only shuffles buttons that have not been solved as a word set
*   - Uses Lerp to "animate" the button movement
*/

public class ButtonLocationManager : MonoBehaviour
{
    [Header("Button References")]
    public List<Button> buttons;
    private List<Vector3> originalPositions = new List<Vector3>();
    private HashSet<Button> solvedButtons = new HashSet<Button>();
    [SerializeField] private GameObject devPanel;
    public float moveDuration = 0.5f; // How long it takes buttons to shuffle
    public float shuffleCooldown = .75f; // Time between shuffles (cooldown)
    private bool isCooldown = false;

    public void Start()
    {
        StoreOriginalPositions();
    }

    public void StoreOriginalPositions()
    {
        originalPositions.Clear();
        foreach (Button button in buttons)
        {
            originalPositions.Add(button.transform.position);
        }
    }

    public void ResetUnsolvedList()
    {
        solvedButtons.Clear();
    }

    public void ShuffleAndRepositionButtons()
    {
        if (isCooldown) return;

        StartCoroutine(Cooldown());

        StoreOriginalPositions();

        // Create a copy of the original positions but exclude solved button positions
        List<Vector3> unsolvedPositions = new List<Vector3>();
        Debug.Log(buttons.Count);
        for (int i = 0; i < buttons.Count; i++)
        {
            if (!solvedButtons.Contains(buttons[i]))
            {
                unsolvedPositions.Add(originalPositions[i]); // Add positions of unsolved buttons
            }
        }
        Debug.Log("unsolved:" + unsolvedPositions.Count);

        // Shuffle the positions
        for (int i = 0; i < unsolvedPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, unsolvedPositions.Count);
            Vector3 temp = unsolvedPositions[i];
            unsolvedPositions[i] = unsolvedPositions[randomIndex];
            unsolvedPositions[randomIndex] = temp;
        }

        // Start moving only the unsolved buttons to new positions
        StartCoroutine(MoveUnsolvedButtonsSmoothly(unsolvedPositions));
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(shuffleCooldown);
        isCooldown = false;
    }

    private IEnumerator MoveUnsolvedButtonsSmoothly(List<Vector3> targetPositions)
    {
        float elapsedTime = 0f;

        // Keep track of the starting positions of unsolved buttons
        List<Vector3> startPositions = new List<Vector3>();
        List<Button> unsolvedButtons = new List<Button>();

        for (int i = 0; i < buttons.Count; i++)
        {
            if (!solvedButtons.Contains(buttons[i]))
            {
                startPositions.Add(buttons[i].transform.position);
                unsolvedButtons.Add(buttons[i]);
            }
        }

        // Show the movement of each button
        while (elapsedTime < moveDuration)
        {
            for (int i = 0; i < unsolvedButtons.Count; i++)
            {
                unsolvedButtons[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], elapsedTime / moveDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the unsolved buttons end up at their final positions
        for (int i = 0; i < unsolvedButtons.Count; i++)
        {
            unsolvedButtons[i].transform.position = targetPositions[i];
        }
    }

    public void OnButtonClicked(Button clickedButton)
    {
        Debug.Log($"Button {clickedButton.name} clicked!");
        ShuffleAndRepositionButtons();
    }

    public void MarkButtonsAsSolved(List<Button> solvedGroup)
    {
        foreach (Button button in solvedGroup)
        {
            solvedButtons.Add(button);
        }
    }

    public void ShowDevText()
    {
        devPanel.SetActive(!devPanel.activeSelf);
    }
}

