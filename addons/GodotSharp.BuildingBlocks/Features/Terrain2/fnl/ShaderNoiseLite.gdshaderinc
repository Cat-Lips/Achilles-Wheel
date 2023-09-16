// MIT License
//
// Copyright(c) 2023 Jordan Peck (jordan.me2@gmail.com)
// Copyright(c) 2023 Contributors
//
// VERSION: 1.1.0
// https://github.com/Auburn/FastNoiseLite

uniform int _fnl_seed;
uniform float _fnl_frequency = 0.01f;
uniform int _fnl_octaves = 3;
uniform float _fnl_lacunarity = 2.f;
uniform float _fnl_gain = 0.5f;
uniform float _fnl_weighted_strength = 0.f;
uniform float _fnl_ping_pong_strength = 2.f;

struct fnl_state
{
    int seed;
    float frequency;

    int octaves;
    float lacunarity;
    float gain;
    float weighted_strength;
    float ping_pong_strength;
};

fnl_state fnl()
{
    fnl_state newState;
    newState.seed = _fnl_seed;
    newState.frequency = _fnl_frequency;
    newState.octaves = _fnl_octaves;
    newState.lacunarity = _fnl_lacunarity;
    newState.gain = _fnl_gain;
    newState.weighted_strength = _fnl_weighted_strength;
    newState.ping_pong_strength = _fnl_ping_pong_strength;
    return newState;
}

// ====================
// UTILITIES (private)
// ====================

#define FNLfloat float
const int PRIME_X = 501125321;
const int PRIME_Y = 1136930381;

