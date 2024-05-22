using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
    private Outline currentOutline;

    public RectTransform topPanel;
    public RectTransform gameArea;
    public RectTransform bottomPanel;

    CellularTools currentTool = CellularTools.Draw;
    public enum CellularTools { Spacer, Draw, Erase, Pause, Reset, Save, Exit }

    private Dictionary<CellularTools, Button> toolButtons = new Dictionary<CellularTools, Button>();
    private GameObject previewContainer;
    private List<GameObject> previewPixels = new List<GameObject>();
    private const float pixelSize = 0.125f;
    private Sprite pixelSprite;

    private Texture2D defaultCursor;
    private Texture2D handCursor;

    private void Start()
    {
        CreatePreviewContainer();
        CreatePixelTexture();
        CreateUI();
        GenerateToolButtons();
        GenerateMaterialButtons();
        SelectTool(CellularTools.Draw);
        SelectDefaultMaterial();
        UpdatePreview();
        UpdatePreviewPixels();
        LoadCursors();
    }

    private void Update()
    {
        HandleMouseCursor();
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, Camera.main, out localMousePosition);
            if (gameArea.rect.Contains(localMousePosition))
            {
                DrawMaterial();
            }
        }

        Shortcuts();
        UpdateBrushSizeWithMouseWheel();
        UpdatePreview();
    }

    private void Shortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CellularThermodynamics.instance.debugTemperatures = !CellularThermodynamics.instance.debugTemperatures;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private void LoadCursors()
    {
        defaultCursor = Resources.Load<Texture2D>("Cursors/Atto");
        defaultCursor.filterMode = FilterMode.Point;
        handCursor = Resources.Load<Texture2D>("Cursors/Hand");
        handCursor.filterMode = FilterMode.Point;
    }

    private void HandleMouseCursor()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, Camera.main, out localMousePosition);
        if (gameArea.rect.Contains(localMousePosition))
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    void SelectTool(CellularTools tool)
    {
        if (toolButtons.ContainsKey(tool))
        {
            currentTool = tool;
            foreach (var buttonPair in toolButtons)
            {
                var button = buttonPair.Value;
                ResetButton(button);

                if (buttonPair.Key == tool && buttonPair.Key != CellularTools.Save && buttonPair.Key != CellularTools.Reset && buttonPair.Key != CellularTools.Exit)
                {
                    HighlightButton(button);
                    bottomButtonPanel.gameObject.SetActive(tool == CellularTools.Draw);
                }
            }

            if (currentTool == CellularTools.Save)
            {
                SaveChunkAsImage();
            }

            if (currentTool == CellularTools.Pause)
            {
                CellularAutomata.instance.TogglePause();
            }

            if (currentTool == CellularTools.Reset)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }

            if (currentTool == CellularTools.Exit)
            {
                Application.Quit();
            }
        }
    }

    private void SaveChunkAsImage()
    {
        CellularChunk currentChunk = PixelCamera.instance.currentChunk;
        if (currentChunk == null) return;

        Color32[] pixelColors = currentChunk.ChunkToColorIdArray(CellRenderLayer.Main);
        Texture2D texture = new Texture2D(currentChunk.pixelSize.x, currentChunk.pixelSize.y);

        texture.SetPixels32(pixelColors);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        int id = GetNextAvailableId();
        string directoryPath = Application.persistentDataPath + "/Saved/";
        string path = directoryPath + "Playground_" + id + ".png";

        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        System.IO.File.WriteAllBytes(path, bytes);

        Debug.Log($"Chunk saved as image at: {path}");
        OpenFolderInExplorer(directoryPath);
    }


    private int GetNextAvailableId()
    {
        string directoryPath = Application.persistentDataPath + "/Saved/";
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        string[] files = System.IO.Directory.GetFiles(directoryPath, "Playground_*.png");
        int maxId = 0;

        foreach (string file in files)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
            string idStr = fileName.Replace("Playground_", "");

            if (int.TryParse(idStr, out int id))
            {
                if (id > maxId)
                {
                    maxId = id;
                }
            }
        }

        return maxId + 1;
    }

    private void OpenFolderInExplorer(string folderPath)
    {
#if UNITY_EDITOR
        System.Diagnostics.Process.Start("explorer.exe", folderPath.Replace('/', '\\'));
#elif UNITY_STANDALONE_WIN
    System.Diagnostics.Process.Start("explorer.exe", folderPath.Replace('/', '\\'));
#elif UNITY_STANDALONE_OSX
    System.Diagnostics.Process.Start("open", folderPath);
#elif UNITY_STANDALONE_LINUX
    System.Diagnostics.Process.Start("xdg-open", folderPath);
#else
    Debug.Log("Open folder not supported on this platform.");
#endif
    }

    private void UpdateBrushSizeWithMouseWheel()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            brushSize = Mathf.Clamp(brushSize + scroll * 2, 1, 12);
            UpdatePreviewPixels();
        }
    }

    private void HighlightButton(Button button)
    {
        if (currentSelectedButton != null && currentOutline != null)
        {
            Destroy(currentOutline);
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
        image.color = isOn ? Color.green : Color.gray;
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
        foreach (CellularTools tool in new CellularTools[] { CellularTools.Draw, CellularTools.Erase, CellularTools.Spacer, CellularTools.Pause, CellularTools.Spacer, CellularTools.Save, CellularTools.Spacer, CellularTools.Reset, CellularTools.Spacer, CellularTools.Exit })
        {
            if (tool == CellularTools.Spacer)
            {
                CreateSpacer(topButtonPanel);
            }
            else
            {
                GameObject buttonObj = CreateToolButton(tool.ToString(), topButtonPanel);
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => SelectTool(tool));
                AddCursorChangeEvents(button, true);
                toolButtons[tool] = button;
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
        iconRectTransform.sizeDelta = new Vector2(20, 20);
        iconRectTransform.anchoredPosition = Vector2.zero;

        Image iconImage = iconObj.AddComponent<Image>();
        string iconPath = $"Icons/Tool_{toolName}";
        Sprite icon = Resources.Load<Sprite>(iconPath);

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

    private GameObject CreateSpacer(GameObject parentPanel)
    {
        GameObject spacerObj = new GameObject("Spacer");
        spacerObj.transform.SetParent(parentPanel.transform);
        spacerObj.transform.localScale = Vector3.one;
        spacerObj.transform.localPosition = Vector3.zero;

        RectTransform rectTransform = spacerObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);

        Image image = spacerObj.AddComponent<Image>();
        image.color = Color.clear;

        return spacerObj;
    }

    private void GenerateMaterialButtons()
    {
        Color[] colors = CellularMaterialLibrary.instance.MaterialColors();
        List<Color> addedColors = new List<Color>();

        // Intercambiar empty y undestructible en la lista de botones
        Color temp = colors[0];
        colors[0] = colors[1];
        colors[1] = temp;

        foreach (var color in colors)
        {
            if (color.a != 0 && !addedColors.Contains(color))
            {
                addedColors.Add(color);
                if (IsColorInMaterials(color, out CellMaterialProperties materialProperty))
                {
                    GameObject buttonObjBottom = CreateButton(color, materialProperty.icon, bottomButtonPanel);
                    buttonObjBottom.GetComponent<Button>().onClick.AddListener(() => PickMaterial(color, buttonObjBottom.GetComponent<Button>()));
                    AddCursorChangeEvents(buttonObjBottom.GetComponent<Button>(), true);

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

        if (currentSelectedButton != null && currentOutline != null)
        {
            Destroy(currentOutline);
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
            if (button != null && GetMaterialFromColor(button.GetComponent<Image>().color) == CellMaterial.Sand)
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

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
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
                            if (Input.GetMouseButton(0))
                            {
                                newCell = CellularAutomata.instance.CreateCellIfEmpty(globalPixelPosition, currentlySelectedMaterial);
                            }
                            else
                            {
                                newCell = CellularAutomata.instance.CreateCell(globalPixelPosition, currentlySelectedMaterial);
                            }
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

    private void CreatePreviewContainer()
    {
        previewContainer = new GameObject("PreviewContainer");
        previewContainer.transform.SetParent(null); // Make it a root object in the scene
        previewContainer.transform.localScale = Vector3.one;

        UpdatePreviewPixels();
    }

    private void UpdatePreviewPixels()
    {
        // Clear previous pixels
        foreach (var pixel in previewPixels)
        {
            Destroy(pixel);
        }
        previewPixels.Clear();

        // Create new pixels based on brush size
        int brushRadius = Mathf.FloorToInt(brushSize * 0.5f);
        for (int i = -brushRadius; i <= brushRadius; i++)
        {
            for (int j = -brushRadius; j <= brushRadius; j++)
            {
                float distance = Mathf.Sqrt(i * i + j * j);
                if (distance <= brushRadius)
                {
                    GameObject pixel = CreatePreviewPixel();
                    previewPixels.Add(pixel);
                }
            }
        }
    }

    private GameObject CreatePreviewPixel()
    {
        GameObject pixel = new GameObject("PreviewPixel");
        pixel.transform.SetParent(previewContainer.transform);
        pixel.transform.localScale = Vector3.one;

        SpriteRenderer spriteRenderer = pixel.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = pixelSprite;
        spriteRenderer.color = new Color(0, 0, 0, 0.5f);

        return pixel;
    }

    private void CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        pixelSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), Global.pixelsPerUnit);
    }

    private void UpdatePreview()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.z = 0;

        previewContainer.SetActive(true);
        previewContainer.transform.position = SnapToPixelGrid(worldMousePosition) + new Vector3(0.5f, 0.5f) / Global.pixelsPerUnit;

        // Update positions of preview pixels
        int index = 0;
        int brushRadius = Mathf.FloorToInt(brushSize * 0.5f);
        for (int i = -brushRadius; i <= brushRadius; i++)
        {
            for (int j = -brushRadius; j <= brushRadius; j++)
            {
                float distance = Mathf.Sqrt(i * i + j * j);
                if (distance <= brushRadius && index < previewPixels.Count)
                {
                    previewPixels[index].transform.localPosition = new Vector2(i * pixelSize, j * pixelSize);
                    index++;
                }
            }
        }
    }

    private Vector3 SnapToPixelGrid(Vector3 position)
    {
        position.x = Mathf.Floor(position.x / pixelSize) * pixelSize;
        position.y = Mathf.Floor(position.y / pixelSize) * pixelSize;
        return position;
    }

    private void AddCursorChangeEvents(Button button, bool isHandCursor)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => { Cursor.SetCursor(isHandCursor ? handCursor : defaultCursor, isHandCursor ? new Vector2(16f, 0) : Vector2.zero, CursorMode.Auto); });
        trigger.triggers.Add(pointerEnter);

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => { Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto); });
        trigger.triggers.Add(pointerExit);
    }

    private void ResetChunk()
    {
        CellularChunk currentChunk = PixelCamera.instance.currentChunk;
        if (currentChunk == null) return;

        for (int x = 0; x < currentChunk.pixelSize.x; x++)
        {
            for (int y = 0; y < currentChunk.pixelSize.y; y++)
            {
                currentChunk[x, y] = new Cell(CellMaterial.Empty);
            }
        }
    }
}
