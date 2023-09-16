#ifdef USE_NORMAL_MAP

uniform sampler2D normal_map;

vec3 NormalMap(vec3 vertex) {
    vec2 texture_size = vec2(textureSize(normal_map, 0));
    vec2 texture_position = (vertex.xz + .5) / texture_size;
    return texture(normal_map, texture_position).rgb;
}

vec3 FromNormalMap(vec3 m) {
    m = vec3(m.x, m.z, m.y);
    m = m * 2. - 1.;
    m *= vec3(1., 1., -1.);
    return m;
}

#else

vec3 Normal(vec3 vertex) {
    vec3 east_vertex = vertex + vec3(1, 0, 0);
    vec3 west_vertex = vertex + vec3(-1, 0, 0);
    vec3 north_vertex = vertex + vec3(0, 0, 1);
    vec3 south_vertex = vertex + vec3(0, 0, -1);

    east_vertex.y = Height(east_vertex);
    west_vertex.y = Height(west_vertex);
    north_vertex.y = Height(north_vertex);
    south_vertex.y = Height(south_vertex);

    vec3 dx = east_vertex - west_vertex;
    vec3 dy = north_vertex - south_vertex;
    return normalize(cross(dy, dx));
}

vec3 ToNormalMap(vec3 n) {
    n *= vec3(1., 1., -1.);
    n = n * .5 + .5;
    n = vec3(n.x, n.z, n.y);
    return n;
}

#endif
