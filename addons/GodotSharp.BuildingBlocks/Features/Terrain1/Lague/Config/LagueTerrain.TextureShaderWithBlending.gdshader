shader_type spatial;

#include "LagueTerrain.gdshaderinc"

void fragment() {
    ALBEDO = get_texture_color_blend(height_percent());
}
