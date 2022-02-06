shader_type spatial;

uniform vec3 playerPosition;
uniform float playerMovementDistance;

uniform vec3 displayColour = vec3(1.0, 0.0, 0.0);
uniform float alpha = 0.8;

void fragment(){    
	
	vec2 worldVertex = (CAMERA_MATRIX * vec4(VERTEX, 1.0)).xz;
	
	vec2 playerPositionWithoutY = playerPosition.xz;
	
    float distanceToPlayer = distance(worldVertex, playerPositionWithoutY);
	
    bool withinWalkingDistance = distanceToPlayer <= playerMovementDistance; 
	bool notOccupiedByPlayer = distanceToPlayer > 1.5;
	
	if(!withinWalkingDistance || !notOccupiedByPlayer)
	{
		ALPHA = 0.0;
		ALBEDO = vec3(0);
	}
	else
	{
		ALPHA = alpha;
		ALBEDO = displayColour;
	}
}

