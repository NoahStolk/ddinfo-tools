#version 330 core
layout (location = 0) in vec2 aPosition;

uniform mat4 model;
uniform mat4 projection;

void main()
{
	gl_Position = projection * model * vec4(aPosition, 0, 1.0);
}
