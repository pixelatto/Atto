using System.Collections.Generic;
using UnityEngine;

public class CellularThermodynamics : SingletonMonobehaviour<CellularThermodynamics>
{
    public float ambientTemperature = 20;
    public float ambientDissipation = 0.5f;
    public float dampingFactor = 0.95f;
    public bool debugTemperatures = false;
    [HideInInspector] public SpriteRenderer debugSpriteRenderer;

    private Texture2D debugTexture;
    private int textureWidth;
    private int textureHeight;
    private ComputeShader computeShader;

    CellularAutomata cellularAutomata;
    TemperatureCellData[] cells;
    RenderTexture currentTemperatureRT;
    RenderTexture nextTemperatureRT;
    Cycle updateRateCycle;
    CellularChunk currentChunk => PixelCamera.instance.currentChunk;

    bool isDirty = true;

    struct TemperatureCellData
    {
        public float temperature;
        public float thermalConductivity;
        public int material;
    }

    private void Start()
    {
        textureWidth = Global.roomPixelSize.x;
        textureHeight = Global.roomPixelSize.y;

        InitializeDebugTexture();
        InitializeComputeShader();

        cellularAutomata = CellularAutomata.instance;
        cells = new TemperatureCellData[textureWidth * textureHeight];

        updateRateCycle = new Cycle(1f / Global.simulationsFPS, SetDirty);
        updateRateCycle.Start();
    }

    private void InitializeDebugTexture()
    {
        debugTexture = new Texture2D(textureWidth, textureHeight);
        debugTexture.filterMode = FilterMode.Point;

        var debugGameObject = new GameObject("ThermalTexture");
        debugSpriteRenderer = debugGameObject.AddComponent<SpriteRenderer>();
        debugSpriteRenderer.sprite = Sprite.Create(debugTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.one * 0.5f, Global.pixelsPerUnit);
        debugSpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        //debugSpriteRenderer.sortingLayerName = "Default";

        var pixelCamera = FindObjectOfType<RoomPixelCamera>();
        if (pixelCamera != null)
        {
            debugSpriteRenderer.transform.SetParent(pixelCamera.transform);
            debugSpriteRenderer.transform.position = pixelCamera.transform.position + Vector3.forward;
        }
        debugSpriteRenderer.transform.localScale = Vector3.one;
    }

    private void InitializeComputeShader()
    {
        computeShader = Resources.Load<ComputeShader>("Shaders/Compute/TemperatureComputeShader");

        currentTemperatureRT = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        currentTemperatureRT.enableRandomWrite = true;
        currentTemperatureRT.Create();

        nextTemperatureRT = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        nextTemperatureRT.enableRandomWrite = true;
        nextTemperatureRT.Create();
    }

    void SetDirty()
    {
        isDirty = true;
    }

    private void Update()
    {
        if (isDirty)
        {
            Step();
            isDirty = false;
        }
    }

    private void Step()
    {
        UpdateTemperatures();
        TriggerMaterialChanges();
        if (debugTemperatures)
        {
            UpdateDebugTexture();
            debugSpriteRenderer.enabled = true;
        }
        else
        {
            debugSpriteRenderer.enabled = false;
        }
    }

