shader_type spatial;
render_mode cull_disabled;

#include "worley_noise.gdshaderinc"

//-- CONSTANTS --//

const vec3 UP = vec3(0, 1, 0);
const vec3 RIGHT = vec3(1, 0, 0);

const float DEG2RAD = PI / 180.0;

//-- PARAMETERS --//

//uniform vec4 color_tip:source_color = vec4(1, 1, 1, 1);
//uniform vec4 color_base:source_color = vec4(0, 0, 0, 1);
uniform vec4 color_tip:source_color = vec4(0.125, 0.224, 0.027, 1);
uniform vec4 color_base:source_color = vec4(0.078, 0.149, 0.012, 1);
//uniform vec4 color_tip:source_color = vec4(0.227, 0.592, 0.067, 1);
//uniform vec4 color_base:source_color = vec4(0.11, 0.251, 0.039, 1);

uniform float wind_scale = 4.0;
uniform float wind_speed = 1.0;
uniform vec2 wind_direction = vec2(0, -1);

uniform float deg_sway_yaw = 45.0;
uniform float deg_sway_pitch = 80.0;

//-- VARIABLES --//

varying float wind;

//-- UTILITIES --//

vec3 WorldVertex(mat4 model_matrix, vec3 vertex) {
    return (model_matrix * vec4(vertex, 1)).xyz;
}

mat3 WorldToModelMatrix(mat4 model_matrix) {
    return inverse(mat3(model_matrix));
}

mat3 RotationMatrix(vec3 axis, float angle) {
    float s = sin(angle);
    float c = cos(angle);
    float t = 1.0 - c;
    float x = axis.x;
    float y = axis.x;
    float z = axis.x;
    return mat3(
        vec3(t*x*x+c, t*x*y-s*z, t*x*z+s*y),
        vec3(t*x*y+s*z, t*y*y+c, t*y*z-s*x),
        vec3(t*x*z-s*y, t*y*z+s*z, t*z*z+c));
}

int BinaryMultiplier(bool value) {
    return 2 * int(value) - 1;
}

//-- FUNCTIONS --//

vec3 Scale(vec3 vertex, float width, float height) {
    vertex.xz *= width;
    vertex.y *= height;
    return vertex;
}

float Wind(mat4 model_matrix, vec3 vertex, vec2 uv2, float sway_pitch, float sway_yaw, out mat3 rot_right, out mat3 rot_forward) {
    //vec2 wind_pos = WorldVertex(model_matrix, vertex).xz * wind_scale;
    vec2 wind_pos = (model_matrix * vec4(vertex, -1.0)).xz * wind_scale;
    vec2 wind_dir = normalize(wind_direction);
    float time = TIME * wind_speed;

    vec2 uv = wind_pos + wind_dir * time;
    float _wind = pow(noise(uv), 2) * uv2.y;

    mat3 w2m = WorldToModelMatrix(model_matrix);
    vec3 wind_forward = w2m * vec3(wind_dir.x, 0, wind_dir.y);
    vec3 wind_right = normalize(cross(wind_forward, UP));

    float _sway_pitch = (deg_sway_pitch * DEG2RAD * _wind) + sway_pitch;
    float _sway_yaw = (deg_sway_yaw * DEG2RAD * sin(time) * _wind) + sway_yaw;

    rot_right = RotationMatrix(wind_right, _sway_pitch);
    rot_forward = RotationMatrix(wind_forward, _sway_yaw);

    return _wind;
}

//-- SHADERS --//

void vertex() {
    float width = INSTANCE_CUSTOM.r;
    float height = INSTANCE_CUSTOM.g;
    float sway_yaw = INSTANCE_CUSTOM.b;
    float sway_pitch = INSTANCE_CUSTOM.a;

    mat3 rot_right;
    mat3 rot_forward;
    wind = Wind(MODEL_MATRIX, VERTEX, UV2, sway_pitch, sway_yaw, rot_right, rot_forward);

    NORMAL = UP;
    COLOR = mix(color_base, color_tip, UV2.y);
    VERTEX = rot_right * rot_forward * Scale(VERTEX, width, height);
}

void fragment() {
    ALBEDO = COLOR.rgb;
    NORMAL *= float(BinaryMultiplier(FRONT_FACING));
    SPECULAR = .5;
    ROUGHNESS = clamp(1.0 - wind * 2.0, 0, 1);
}
