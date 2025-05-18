// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpareParts/VertexColor_BirghtnessFalloff"
{
	Properties
	{
		_VertexStreamsWeight("_VertexStreamsWeight", Range( 0 , 1)) = 0
		_MainTex("_MainTex", 2D) = "white" {}
		_1X_brightnessMinimum("_1X_brightnessMinimum", Range( 0 , 2048)) = 0
		_1Y_brightnessMaximum("_1Y_brightnessMaximum", Range( 0 , 2048)) = 0
		_1Z_ProceduralTextureOpacity("_1Z_ProceduralTextureOpacity", Range( 0 , 1)) = 0
		_proceduralTextureFalloff("proceduralTextureFalloff", Range( 0 , 32)) = 0
		_falloffIntensity("falloffIntensity", Range( 0 , 1)) = 0
		_alphaFalloff("alphaFalloff", Range( 0 , 8)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha , OneMinusDstColor One
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _proceduralTextureFalloff;
		uniform float _1Z_ProceduralTextureOpacity;
		uniform float _VertexStreamsWeight;
		uniform float _alphaFalloff;
		uniform float _falloffIntensity;
		uniform float _1X_brightnessMinimum;
		uniform float _1Y_brightnessMaximum;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float temp_output_28_0 = ( 1.0 - saturate( length( uv_MainTex ) ) );
			float smoothstepResult76 = smoothstep( 0.0 , 1.0 , temp_output_28_0);
			float saferPower30 = max( ( smoothstepResult76 * temp_output_28_0 ) , 0.0001 );
			float temp_output_30_0 = pow( saferPower30 , _proceduralTextureFalloff );
			float4 temp_cast_0 = (temp_output_30_0).xxxx;
			float useVertexStreams69 = _VertexStreamsWeight;
			float lerpResult66 = lerp( _1Z_ProceduralTextureOpacity , i.uv2_tex4coord2.z , useVertexStreams69);
			float4 lerpResult59 = lerp( ( tex2D( _MainTex, uv_MainTex ) * temp_output_30_0 ) , temp_cast_0 , lerpResult66);
			float4 proceduralFalloff36 = lerpResult59;
			float4 temp_output_41_0 = ( i.vertexColor * proceduralFalloff36 );
			float4 temp_cast_1 = (_alphaFalloff).xxxx;
			float4 temp_output_107_0 = ( proceduralFalloff36 * i.vertexColor.a );
			float4 saferPower12 = max( temp_output_107_0 , 0.0001 );
			float4 temp_cast_2 = (_alphaFalloff).xxxx;
			float4 smoothstepResult15 = smoothstep( float4( 0,0,0,0 ) , temp_cast_1 , ( pow( saferPower12 , temp_cast_2 ) * _alphaFalloff ));
			float4 lerpResult9 = lerp( temp_output_41_0 , ( temp_output_41_0 * smoothstepResult15 ) , _falloffIntensity);
			float lerpResult94 = lerp( _1X_brightnessMinimum , i.uv2_tex4coord2.x , useVertexStreams69);
			float4 lerpResult19 = lerp( temp_output_107_0 , smoothstepResult15 , _falloffIntensity);
			float4 temp_output_108_0 = ( proceduralFalloff36 * lerpResult19 );
			float lerpResult101 = lerp( _1Y_brightnessMaximum , i.uv2_tex4coord2.y , useVertexStreams69);
			o.Emission = (( lerpResult94 * temp_output_108_0 ) + (( lerpResult9 * proceduralFalloff36 ) - float4( 0,0,0,0 )) * (( lerpResult101 * temp_output_108_0 ) - ( lerpResult94 * temp_output_108_0 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 ))).rgb;
			o.Alpha = temp_output_108_0.r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18707
252.8;108;1052;589;3162.613;1521.5;1.052903;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;62;-3456.067,-1281.631;Inherit;True;Property;_MainTex;_MainTex;1;0;Create;True;0;0;False;0;False;37e6f91f3efb0954cbdce254638862ea;37e6f91f3efb0954cbdce254638862ea;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-3130.5,-920.6721;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0,1;False;1;FLOAT2;0,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LengthOpNode;24;-2918.84,-920.6918;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;-2787.654,-921.7192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;28;-2594.462,-918.3499;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;76;-2425.961,-992.7413;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-4205.166,-1836.843;Inherit;False;Property;_VertexStreamsWeight;_VertexStreamsWeight;0;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2421.762,-824.001;Inherit;False;Property;_proceduralTextureFalloff;proceduralTextureFalloff;10;0;Create;True;0;0;False;0;False;0;1.5;0;32;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2254.542,-944.1873;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-3907.788,-1838.957;Inherit;False;useVertexStreams;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-3149.653,-1226.085;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;58;-1799.467,-769.6892;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;67;-1882.48,-846.6949;Inherit;False;Property;_1Z_ProceduralTextureOpacity;_1Z_ProceduralTextureOpacity;5;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;30;-2117.525,-921.1173;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1832.85,-599.4048;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-2885.507,-1286.495;Inherit;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-1844.236,-1274.204;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;66;-1517.002,-843.648;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;-1322.866,-944.7464;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-997.3036,-949.6989;Inherit;True;proceduralFalloff;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;2;-2368.504,-167.2663;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;37;-2171.786,-99.57762;Inherit;False;36;proceduralFalloff;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-1922.856,4.194573;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2054.352,210.3133;Inherit;False;Property;_alphaFalloff;alphaFalloff;12;0;Create;True;0;0;False;0;False;0;1.25;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;12;-1700.585,109.9609;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1552.989,110.9895;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;15;-1403.232,108.6243;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1905.636,-165.915;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1205.835,-179.7736;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1017.517,-112.4931;Inherit;False;Property;_falloffIntensity;falloffIntensity;11;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-679.47,-115.765;Inherit;False;36;proceduralFalloff;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-643.0396,-347.0678;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;19;-649.806,84.9144;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;100;419.8232,13.73223;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;376.5208,-225.8481;Inherit;False;Property;_1Y_brightnessMaximum;_1Y_brightnessMaximum;4;0;Create;True;0;0;False;0;False;0;64;0;2048;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;99;452.3008,-152.8315;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-41.73535,-423.001;Inherit;False;Property;_1X_brightnessMinimum;_1X_brightnessMinimum;3;0;Create;True;0;0;False;0;False;0;0;0;2048;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-376.1875,-350.3804;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;96;23.05849,-335.1933;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;95;-7.991313,-169.5878;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;94;289.9097,-418.1488;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;101;701.1813,-219.8126;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;112;-216.9135,-452.8963;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-325.5564,60.62102;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;888.7541,49.32609;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;113;-192.621,-463.7556;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;472.9597,-417.983;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;86;-4079.714,-594.165;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;55;-3368.727,-844.4167;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-3366.22,-932.7091;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-4156.697,-1045.526;Inherit;False;Property;_2Y_tilingY;_2Y_tilingY;7;0;Create;True;0;0;False;0;False;0;1;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-4144.565,-313.1626;Inherit;False;Property;_2W_offsetY;_2W_offsetY;9;0;Create;True;0;0;False;0;False;0;-0.5;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;88;-3806.689,-691.6262;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-4118.01,-67.30573;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-4112.963,-791.2487;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;84;-4085.922,-953.9517;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-4134.702,-1396.214;Inherit;False;Property;_2X_tilingX;_2X_tilingX;6;0;Create;True;0;0;False;0;False;0;1;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-4143.004,-676.5338;Inherit;False;Property;_2Z_offsetX;_2Z_offsetX;8;0;Create;True;0;0;False;0;False;0;-0.5;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;83;-3815.176,-1051.413;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;8;1105.53,-491.8133;Inherit;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-4100.697,-1150.46;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;81;-3817.229,-1389.94;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;-4104.476,-433.7408;Inherit;False;69;useVertexStreams;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;80;-4090.89,-1317.222;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;90;-4090.97,-230.0088;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;89;-3820.225,-327.4699;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;4;1580.828,53.55218;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;SpareParts/VertexColor_BirghtnessFalloff;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;True;False;False;False;False;False;Off;1;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;5;4;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;2;62;0
WireConnection;46;0;47;0
WireConnection;46;1;55;0
WireConnection;24;0;46;0
WireConnection;43;0;24;0
WireConnection;28;0;43;0
WireConnection;76;0;28;0
WireConnection;77;0;76;0
WireConnection;77;1;28;0
WireConnection;69;0;68;0
WireConnection;64;2;62;0
WireConnection;30;0;77;0
WireConnection;30;1;31;0
WireConnection;63;0;62;0
WireConnection;63;1;64;0
WireConnection;75;0;63;0
WireConnection;75;1;30;0
WireConnection;66;0;67;0
WireConnection;66;1;58;3
WireConnection;66;2;70;0
WireConnection;59;0;75;0
WireConnection;59;1;30;0
WireConnection;59;2;66;0
WireConnection;36;0;59;0
WireConnection;107;0;37;0
WireConnection;107;1;2;4
WireConnection;12;0;107;0
WireConnection;12;1;13;0
WireConnection;10;0;12;0
WireConnection;10;1;13;0
WireConnection;15;0;10;0
WireConnection;15;2;13;0
WireConnection;41;0;2;0
WireConnection;41;1;37;0
WireConnection;16;0;41;0
WireConnection;16;1;15;0
WireConnection;9;0;41;0
WireConnection;9;1;16;0
WireConnection;9;2;11;0
WireConnection;19;0;107;0
WireConnection;19;1;15;0
WireConnection;19;2;11;0
WireConnection;110;0;9;0
WireConnection;110;1;109;0
WireConnection;94;0;5;0
WireConnection;94;1;96;1
WireConnection;94;2;95;0
WireConnection;101;0;6;0
WireConnection;101;1;99;2
WireConnection;101;2;100;0
WireConnection;112;0;110;0
WireConnection;108;0;109;0
WireConnection;108;1;19;0
WireConnection;114;0;101;0
WireConnection;114;1;108;0
WireConnection;113;0;112;0
WireConnection;115;0;94;0
WireConnection;115;1;108;0
WireConnection;55;0;88;0
WireConnection;55;1;89;0
WireConnection;47;0;81;0
WireConnection;47;1;83;0
WireConnection;88;0;53;0
WireConnection;88;1;86;1
WireConnection;88;2;87;0
WireConnection;83;0;54;0
WireConnection;83;1;84;1
WireConnection;83;2;85;0
WireConnection;8;0;113;0
WireConnection;8;3;115;0
WireConnection;8;4;114;0
WireConnection;81;0;57;0
WireConnection;81;1;80;1
WireConnection;81;2;82;0
WireConnection;89;0;56;0
WireConnection;89;1;90;1
WireConnection;89;2;91;0
WireConnection;4;2;8;0
WireConnection;4;9;108;0
ASEEND*/
//CHKSM=001BA789BAECDA872EA032CB77C5337071E3A878