const float GRADIENTS_2D[] =
{
    0.130526192220052f, 0.99144486137381f, 0.38268343236509f, 0.923879532511287f, 0.608761429008721f, 0.793353340291235f, 0.793353340291235f, 0.608761429008721f,
    0.923879532511287f, 0.38268343236509f, 0.99144486137381f, 0.130526192220051f, 0.99144486137381f, -0.130526192220051f, 0.923879532511287f, -0.38268343236509f,
    0.793353340291235f, -0.60876142900872f, 0.608761429008721f, -0.793353340291235f, 0.38268343236509f, -0.923879532511287f, 0.130526192220052f, -0.99144486137381f,
    -0.130526192220052f, -0.99144486137381f, -0.38268343236509f, -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
    -0.923879532511287f, -0.38268343236509f, -0.99144486137381f, -0.130526192220052f, -0.99144486137381f, 0.130526192220051f, -0.923879532511287f, 0.38268343236509f,
    -0.793353340291235f, 0.608761429008721f, -0.608761429008721f, 0.793353340291235f, -0.38268343236509f, 0.923879532511287f, -0.130526192220052f, 0.99144486137381f,
    0.130526192220052f, 0.99144486137381f, 0.38268343236509f, 0.923879532511287f, 0.608761429008721f, 0.793353340291235f, 0.793353340291235f, 0.608761429008721f,
    0.923879532511287f, 0.38268343236509f, 0.99144486137381f, 0.130526192220051f, 0.99144486137381f, -0.130526192220051f, 0.923879532511287f, -0.38268343236509f,
    0.793353340291235f, -0.60876142900872f, 0.608761429008721f, -0.793353340291235f, 0.38268343236509f, -0.923879532511287f, 0.130526192220052f, -0.99144486137381f,
    -0.130526192220052f, -0.99144486137381f, -0.38268343236509f, -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
    -0.923879532511287f, -0.38268343236509f, -0.99144486137381f, -0.130526192220052f, -0.99144486137381f, 0.130526192220051f, -0.923879532511287f, 0.38268343236509f,
    -0.793353340291235f, 0.608761429008721f, -0.608761429008721f, 0.793353340291235f, -0.38268343236509f, 0.923879532511287f, -0.130526192220052f, 0.99144486137381f,
    0.130526192220052f, 0.99144486137381f, 0.38268343236509f, 0.923879532511287f, 0.608761429008721f, 0.793353340291235f, 0.793353340291235f, 0.608761429008721f,
    0.923879532511287f, 0.38268343236509f, 0.99144486137381f, 0.130526192220051f, 0.99144486137381f, -0.130526192220051f, 0.923879532511287f, -0.38268343236509f,
    0.793353340291235f, -0.60876142900872f, 0.608761429008721f, -0.793353340291235f, 0.38268343236509f, -0.923879532511287f, 0.130526192220052f, -0.99144486137381f,
    -0.130526192220052f, -0.99144486137381f, -0.38268343236509f, -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
    -0.923879532511287f, -0.38268343236509f, -0.99144486137381f, -0.130526192220052f, -0.99144486137381f, 0.130526192220051f, -0.923879532511287f, 0.38268343236509f,
    -0.793353340291235f, 0.608761429008721f, -0.608761429008721f, 0.793353340291235f, -0.38268343236509f, 0.923879532511287f, -0.130526192220052f, 0.99144486137381f,
    0.130526192220052f, 0.99144486137381f, 0.38268343236509f, 0.923879532511287f, 0.608761429008721f, 0.793353340291235f, 0.793353340291235f, 0.608761429008721f,
    0.923879532511287f, 0.38268343236509f, 0.99144486137381f, 0.130526192220051f, 0.99144486137381f, -0.130526192220051f, 0.923879532511287f, -0.38268343236509f,
    0.793353340291235f, -0.60876142900872f, 0.608761429008721f, -0.793353340291235f, 0.38268343236509f, -0.923879532511287f, 0.130526192220052f, -0.99144486137381f,
    -0.130526192220052f, -0.99144486137381f, -0.38268343236509f, -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
    -0.923879532511287f, -0.38268343236509f, -0.99144486137381f, -0.130526192220052f, -0.99144486137381f, 0.130526192220051f, -0.923879532511287f, 0.38268343236509f,
    -0.793353340291235f, 0.608761429008721f, -0.608761429008721f, 0.793353340291235f, -0.38268343236509f, 0.923879532511287f, -0.130526192220052f, 0.99144486137381f,
    0.130526192220052f, 0.99144486137381f, 0.38268343236509f, 0.923879532511287f, 0.608761429008721f, 0.793353340291235f, 0.793353340291235f, 0.608761429008721f,
    0.923879532511287f, 0.38268343236509f, 0.99144486137381f, 0.130526192220051f, 0.99144486137381f, -0.130526192220051f, 0.923879532511287f, -0.38268343236509f,
    0.793353340291235f, -0.60876142900872f, 0.608761429008721f, -0.793353340291235f, 0.38268343236509f, -0.923879532511287f, 0.130526192220052f, -0.99144486137381f,
    -0.130526192220052f, -0.99144486137381f, -0.38268343236509f, -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
    -0.923879532511287f, -0.38268343236509f, -0.99144486137381f, -0.130526192220052f, -0.99144486137381f, 0.130526192220051f, -0.923879532511287f, 0.38268343236509f,
    -0.793353340291235f, 0.608761429008721f, -0.608761429008721f, 0.793353340291235f, -0.38268343236509f, 0.923879532511287f, -0.130526192220052f, 0.99144486137381f,
    0.38268343236509f, 0.923879532511287f, 0.923879532511287f, 0.38268343236509f, 0.923879532511287f, -0.38268343236509f, 0.38268343236509f, -0.923879532511287f,
    -0.38268343236509f, -0.923879532511287f, -0.923879532511287f, -0.38268343236509f, -0.923879532511287f, 0.38268343236509f, -0.38268343236509f, 0.923879532511287f
};

float _fnlFastAbs(float f) { return abs(f); }
int _fnlFastFloor(FNLfloat f) { return int(floor(f)); }
float _fnlFastMin(float x, float y) { return min(x, y); }
float _fnlLerp(float a, float b, float t) { return mix(a, b, t); }
float _fnlInterpHermite(float t) { return t * t * (3.f - 2.f * t); }
float _fnlInterpQuintic(float t) { return t * t * t * (t * (t * 6.f - 15.f) + 10.f); }

float _fnlCubicLerp(float a, float b, float c, float d, float t)
{
    float p = (d - c) - (a - b);
    return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
}

int _fnlHash2D(int seed, int xPrimed, int yPrimed)
{
    int hash = seed ^ xPrimed ^ yPrimed;

    hash *= 0x27d4eb2d;
    return hash;
}

