using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CharacterParameters : MonoBehaviour
{

    static Color defaultHairColor => new Color32(251, 242, 54, 255);
    static Color defaultEyesColor => Color.black;
    static Color defaultSkinColor => new Color32(238, 195, 154, 255);
    static Color defaultFeetColor => new Color32(102, 57, 49, 255);
    static Color defaultClothesColor => new Color32(55, 148, 110, 255);

    [Header("Parameters")]
    [Range(0, 10)] public int hair = 4;
    [Range(0, 7)] public int beard = 0;
    [Range(0, 2)] public int rightEye = 1;
    [Range(0, 2)] public int leftEye = 1;
    [Range(0, 1)] public int leftHand = 1;
    [Range(0, 1)] public int rightHand = 1;
    [Range(0, 1)] public int leftFoot = 1;
    [Range(0, 1)] public int rightFoot = 1;
    [Range(0, 8)] public int head = 3;
    [Range(0, 9)] public int torso = 3;
    [Range(0, 3)] public int legs = 3;

    [Header("Colors")]
    public Color hairColor = defaultHairColor;
    public Color beardColor = defaultHairColor;
    public Color rightEyeColor = defaultEyesColor;
    public Color leftEyeColor = defaultEyesColor;
    public Color leftHandColor = defaultSkinColor;
    public Color rightHandColor = defaultSkinColor;
    public Color leftFootColor = defaultFeetColor;
    public Color rightFootColor = defaultFeetColor;
    public Color headColor = defaultSkinColor;
    public Color torsoColor = defaultClothesColor;
    public Color legsColor = defaultClothesColor;

    SpriteRenderer[] spriteRenderers = null;

    private void OnValidate()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && Application.isPlaying)
        {
            Randomize();
        }

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.sharedMaterial.shader.name != "Atto/GrayscaleMask")
            {
                var tempMaterial = Resources.Load<Material>("Shaders/GrayscaleMask");
                spriteRenderer.sharedMaterial = tempMaterial;
            }

            if (spriteRenderer != null && spriteRenderer.sharedMaterial.HasProperty("_Threshold"))
            {
                var parameterName = spriteRenderer.gameObject.name;
                var currentParameter = 0f;
                var currentColor = Color.magenta;
                switch (parameterName)
                {
                    case "Hair": currentParameter = hair; currentColor = hairColor; break;
                    case "Beard": currentParameter = beard; currentColor = beardColor; break;
                    case "RightEye": currentParameter = rightEye; currentColor = rightEyeColor; break;
                    case "LeftEye": currentParameter = leftEye; currentColor = leftEyeColor; break;
                    case "RightHand": currentParameter = rightHand; currentColor = rightHandColor; break;
                    case "LeftHand": currentParameter = leftHand; currentColor = leftHandColor; break;
                    case "RightFoot": currentParameter = rightFoot; currentColor = rightFootColor; break;
                    case "LeftFoot": currentParameter = leftFoot; currentColor = leftFootColor; break;
                    case "Head": currentParameter = head; currentColor = headColor; break;
                    case "Torso": currentParameter = torso; currentColor = torsoColor; break;
                    case "Legs": currentParameter = legs; currentColor = legsColor; break;
                }
                spriteRenderer.sharedMaterial.SetFloat("_Threshold", currentParameter * 0.1f+0.025f);
                spriteRenderer.color = currentColor;
            }
        }
    }

    public void Randomize()
    {
        hair = Random.Range(0, 11);
        beard = Random.Range(0, 8);
        rightEye = Random.Range(1, 3);
        leftEye = Random.Range(1, 3);
        leftHand = Random.Range(1, 2);
        rightHand = Random.Range(1, 2);
        leftFoot = Random.Range(1, 2);
        rightFoot = Random.Range(1, 2);
        head = Random.Range(1, 9);
        torso = Random.Range(1, 10);
        legs = Random.Range(1, 4);

        hairColor = RandomColor();
        beardColor = hairColor;
        rightEyeColor = RandomColor();
        leftEyeColor = rightEyeColor;
        leftHandColor = RandomColor();
        rightHandColor = leftHandColor;
        leftFootColor = RandomColor();
        rightFootColor = leftFootColor;
        headColor = RandomColor();
        torsoColor = RandomColor();
        legsColor = RandomColor();
    }

    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }

}
