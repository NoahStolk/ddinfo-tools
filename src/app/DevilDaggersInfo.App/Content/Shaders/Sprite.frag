#version 330 core
in vec2 texCoord;

out vec4 FragColor;

uniform sampler2D image;
uniform vec4 spriteColor;

void main()
{
	vec4 coord = texture(image, texCoord);
	if (coord.a == 0)
		discard;

	FragColor = spriteColor * coord;
}
