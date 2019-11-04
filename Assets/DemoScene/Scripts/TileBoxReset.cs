using UnityEngine;

namespace CustomStage.DemoScene
{
    public class TileBoxReset : MonoBehaviour {
        [SerializeField]
        private TileManager tileManager;

        void OnTriggerEnter(Collider other) {
            tileManager.UpdateTile();
        }
    }
}