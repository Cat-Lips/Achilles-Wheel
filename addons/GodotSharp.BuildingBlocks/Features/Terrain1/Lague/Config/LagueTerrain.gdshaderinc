const float epsilon = 1e-4;

uniform int layer_count;
const int max_layer_count = 8;

//uniform sampler2DArray textures;
uniform vec3 colors[max_layer_count];
uniform float heights[max_layer_count];
uniform sampler2D textures[max_layer_count];

uniform float texture_scales[max_layer_count];
uniform float color_strengths[max_layer_count];
uniform float blend_strengths[max_layer_count];

uniform float min_height;
uniform float max_height;

varying vec3 world_vertex;
varying vec3 world_normal;

//-- SHARED FUNCTIONS --//

float inverse_lerp(float a, float b, float value) {
    return (value - a) / (b - a);
}

float height_percent() {
    return inverse_lerp(min_height, max_height, world_vertex.y);
}

//-- COLOR FUNCTIONS --//

vec3 get_color(float height) {
    vec3 color = colors[0];

    for (int i = 0; i < layer_count; ++i) {
        float strength = clamp(sign(height - heights[i]), 0, 1);
        color = color * (1.0 - strength) + colors[i] * strength;
    }

    return color;
}

vec3 get_color_blend(float height) {
    vec3 color = colors[0];

    for (int i = 0; i < layer_count; ++i) {
        float strength = inverse_lerp(-blend_strengths[i] * 0.5 - epsilon, blend_strengths[i] * 0.5, height - heights[i]);
        color = color * (1.0 - strength) + colors[i] * strength;
    }

    return color;
}

//-- TEXTURE FUNCTIONS --//

vec4 triplanar(sampler2D source, vec3 scaled_position, vec3 weighted_normal) {
    return
        texture(source, scaled_position.yz) * weighted_normal.x +
        texture(source, scaled_position.xz) * weighted_normal.y +
        texture(source, scaled_position.xy) * weighted_normal.z;
}

vec4 _triplanar(sampler2DArray source, float layer, vec3 scaled_position, vec3 weighted_normal) {
    return
        texture(source, vec3(scaled_position.yz, layer)) * weighted_normal.x +
        texture(source, vec3(scaled_position.xz, layer)) * weighted_normal.y +
        texture(source, vec3(scaled_position.xy, layer)) * weighted_normal.z;
}

vec3 get_texture_color(float height) {
    vec3 weighted_normal = abs(world_normal);
    weighted_normal /= weighted_normal.x + weighted_normal.y + weighted_normal.z;

    vec3 color = colors[0];

    for (int i = 0; i < layer_count; ++i) {
        vec3 scaled_position = world_vertex / texture_scales[i];

        vec3 tint = colors[i] * color_strengths[i];
        vec3 tex = triplanar(textures[i], scaled_position, weighted_normal).rgb * (1.0 - color_strengths[i]);

        float strength = clamp(sign(height - heights[i]), 0, 1);
        color = color * (1.0 - strength) + (tint + tex) * strength;
    }

    return color;
}

vec3 get_texture_color_blend(float height) {
    vec3 weighted_normal = abs(world_normal);
    weighted_normal /= weighted_normal.x + weighted_normal.y + weighted_normal.z;

    vec3 color = colors[0];

    for (int i = 0; i < layer_count; ++i) {
        vec3 scaled_position = world_vertex / texture_scales[i];

        vec3 tint = colors[i] * color_strengths[i];
        vec3 tex = triplanar(textures[i], scaled_position, weighted_normal).rgb * (1.0 - color_strengths[i]);

        float strength = inverse_lerp(-blend_strengths[i] * 0.5 - epsilon, blend_strengths[i] * 0.5, height - heights[i]);
        color = color * (1.0 - strength) + (tint + tex) * strength;
    }

    return color;
}

//-- SHARED SHADERS --//

void vertex() {
    world_vertex = VERTEX;
    world_normal = NORMAL;
}
