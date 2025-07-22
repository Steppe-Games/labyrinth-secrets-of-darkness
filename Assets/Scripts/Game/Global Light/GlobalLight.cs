using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GlobalLight : MonoBehaviour {

    private void Awake() {
        var globalLight = GetComponent<Light2D>();
        globalLight.intensity = 0.01f;
    }

}