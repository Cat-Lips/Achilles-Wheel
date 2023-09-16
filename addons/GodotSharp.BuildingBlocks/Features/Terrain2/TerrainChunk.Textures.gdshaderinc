uniform vec3 tints[max_layer_count];
uniform sampler2D textures[max_layer_count];

uniform float min_slopes[max_layer_count];
uniform float max_slopes[max_layer_count];
uniform float min_heights[max_layer_count];
uniform float max_heights[max_layer_count];
uniform float texture_scales[max_layer_count];
uniform float tint_strengths[max_layer_count];
uniform float blend_strengths[max_layer_count];

//-- FUNCTIONS --//

vec3 BlendTextures(vec3 vertex, vec3 normal) {
    vec3 weighted_normal = abs(normal);
    weighted_normal /= weighted_normal.x + weighted_normal.y + weighted_normal.z;

    vec3 result = vec3(0);
    float slope = 1. - abs(normal.y);
    float height = RawHeight(vertex);

    for (int i = 0; i < layer_count; ++i) {
        float min_slope = min_slopes[i];
        float max_slope = max_slopes[i];
        float min_height = min_heights[i];
        float max_height = max_heights[i];
        float texture_scale = texture_scales[i];
        float tint_strength = tint_strengths[i];
        float blend_strength = blend_strengths[i];
        vec3 scaled_position = vertex / texture_scale;

        vec3 tint = tints[i] * tint_strength;
        vec3 tex = triplane_texture(textures[i], scaled_position, weighted_normal).rgb * (1. - tint_strength);

        float slope_weight = Blend(slope, min_slope, max_slope, blend_strength);
        float height_weight = Blend(height, min_height, max_height, blend_strength);

        float weight = slope_weight * height_weight;
        result = result * (1. - weight) + (tint + tex) * weight;
    }

    return result;
}
