// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpareParts/VertexColor_BirghtnessFalloff"
{
	Properties
	{
		_tilingX("tilingX", Range( -4 , 4)) = 0
		_tilingY("tilingY", Range( -4 , 4)) = 0
		_offsetX("offsetX", Range( -4 , 4)) = 0
		_offsetY("offsetY", Range( -4 , 4)) = 0
		_proceduralTextureFalloff("proceduralTextureFalloff", Range( 0 , 32)) = 0
		_brightnessMinimum("brightnessMinimum", Range( 0 , 2048)) = 0
		_brightnessMaximum("brightnessMaximum", Range( 0 , 2048)) = 0
		_falloffIntensity("falloffIntensity", Range( 0 , 1)) = 0
		_alphaFalloff("alphaFalloff", Range( 0 , 8)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv_tex4coord;
		};

		uniform float _tilingX;
		uniform float _tilingY;
		uniform float _offsetX;
		uniform float _offsetY;
		uniform float _proceduralTextureFalloff;
		uniform float _alphaFalloff;
		uniform float _falloffIntensity;
		uniform float _brightnessMinimum;
		uniform float _brightnessMaximum;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult47 = (float2(_tilingX , _tilingY));
			float2 appendResult55 = (float2(_offsetX , _offsetY));
			float2 uv_TexCoord46 = i.uv_texcoord * appendResult47 + appendResult55;
			float saferPower30 = max( ( 1.0 - saturate( length( uv_TexCoord46 ) ) ) , 0.0001 );
			float lerpResult59 = lerp( pow( saferPower30 , _proceduralTextureFalloff ) , 1.0 , i.uv_tex4coord.z);
			float proceduralFalloff36 = lerpResult59;
			float4 temp_output_41_0 = ( i.vertexColor * proceduralFalloff36 );
			float temp_output_38_0 = ( i.vertexColor.a * proceduralFalloff36 );
			float saferPower12 = max( temp_output_38_0 , 0.0001 );
			float smoothstepResult15 = smoothstep( 0.0 , _alphaFalloff , ( pow( saferPower12 , _alphaFalloff ) * _alphaFalloff ));
			float4 lerpResult9 = lerp( temp_output_41_0 , ( temp_output_41_0 * smoothstepResult15 ) , _falloffIntensity);
			float4 temp_cast_0 = (_brightnessMinimum).xxxx;
			float4 temp_cast_1 = (_brightnessMaximum).xxxx;
			o.Emission = (temp_cast_0 + (lerpResult9 - float4( 0,0,0,0 )) * (temp_cast_1 - temp_cast_0) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 ))).rgb;
			float lerpResult19 = lerp( temp_output_38_0 , smoothstepResult15 , _falloffIntensity);
			o.Alpha = lerpResult19;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
195.2;201.6;1192;615;1271.45;-6.819824;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;57;-3421.588,-605.4841;Inherit;False;Property;_tilingX;tilingX;0;0;Create;True;0;0;False;0;False;0;0;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-3418.964,-532.3741;Inherit;False;Property;_tilingY;tilingY;1;0;Create;True;0;0;False;0;False;0;1;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3427.151,-382.751;Inherit;False;Property;_offsetY;offsetY;3;0;Create;True;0;0;False;0;False;0;-0.5;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-3422.22,-457.8632;Inherit;False;Property;_offsetX;offsetX;2;0;Create;True;0;0;False;0;False;0;0;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;-3148.873,-460.8737;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-3146.367,-604.1661;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-3008.647,-597.1291;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0,1;False;1;FLOAT2;0,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LengthOpNode;24;-2796.986,-597.1487;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;-2637.508,-593.563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2582.048,-367.2739;Inherit;False;Property;_proceduralTextureFalloff;proceduralTextureFalloff;4;0;Create;True;0;0;False;0;False;0;2;0;32;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;28;-2474.609,-590.8068;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;30;-2257.071,-613.0264;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-2174.653,-406.6554;Inherit;False;Constant;_Default;Default;9;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;58;-2206.653,-327.6554;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;59;-1987.653,-614.6554;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1797.198,-620.595;Inherit;True;proceduralFalloff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-1989.001,-95.97998;Inherit;False;36;proceduralFalloff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;2;-2160.978,-159.2568;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-1755,67.62732;Inherit;False;Property;_alphaFalloff;alphaFalloff;8;0;Create;True;0;0;False;0;False;0;1.25;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1752.052,-61.25757;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;12;-1404,-67.3727;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1246.356,-67.1994;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1754.156,-157.0975;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;15;-1115.9,-68.37271;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-805.2878,-92.87271;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1057.6,145.6142;Inherit;False;Property;_falloffIntensity;falloffIntensity;7;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-651.2958,-154.2925;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-522.5671,-81.13002;Inherit;False;Property;_brightnessMinimum;brightnessMinimum;5;0;Create;True;0;0;False;0;False;0;0;0;2048;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-521.267,-1.829955;Inherit;False;Property;_brightnessMaximum;brightnessMaximum;6;0;Create;True;0;0;False;0;False;0;32;0;2048;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;8;-249.5666,-152.6299;Inherit;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;19;-639.2429,101.8154;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;4;102.4505,-84.46111;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;SpareParts/VertexColor_BirghtnessFalloff;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;True;False;False;False;False;False;Off;1;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;5;4;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;55;0;53;0
WireConnection;55;1;56;0
WireConnection;47;0;57;0
WireConnection;47;1;54;0
WireConnection;46;0;47;0
WireConnection;46;1;55;0
WireConnection;24;0;46;0
WireConnection;43;0;24;0
WireConnection;28;0;43;0
WireConnection;30;0;28;0
WireConnection;30;1;31;0
WireConnection;59;0;30;0
WireConnection;59;1;60;0
WireConnection;59;2;58;3
WireConnection;36;0;59;0
WireConnection;38;0;2;4
WireConnection;38;1;37;0
WireConnection;12;0;38;0
WireConnection;12;1;13;0
WireConnection;10;0;12;0
WireConnection;10;1;13;0
WireConnection;41;0;2;0
WireConnection;41;1;37;0
WireConnection;15;0;10;0
WireConnection;15;2;13;0
WireConnection;16;0;41;0
WireConnection;16;1;15;0
WireConnection;9;0;41;0
WireConnection;9;1;16;0
WireConnection;9;2;11;0
WireConnection;8;0;9;0
WireConnection;8;3;5;0
WireConnection;8;4;6;0
WireConnection;19;0;38;0
WireConnection;19;1;15;0
WireConnection;19;2;11;0
WireConnection;4;2;8;0
WireConnection;4;9;19;0
ASEEND*/
//CHKSM=E2C0645EC6AC2FFA4ECA7DFF471E769712F05C9B