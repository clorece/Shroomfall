using UnityEngine;
using UnityEngine.UI;

public class HueSlider : MonoBehaviour
{
    public Slider slider;     // assign a UI Slider (min 0, max 1, wholeNumbers=false)
    public Image  bgImage;    // the slider's background Image to show the hue gradient
    public Image  handle;     // optional: tint the handle to current color

    public System.Action<float> onHueChanged;  // hue in [0..1]

    void Awake(){
        if (!slider) slider = GetComponentInChildren<Slider>();
        if (slider){
            slider.minValue = 0f; slider.maxValue = 1f; slider.wholeNumbers = false;
            slider.onValueChanged.AddListener(OnChanged);
        }
        ApplyGradientToBackground();
        OnChanged(slider ? slider.value : 0f);
    }

    void ApplyGradientToBackground(){
        if (!bgImage) return;
        const int W = 512, H = 1;
        var tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int x = 0; x < W; x++){
            float h = x / (W - 1f);
            var c = Color.HSVToRGB(h, 1f, 1f);
            tex.SetPixel(x, 0, c);
        }
        tex.Apply(false, false);
        // make a sprite so we can assign to an Image
        var sp = Sprite.Create(tex, new Rect(0,0,W,H), new Vector2(0.5f,0.5f), 1f);
        bgImage.sprite = sp;
        bgImage.type = Image.Type.Sliced; // keeps it from stretching weirdly
        bgImage.pixelsPerUnitMultiplier = 1f;
        bgImage.color = Color.white;
    }

    void OnChanged(float h){
        h = Mathf.Repeat(h, 1f);
        if (handle) handle.color = Color.HSVToRGB(h, 1f, 1f);
        onHueChanged?.Invoke(h);
    }

    // optional: set from code (e.g., load/save/randomize)
    public void SetHue(float h, bool notify=true){
        if (!slider) return;
        if (notify) slider.value = h;
        else slider.SetValueWithoutNotify(h);
        OnChanged(slider.value);
    }
}
