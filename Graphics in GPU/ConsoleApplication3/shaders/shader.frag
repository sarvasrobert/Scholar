#version 400
 
in vec3 normal;
in vec4 ecPos;
in vec2 texcoord;
in vec3 world_normal;
in vec2 FogFactor;
in vec4 viewSpace;
 
uniform vec3 L_pos;
uniform vec3 L1_pos;
uniform vec3 L2_pos;
uniform sampler2D tex;
uniform float hmla;
uniform bool linear;


const vec4 DiffuseLight = vec4(0.15, 0.05, 0.0, 1.0);
const vec4 RimColor = vec4(0.2, 0.2, 0.2, 1.0);
const vec4 fogColor = vec4(0.0, 0.0, 0.75,1.0);
const float FogDensity = 0.5;
///////////////////////////////// 
varying float fogFactor;

layout(location = 0) out vec4 colorOut;

 
void main()
{

  float fogFactor = 0.0;
  float dist = 0;
  vec4 fogColor1 = vec4(0,0,0,0);
  vec3 N = normalize(normal);
  vec3 L1 = normalize(L_pos - ecPos.xyz);
  vec3 L2 = normalize(L1_pos - ecPos.xyz);
  vec3 L3 = normalize(L2_pos - ecPos.xyz);

  float NdotL1 = clamp(dot(N, L1), 0.0, 1.0);
  float NdotL2 = clamp(dot(N, L2), 0.0, 1.0);
  float NdotL3 = clamp(dot(N, L3), 0.0, 1.0);


  float F = 0.1;
  float RR = 0.5;
  float G = 0.8;

   
  vec3 R1 = reflect(-L1, N);
  vec3 R2 = reflect(-L2, N);
  vec3 R3 = reflect(-L3, N);
  vec3 V = normalize(-ecPos.xyz);


  // phong lightning 
  float specular = pow(clamp(dot(R1, V), 0.0, 1.0),20);
  float specular1 = pow(clamp(dot(R2, V), 0.0, 1.0),30);
  float specular2 = pow(clamp(dot(R3, V), 0.0, 1.0),10);

  // cook-torrance lighting
  //float specular =  pow(clamp(dot(dot(F,RR),G), 0.0, 1.0), dot( cross(normal,V), cross(normal,L1) ));
  //float specular1 =  pow(clamp(dot(dot(F,RR),G), 0.0, 1.0), dot( cross(normal,V), cross(normal,L2) ));
  //float specular2 =  pow(clamp(dot(dot(F,RR),G), 0.0, 1.0), dot( cross(normal,V), cross(normal,L3) ));

  //rim lighting
  float rim = 1 - max(dot(V, world_normal), 0.0);
  rim = smoothstep(0.6, 1.0, rim);
  vec3 finalRim = RimColor * vec3(rim, rim, rim); 

  dist = abs(length(ecPos));
  if (linear == false){
	fogFactor = 1.0 / exp ( (dist*FogDensity) );
	fogFactor = clamp( fogFactor, 0.5, 1.0 );  // skus 0.2, 0.6
	//fogFactor = pow(fogFactor,2);   
  } else {
	// linear fog implementation
	fogFactor = (70000 - dist)/(70000 - 2);   /// fog seeing on ground
	fogFactor = clamp( fogFactor, 0.0, 1.0 );
  }

  fogColor1 = fogColor + RimColor;


  colorOut =  (texture( tex,  texcoord) * pow(NdotL1,0.75)) + (specular *  vec4(0,5,0,1));
  colorOut += (texture( tex,  texcoord) * pow(NdotL2,0.75)) + (specular1 * vec4(0,0,5,1));
  colorOut += (texture( tex,  texcoord) * pow(NdotL3,0.75)) + (specular2 * vec4(5,0,0,1));
  if (hmla == 1.0){
	colorOut = mix(fogColor1, colorOut, fogFactor);
  }
  // gray fog_factor gray level 
  //colorOut = vec4( fogFactor, fogFactor, fogFactor,1.0 );
}