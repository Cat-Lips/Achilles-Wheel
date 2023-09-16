#ifdef USE_HEIGHT_CURVE

uniform sampler2D height_curve;

float Gradient(float height) {
	return texture(height_curve, vec2(height)).r;
}

#else

float Gradient(float height) {
	return 1.;
}

#endif
