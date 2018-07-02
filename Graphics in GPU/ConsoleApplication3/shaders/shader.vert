#version 400

layout(location = 0) in vec3 vertex_position;
layout(location = 1) in vec3 vertex_normal;
layout(location = 2) in vec2 tex_coord;

uniform mat4 camera_model_view_matrix;
uniform mat3 normal_matrix;
uniform mat4 camera_projection_matrix;
uniform mat4 model_matrix1;
uniform mat4 camera;
uniform float posunZ;


out vec3 normal;
out vec4 ecPos;
out vec2 texcoord;
out vec4 pos;
out vec3 world_normal;
out float fogFactor;
out vec4 viewSpace;


void main () {

	pos = model_matrix1 * vec4 (vertex_position, 1.0);
	world_normal = normalize(mat3(model_matrix1) * vertex_normal);

	//compute the vertex position  in camera space
    
	//send it to fragment shader
	viewSpace = camera_model_view_matrix  * camera * vec4(vertex_position,1);
	gl_Position = camera_projection_matrix * viewSpace;
	
	ecPos =  camera_model_view_matrix * vec4 (vertex_position, 1.0)  * camera;
	ecPos.z = ecPos.z + posunZ;
	//ecPos = camera_model_view_matrix  *camera * vec4 (vertex_position, 1.0);
	
	normal = normal_matrix * vertex_normal;
	
	texcoord = tex_coord;

	gl_Position = camera_projection_matrix * ecPos;
}