#version 330

layout(location = 0) in vec2 a_Position;
layout(location = 1) in vec4 a_Color;
layout(location = 2) in vec2 a_TexCoords;

out vec4 color;
out vec2 texCoord;

uniform mat4 u_ViewProjection;

void main() 
{
  color = a_Color;
  gl_Position =  u_ViewProjection * vec4(a_Position, 1.0f, 1.0f);
  texCoord = a_TexCoords;
}