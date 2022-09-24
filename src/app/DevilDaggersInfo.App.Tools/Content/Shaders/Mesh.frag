#version 330 core
in vec2 texCoord;
in vec3 normal;
in vec3 fragPosition;

out vec4 FragColor;

uniform sampler2D textureDiffuse;

void main()
{
	FragColor = texture(textureDiffuse, texCoord);
}
