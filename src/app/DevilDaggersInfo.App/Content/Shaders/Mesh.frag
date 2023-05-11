#version 330 core
in vec2 texCoord;
in vec3 fragPosition;

out vec4 FragColor;

const int maxLight = 128;

uniform sampler2D textureDiffuse;
uniform sampler2D textureLut;
uniform float lutScale;

uniform int lightCount;
uniform vec3 lightPosition[maxLight];
uniform vec3 lightColor[maxLight];
uniform float lightRadius[maxLight];

float distSquared(vec3 a, vec3 b)
{
	vec3 c = a - b;
	return dot(c, c);
}

void main()
{
	vec4 diffuse = texture(textureDiffuse, texCoord);
	float originalAlpha = diffuse.a;
	diffuse *= 2;

	vec3 lutIn = clamp(diffuse.rgb, vec3(0.0), vec3(1.0));
	float lutTextureSize = textureSize(textureLut, 0).x;
	float lutB = lutIn.b * 15.0;
	float lutBFract = fract(lutB);
	lutB -= lutBFract;
	vec2 lutRg = lutIn.rg * 15.0 / vec2(lutTextureSize, 256.0);
	lutRg.g += lutB * 16.0 / 256.0;
	lutRg += vec2(0.5 / lutTextureSize, 0.5 / 256.0);
	lutRg.g = 1.0 - lutRg.g;
	vec3 lutRgb = mix(textureLod(textureLut, lutRg, 0), textureLod(textureLut, lutRg - vec2(0.0, 16.0 / 256.0), 0), lutBFract).rgb;

	diffuse.rgb = mix(diffuse.rgb, lutRgb, lutScale);

	vec4 black = vec4(0.0, 0.0, 0.0, 1.0);
	float darkness = clamp(fragPosition.y, -1, 0) + 1;
	vec4 final = mix(black, diffuse, darkness);

	for (int i = 0; i < min(maxLight, lightCount); i++)
	{
		float distanceToLight = distSquared(lightPosition[i], fragPosition);
		float radius = lightRadius[i];
		if (distanceToLight < radius * radius)
		{
			float percentage = 1 - clamp(distanceToLight / lightRadius[i], 0, 1);

			float dist = sqrt(distanceToLight);
			float du = dist / (1 - distanceToLight / (radius * radius - 1));
			float denom = du / abs(radius) + 1;
			float attenuation = 1 / (denom * denom);

			final += vec4(lightColor[i], 1) * diffuse * attenuation;
		}
	}
	
	float posterizeAmount = 16;
	final.r = floor(final.r * posterizeAmount) / posterizeAmount;
	final.g = floor(final.g * posterizeAmount) / posterizeAmount;
	final.b = floor(final.b * posterizeAmount) / posterizeAmount;
	final.a = originalAlpha;
	FragColor = final;
}
