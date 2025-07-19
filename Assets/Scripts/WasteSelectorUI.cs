using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WasteSelectorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image wastePreviewImage;           // Assign the Canvas Image
    public TextMeshProUGUI wasteTypeText;     // Assign the TMP Text

    [Header("Waste Options")]
    public Sprite[] wasteSprites;            // UI sprites
    public string[] wasteTypeNames;          // Waste type names
    public GameObject[] wasteObjects;        // Scene GameObjects already placed

    private int currentIndex = 0;

    private void Start()
    {
        if (wasteSprites.Length != wasteTypeNames.Length || wasteSprites.Length != wasteObjects.Length)
        {
            Debug.LogError("WasteSelectorUI: wasteSprites, wasteTypeNames, and wasteObjects must all have the same length!");
            return;
        }

        UpdateWastePreview();
    }

    public void NextWaste()
    {
        currentIndex = (currentIndex + 1) % wasteSprites.Length;
        UpdateWastePreview();
    }

    public void PrevWaste()
    {
        currentIndex = (currentIndex - 1 + wasteSprites.Length) % wasteSprites.Length;
        UpdateWastePreview();
    }

    private void UpdateWastePreview()
    {
        // Update UI
        wastePreviewImage.sprite = wasteSprites[currentIndex];
        wasteTypeText.text = "Waste: " + wasteTypeNames[currentIndex];

        // Update active GameObject
        for (int i = 0; i < wasteObjects.Length; i++)
        {
            if (wasteObjects[i] != null)
            {
                wasteObjects[i].SetActive(i == currentIndex);
            }
        }
    }

    /// <summary>
    /// Get the currently selected waste type name.
    /// </summary>
    public string GetCurrentWasteType()
    {
        return wasteTypeNames[currentIndex];
    }

    /// <summary>
    /// Get the currently active GameObject in scene.
    /// </summary>
    public GameObject GetCurrentWasteObject()
    {
        return wasteObjects[currentIndex];
    }
}
