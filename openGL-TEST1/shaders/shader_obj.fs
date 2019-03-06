#version 330 core
out vec4 FragColor;

in vec3 normal;
in vec3 FragPos;
in vec2 TexCoords;

// material
struct Material{
    sampler2D diffuse;
    sampler2D specular;
    float shininess; // 反光度
};

// 定向光
struct DirLight{
    vec3 direction; // 新增光的方向
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

// 聚光灯源
struct SpotLight {
    vec3 position;
    vec3 direction;
    
    float cutoff; // 手电筒光照的内切向角
    float outerCutoff;  // 外切光角度
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform vec3 viewPos;
uniform vec3 lightColor;
uniform Material material;
uniform SpotLight spotLight;
uniform DirLight dirLight;

// 定向光 计算
vec3 calDirLight(DirLight light, vec3 normal, vec3 viewDir);
// 聚光灯源 计算
vec3 calSpotLight(SpotLight light, vec3 norm, vec3 fragPos, vec3 viewDir);

void main(){
    
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(viewPos - FragPos); // 片段指向光源的向量
    
    vec3 result = calDirLight(dirLight, norm, viewDir);
    FragColor = vec4(result, 1.0f);
}

// 定向光源
vec3 calDirLight(DirLight light, vec3 norm, vec3 viewDir){
    vec3 lightDir = normalize(light.direction);
    // 计算漫反射
    float diff = max(dot(norm, lightDir), 0.0);
    // 设置镜面反射
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0f), material.shininess);
    
    // 合并结果
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    
    return (ambient + diffuse + specular);
}

// 聚光灯光源
vec3 calSpotLight(SpotLight light, vec3 norm, vec3 fragPos, vec3 viewDir){
    vec3 lightDir = normalize(light.position - fragPos);
    
    float diff = max(dot(norm, lightDir), 0.0);
    // 镜面反射
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0f), material.shininess);
    
    // 设置手电筒光照效果
    float theta = dot(lightDir, normalize(-light.direction));
    float episilon = light.cutoff - light.outerCutoff;
    // clamp函数限制表达式的值在[0,1]区间内
    float intensity = clamp((theta - light.outerCutoff) / episilon, 0.0f, 1.0f);
    
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    
    diffuse *= intensity;
    specular *= intensity;
    return (ambient + diffuse + specular);
}
