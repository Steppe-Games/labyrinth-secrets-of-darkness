using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Labyrinth Settings")]
    public class LabyrinthSettings : ScriptableObject {

        public TileProperties[] tiles;

    }

    [Serializable]
    public class TileProperties {

        public TileBase tileBase;
        public bool blocking;
        public bool castShadows;
        public GameObject spawnObject;

    }

}