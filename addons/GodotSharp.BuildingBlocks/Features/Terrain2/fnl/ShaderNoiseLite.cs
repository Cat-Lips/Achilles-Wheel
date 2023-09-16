// MIT License
//
// Copyright(c) 2023 Jordan Peck (jordan.me2@gmail.com)
// Copyright(c) 2023 Contributors
//
// VERSION: 1.1.0
// https://github.com/Auburn/FastNoiseLite

using System.Runtime.CompilerServices;
using Godot;
using FNLfloat = System.Single;

namespace GodotSharp.BuildingBlocks.Terrain2.Internal
{
    public enum NoiseType
    {
        Perlin,
        ValueCubic,
        Value
    };

    public enum FractalType
    {
        None,
        FBM,
        Ridged,
        PingPong,
    };

    [Tool, GlobalClass]
    public partial class ShaderNoiseLite : Resource
    {
        private const short INLINE = 256; // MethodImplOptions.AggressiveInlining;
        private const short OPTIMISE = 512; // MethodImplOptions.AggressiveOptimization;

        private int _seed = 0;
        private float _frequency = 0.01f;
        private NoiseType _noiseType = NoiseType.Perlin;

        private FractalType _fractalType = FractalType.Ridged;
        private int _octaves = 3;
        private float _gain = 0.5f;
        private float _lacunarity = 2.0f;
        private float _weightedStrength = 0.0f;
        private float _pingPongStrength = 2.0f;

        public event Action TypesChanged;
        public event Action ValuesChanged;

        private readonly AutoAction OnTypesChanged;
        private readonly AutoAction OnValuesChanged;

        [Export] public NoiseType NoiseType { get => _noiseType; set => Set(ref _noiseType, value, OnTypesChanged.Run); }
        [Export] public int Seed { get => _seed; set => Set(ref _seed, value, OnValuesChanged.Run); }
        [Export(PropertyHint.Range, "0,1")] public float Frequency { get => _frequency; set => Set(ref _frequency, Math.Clamp(value, 0, 1), OnValuesChanged.Run); }

        [ExportGroup("Fractal", "Fractal")]
        [Export] public FractalType FractalType { get => _fractalType; set => Set(ref _fractalType, value, notify: true, OnTypesChanged.Run); }
        [ExportGroup("Fractal")]
        [Export(PropertyHint.Range, "1,10")] public int Octaves { get => _octaves; set => Set(ref _octaves, Math.Clamp(value, 1, 10), CalculateFractalBounding, OnValuesChanged.Run); }
        [Export] public float Lacunarity { get => _lacunarity; set => Set(ref _lacunarity, value, OnValuesChanged.Run); }
        [Export] public float Gain { get => _gain; set => Set(ref _gain, value, CalculateFractalBounding, OnValuesChanged.Run); }
        [Export(PropertyHint.Range, "0,1")] public float WeightedStrength { get => _weightedStrength; set => Set(ref _weightedStrength, value, OnValuesChanged.Run); }
        [Export] public float PingPongStrength { get => _pingPongStrength; set => Set(ref _pingPongStrength, value, OnValuesChanged.Run); }

        private float fractalBounding;

        public ShaderNoiseLite()
        {
            OnTypesChanged = new(() => TypesChanged?.Invoke());
            OnValuesChanged = new(() => ValuesChanged?.Invoke());

            CalculateFractalBounding();
        }

        [MethodImpl(OPTIMISE)]
        public float GetNoise(FNLfloat x, FNLfloat y)
        {
            x *= Frequency;
            y *= Frequency;

            return FractalType switch
            {
                FractalType.FBM => GenFractalFBm(x, y),
                FractalType.Ridged => GenFractalRidged(x, y),
                FractalType.PingPong => GenFractalPingPong(x, y),
                FractalType.None => GenNoiseSingle(Seed, x, y),
                _ => throw new NotImplementedException(),
            };
        }

        #region Noise

        private static float SinglePerlin(int seed, FNLfloat x, FNLfloat y)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);

            var xd0 = (float)(x - x0);
            var yd0 = (float)(y - y0);
            var xd1 = xd0 - 1;
            var yd1 = yd0 - 1;

            var xs = InterpQuintic(xd0);
            var ys = InterpQuintic(yd0);

