using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ActorParameters : MonoBehaviour
{

    static Color defaultHairColor => new Color32(251, 242, 54, 255);
    static Color defaultEyesColor => Color.black;
    static Color defaultSkinColor => new Color32(238, 195, 154, 255);
    static Color defaultFeetColor => new Color32(102, 57, 49, 255);
    static Color defaultClothesColor => new Color32(55, 148, 110, 255);

    public int verticalLookDirection = 0;

    [Header("Parameters")]
    [Range(1, 4)] public int height = 2;
    [Range(0, 10)] public int hair = 4;
    [Range(0, 7)] public int beard = 0;
    [Range(0, 2)] public int rightEye = 1;
    [Range(0, 2)] public int leftEye = 1;
    [Range(0, 1)] public int leftHand = 1;
    [Range(0, 1)] public int rightHand = 1;
    [Range(0, 2)] public int leftFoot = 1;
    [Range(0, 2)] public int rightFoot = 1;
    [Range(0, 8)] public int head = 3;
    [Range(0, 10)] public int torso = 3;
    [Range(0, 5)] public int legs = 3;

    [Header("Colors")]
    public Color hairColor = defaultHairColor;
    public Color beardColor = defaultHairColor;
    public Color eyesColor = defaultEyesColor;
    public Color leftHandColor = defaultSkinColor;
    public Color rightHandColor = defaultSkinColor;
    public Color leftFootColor = defaultFeetColor;
    public Color rightFootColor = defaultFeetColor;
    public Color headColor = defaultSkinColor;
    public Color torsoColor = defaultClothesColor;
    public Color legsColor = defaultClothesColor;

    [HideInInspector] public Transform hairTransform;
    [HideInInspector] public Transform headTransform;
    [HideInInspector] public Transform torsoTransform;
    [HideInInspector] public Transform abdomenTransform;
    [HideInInspector] public Transform beardTransform;
    [HideInInspector] public Transform rightEyeTransform;
    [HideInInspector] public Transform leftEyeTransform;

    ParametricSprite[] parametricSprites = null;

    Actor character;

    private void Awake()
    {
        GetReferences();
    }

    private void Start()
    {
        parametricSprites = GetComponentsInChildren<ParametricSprite>();
    }

    private void OnValidate()
    {
        parametricSprites = GetComponentsInChildren<ParametricSprite>();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            GetReferences();
        }

        if (Input.GetKeyDown(KeyCode.O) && Application.isPlaying)
        {
            Randomize();
        }

        if (parametricSprites == null)
        {
            parametricSprites = GetComponentsInChildren<ParametricSprite>();
        }

        foreach (var parametricSprite in parametricSprites)
        {
            if (parametricSprite != null)
            {
                var parameterName = parametricSprite.gameObject.name;
                int currentParameter = 0;
                var currentColor = Color.magenta;
                switch (parameterName)
                {
                    case "Hair": currentParameter = hair; currentColor = hairColor; break;
                    case "Beard": currentParameter = beard; currentColor = beardColor; break;
                    case "RightEye": currentParameter = rightEye; currentColor = eyesColor; break;
                    case "LeftEye": currentParameter = leftEye; currentColor = eyesColor; break;
                    case "RightHand": currentParameter = rightHand; currentColor = rightHandColor; break;
                    case "LeftHand": currentParameter = leftHand; currentColor = leftHandColor; break;
                    case "RightFoot": currentParameter = rightFoot; currentColor = rightFootColor; break;
                    case "LeftFoot": currentParameter = leftFoot; currentColor = leftFootColor; break;
                    case "Head": currentParameter = head; currentColor = headColor; break;
                    case "Torso": currentParameter = torso; currentColor = torsoColor; break;
                    case "Abdomen": currentParameter = torso; currentColor = torsoColor; break;
                    case "Legs": currentParameter = legs; currentColor = legsColor; break;
                }

                parametricSprite.SetParametes(currentParameter, currentColor);
            }
        }

        SetLook();
        SetHeight(height);
    }

    private void SetLook()
    {
        verticalLookDirection = character.wantsToGoUp ? 1 : 0;
    }

    private void GetReferences()
    {
        if (character == null)         { character = GetComponentInParent<Actor>(); }
        if (hairTransform     == null) { hairTransform      = transform.Find("Hair"); }
        if (headTransform     == null) { headTransform      = transform.Find("Head"); }
        if (torsoTransform    == null) { torsoTransform     = transform.Find("Torso"); }
        if (abdomenTransform  == null) { abdomenTransform   = transform.Find("Abdomen"); }
        if (beardTransform    == null) { beardTransform     = transform.Find("Beard"); }
        if (rightEyeTransform == null) { rightEyeTransform  = transform.Find("RightEye"); }
        if (leftEyeTransform  == null) { leftEyeTransform   = transform.Find("LeftEye"); }
    }

    private void SetHeight(int height)
    {
        var topBodyPixelOffset = (height-2).PixelsToUnits();
        var youngHairPixelOffset = (height == 1) ? 1f / Global.pixelsPerUnit : 0;
        hairTransform.localPosition = new Vector3(0, topBodyPixelOffset + youngHairPixelOffset, 0);
        headTransform.localPosition = new Vector3(0, topBodyPixelOffset, 0);
        torsoTransform.localPosition = new Vector3(0, topBodyPixelOffset, 0);
        beardTransform.localPosition = new Vector3(0, topBodyPixelOffset, 0);
        rightEyeTransform.localPosition = new Vector3(0, topBodyPixelOffset + verticalLookDirection * 1f / Global.pixelsPerUnit , 0);
        leftEyeTransform.localPosition = new Vector3(0,  topBodyPixelOffset + verticalLookDirection * 1f / Global.pixelsPerUnit, 0);
        abdomenTransform.localPosition = new Vector3(0, -1.PixelsToUnits(), 0);
        abdomenTransform.localScale = new Vector3(1, Mathf.Max(0, height-1), 1);
        character.pixelSize = height + 1;
    }

    public void Randomize()
    {
        height = Random.Range(1, 4);
        hair = Random.Range(0, 11);
        beard = Random.Range(0, 8);
        rightEye = Random.value > 0.9f ? 2 : 1;
        leftEye = Random.value > 0.9f ? 2 : 1;
        leftHand = Random.Range(1, 2);
        rightHand = Random.Range(1, 2);
        leftFoot = Random.Range(1, 2);
        rightFoot = Random.Range(1, 2);
        head = Random.Range(1, 9);
        torso = Random.Range(1, 10);
        legs = Random.Range(1, 4);


        var skinColor = RandomColor(skinPalette);
        leftHandColor = skinColor;
        rightHandColor = skinColor;
        headColor = skinColor;

        hairColor = RandomColor(hairColors);
        beardColor = hairColor;

        eyesColor = RandomColor(eyeColors);

        torsoColor = RandomColor(ColorExtensions.db32Palette);
        legsColor = Random.value > 0.8f ? torsoColor : RandomColor(ColorExtensions.db32Palette);

        leftFootColor = Random.value > 0.8f ? torsoColor : RandomColor(ColorExtensions.db32Palette);
        rightFootColor = leftFootColor;
    }

    public Color RandomColor(Color[] palette)
    {
        int index = Random.Range(0, palette.Length);
        return palette[index];
    }

    private Color[] eyeColors = new Color[]
    {
        new Color32(102, 57, 49, 255), // Brown
        new Color32(143, 86, 59, 255), // Brown
        new Color32(159, 224, 80, 255), // Green
        new Color32(106, 190, 48, 255), // Green
        new Color32(75, 105, 47, 255), // Green
        new Color32(99, 155, 255, 255), // Blue
        new Color32(177, 216, 252, 255), // Blue
        new Color32(95, 115, 225, 255), // Blue
        new Color32(155, 173, 183, 255), // Grey
        new Color32(105, 106, 106, 255)  // Grey
    };

    private Color[] skinPalette = new Color[]
    {
        new Color32(22, 18, 35, 255),
        new Color32(56, 35, 58, 255),
        new Color32(82, 41, 34, 255),
        new Color32(120, 68, 49, 255),
        new Color32(138, 55, 43, 255),
        new Color32(224, 87, 36, 255),
        new Color32(229, 161, 106, 255),
        new Color32(248, 199, 164, 255)
    };

    private Color[] hairColors = new Color[]
    {
        new Color32(0, 0, 0, 255), // Black
        new Color32(102, 57, 49, 255), // Brown
        new Color32(143, 86, 59, 255), // Brown
        new Color32(218, 128, 66, 255), // Brown
        new Color32(217, 160, 102, 255), // Blonde
        new Color32(238, 195, 154, 255), // Blonde
        new Color32(172, 50, 50, 255), // Red
        new Color32(217, 87, 99, 255), // Red
        new Color32(134, 130, 131, 255), // Grey
        new Color32(137, 135, 106, 255)  // Grey
    };

}
