using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CellularUI : MonoBehaviour
{
    [Header("Global")]
    public CellMaterial currentlySelectedMaterial = CellMaterial.Dirt;
    public float brushSize = 3;
    public bool randomColors = false;

    [Header("Particles")]
    public bool spawnAsParticles = false;
    public float particleSpawnSpeed = 5;

    private GameObject topButtonPanel;
    private GameObject bottomButtonPanel;
    private Button currentSelectedButton;
    private Button currentBrushSizeButton;
    private Outline currentOutline;
    private Outline currentBrushOutline;

    public RectTransform topPanel;
    public RectTransform gameArea;
    public RectTransform bottomPanel;

    CellularTools currentTool = CellularTools.Draw;
    public enum CellularTools { Draw, Erase, Small, Medium, Large, Huge, Thermal }

    private Dictionary<CellularTools, Button> toolButtons = new Dictionary<CellularTools, Button>();
    private Dictionary<CellularTools, Button> brushSizeButtons = new Dictionary<CellularTools, Button>();

    // Preview object
    private GameObject previewObject;
    private Image previewImage;

    private void Start()
    {
        CreateUI();
        GenerateToolButtons();
        GenerateMaterialButtons();
        SelectTool(CellularTools.Medium);
        SelectTool(CellularTools.Draw);
        SelectDefaultMaterial();
        CreatePreviewObject();
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
        UpdatePreview();
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
                switch (tool)
                {
                    case CellularTools.Draw:
                        HighlightButton(button);
                        bottomButtonPanel.gameObject.SetActive(true);
                        break;
                    case CellularTools.Erase:
                        HighlightButton(button);
                        bottomButtonPanel.gameObject.SetActive(false);
                        break;
                    case CellularTools.Thermal:
                        CellularThermodynamics.instance.debugTemperatures.Toggle();
                        bool isOn = CellularThermodynamics.instance.debugTemperatures;
                        UpdateToggleButtonState(button, isOn);
                        break;
                    case CellularTools.Small:
                        brushSize = 1;
                        HighlightBrushSizeButton(button);
                        break;
                    case CellularTools.Medium:
                        brushSize = 3;
                        HighlightBrushSizeButton(button);
                        break;
                    case CellularTools.Large:
                        brushSize = 6;
                        HighlightBrushSizeButton(button);
                        break;
                    case CellularTools.Huge:
                        brushSize = 12;
                        HighlightBrushSizeButton(button);
                        break;
                }
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

    private void HighlightBrushSizeButton(Button button)
    {
        if (currentBrushSizeButton != null)
        {
            if (currentBrushOutline != null)
            {
                Destroy(currentBrushOutline);
            }
        }

        currentBrushSizeButton = button;
        currentBrushOutline = currentBrushSizeButton.gameObject.AddComponent<Outline>();
        currentBrushOutline.effectColor = Color.green;
        currentBrushOutline.effectDistance = new Vector2(8, 8);
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
            image.color = Color.gray;
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
        // Botones de herramientas
        AddSeparator(topButtonPanel);
        foreach (CellularTools tool in new CellularTools[] { CellularTools.Draw, CellularTools.Erase, CellularTools.Thermal })
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

        // Separador
        AddSeparator(topButtonPanel);

        // Botones de tamaños de brush
        foreach (CellularTools tool in new CellularTools[] { CellularTools.Small, CellularTools.Medium, CellularTools.Large, CellularTools.Huge })
        {
            GameObject buttonObj = CreateToolButton(tool.ToString(), topButtonPanel);
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => SelectTool(tool));
            brushSizeButtons[tool] = button;
        }
        AddSeparator(topButtonPanel);
    }

    private void AddSeparator(GameObject parentPanel)
    {
        GameObject separator = new GameObject("Separator");
        separator.transform.SetParent(parentPanel.transform);
        separator.transform.localScale = Vector3.one;
        separator.transform.localPosition = Vector3.zero;

        LayoutElement layoutElement = separator.AddComponent<LayoutElement>();
        layoutElement.minWidth = 20; // Ajusta el tamaño del separador según sea necesario
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
        iconRectTransform.sizeDelta = new Vector2(20, 20);
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

    private void GenerateMaterialButtons()
    {
        Color[] colors = CellularMaterialLibrary.instance.MaterialColors();
        List<Color> addedColors = new List<Color>();

        //Intercambiar empty y undestructible en la lista de botones
        Color temp = colors[0];
        colors[0] = colors[1];
        colors[1] = temp;

        foreach (var color in colors)
        {
            if (!addedColors.Contains(color))
            {
                addedColors.Add(color);
                if (IsColorInMaterials(color, out CellMaterialProperties materialProperty))
                {
                    GameObject buttonObjBottom = CreateButton(color, materialProperty.icon, bottomButtonPanel);
                    buttonObjBottom.GetComponent<Button>().onClick.AddListener(() => PickMaterial(color, buttonObjBottom.GetComponent<Button>()));

                    // Seleccionar por defecto el material "Dirt"
                    if (materialProperty.cellMaterial == CellMaterial.Dirt)
                    {
                        PickMaterial(color, buttonObjBottom.GetComponent<Button>());
                    }
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
        currentlySelectedMaterial = GetMaterialFromColor(color);

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

    private void SelectDefaultMaterial()
    {
        foreach (Transform child in bottomButtonPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button != null && GetMaterialFromColor(button.GetComponent<Image>().color) == CellMaterial.Dirt)
            {
                PickMaterial(button.GetComponent<Image>().color, button);
                break;
            }
        }
    }

    private void DrawMaterial()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pixelPosition = CellularAutomata.WorldToPixelPosition(worldPosition);

        if (Input.GetMouseButton(0))
        {
            int brushRadius = Mathf.FloorToInt(brushSize * 0.5f);
            for (int i = -brushRadius; i <= brushRadius; i++)
            {
                for (int j = -brushRadius; j <= brushRadius; j++)
                {
                    // Calcula la distancia desde el centro del pincel
                    float distance = Mathf.Sqrt(i * i + j * j);

                    // Si la distancia es menor o igual al radio del pincel, está dentro del círculo
                    if (distance <= brushRadius)
                    {
                        var globalPixelPosition = pixelPosition + new Vector2Int(i, j);

                        Cell newCell;
                        if (currentTool == CellularTools.Erase)
                        {
                            newCell = CellularAutomata.instance.CreateCell(globalPixelPosition, CellMaterial.Empty);
                        }
                        else
                        {
                            newCell = CellularAutomata.instance.CreateCellIfEmpty(globalPixelPosition, currentlySelectedMaterial);
                        }
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
        }
    }

    private void CreatePreviewObject()
    {
        previewObject = new GameObject("PreviewObject");
        previewObject.transform.SetParent(gameArea);
        previewObject.transform.localScale = Vector3.one;

        RectTransform rectTransform = previewObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(brushSize, brushSize);

        previewImage = previewObject.AddComponent<Image>();
        previewImage.color = new Color(1, 1, 1, 0.5f); // Color semi-transparente
    }

    private void UpdatePreview()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, Camera.main, out localMousePosition);
        if (gameArea.rect.Contains(localMousePosition))
        {
            previewObject.SetActive(true);
            previewObject.transform.localPosition = localMousePosition;
            previewObject.GetComponent<RectTransform>().sizeDelta = new Vector2(brushSize, brushSize);
        }
        else
        {
            previewObject.SetActive(false);
        }
    }
}
