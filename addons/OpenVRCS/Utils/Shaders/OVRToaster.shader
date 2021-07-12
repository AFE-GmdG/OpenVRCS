shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_disabled,unshaded;

uniform float alpha : hint_range(0.0, 1.0, 0.1) = 1.0;
uniform sampler2D albedo : hint_albedo;

void fragment() {
    vec4 tex = texture(albedo, UV);
    ALBEDO = tex.rgb;
    ALPHA = tex.a * alpha;
}
