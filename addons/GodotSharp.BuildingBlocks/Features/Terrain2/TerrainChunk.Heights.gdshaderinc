uniform vec2 offset;

#ifdef USE_HEIGHT_MAP

uniform sampler2D height_map;

float RawHeight(vec3 vertex) {
    vec2 pos = vertex.xz + offset;
    vec2 heightmap_size = vec2(textureSize(height_map, 0));
    vec2 heightmap_offset = heightmap_size * .5;
    vec2 heightmap_position = (pos + .5 + heightmap_offset) / heightmap_size;
    return texture(height_map, heightmap_position).r;
}

#else

#include "fnl/ShaderNoiseLite.gdshaderinc"

float RawHeight(vec3 vertex) {
    vec2 pos = vertex.xz + offset;
    float noise = fnlGetNoise2D(fnl(), pos.x, pos.y);
    return (noise + 1.0) / 2.0;
}

#endif

uniform float amplitude;
uniform float chunk_scale;

float Height(vec3 vertex) {
    float height = RawHeight(vertex);
    float gradient = Gradient(height);
    return height * gradient * amplitude * chunk_scale;
}
