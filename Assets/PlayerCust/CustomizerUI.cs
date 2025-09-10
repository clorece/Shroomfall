using UnityEngine;
using UnityEngine.UI;            // only needed if you wire optional labels/images
// using TMPro;                  // uncomment if you use TMP labels

public class CustomizerUI_Sliders : MonoBehaviour
{
    [Header("Target character")]
    public ShroomCustomizerMPB shroom;

    [Header("Hue sliders")]
    public HueSlider bodyHue;   // pastel body
    public HueSlider capHue;    // full-sat cap

    [Range(0f,1f)] public float bodySaturation = 0.35f; // pastel feel

    // ---- indices we keep for Prev/Next ----
    [Header("Selections (runtime)")]
    public int eyesIdx = 0;
    public int mouthIdx = 0;

    // (Optional) UI readouts if you want to show names/counters
    // public TextMeshProUGUI eyesLabel;
    // public TextMeshProUGUI mouthLabel;

    void Awake()
    {
        // Hue → live color
        if (bodyHue) bodyHue.onHueChanged += h =>
            shroom.SetBodyColor(Color.HSVToRGB(h, bodySaturation, 1f));
        if (capHue)  capHue.onHueChanged  += h =>
            shroom.SetCapColor(Color.HSVToRGB(h, 1f, 1f)); // band auto-follows

        // Initialize selections
        ApplyEyes(eyesIdx);
        ApplyMouth(mouthIdx);
    }

    // ---------- Eyes ----------
    public void NextEyes() { ShiftEyes(+1); }
    public void PrevEyes() { ShiftEyes(-1); }

    void ShiftEyes(int delta)
    {
        int len = shroom.eyeVariants != null ? shroom.eyeVariants.Length : 0;
        if (len == 0) return;
        eyesIdx = Loop(eyesIdx + delta, len);
        ApplyEyes(eyesIdx);
    }

    void ApplyEyes(int i)
    {
        shroom.SetEyesIndex(i);
        // if (eyesLabel) eyesLabel.text = $"Eyes {i+1}/{shroom.eyeVariants.Length}";
    }

    // ---------- Mouth ----------
    public void NextMouth() { ShiftMouth(+1); }
    public void PrevMouth() { ShiftMouth(-1); }

    void ShiftMouth(int delta)
    {
        int len = shroom.mouthOptions != null ? shroom.mouthOptions.Length : 0;
        if (len == 0) return;
        mouthIdx = Loop(mouthIdx + delta, len);
        ApplyMouth(mouthIdx);
    }

    void ApplyMouth(int i)
    {
        shroom.SetMouthIndex(i);
        // if (mouthLabel) mouthLabel.text = $"Mouth {i+1}/{shroom.mouthOptions.Length}";
    }

    // ---------- Helpers ----------
    public void Randomize()
    {
        float hb = Random.value, hc = Random.value;
        if (bodyHue) bodyHue.SetHue(hb);
        if (capHue)  capHue.SetHue(hc);

        // pick random indices if arrays exist
        if (shroom.eyeVariants != null && shroom.eyeVariants.Length > 0)
            ApplyEyes(Random.Range(0, shroom.eyeVariants.Length));
        if (shroom.mouthOptions != null && shroom.mouthOptions.Length > 0)
            ApplyMouth(Random.Range(0, shroom.mouthOptions.Length));
    }

    int Loop(int v, int len) => (v % len + len) % len;
}