float _fnlValCoord2D(int seed, int xPrimed, int yPrimed)
{
    int hash = _fnlHash2D(seed, xPrimed, yPrimed);
    hash *= hash;
    hash ^= hash << 19;
    return float(hash) * (1.f / 2147483648.f);
}

float _fnlGradCoord2D(int seed, int xPrimed, int yPrimed, float xd, float yd)
{
    int hash = _fnlHash2D(seed, xPrimed, yPrimed);
    hash ^= hash >> 15;
    hash &= 127 << 1;
    return xd * GRADIENTS_2D[hash] + yd * GRADIENTS_2D[hash | 1];
}

float _fnlCalculateFractalBounding(fnl_state state)
{
    float gain = _fnlFastAbs(state.gain);
    float amp = gain;
    float ampFractal = 1.f;
    for (int i = 1; i < state.octaves; i++)
    {
        ampFractal += amp;
        amp *= gain;
    }
    return 1.f / ampFractal;
}

float _fnlPingPong(float t)
{
    t -= float(int(t * 0.5f)) * 2.f;
    return t < 1.f ? t : 2.f - t;
}

// ====================
// NOISE (private)
// ====================

#ifdef FNL_NOISE_PERLIN
float _fnlSinglePerlin2D(int seed, FNLfloat x, FNLfloat y)
{
    int x0 = _fnlFastFloor(x);
    int y0 = _fnlFastFloor(y);

    float xd0 = x - float(x0);
    float yd0 = y - float(y0);
    float xd1 = xd0 - 1.f;
    float yd1 = yd0 - 1.f;

    float xs = _fnlInterpQuintic(xd0);
    float ys = _fnlInterpQuintic(yd0);

    x0 *= PRIME_X;
    y0 *= PRIME_Y;
    int x1 = x0 + PRIME_X;
    int y1 = y0 + PRIME_Y;

    float xf0 = _fnlLerp(_fnlGradCoord2D(seed, x0, y0, xd0, yd0), _fnlGradCoord2D(seed, x1, y0, xd1, yd0), xs);
    float xf1 = _fnlLerp(_fnlGradCoord2D(seed, x0, y1, xd0, yd1), _fnlGradCoord2D(seed, x1, y1, xd1, yd1), xs);

    return _fnlLerp(xf0, xf1, ys) * 1.4247691104677813;
}
#endif

#ifdef FNL_NOISE_VALUE_CUBIC
float _fnlSingleValueCubic2D(int seed, FNLfloat x, FNLfloat y)
{
    int x1 = _fnlFastFloor(x);
    int y1 = _fnlFastFloor(y);

    float xs = x - float(x1);
    float ys = y - float(y1);

    x1 *= PRIME_X;
    y1 *= PRIME_Y;

    int x0 = x1 - PRIME_X;
    int y0 = y1 - PRIME_Y;
    int x2 = x1 + PRIME_X;
    int y2 = y1 + PRIME_Y;
    int x3 = x1 + PRIME_X * 2;
    int y3 = y1 + PRIME_Y * 2;

    return _fnlCubicLerp(
        _fnlCubicLerp(_fnlValCoord2D(seed, x0, y0), _fnlValCoord2D(seed, x1, y0), _fnlValCoord2D(seed, x2, y0), _fnlValCoord2D(seed, x3, y0),
                      xs),
        _fnlCubicLerp(_fnlValCoord2D(seed, x0, y1), _fnlValCoord2D(seed, x1, y1), _fnlValCoord2D(seed, x2, y1), _fnlValCoord2D(seed, x3, y1),
                      xs),
        _fnlCubicLerp(_fnlValCoord2D(seed, x0, y2), _fnlValCoord2D(seed, x1, y2), _fnlValCoord2D(seed, x2, y2), _fnlValCoord2D(seed, x3, y2),
                      xs),
        _fnlCubicLerp(_fnlValCoord2D(seed, x0, y3), _fnlValCoord2D(seed, x1, y3), _fnlValCoord2D(seed, x2, y3), _fnlValCoord2D(seed, x3, y3),
                      xs),
        ys) * (1.f / (1.5f * 1.5f));
}
#endif

