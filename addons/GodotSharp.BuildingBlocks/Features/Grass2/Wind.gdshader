shader_type spatial;

#include "worley_noise.gdshaderinc"

//-- PARAMETERS --//

uniform float wind_scale = 4.0;
uniform float wind_speed = 1.0;
uniform vec2 wind_direction = vec2(0, -1);

//-- UTILITIES --//

//-- FUNCTIONS --//

vec2 WindPos(mat4 model_matrix, vec3 vertex) {
    return (model_matrix * vec4(vertex, -1)).xz * wind_scale;
}

//-- SHADERS --//

void vertex() {
    vec2 uv = WindPos(MODEL_MATRIX, VERTEX);
    uv += normalize(wind_direction) * TIME * wind_speed;

    float wind = pow(noise(uv), 2);
    COLOR = vec4(wind, wind, wind, 1);
}

void fragment() {
    ALBEDO = COLOR.rgb;
}