            x0 *= PrimeX;
            y0 *= PrimeY;
            var x1 = x0 + PrimeX;
            var y1 = y0 + PrimeY;

            var xf0 = Lerp(GradCoord(seed, x0, y0, xd0, yd0), GradCoord(seed, x1, y0, xd1, yd0), xs);
            var xf1 = Lerp(GradCoord(seed, x0, y1, xd0, yd1), GradCoord(seed, x1, y1, xd1, yd1), xs);

            return Lerp(xf0, xf1, ys) * 1.4247691104677813f;
        }

        private static float SingleValueCubic(int seed, FNLfloat x, FNLfloat y)
        {
            var x1 = FastFloor(x);
            var y1 = FastFloor(y);

            var xs = (float)(x - x1);
            var ys = (float)(y - y1);

            x1 *= PrimeX;
            y1 *= PrimeY;
            var x0 = x1 - PrimeX;
            var y0 = y1 - PrimeY;
            var x2 = x1 + PrimeX;
            var y2 = y1 + PrimeY;
            var x3 = x1 + unchecked(PrimeX * 2);
            var y3 = y1 + unchecked(PrimeY * 2);

            return CubicLerp(
                CubicLerp(ValCoord(seed, x0, y0), ValCoord(seed, x1, y0), ValCoord(seed, x2, y0), ValCoord(seed, x3, y0),
                xs),
                CubicLerp(ValCoord(seed, x0, y1), ValCoord(seed, x1, y1), ValCoord(seed, x2, y1), ValCoord(seed, x3, y1),
                xs),
                CubicLerp(ValCoord(seed, x0, y2), ValCoord(seed, x1, y2), ValCoord(seed, x2, y2), ValCoord(seed, x3, y2),
                xs),
                CubicLerp(ValCoord(seed, x0, y3), ValCoord(seed, x1, y3), ValCoord(seed, x2, y3), ValCoord(seed, x3, y3),
                xs),
                ys) * (1 / (1.5f * 1.5f));
        }

        private static float SingleValue(int seed, FNLfloat x, FNLfloat y)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);

            var xs = InterpHermite((float)(x - x0));
            var ys = InterpHermite((float)(y - y0));

            x0 *= PrimeX;
            y0 *= PrimeY;
            var x1 = x0 + PrimeX;
            var y1 = y0 + PrimeY;

            var xf0 = Lerp(ValCoord(seed, x0, y0), ValCoord(seed, x1, y0), xs);
            var xf1 = Lerp(ValCoord(seed, x0, y1), ValCoord(seed, x1, y1), xs);

            return Lerp(xf0, xf1, ys);
        }

        private float GenNoiseSingle(int seed, FNLfloat x, FNLfloat y)
        {
            return NoiseType switch
            {
                NoiseType.Perlin => SinglePerlin(seed, x, y),
                NoiseType.ValueCubic => SingleValueCubic(seed, x, y),
                NoiseType.Value => SingleValue(seed, x, y),
                _ => throw new NotImplementedException(),
            };
        }

        #endregion

        #region Fractals

        private float GenFractalFBm(FNLfloat x, FNLfloat y)
        {
            var seed = Seed;
            float sum = 0;
            var amp = fractalBounding;

            for (var i = 0; i < Octaves; i++)
            {
                var noise = GenNoiseSingle(seed++, x, y);
                sum += noise * amp;
                amp *= Lerp(1.0f, FastMin(noise + 1, 2) * 0.5f, WeightedStrength);

                x *= Lacunarity;
                y *= Lacunarity;
                amp *= Gain;
            }

            return sum;
        }

        private float GenFractalRidged(FNLfloat x, FNLfloat y)
        {
            var seed = Seed;
            float sum = 0;
            var amp = fractalBounding;

            for (var i = 0; i < Octaves; i++)
            {
                var noise = FastAbs(GenNoiseSingle(seed++, x, y));
                sum += (noise * -2 + 1) * amp;
                amp *= Lerp(1.0f, 1 - noise, WeightedStrength);

                x *= Lacunarity;
                y *= Lacunarity;
                amp *= Gain;
            }

            return sum;
        }

        private float GenFractalPingPong(FNLfloat x, FNLfloat y)
        {
            var seed = Seed;
            float sum = 0;
            var amp = fractalBounding;

            for (var i = 0; i < Octaves; i++)
            {
                var noise = PingPong((GenNoiseSingle(seed++, x, y) + 1) * PingPongStrength);
                sum += (noise - 0.5f) * 2 * amp;
                amp *= Lerp(1.0f, noise, WeightedStrength);

                x *= Lacunarity;
                y *= Lacunarity;
                amp *= Gain;
            }

            return sum;
        }

        #endregion

        #region Utilities

        private const int PrimeX = 501125321;
        private const int PrimeY = 1136930381;

        private static readonly float[] Gradients2D =
        {
             0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
             0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
             0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
            -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
            -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
            -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
             0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
             0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
             0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
            -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
            -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
            -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
             0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
             0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
             0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
            -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
            -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
            -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
             0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
             0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
             0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
            -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
            -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
            -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
             0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
             0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
             0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
            -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
            -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
            -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
             0.38268343236509f,   0.923879532511287f,  0.923879532511287f,  0.38268343236509f,   0.923879532511287f, -0.38268343236509f,   0.38268343236509f,  -0.923879532511287f,
            -0.38268343236509f,  -0.923879532511287f, -0.923879532511287f, -0.38268343236509f,  -0.923879532511287f,  0.38268343236509f,  -0.38268343236509f,   0.923879532511287f,
        };

        [MethodImpl(INLINE)] private static float FastAbs(float f) => f < 0 ? -f : f;
        [MethodImpl(INLINE)] private static float FastMin(float a, float b) => a < b ? a : b;
        [MethodImpl(INLINE)] private static float Lerp(float a, float b, float t) => a + t * (b - a);
        [MethodImpl(INLINE)] private static int FastFloor(FNLfloat f) => f >= 0 ? (int)f : (int)f - 1;

        [MethodImpl(INLINE)] private static float InterpHermite(float t) => t * t * (3 - 2 * t);
        [MethodImpl(INLINE)] private static float InterpQuintic(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        [MethodImpl(INLINE)]
        private static float CubicLerp(float a, float b, float c, float d, float t)
        {
            var p = d - c - (a - b);
            return t * t * t * p + t * t * (a - b - p) + t * (c - a) + b;
        }

        [MethodImpl(INLINE)]
        private static float GradCoord(int seed, int xPrimed, int yPrimed, float xd, float yd)
        {
            var hash = Hash(seed, xPrimed, yPrimed);
            hash ^= hash >> 15;
            hash &= 127 << 1;

            var xg = Gradients2D[hash];
            var yg = Gradients2D[hash | 1];

            return xd * xg + yd * yg;
        }

        [MethodImpl(INLINE)]
        private static float ValCoord(int seed, int xPrimed, int yPrimed)
        {
            var hash = Hash(seed, xPrimed, yPrimed);

            hash *= hash;
            hash ^= hash << 19;
            return hash * (1 / 2147483648.0f);
        }

        [MethodImpl(INLINE)]
        private static int Hash(int seed, int xPrimed, int yPrimed)
        {
            var hash = seed ^ xPrimed ^ yPrimed;

            hash *= 0x27d4eb2d;
            return hash;
        }

        private void CalculateFractalBounding()
        {
            var gain = FastAbs(Gain);
            var amp = gain;
            var ampFractal = 1.0f;
            for (var i = 1; i < Octaves; i++)
            {
                ampFractal += amp;
                amp *= gain;
            }
            fractalBounding = 1 / ampFractal;
        }

        [MethodImpl(INLINE)]
        private static float PingPong(float t)
        {
            t -= (int)(t * 0.5f) * 2;
            return t < 1 ? t : 2 - t;
        }

        #endregion

        private void Set<T>(ref T field, T value, params Action[] onChanged) => Set(ref field, value, false, onChanged);
        private void Set<T>(ref T field, T value, bool notify, params Action[] onChanged)
        {
            if (!Equals(field, value))
            {
                field = value;

                NotifyEditor();
                EmitChanged();
            }

            void EmitChanged()
            {
                onChanged.ForEach(x => x());
                EmitSignal(SignalName.Changed);
            }

            void NotifyEditor()
            {
                if (notify)
                    NotifyPropertyListChanged();
            }
        }
    }
}
