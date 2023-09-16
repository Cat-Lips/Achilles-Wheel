shader_type spatial;
render_mode cull_disabled;

uniform vec3 color_base : source_color = vec3(0.11, 0.251, 0.039); // 1c400a
uniform vec3 color_tip : source_color = vec3(0.227, 0.592, 0.067); // 3a9711

uniform sampler2D noise;
uniform float noise_scale = 20.0;

varying vec3 world_pos;

//-- UTILITIES --//

vec3 WorldVertex(mat4 model_matrix, vec3 vertex) {
    return (model_matrix * vec4(vertex, 1.0)).xyz;
}

//-- FUNCTIONS --//

vec3 GetNoiseLevel(vec2 xz) {
    return texture(noise, xz / noise_scale).rgb;
}

//-- SHADERS --//

void vertex() {
    world_pos = WorldVertex(MODEL_MATRIX, VERTEX);
}

void fragment() {
    vec3 noise_level = GetNoiseLevel(world_pos.xz);
    ALBEDO = mix(color_base, color_tip, UV.y) * mix(color_base, color_tip, noise_level);

    if (!FRONT_FACING) {
        NORMAL = -NORMAL;
    }
}
