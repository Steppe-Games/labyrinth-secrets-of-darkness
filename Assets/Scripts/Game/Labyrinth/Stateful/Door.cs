using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Labyrinth.Stateful {

    public class Door : MonoBehaviour {

        //@formatter:off
        [Header("Door Settings")] 
        public bool isClosed = true;

        public GameObject openDoor;
        public GameObject closedDoor;
        //@formatter:off

        private void Awake() {
            Redraw();
        }

        public void ToggleDoor() {
            isClosed = !isClosed;

            Redraw();
        }

        private void Redraw() {
            openDoor.SetActive(!isClosed);
            closedDoor.SetActive(isClosed);
        }

    }

}