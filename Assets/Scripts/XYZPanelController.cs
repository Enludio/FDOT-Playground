using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class XYZPanelController : MonoBehaviour
{
    [Tooltip("Defaults to Camera.main if left empty.")]
    [SerializeField] private Camera targetCamera;

    [Tooltip("Total travel in meters per axis (X, Y, Z), centered on the camera's starting position.")]
    [SerializeField] private Vector3 rangeMeters = new Vector3(1000f, 100f, 1000f);

    private Vector3 origin;

    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        origin = targetCamera.transform.position;
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Setup(root.Q<Slider>("slider-x"), rangeMeters.x, 0);
        Setup(root.Q<Slider>("slider-y"), rangeMeters.y, 1);
        Setup(root.Q<Slider>("slider-z"), rangeMeters.z, 2);
    }

    // Each slider spans -range/2 .. +range/2 (meters), 0 = camera's starting position.
    private void Setup(Slider slider, float range, int axis)
    {
        slider.lowValue = -range * 0.5f;
        slider.highValue = range * 0.5f;
        slider.SetValueWithoutNotify(0f);

        slider.RegisterValueChangedCallback(evt =>
        {
            Vector3 pos = origin;
            pos[axis] += evt.newValue;
            // Keep the other axes at their current slider-driven offsets
            Vector3 current = targetCamera.transform.position;
            for (int i = 0; i < 3; i++)
                if (i != axis) pos[i] = current[i];
            targetCamera.transform.position = pos;
        });
    }
}
