using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CellularUI : MonoBehaviour
{
    [Header("Global")]
    public CellMaterial mouseSpawnMaterial = CellMaterial.Dirt;
    public float brushSize = 3;
    public bool randomColors = false;

    [Header("Particles")]
    public bool spawnAsParticles = false;
    public float particleSpawnSpeed = 5;

    private GameObject topButtonPanel;
    private GameObject bottomButtonPanel;
    private Button currentSelectedButton;
    private Outline currentOutline;

    public RectTransform topPanel;
    public RectTransform gameArea;
    public RectTransform bottomPanel;

    CellularTools currentTool = CellularTools.Draw;
    public enum CellularTools { Draw, Thermal }

    private Dictionary<CellularTools, Button> toolButtons = new Dictionary<CellularTools, Button>();

    private void Start()
    {
        CreateUI();
        GenerateToolButtons();
        GenerateColorButtons();
        SelectTool(currentTool);
    }

    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Vector2 localMousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, Camera.main, out localMousePosition);
                if (gameArea.rect.Contains(localMousePosition))
                {
                    DrawMaterial();
                }
            }
        }
    }

    void SelectTool(CellularTools tool)
    {
        currentTool = tool;

        foreach (var buttonPair in toolButtons)
        {
            var button = buttonPair.Value;
            var buttonTool = buttonPair.Key;

            if (buttonTool == tool)
            {
                if (buttonTool == CellularTools.Thermal)
                {
                    // Toggle the Thermal tool
                    CellularThermodynamics.instance.debugTemperatures.Toggle();
                    bool isOn = CellularThermodynamics.instance.debugTemperatures;
                    UpdateToggleButtonState(button, isOn);
                }
                else
                {
                    HighlightButton(button);
                }
            }
            else
            {
                ResetButton(button);
            }
        }
    }

    private void HighlightButton(Button button)
    {
        if (currentSelectedButton != null)
        {
            if (currentOutline != null)
            {
                Destroy(currentOutline);
            }
        }

        currentSelectedButton = button;
        currentOutline = currentSelectedButton.gameObject.AddComponent<Outline>();
        currentOutline.effectColor = Color.yellow;
        currentOutline.effectDistance = new Vector2(8, 8);
    }

    private void ResetButton(Button button)
    {
        var outline = button.GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
        }
    }

    private void UpdateToggleButtonState(Button button, bool isOn)
    {
        var image = button.GetComponent<Image>();
        if (isOn)
        {
            image.color = Color.green;
        }
        else
        {
            image.color = Color.red;
        }
    }

    private void CreateUI()
    {
        topButtonPanel = CreateButtonPanel(topPanel, "TopButtonPanel");
        bottomButtonPanel = CreateButtonPanel(bottomPanel, "BottomButtonPanel");
    }

    private GameObject CreateButtonPanel(Transform parent, string name)
    {
        GameObject buttonPanel = new GameObject(name);
        RectTransform panelRect = buttonPanel.AddComponent<RectTransform>();
        buttonPanel.transform.SetParent(parent);
        buttonPanel.transform.position = Vector3.zero;
        buttonPanel.transform.localScale = Vector3.one;
        panelRect.anchorMin = Vector3.zero;
        panelRect.anchorMax = Vector3.one;
        panelRect.pivot = Vector3.zero;
        panelRect.anchoredPosition = Vector3.zero;
        panelRect.sizeDelta = new Vector2(0, 0);

        HorizontalLayoutGroup layoutGroup = buttonPanel.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;
        layoutGroup.spacing = 8;
        layoutGroup.padding = new RectOffset(8, 8, 8, 8);

        return buttonPanel;
    }

    private void GenerateToolButtons()
    {
        foreach (CellularTools tool in System.Enum.GetValues(typeof(CellularTools)))
        {
            GameObject buttonObj = CreateToolButton(tool.ToString(), topButtonPanel);
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => SelectTool(tool));
            toolButtons[tool] = button;

            if (tool == CellularTools.Thermal)
            {
                UpdateToggleButtonState(button, CellularThermodynamics.instance.debugTemperatures);
            }
        }
    }

    private GameObject CreateToolButton(string toolName, GameObject parentPanel)
    {
        GameObject buttonObj = new GameObject("Button_" + toolName);
        buttonObj.transform.SetParent(parentPanel.transform);
        buttonObj.transform.localScale = Vector3.one;
        buttonObj.transform.localPosition = Vector3.zero;

        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);

        Image image = buttonObj.AddComponent<Image>();
        image.color = Color.white;

        Button button = buttonObj.AddComponent<Button>();

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(buttonObj.transform);
        iconObj.transform.localScale = Vector3.one;
        iconObj.transform.localPosition = Vector3.zero;

        RectTransform iconRectTransform = iconObj.AddComponent<RectTransform>();
        iconRectTransform.sizeDelta = new Vector2(30, 30);
        iconRectTransform.anchoredPosition = Vector2.zero;

        Image iconImage = iconObj.AddComponent<Image>();
        string iconPath = $"Icons/Tool_{toolName}";
        Sprite icon = Resources.Load<Sprite>(iconPath);

        // Check if the icon was successfully loaded
        if (icon != null)
        {
            iconImage.sprite = icon;
        }
        else
        {
            Debug.LogError($"Icon for tool '{toolName}' not found at path '{iconPath}'");
        }

        return buttonObj;
    }

    private void GenerateColorButtons()
    {
        Color[] colors = CellularMaterialLibrary.instance.MaterialColors();
        HashSet<Color> addedColors = new HashSet<Color>();

        foreach (var color in colors)
        {
            if (color.a > 0 && !addedColors.Contains(color))
            {
                addedColors.Add(color);
                if (IsColorInMaterials(color, out CellMaterialProperties materialProperty))
                {
                    GameObject buttonObjBottom = CreateButton(color, materialProperty.icon, bottomButtonPanel);
                    buttonObjBottom.GetComponent<Button>().onClick.AddListener(() => PickMaterial(color, buttonObjBottom.GetComponent<Button>()));
                }
            }
        }
    }

    private GameObject CreateButton(Color color, Sprite icon, GameObject parentPanel)
    {
        GameObject buttonObj = new GameObject("Button");
        buttonObj.transform.SetParent(parentPanel.transform);
        buttonObj.transform.localScale = Vector3.one;
        buttonObj.transform.localPosition = Vector3.zero;

        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(50, 50); // Tamaño del botón

        Image image = buttonObj.AddComponent<Image>();
        image.color = color;

        Button button = buttonObj.AddComponent<Button>();

        if (icon != null)
        {
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(buttonObj.transform);
            iconObj.transform.localScale = Vector3.one;
            iconObj.transform.localPosition = Vector3.zero;

            RectTransform iconRectTransform = iconObj.AddComponent<RectTransform>();
            iconRectTransform.sizeDelta = new Vector2(30, 30);
            iconRectTransform.anchoredPosition = Vector2.zero;

            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.sprite = icon;
        }

        return buttonObj;
    }

    private void PickMaterial(Color color, Button button)
    {
        mouseSpawnMaterial = GetMaterialFromColor(color);

        if (currentSelectedButton != null)
        {
            if (currentOutline != null)
            {
                Destroy(currentOutline);
            }
        }

        // Resaltar el botón seleccionado
        currentSelectedButton = button;
        currentOutline = currentSelectedButton.gameObject.AddComponent<Outline>();
        currentOutline.effectColor = Color.yellow;
        currentOutline.effectDistance = new Vector2(8, 8);
    }

    private CellMaterial GetMaterialFromColor(Color color)
    {
        float minDistance = float.MaxValue;
        CellMaterial closestMaterial = CellMaterial.Empty;

        foreach (var materialProperty in CellularMaterialLibrary.instance.materials)
        {
            float distance = ColorDistance(color, materialProperty.identifierColor);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestMaterial = materialProperty.cellMaterial;
            }
        }

        return closestMaterial;
    }

    private float ColorDistance(Color c1, Color c2)
    {
        float rDiff = c1.r - c2.r;
        float gDiff = c1.g - c2.g;
        float bDiff = c1.b - c2.b;
        return rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;
    }

    private bool IsColorInMaterials(Color color, out CellMaterialProperties materialProperty)
    {
        foreach (var prop in CellularMaterialLibrary.instance.materials)
        {
            float distance = ColorDistance(color, prop.identifierColor);
            if (distance <= 0.01f) // Ajusta el umbral según sea necesario
            {
                materialProperty = prop;
                return true;
            }
        }
        materialProperty = null;
        return false;
    }

    private void DrawMaterial()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pixelPosition = CellularAutomata.WorldToPixelPosition(worldPosition);

        if (Input.GetMouseButton(0))
        {
            for (float i = -brushSize * 0.5f; i < brushSize * 0.5f; i++)
            {
                for (float j = -brushSize * 0.5f; j < brushSize * 0.5f; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(Mathf.FloorToInt(i), Mathf.FloorToInt(j));

                    var newCell = CellularAutomata.instance.CreateCellIfEmpty(globalPixelPosition, mouseSpawnMaterial);
                    if (newCell != null && randomColors)
                    {
                        newCell.overrideColor = new Color(Random.value, Random.value, Random.value, 1);
                    }
                    if (spawnAsParticles)
                    {
                        var newParticle = PixelParticles.instance.CellToParticle(newCell, globalPixelPosition);
                        newParticle.speed = Random.insideUnitCircle * particleSpawnSpeed;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            for (float i = -brushSize * 0.5f; i < brushSize * 0.5f; i++)
            {
                for (float j = -brushSize * 0.5f; j < brushSize * 0.5f; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(Mathf.FloorToInt(i), Mathf.FloorToInt(j));
                    CellularAutomata.instance.DestroyCell(globalPixelPosition);
                }
            }
        }
    }
}
