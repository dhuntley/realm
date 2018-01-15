using UnityEngine;

public class ProgressBar : MonoBehaviour {

    public float total = 100;

    public float progress = 0;

    private Transform foreground;

    void Start() {
        foreground = transform.GetChild(0);
    }

    void OnEnable() {
        UpdateBar();
    }

    // Update is called once per frame
    void Update() {
        UpdateBar();
    }

    void UpdateBar() {
        if (foreground) {
            Vector3 scale = foreground.localScale;
            scale.x = (progress / total);
            foreground.localScale = scale;

            Vector3 position = foreground.localPosition;
            position.x = -0.5f + (scale.x / 2);
            foreground.localPosition = position;
        }
    }
}
