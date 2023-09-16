shader_type spatial;

#include "LagueTerrain.gdshaderinc"

void fragment() {
    ALBEDO = get_color(height_percent());
}
