shader_type spatial;
uniform vec3 colour = vec3(1.0);
uniform float alpha = 1.0;

uniform vec3 inAttackRangeColour = vec3(1.0, 0.0, 0.0);
uniform vec3 highlightedColour = vec3(0.0, 0.0, 1.0);
uniform vec3 selectedColour = vec3(1.0, 1.0, 0.0);

uniform bool inAttackRange = false;
uniform bool isHighlighted = false;
uniform bool isSelected = false;

vec3 GetColour()
{
	if(isSelected)
		return selectedColour;
		
	if(isHighlighted)
		return highlightedColour;
		
	if(inAttackRange)
		return inAttackRangeColour;
	
	return colour;
}


void fragment(){
	ALBEDO = GetColour();
}