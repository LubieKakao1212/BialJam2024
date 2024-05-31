using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.Mathematics.math;

namespace Weather
{
    public class Simulation : MonoBehaviour
    {
        [SerializeField]
        private int2 size = new int2(100, 50);

        [SerializeField]
        private uint seed = 1337;

        [SerializeField]
        private int iterations = 4;

        private State state;

        [SerializeField]
        public Texture2D texturePressure;
        [SerializeField]
        public Texture2D textureTemperature;
        [SerializeField]
        public Texture2D textureVelocity;

        [SerializeField]
        private float timeScale = 1f;

        [SerializeField]
        public float decay = 0.999f;

        [SerializeField]
        private float diffusionP = 0.25f;
        [SerializeField]
        private float diffusionT = 0.25f;

        [SerializeField]
        private float temperaturePressureGen;

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

                state.grid[i] = new Cell() {
                    p = 0f,// noise.cnoise(new float2(xNorm, yNorm) * 2f),
                    t = yNorm < 0.05f && yNorm > -0.045f ? 1f : 0f
                };
            }

            /*for (int i=0; i < state.vX.Length; i++)
            {
                state.vX[i] = 1f;
            }*/

            FillDebug();
        }

        private void Update()
        {
            /*var c = state.grid[2525];
            c.t = 2f;
            state.grid[2525] = c;

            c = state.grid[5500];
            c.p = -4f;
            state.grid[5500] = c;*/

            float dt = Time.fixedDeltaTime * timeScale;
            Scheduler.Tick(ref state, dt, temperaturePressureGen, decay, 
                1f - pow(0.5f, dt * diffusionP),
                1f - pow(0.5f, dt * diffusionT),
                iterations);
            
            FillDebug();
        }

        private void FillDebug()
        {
            var pixels = new Color[size.x * size.y];

            if (texturePressure != null)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    var p = state.grid[i].p;
                    var p10 = p * 10;
                    pixels[i] = new Color(p, -Mathf.Cos(p10), -p);
                }

                texturePressure.SetPixels(pixels);
                texturePressure.Apply();
            }

            if(textureTemperature != null)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    var p = state.grid[i].t;
                    var p10 = p * 10;
                    var lp10 = log(p10);
                    pixels[i] = new Color(lp10, -Mathf.Cos(p10), -lp10);
                }

                textureTemperature.SetPixels(pixels);
                textureTemperature.Apply();
            }

            if (textureVelocity != null)
            {
                for (int y = 0; y < size.y; y++)
                    for (int x = 0; x < size.x; x++)
                    {
                        var pIdx = y * size.x + x;
                        if (x == size.x - 1 || y == size.y - 1)
                        {
                            pixels[pIdx] = new Color(1f, 1f, 1f);
                        }
                        else
                        {
                            var vXIdx = pIdx - y;
                            var vYIdx = pIdx;
                            pixels[pIdx] = new Color(state.vX[vXIdx], state.vY[vYIdx], 0f);
                        }
                    }

                textureVelocity.SetPixels(pixels);
                textureVelocity.Apply();
            }
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
            public static void Tick(ref State state, float dt, in float temperatiurePressureGen, float decay, float diffusionFactorP, float diffusionFactorT, int iterations)
            {
                var next = state.Copy(Allocator.Temp);
                var size = next.size;

                var ddt = dt * dt / 2f;

                Profiler.BeginSample("Pressure Gradient");
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

                        #region Swirl
                        var x1 = (x - size.x / 2f) * 2f / size.x;
                        var y1 = (y - size.y / 2f) * 2f / size.y;
                        var theta = atan2(y1, x1);
                        var d = sqrt(x1 * x1 + y1 * y1);
                        var distanceFactor = d < 0.5f && d > 0.1f ? 1f : 0f;// ( / 10f);
                        //distanceFactor = abs(distanceFactor) + 5f;
                        vXdt = -sin(theta) * dt * distanceFactor;
                        vYdt = cos(theta) * dt * distanceFactor;
                        #endregion

                        //TODO Revert
                        var dXddt = 0;// dX * ddt;
                        var dYddt = 0;// dY * ddt;

                        var xd = vXdt + dXddt;
                        var yd = vYdt + dYddt;

                        nextCell.p += xd;
                        nextCell.p += yd;
                        nextCell.t += xd * 5f;
                        nextCell.t += yd * 5f;
                        //Move(ref nextCell.t, ref nextCellRight.t, xd / 2f, -1f, float.PositiveInfinity);
                        //Move(ref nextCell.t, ref nextCellDown.t, yd / 2f, -1f, float.PositiveInfinity);
                        //nextCell.t = ;max(nextCell.t, 0f);
                        next.grid[gridIdx] = nextCell;

                        nextCellRight.p -= xd;
                        nextCellRight.t -= xd * 5f;
                        next.grid[gridIdx + 1] = nextCellRight;

                        nextCellDown.p -= yd;
                        nextCellDown.t -= yd * 5f;
                        next.grid[gridIdx + size.x] = nextCellDown;

                        vX += dX * dt;
                        vY += dY * dt;

                        //TODO Revert
                        next.vX[velXIdx] = vXdt / dt;//vX;
                        next.vY[velYIdx] = vYdt / dt;//vY;
                    }
                Profiler.EndSample();


                Profiler.BeginSample("Pressurisation");
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
                Profiler.EndSample();


                Profiler.BeginSample("Decay");
                #region decay
                for (int i = 0; i < next.vX.Length; i++)
                {
                    next.vX[i] *= decay;
                }
                for (int i = 0; i < next.vY.Length; i++)
                {
                    next.vY[i] *= decay;
                }
                #endregion
                Profiler.EndSample();

                Profiler.BeginSample("Influence");
                #region influence
                for (int i = 0; i < next.grid.Length; i++)
                {
                    var cell = next.grid[i];
                    //sign(t)*p > |t| -> 0
                    //-1 * p > |t|
                    //1 * p > |t|
                    //sign(f) -> sign(t)
                    //
                    cell.p += CalculateTemperaturePressureGenerationValue(cell.p, cell.t, temperatiurePressureGen);//temperatiurePressureGen.Evaluate(cell.t) * dt;
                    next.grid[i] = cell;
                }
                #endregion
                Profiler.EndSample();


                Profiler.BeginSample("Clear Border");
                #region Clear Border
                for (int x = 0; x < size.x; x++)
                {
                    next.grid[x] = new Cell() { p = -2f };
                    next.grid[size.x * (size.y - 1) + x] = new Cell() { p = 2f };
                }

                for (int y = 0; y < size.y; y++)
                {
                    next.grid[size.x * y] = new Cell() { p = -2f };
                    next.grid[size.x * y + size.x - 1] = new Cell() { p = 2f };
                }
                #endregion
                Profiler.EndSample();

                state.Dispose();
                state = next;
                next = state.Copy(Allocator.Persistent);
                
                Profiler.BeginSample("Diffuse");
                #region Diffuse
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
                                var w = 1f;//1f / (Mathf.Sqrt(i * i + j * j) + 1);
                                ws += w;
                                p += c1.p * w;
                            }

                        p /= ws;

                        var t = 0f;
                        ws = 0f;

                        for (int j = -1; j < 2; j++)
                            for (int i = -1; i < 2; i++)
                            {
                                var c1 = state.grid[gridIdx + i + j * size.x];
                                var w = 1f;//1f / (Mathf.Sqrt(i * i + j * j) + 1);
                                ws += w;
                                t += c1.t * w;
                            }

                        t /= ws;

                        var cell = state.grid[gridIdx];
                        cell.p = lerp(cell.p, p, diffusionFactorP);
                        cell.t = lerp(cell.t, t, diffusionFactorT);
                        next.grid[gridIdx] = cell;
                    }
                #endregion
                Profiler.EndSample();
                state.Dispose();
                state = next;
            }

            private static void Move(ref float a, ref float b, float delta, float lowerBound, float upperBound) {
                if (delta < 0)
                {
                    var da = upperBound - a;
                    var db = b - lowerBound;

                    delta = min(-delta, min(da, db));

                    a -= delta;
                    b += delta;
                }
                else
                {
                    var da = a - lowerBound;
                    var db = upperBound - b;

                    delta = min(delta, min(da, db));

                    a -= delta;
                    b += delta;
                }
            }

            private static float CalculateTemperaturePressureGenerationValue(float p, float t, float scale, float tScale = 1f, float tOffset = 0f)
            {
                t = t * tScale + tOffset;
                var ta = abs(t);
                var ts = sign(t);
                var c = ts * p - ta;
                if (c > 0)
                {
                    return 0;
                }

                //TODO check if "+ 1f" is correct
                var a = (atan2(ts * c, ta) / PI * 8f + 1f) / 3;
                var b = clamp(abs(a), 0f, 1f);

                var s = 1f / (1f + exp(-(1f/b + 1/(b - 1f))));

                return s * t * scale;
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

    [Serializable]
    public struct Sigmoid
    {
        public float a;
        public float low;
        public float high;

        public float Evaluate(float x)
        {
            return Evaluate(x, a, low, high);
        }

        private static float Evaluate(float x, float a, float low, float high)
        {
            var c = high - low;
            return c / (1 + pow(E, -a * x)) + low;
        }
    }
}
