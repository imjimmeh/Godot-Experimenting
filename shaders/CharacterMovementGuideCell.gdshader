shader_type spatial;

uniform float alpha = 0.5;

uniform vec3 highlightedColour = vec3(0.0,1.0,0.0);
uniform vec3 normalColour = vec3(1.0,0.0,0.0);
uniform vec3 pathColour = vec3(1.0,0.0,1.0);

uniform bool isTargetCell = false;
uniform bool isPartOfPath = false;

vec3 GetColour(){
	vec3 colour = normalColour;
	
	if(isPartOfPath)
	{
		colour = pathColour;
	}
	
	if(isTargetCell)
	{
		colour = highlightedColour;
	}
	
	return colour;
}

void fragment(){ 
	vec3 colour = GetColour();   
	
	
	ALBEDO = colour;
	ALPHA = colour == normalColour ? alpha * 0.5 : alpha;
}