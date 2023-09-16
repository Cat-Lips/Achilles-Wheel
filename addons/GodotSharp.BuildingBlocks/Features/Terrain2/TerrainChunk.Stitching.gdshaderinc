uniform float lod_step;
uniform float chunk_size;

global uniform vec3 terrain_position;

//-- FUNCTIONS --//

float Lod(vec3 vertex) {
    vec3 terrain_vertex = vertex - terrain_position;
    float lod = true_round(max(abs(terrain_vertex.x), abs(terrain_vertex.z)) / chunk_size) * lod_step;
    return min(pow(2, lod), chunk_size);
}

float StitchedHeight(vec3 vertex, float lod) {
    vec3 fraction = fract((vertex + chunk_size * .5) / lod);

    float x_height = mix(
        Height(vertex - vec3(fraction.x * lod, 0, 0)),
        Height(vertex + vec3((1. - fraction.x) * lod, 0, 0)),
        fraction.x);

    float z_height = mix(
        Height(vertex - vec3(0, 0, fraction.z * lod)),
        Height(vertex + vec3(0, 0, (1. - fraction.z) * lod)),
        fraction.z);

    return mix(x_height, z_height, ceil(fraction.z));
}
