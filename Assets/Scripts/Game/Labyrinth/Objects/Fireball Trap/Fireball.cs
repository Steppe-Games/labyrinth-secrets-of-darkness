using Game.Player;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Labyrinth.Objects.Fireball_Trap {

    public class Fireball : MonoBehaviour {

        //@formatter:off
        [SerializeField] private TrapsSettings trapSettings;
        //@formatter:on
        
        private Light2D light2D;
        private void Awake() {
            light2D = GetComponent<Light2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerController>().TakeHit(trapSettings.fireballDamage);
                Destroy(gameObject);
                return;
            }

            if (other.CompareTag("Wall")) {
                Destroy(gameObject);
            }
        }
   }
}