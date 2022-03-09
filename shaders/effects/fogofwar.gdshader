shader_type spatial;
varying vec3 world_position;

void vertex() {
	world_position = VERTEX;
}

const float gridSize = 1.0;

void fragment(){
	vec3 pos = world_position;
	vec3 col = vec3(0);
	ALBEDO = col;
	ALPHA = 1.0;
}