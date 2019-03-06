#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
uniform sampler2D diffuseTexture;

void main(){
    vec3 color = texture(diffuseTexture, TexCoords).rgb;
    vec3 lighting = vec3(1.0) * color;
    FragColor = vec4(lighting, 1.0); // 光照设置为白色， 四个分量都为1.0
}
