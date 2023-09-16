shader_type spatial;

#include "LagueTerrain.gdshaderinc"

void fragment() {
    ALBEDO = get_texture_color(height_percent());
}
