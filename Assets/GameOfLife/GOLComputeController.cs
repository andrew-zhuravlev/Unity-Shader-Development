using UnityEngine;

namespace GOL {
    public class GOLComputeController : MonoBehaviour {

        [SerializeField] int textureSize;
        [SerializeField] ComputeShader golShader;
        [SerializeField] Renderer rend;

        // The next state of the game of life, based on the back texture. Will be drawn on screen.
        RenderTexture frontTexture;
        // Holds the previous state of the game of life.
        RenderTexture backTexture;

        int NextGenerationKernel;
        int CopyTextureKernel;
        // Create texture from int[].
        int CreateTextureFromCellsKernel;

        void Start() {
            frontTexture = CreateRandomWriteTexture(textureSize);
            backTexture = CreateRandomWriteTexture(textureSize);

            NextGenerationKernel = golShader.FindKernel("NextGeneration");
            CopyTextureKernel = golShader.FindKernel("CopyTexture");
            CreateTextureFromCellsKernel = golShader.FindKernel("CreateTextureFromCells");

            int[] createdCells = CreateRandomCells(textureSize, textureSize);
            var computeBuffer = new ComputeBuffer(createdCells.Length, sizeof(int));
            computeBuffer.SetData(createdCells);
            golShader.SetBuffer(CreateTextureFromCellsKernel, "CellsBuffer", computeBuffer);
            golShader.SetInt("CellsBufferWidth", textureSize);

            golShader.SetTexture(NextGenerationKernel, "FrontBuffer", frontTexture);
            golShader.SetTexture(CopyTextureKernel, "FrontBuffer", frontTexture);

            golShader.SetTexture(NextGenerationKernel, "BackBuffer", backTexture);
            golShader.SetTexture(CopyTextureKernel, "BackBuffer", backTexture);
            golShader.SetTexture(CreateTextureFromCellsKernel, "BackBuffer", backTexture);

            int threadGroupsX = Mathf.CeilToInt(textureSize / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(textureSize / 8.0f);
            golShader.Dispatch(CreateTextureFromCellsKernel, threadGroupsX, threadGroupsY, 1);

            rend.material.mainTexture = backTexture;

            computeBuffer.Release();
        }

        void Update() {
            NextGeneration();
        }

        void NextGeneration() {
            int threadGroupsX = Mathf.CeilToInt(textureSize / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(textureSize / 8.0f);
            golShader.Dispatch(NextGenerationKernel, threadGroupsX, threadGroupsY, 1);
            golShader.Dispatch(CopyTextureKernel, threadGroupsX, threadGroupsY, 1);
        }

        static int[] CreateRandomCells(int rows, int columns) {
            int[] result = new int[rows * columns];
            for(int i = 0; i < result.Length; ++i) {
                result[i] = Random.Range((int)0, 2);
            }
            return result;
        }

        static RenderTexture CreateRandomWriteTexture(int textureSize) {
            RenderTexture result = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat,
                                                                            RenderTextureReadWrite.Linear);
            result.filterMode = FilterMode.Point;
            result.enableRandomWrite = true;
            result.Create();

            return result;
        }
    }
}