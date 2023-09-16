vec2 rand(vec2 p) {
    return fract(sin(vec2(
        dot(p, vec2(127.32, 231.4)),
        dot(p, vec2(12.3, 146.3))
    )) * 231.23);
}

float noise(vec2 p) {
    float dist = 1.0;

    vec2 p_floor = floor(p);
    vec2 p_fract = fract(p);

    for (int x = -1 ; x <= 1 ; ++x) {
        for (int y = -1 ; y <= 1 ; ++y) {
            vec2 n = vec2(float(x), float(y));
            vec2 diff = n + rand(p_floor + n) - p_fract;
            dist = min(dist, length(diff));
        }
    }

    return dist;
}
