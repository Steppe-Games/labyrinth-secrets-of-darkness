using System;
using Scriptable_Objects;
using UnityEngine;

namespace Common {

    public class CameraController : MonoBehaviour {

        //@formatter:off
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private Transform playerTransform;
        //@formatter:on

        private void Awake() {
            GetComponent<Camera>().orthographicSize = gameSettings.cameraSize;
            
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        }

    }

}