#ifdef FNL_NOISE_VALUE
float _fnlSingleValue2D(int seed, FNLfloat x, FNLfloat y)
{
    int x0 = _fnlFastFloor(x);
    int y0 = _fnlFastFloor(y);

    float xs = _fnlInterpHermite(x - float(x0));
    float ys = _fnlInterpHermite(y - float(y0));

    x0 *= PRIME_X;
    y0 *= PRIME_Y;
    int x1 = x0 + PRIME_X;
    int y1 = y0 + PRIME_Y;

    float xf0 = _fnlLerp(_fnlValCoord2D(seed, x0, y0), _fnlValCoord2D(seed, x1, y0), xs);
    float xf1 = _fnlLerp(_fnlValCoord2D(seed, x0, y1), _fnlValCoord2D(seed, x1, y1), xs);

    return _fnlLerp(xf0, xf1, ys);
}
#endif

float _fnlGenNoiseSingle2D(fnl_state state, int seed, FNLfloat x, FNLfloat y)
{
#if defined(FNL_NOISE_PERLIN)
    return _fnlSinglePerlin2D(seed, x, y);
#elif defined(FNL_NOISE_VALUE_CUBIC)
    return _fnlSingleValueCubic2D(seed, x, y);
#elif defined(FNL_NOISE_VALUE)
    return _fnlSingleValue2D(seed, x, y);
#else
    return 0.f;
#endif
}

// ====================
// FRACTALS (private)
// ====================

#ifdef FNL_FRACTAL_FBM
float _fnlGenFractalFBM2D(fnl_state state, FNLfloat x, FNLfloat y)
{
    int seed = state.seed;
    float sum = 0.f;
    float amp = _fnlCalculateFractalBounding(state);

    for (int i = 0; i < state.octaves; i++)
    {
        float noise = _fnlGenNoiseSingle2D(state, seed++, x, y);
        sum += noise * amp;
        amp *= _fnlLerp(1.f, _fnlFastMin(noise + 1.f, 2.f) * 0.5f, state.weighted_strength);

        x *= state.lacunarity;
        y *= state.lacunarity;
        amp *= state.gain;
    }

    return sum;
}
#endif

#ifdef FNL_FRACTAL_RIDGED
float _fnlGenFractalRidged2D(fnl_state state, FNLfloat x, FNLfloat y)
{
    int seed = state.seed;
    float sum = 0.f;
    float amp = _fnlCalculateFractalBounding(state);

    for (int i = 0; i < state.octaves; i++)
    {
        float noise = _fnlFastAbs(_fnlGenNoiseSingle2D(state, seed++, x, y));
        sum += (noise * -2.f + 1.f) * amp;
        amp *= _fnlLerp(1.f, 1.f - noise, state.weighted_strength);

        x *= state.lacunarity;
        y *= state.lacunarity;
        amp *= state.gain;
    }

    return sum;
}
#endif

#ifdef FNL_FRACTAL_PINGPONG
float _fnlGenFractalPingPong2D(fnl_state state, FNLfloat x, FNLfloat y)
{
    int seed = state.seed;
    float sum = 0.f;
    float amp = _fnlCalculateFractalBounding(state);

    for (int i = 0; i < state.octaves; i++)
    {
        float noise = _fnlPingPong((_fnlGenNoiseSingle2D(state, seed++, x, y) + 1.f) * state.ping_pong_strength);
        sum += (noise - 0.5f) * 2.f * amp;
        amp *= _fnlLerp(1.f, noise, state.weighted_strength);

        x *= state.lacunarity;
        y *= state.lacunarity;
        amp *= state.gain;
    }

    return sum;
}
#endif

// ====================
// Public API
// ====================

float fnlGetNoise2D(fnl_state state, FNLfloat x, FNLfloat y)
{
    x *= state.frequency;
    y *= state.frequency;

#if defined(FNL_FRACTAL_FBM)
    return _fnlGenFractalFBM2D(state, x, y);
#elif defined(FNL_FRACTAL_RIDGED)
    return _fnlGenFractalRidged2D(state, x, y);
#elif defined(FNL_FRACTAL_PINGPONG)
    return _fnlGenFractalPingPong2D(state, x, y);
#else
    return _fnlGenNoiseSingle2D(state, state.seed, x, y);
#endif
}