    private void UpdateTemperatures()
    {
        if (currentChunk == null) return;

        Global.ambientTemperature = ambientTemperature;
        Global.ambientDissipation = ambientDissipation;

        // Llenar los datos de las celdas
        FillCellData();

        // Configurar el shader y despachar
        computeShader.SetInt("width", textureWidth);
        computeShader.SetInt("height", textureHeight);
        computeShader.SetFloat("ambientTemperature", ambientTemperature);
        computeShader.SetFloat("ambientDissipation", ambientDissipation);
        computeShader.SetFloat("dampingFactor", dampingFactor);

        // Copiar datos de las celdas a la textura actual
        ComputeBuffer cellBuffer = new ComputeBuffer(cells.Length, sizeof(float) * 2 + sizeof(int));
        cellBuffer.SetData(cells);
        computeShader.SetBuffer(0, "cells", cellBuffer);

        computeShader.SetTexture(0, "CurrentTemperatures", currentTemperatureRT);
        computeShader.SetTexture(0, "Result", nextTemperatureRT);

        int threadGroupsX = Mathf.CeilToInt(textureWidth / 8f);
        int threadGroupsY = Mathf.CeilToInt(textureHeight / 8f);

        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Intercambiar las texturas
        RenderTexture temp = currentTemperatureRT;
        currentTemperatureRT = nextTemperatureRT;
        nextTemperatureRT = temp;

        // Leer los resultados
        Texture2D resultTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RFloat, false);
        RenderTexture.active = currentTemperatureRT;
        resultTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        resultTexture.Apply();
        RenderTexture.active = null;

        float[] resultTemperatures = resultTexture.GetRawTextureData<float>().ToArray();

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                int index = y * textureWidth + x;
                var cell = currentChunk[x, y];
                if (cell != null)
                {
                    cell.temperature = resultTemperatures[index];
                }
            }
        }

        cellBuffer.Release();
    }

    private void FillCellData()
    {
        if (currentChunk == null) return;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                int index = y * textureWidth + x;
                var cell = currentChunk[x, y];
                cells[index] = new TemperatureCellData
                {
                    temperature = cell.temperature,
                    thermalConductivity = cell.thermalConductivity,
                    material = (int)cell.material
                };
            }
        }
    }

    private void UpdateDebugTexture()
    {
        if (currentChunk == null) return;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                var cell = currentChunk[x, y];
                if (cell != null)
                {
                    float temperature = cell.temperature;
                    Color color = GetTemperatureColor(temperature);
                    debugTexture.SetPixel(x, y, color);
                }
            }
        }
        debugTexture.Apply();
    }

    private Color GetTemperatureColor(float temperature)
    {
        Color coldColor = Color.cyan;
        Color coolColor = Color.blue;
        Color neutralColor = Color.green;
        Color warmColor = Color.red;
        Color hotColor = new Color(1f, 0f, 1f);

        if (temperature <= -20)
        {
            return coldColor;
        }
        else if (temperature <= 0)
        {
            float t = Mathf.InverseLerp(-20, 0, temperature);
            return Color.Lerp(coldColor, coolColor, t);
        }
        else if (temperature <= 20)
        {
            float t = Mathf.InverseLerp(0, 20, temperature);
            return Color.Lerp(coolColor, neutralColor, t);
        }
        else if (temperature <= 100)
        {
            float t = Mathf.InverseLerp(20, 100, temperature);
            return Color.Lerp(neutralColor, warmColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(100, 120, temperature);
            return Color.Lerp(warmColor, hotColor, t);
        }
    }

    private void TriggerMaterialChanges()
    {
        if (currentChunk == null) return;

        for (int y = 0; y < currentChunk.pixelSize.y; y++)
        {
            for (int x = 0; x < currentChunk.pixelSize.x; x++)
            {
                var cell = currentChunk[x, y];

                if (cell.material != CellMaterial.Empty && cell.elapsedLifetime > 1)
                {
                    var globalPixelPosition = currentChunk.pixelPosition + new Vector2Int(x, y);
                    CheckAndTriggerChange(cell, globalPixelPosition);
                }
            }
        }
    }

    private void CheckAndTriggerChange(Cell cell, Vector2Int globalPixelPosition)
    {
        float temperature = cell.temperature;
        CellMaterialProperties properties = cell.materialProperties;

        if (temperature >= properties.heatPoint && properties.heatMaterial != CellMaterial.Empty)
        {
            cellularAutomata.CreateCell(globalPixelPosition, properties.heatMaterial);
        }
        else if (temperature <= properties.coldPoint && properties.coldMaterial != CellMaterial.Empty)
        {
            cellularAutomata.CreateCell(globalPixelPosition, properties.coldMaterial);
        }
    }
}
