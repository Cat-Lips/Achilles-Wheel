#include "TerrainChunk.Utilities.gdshaderinc"

#include "TerrainChunk.Curves.gdshaderinc"
#include "TerrainChunk.Heights.gdshaderinc"
#include "TerrainChunk.Normals.gdshaderinc"
#include "TerrainChunk.Textures.gdshaderinc"
#include "TerrainChunk.Stitching.gdshaderinc"

varying vec3 world_vertex;

void vertex() {
    world_vertex = WorldVertex(VERTEX, MODEL_MATRIX);

    float lod = Lod(world_vertex);
    world_vertex.y = StitchedHeight(world_vertex, lod);

    VERTEX.y = world_vertex.y;
}

void fragment() {

#ifdef USE_NORMAL_MAP
    NORMAL_MAP = NormalMap(world_vertex);
    vec3 normal = FromNormalMap(NORMAL_MAP);
#else
    vec3 normal = Normal(world_vertex);
    NORMAL_MAP = ToNormalMap(normal);
#endif

    //ALBEDO = vec3(.15, .5, .10);
    ALBEDO = BlendTextures(world_vertex, normal);
}
