#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

out vec2 texCoord;
out vec3 fragPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	texCoord = aTexCoord;
	vec4 translation = model * vec4(aPosition, 1.0);
	fragPosition = vec3(translation);
	gl_Position = projection * view * translation;
}
