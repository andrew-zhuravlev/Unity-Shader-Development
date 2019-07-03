using UnityEngine;

namespace GOL {
    public class GOLVisualizer : MonoBehaviour {

        [SerializeField] int gridSize = 100;
        [SerializeField] Renderer rend;
        [SerializeField] float updateTime = 1f;

        Texture2D texture;
        Color[] colors;

        GOLLogic logic;

        float curTime = 0f;

        void Start() {
            texture = new Texture2D(gridSize, gridSize) {
                filterMode = FilterMode.Point,
                anisoLevel = 1
            };

            colors = new Color[gridSize * gridSize];
            rend.material.mainTexture = texture;

            // TODO: DI
            logic = new GOLLogic(gridSize, true);
        }

        void Update() {
            //curTime += Time.deltaTime;
            //if(curTime >= updateTime) {
                //curTime -= updateTime;

                logic.NextGeneration();
                UpdateTexture();
            //}
        }

        

        void UpdateTexture() {
            bool[,] currentGeneration = logic.GetCurrentGeneration();
            for (int r = 0; r < gridSize; ++r) {
                for (int c = 0; c < gridSize; ++c) {
                    bool isAliveCell = currentGeneration[r, c];
                    colors[r * gridSize + c] = isAliveCell ? Color.black : Color.white;
                }
            }

            texture.SetPixels(colors);

            //for (int r = 0; r < gridSize; ++r) {
            //    for (int c = 0; c < gridSize; ++c) {
            //        bool isAliveCell = cells[r, c];
            //        texture.SetPixel(r, c, isAliveCell ? Color.black : Color.white);
            //    }
            //}

            texture.Apply();
        }
    }
}
