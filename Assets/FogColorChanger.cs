using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using HauntedPSX.RenderPipelines.PSX.Runtime;

public class FogColorChanger : MonoBehaviour
{
    public float lerpSpeed = 0f;
    float tempSpeed = 0f;
    Volume v; 
    FogVolume f;
    int colorIndex = 0;
    int colorLen = 4;
    Color[] colors;
    public Color col1;
    public Color col2;
    public Color col3;
    // Start is called before the first frame update
    void Start()
    {
        colors = new Color[4];


        v = GetComponent<Volume>();
        if (v.profile.TryGet<FogVolume>(out FogVolume fogVolume))
        {
            // Found fog volume, cache off reference.
            f = fogVolume;
        }

        colors[0] = f.color.value;
        colors[1] = col1;
        colors[2] = col2;
        colors[3] = col3;
        tempSpeed = lerpSpeed;
    }

    void Update()
    {
        Color nextColor = colors[(colorIndex + 1) % colorLen];
        if (f.color.value == nextColor)
        {
            tempSpeed = lerpSpeed;
            colorIndex = (colorIndex + 1) % colorLen;
        }
        else
        {
            f.color.value = Color.Lerp(f.color.value, nextColor, tempSpeed);
            tempSpeed += 0.05f * Time.deltaTime;
        }
    }
}
