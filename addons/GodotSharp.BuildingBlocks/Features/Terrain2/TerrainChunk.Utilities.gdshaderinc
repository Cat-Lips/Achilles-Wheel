const int max_layer_count = 8;
const float epsilon = .000001;

uniform int layer_count;

//-- UTILITIES --//

float true_round(float value) {
    return floor(value + .5);
}

float inverse_lerp(float value, float min, float max) {
    return (value - min) / (max - min);
}

float binary_step(float value, float min, float max) {
    return float(value >= min && value <= max);
}

float weight(float value, float min, float max) {
	float w = max(inverse_lerp(value, min, max), 0);
	return w * max(sign(max - value + epsilon), 0);
}

float bell_weight(float value, float min, float max) {
	float w = weight(value, min, max);
	return 4. * w * (1. - w);
}

vec3 WorldVertex(vec3 vertex, mat4 model_matrix) {
    return vertex + model_matrix[3].xyz;
}

vec3 WorldFragment(vec3 fragment, mat4 inv_view_matrix) {
    return (inv_view_matrix * vec4(fragment, 0)).xyz;
}

vec4 triplane_texture(sampler2D source, vec3 scaled_position, vec3 weighted_normal) {
    return
        texture(source, scaled_position.yz) * weighted_normal.x +
        texture(source, scaled_position.xz) * weighted_normal.y +
        texture(source, scaled_position.xy) * weighted_normal.z;
}

float Blend(float value, float min, float max, float blend) {
#ifdef USE_BLENDING
    blend = (max - min) * blend * .5;
    return bell_weight(value, min - blend, max + blend);
#else
    return binary_step(value, min, max);
#endif
}
