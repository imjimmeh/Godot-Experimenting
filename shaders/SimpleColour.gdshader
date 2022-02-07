shader_type spatial;
uniform vec3 colour = vec3(1.0);
uniform float alpha = 1.0;

void fragment(){    
	ALBEDO = colour;
	ALPHA = alpha;
}