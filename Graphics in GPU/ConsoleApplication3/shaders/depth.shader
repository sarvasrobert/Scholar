#version 400

uniform sampler2DMS texture0;
uniform sampler2D texture1;
uniform bool isbackbuffer;
uniform vec2 buffersize;
uniform vec2 camerarange;

out vec4 fragData0;

float DepthToZPosition(float depth) {
	return (camerarange.x * camerarange.y) / 
	camerarange.y - depth * (camerarange.y - camerarange.x));
}
void main() {
	vec2 coords = vec2(gl_FragCoord.xy/buffersize);
	
	if (isbackbuffer) coords.y = 1.0 - coords.y;

	ivec2 icoords = ivec2(coords * buffersize);

	vec4 color = texture(texture1, coords);

	float depth = texelFetch(texture1, coords, 0).r;

	float z  =DepthToZPosition(depth);
	z *= 0.1;
	fragData0 = vec4(depth,depth,depth,1.0);

}