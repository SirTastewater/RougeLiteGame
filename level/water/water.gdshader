shader_type spatial;

// AMPLIDUTE * sin(x * (2 / wavelength)) + TIME)
uniform sampler2D normal: hint_normal;

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898,78.233))) * 43758.5453);
}

void vertex() {
    float height = 0.0;

    // big rolling waves
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float angle = fi * 2.0944; // 0°, 120°, 240°
        vec2 dir = vec2(cos(angle), sin(angle));

        height += 0.15 * sin(
            dot(UV * 3.0, dir) + TIME * (0.6 + fi * 0.2)
        );
    }

    // mid-frequency choppy waves
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        float angle = fi * 1.5708;  // 0°, 90°, 180°, 270°
        vec2 dir = vec2(cos(angle), sin(angle));

        height += 0.05 * sin(
            dot(UV * 10.0, dir) + TIME * (1.0 + fi * 0.3)
        );
    }

    // tiny ripples from hash
    height += 0.015 * sin(
        TIME * 3.0 + hash(UV * 20.0) * 6.2831
    );

    VERTEX.y += height;
}

void fragment() {
	ALBEDO = vec3(0, 0.1, 0.2);
	//ALPHA = 0.99;
	NORMAL_MAP = texture(normal, UV.xy * 2.0f + (TIME * 0.01f)).xyz * 18.0f;
	//ALBEDO = vec3(0.01f, 0.01f, 0.06f);
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
