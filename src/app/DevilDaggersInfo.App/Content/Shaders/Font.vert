#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 projection;
uniform float offset;

void main()
{
	gl_Position = projection * model * vec4(aPosition, 0, 1.0);
	texCoord = aTexCoord + vec2(offset, 0);
}
