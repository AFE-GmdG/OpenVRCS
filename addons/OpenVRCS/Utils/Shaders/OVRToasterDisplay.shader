shader_type canvas_item;
render_mode unshaded;

void fragment() {
    float baseAlpha = 0.7;
    vec3 baseColor = vec3(0.13, 0.14, 0.19);
    vec3 borderColor = baseColor * 5.0;
    vec2 sUV = FRAGCOORD.xy;
    vec2 fUV = UV.xy;
    vec2 maxFragCoord = sUV / fUV;
    vec2 radius = maxFragCoord * 0.5;
    sUV -= radius;
    radius -= 10.0;

    float d = length(max(abs(sUV) - radius, 0));
    float a = smoothstep(10, 7, d);
    float b = smoothstep(5, 7, d);
    float c = min(a, max(a * baseAlpha, b));
    
    vec3 color = mix(baseColor, borderColor, b);

    COLOR = vec4(color, c);
}
