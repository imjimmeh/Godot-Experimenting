shader_type canvas_item;

uniform sampler2D FogOfWarTexture;

void fragment() {
    vec4 col = texture(TEXTURE, UV);
	vec4 fogCol = texture(FogOfWarTexture, UV);
	
	if(fogCol.a > 0.0)
	{
		float avg = (col.x + col.y + col.z) / 3.0;
		COLOR.xyz = vec3(avg);
		COLOR.a = col.a;
	}
	else
		COLOR = col;
}