using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Weather
{
    public class Simulation : MonoBehaviour
    {
        [SerializeField]
        private int2 size = new int2(100, 50);

        [SerializeField]
        private uint seed = 1337;
        
        private State state;

        [SerializeField]
        public Texture2D texture;

        [SerializeField]
        private float timeScale = 1f;

        [SerializeField]
        public float decay = 0.999f;

        [SerializeField]
        private float diffusion = 0.25f;

        private void Start()
        {
            state = new State(Allocator.Persistent, size);

            var random = Unity.Mathematics.Random.CreateFromIndex(seed);

            for (int i = 0; i < size.x * size.y; i++)
            {
                var r = (random.NextFloat() -0.5f) * 2f;

                var x = (i % size.x);
                var xNorm = (x - size.x / 2f) * 2f / size.x;

                var y = (i / size.x);
                var yNorm = (y - size.y / 2f) * 2f / size.y;

                state.grid[i] = new Cell() { p = noise.cnoise(new float2(xNorm, yNorm) * 2f) };
            }

            for (int i=0; i < state.vX.Length; i++)
            {
                state.vX[i] = 4f;
            }

            FillDebug();
        }

        private void Update()
        {
            Scheduler.Tick(ref state, Time.deltaTime * timeScale, decay, 1f / diffusion, 16);

            if(texture != null) {
                FillDebug();
            }
        }

        private void FillDebug()
        {
            var pixels = new Color[size.x * size.y];

            for (int i = 0; i < pixels.Length; i++)
            {
                var p = state.grid[i].p;
                var p10 = p * 10;
                pixels[i] = new Color(p10, -Mathf.Cos(p10), -p10);
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }

        private void OnDestroy()
        {
            state.Dispose();
        }

        private struct Cell { public float p; public float t; };
        
        private struct State : IDisposable { 
            
            public NativeArray<Cell> grid;
            public NativeArray<float> vX; 
            public NativeArray<float> vY;

            public int2 size;

            public State(Allocator allocator, int2 size)
            {
                var cellCount = size.x * size.y;
                var velXCount = (size.x - 1) * size.y;
                var velYCount = size.x * (size.y - 1);
                grid = new NativeArray<Cell>(cellCount, allocator, NativeArrayOptions.UninitializedMemory);
                vX = new NativeArray<float>(velXCount, allocator, NativeArrayOptions.ClearMemory);
                vY = new NativeArray<float>(velYCount, allocator, NativeArrayOptions.ClearMemory);
                this.size = size;
            }

            public void Dispose()
            {
                grid.Dispose();
                vX.Dispose(); 
                vY.Dispose();
            }

            public State Copy(Allocator allocator)
            {
                var next = new State(allocator, size);
                grid.CopyTo(next.grid);
                vX.CopyTo(next.vX);
                vY.CopyTo(next.vY);
                return next;
            }
        }

        private class Scheduler
        {
            public static void Tick(ref State state, float dt, float decay, float diffusion, int iterations)
            {
                var next = state.Copy(Allocator.Temp);
                var size = next.size;

                var ddt = dt * dt / 2f;

                //TODO handle Bound
                for (int y = 0; y < size.y - 1; y++)
                    for (int x = 0; x < size.x - 1; x++)
                    {
                        var gridIdx = y * size.x + x;
                        var velXIdx = y * (size.x - 1) + x;
                        var velYIdx = y * size.x + x;

                        var cell = state.grid[gridIdx];
                        var nextCell = next.grid[gridIdx];

                        var cellRight = state.grid[gridIdx + 1];
                        var cellDown = state.grid[gridIdx + size.x];
                        var nextCellRight = next.grid[gridIdx + 1];
                        var nextCellDown = next.grid[gridIdx + size.x];
                        
                        var dX = cellRight.p - cell.p;
                        var dY = cellDown.p - cell.p;

                        var vX = state.vX[velXIdx];
                        var vY = state.vY[velYIdx];

                        var vXdt = vX * dt;
                        var vYdt = vY * dt;

                        var dXddt = dX * ddt;
                        var dYddt = dY * ddt;

                        var p = nextCell.p;
                        p += vXdt + dXddt;
                        p += vYdt + dYddt;
                        nextCell.p = p;
                        next.grid[gridIdx] = nextCell;

                        var pr = nextCellRight.p;
                        pr -= vXdt + dXddt;
                        nextCellRight.p = pr;
                        next.grid[gridIdx + 1] = nextCellRight;

                        var pd = nextCellDown.p;
                        pd -= vYdt + dYddt;
                        nextCellDown.p = pd;
                        next.grid[gridIdx + size.x] = nextCellDown;

                        vX += dX * dt;
                        vY += dY * dt;

                        next.vX[velXIdx] = vX;
                        next.vY[velYIdx] = vY;
                    }

                //Pressurisation
                for (int i = 0; i < iterations; i++)
                {
                    state.Dispose();
                    state = next;
                    next = state.Copy(Allocator.Temp);

                    for (int y = 1; y < size.y - 1; y++)
                        for (int x = 1; x < size.x - 1; x++)
                        {
                            var velRIdx = y * (size.x - 1) + x;
                            var velDIdx = y * size.x + x;

                            var velLIdx = y * (size.x - 1) + x - 1;
                            var velUIdx = y * size.x + x - size.x;

                            var d = state.vX[velLIdx] + state.vY[velUIdx] - state.vX[velRIdx] - state.vY[velDIdx];

                            next.vX[velLIdx] -= d / 4;
                            next.vY[velUIdx] -= d / 4;
                            next.vX[velRIdx] += d / 4;
                            next.vY[velDIdx] += d / 4;
                        }
                }

                for (int i = 0; i < next.vX.Length; i++)
                {
                    next.vX[i] *= decay;
                }

                for (int i = 0; i < next.vY.Length; i++)
                {
                    next.vY[i] *= decay;
                }

                state.Dispose();
                state = next;
                next = state.Copy(Allocator.Persistent);
                if (float.IsFinite(diffusion))
                {
                    for (int y = 1; y < size.y - 1; y++)
                        for (int x = 1; x < size.x - 1; x++)
                        {
                            var gridIdx = y * size.x + x;
                            var velXIdx = y * (size.x - 1) + x;
                            var velYIdx = y * size.x + x;


                            var p = 0f;
                            var ws = 0f;

                            for (int j = -1; j < 2; j++)
                                for (int i = -1; i < 2; i++)
                                {
                                    var c1 = state.grid[gridIdx + i + j * size.x];
                                    var w = 1f / (Mathf.Sqrt(i * i + j * j) * diffusion + 1);
                                    ws += w;
                                    p += c1.p * w;
                                }

                            p /= ws;


                            var cell = state.grid[gridIdx];
                            cell.p = p;
                            next.grid[gridIdx] = cell;
                        }
                }

                state.Dispose();
                state = next;
            }

            /*public static

            private struct Sample
            {
                private Cell cell;
                private float vX;
                private float vY;
            }*/
        }
    }
}
