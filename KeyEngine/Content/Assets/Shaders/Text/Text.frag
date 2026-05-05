#version 330 core
in vec4 color;
in vec2 texCoord;

out vec4 outputColor;
uniform sampler2D texture0;

void main()
{
    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(texture0, texCoord).r);
    outputColor = vec4(color.xyz, 1.0) * sampled;
}  