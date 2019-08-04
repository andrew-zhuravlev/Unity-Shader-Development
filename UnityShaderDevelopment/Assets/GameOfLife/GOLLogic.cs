using System;
using Random = UnityEngine.Random;

namespace GOL {
    public class GOLLogic {

        public GOLLogic(int gridSize, bool populateRandomly) {
            this.gridSize = gridSize;

            currentCells = new bool[gridSize, gridSize];
            nextGenCells = new bool[gridSize, gridSize];

            if(populateRandomly)
                PopulateRandomly(currentCells, gridSize);
        }

        bool[,] currentCells;
        bool[,] nextGenCells;
        int gridSize;

        public bool[,] GetCurrentGeneration() {
            return currentCells;
        }

        public void NextGeneration() {
            for (int r = 0; r < gridSize; ++r) {
                for (int c = 0; c < gridSize; ++c) {
                    bool isAlive = currentCells[r, c];

                    int aliveNeighbours = GetNeighbours(currentCells, r, c);

                    if (isAlive) {
                        // Die
                        if (aliveNeighbours < 2 || aliveNeighbours >= 4)
                            nextGenCells[r, c] = false;
                        else
                            nextGenCells[r, c] = true;
                    }
                    else if (aliveNeighbours == 3)
                        nextGenCells[r, c] = true;
                    else
                        nextGenCells[r, c] = false;
                }
            }

            Array.Copy(nextGenCells, currentCells, nextGenCells.Length);
        }

        static int GetNeighbours(bool[,] cells, int r, int c) {
            int result = 0;
            for (int row = -1; row <= 1; ++row) {
                for (int col = -1; col <= 1; ++col) {

                    if (!(row == 0 && col == 0) && IsInsideBounds(cells, r + row, c + col) && cells[r + row, c + col])
                        ++result;
                }
            }
            return result;
        }

        static bool IsInsideBounds(bool[,] cells, int r, int c) {
            return r >= 0 && r < cells.GetLength(0) && c >= 0 && c < cells.GetLength(1);
        }

        static void PopulateRandomly(bool[,] cells, int gridSize) {
            for (int r = 0; r < gridSize; ++r) {
                for (int c = 0; c < gridSize; ++c) {
                    cells[r, c] = Random.Range(0, 2) == 1;
                }
            }
        }
    }
}