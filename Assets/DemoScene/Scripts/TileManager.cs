using UnityEngine;
using DG.Tweening;

namespace CustomStage.DemoScene
{
    public class TileManager : MonoBehaviour {

        private static TileManager s_instance;

        [SerializeField]
        private Transform[] m_tiles;
        [SerializeField]
        private float m_tileSize = 4;

        [SerializeField]
        private float m_speed = 20f;

        [SerializeField]
        private float m_elevationDuration = 2.0f;

        [SerializeField]
        private Ease m_elevationEase = Ease.Unset;

        private int currentIndex = 0;

        private float lastZ = 0;

        private Vector3[] defaultPositions;
        private Vector3 defaultStagePos;
        
        private Transform myTransform;
        private GameObject myGameObject;

        public new Transform transform {
            get {
                if(myTransform == null) myTransform = base.transform;
                return myTransform;
            }
        } 

        public new GameObject gameObject {
            get {
                if(myGameObject == null) myGameObject = base.gameObject;
                return myGameObject;
            }
        }  

        void Awake() {
            s_instance = this;
        }

        private void Start()
        {
            lastZ = m_tileSize * (m_tiles.Length - 1);
            defaultPositions = new Vector3[m_tiles.Length];

            for (int i = 0; i < m_tiles.Length; i++)
            {
                defaultPositions[i] = m_tiles[i].localPosition;
            }

            defaultStagePos = transform.position;
        }

        private void Update()
        {
            transform.Translate(Vector3.back * 1f * Time.deltaTime * (m_speed * 1.5f));
        }

        public void UpdateTile()
        {
            Transform tile = m_tiles[currentIndex];
            lastZ += m_tileSize;
            
            tile.DOKill();
            tile.DOLocalMoveY((m_tileSize * -1)/3, m_elevationDuration)
            .SetDelay(0)
            .SetEase(m_elevationEase)
            .OnComplete(() =>
            {
                TileUpdateComplete(tile, lastZ);
            });

            currentIndex++;
            if(currentIndex >= m_tiles.Length)
            {
                currentIndex = 0;
            }
        }

        private void TileUpdateComplete(Transform tile, float zval) {
            tile.DOLocalMove(new Vector3(
                tile.localPosition.x,
                (m_tileSize * -1)/3,
                zval
            ), 0)
            .OnComplete(() =>
            {
                tile.DOLocalMoveY(0, m_elevationDuration/3).SetEase(m_elevationEase);
            });
        }
    }
}
