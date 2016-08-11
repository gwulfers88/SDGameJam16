Shader "Soriano/ReflectionSurfaceNormalMap"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_BumpAmt("Distortion", range(0,64)) = 3
		_BumpMap("Normalmap", 2D) = "bump" {}
		_Tile1("Tile <1> Columns(X), Rows(Y), FPS(Z), Frame(W)", Vector) = (1,1,0,0)
		_Pan1("Pan <1> (Speed(XY))", Vector) = (0,0,0,0)
		_Rot1("Rot <1> (Pivot(XY), Angle Speed(Z), Angle(W))", Vector) = (0.5,0.5,0,0)
		_BumpMap2("Normalmap", 2D) = "bump" {}
		_Tile2("Tile <2> Columns(X), Rows(Y), FPS(Z), Frame(W)", Vector) = (1,1,0,0)
		_Pan2("Pan <2> (Speed(XY))", Vector) = (0,0,0,0)
		_Rot2("Rot <2> (Pivot(XY), Angle Speed(Z), Angle(W))", Vector) = (0.5,0.5,0,0)

		//_aDslTex("Dissolve Texture", 2D) = "gray" {}
		_aDslGl("Global Dissolve", Float) = 0.0
		_aTileDsl("Tile Dissolve Columns(X), Rows(Y), FPS(Z), Frame(W)", Vector) = (1,1,0,0)
		_aPanDsl("Pan Dissolve (Speed(XY))", Vector) = (0,0,0,0)
		_aRotDsl("Rot Dissolve (Pivot(XY), Angle Speed(Z), Angle(W))", Vector) = (0.5,0.5,0,0)
		_Cube("Cubemap Dissolve Texture", Cube) = "" { /* used to be TexGen CubeNormal */ }

		_Displacement("Displacement", Range(0.0, 10.0)) = 5.0
		_DisplacementTiling("DisplacementTiling", Range(0.1, 8.0)) = 5.0
		_DisSpeed("DisplacementSpeed", Range(0.0, 10.0)) = 0.1
	}
		CGINCLUDE
		#include "UnityCG.cginc"


		struct v2f_aniOnly
		{
			float4 vertex : POSITION;
		};

		float _Displacement;
		float _DisplacementTiling;
		float _DisSpeed;

		half3 vertexOffsetObjectSpace(appdata_full v)
		{
			return v.normal.xyz * sin((length(v.vertex.zy + v.color.rgb - 0.5) + _Time.w * _DisSpeed)*_DisplacementTiling) * _Displacement * 1.5 * v.color.a;
		}

		v2f_aniOnly vert_onlyAnimation(inout appdata_full v)
		{
			v2f_aniOnly o;

			v.vertex.xyz += vertexOffsetObjectSpace(v);
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

			return o;
		}
		ENDCG
		
	Category
	{

			// We must be transparent, so other objects are drawn before this one.
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			ZWrite Off
			Cull Off

			SubShader
			{

				// This pass grabs the screen behind the object into a texture.
				// We can access the result in the next pass as _GrabTexture
				GrabPass
				{
					Name "BASE"
					Tags{ "LightMode" = "Always" }
				}

					// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
					// on to the screen
				Pass
				{
					Name "BASE"
					Tags{ "LightMode" = "Always" }

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex vert
					#pragma fragment frag
					/*#pragma surface surf BlinnPhong*/
					#pragma vertex vert_onlyAnimation
					#pragma multi_compile_fog
					#include "UnityCG.cginc"

					struct appdata_t
					{
						float4 vertex : POSITION;
						float2 texcoord: TEXCOORD0;
					};

					struct v2f
					{
						float4 vertex : SV_POSITION;
						float4 uvgrab : TEXCOORD0;
						float2 uvbump : TEXCOORD1;
						float2 uvbump2 : TEXCOORD2;
						float2 dslcoord3 : TEXCOORD3;
						UNITY_FOG_COORDS(4)
					};

					float _BumpAmt;
					float4 _BumpMap_ST;
					float4 _BumpMap2_ST;
					float4 _aDslTex_ST;
					fixed4 _Color;
					v2f vert(appdata_t v)
					{
						v2f o;
						o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					#if UNITY_UV_STARTS_AT_TOP
						float scale = -1.0;
					#else
						float scale = 1.0;
					#endif
						o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
						o.uvgrab.zw = o.vertex.zw;
						o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
						o.uvbump2 = TRANSFORM_TEX(v.texcoord, _BumpMap2);
						o.dslcoord3 = TRANSFORM_TEX(v.texcoord, _aDslTex);
						UNITY_TRANSFER_FOG(o,o.vertex);
						return o;
					}

					sampler2D _GrabTexture;
					float4 _GrabTexture_TexelSize;

					sampler2D _BumpMap;
					float4 _Tile1;
					float4 _Pan1;
					float4 _Rot1;

					sampler2D _BumpMap2;
					float4 _Tile2;
					float4 _Pan2;
					float4 _Rot2;

					sampler2D _Cube/*_aDslTex*/;
					half _aDslGl;
					float4 _aTileDsl;
					float4 _aPanDsl;
					float4 _aRotDsl;

					
					/*samplerCUBE _Cube;*/
					float2 UV_TPR(float2 u, float4 t, float4 p, float4 r)
					{
						float2 v = float2((r.x - ((u.x - r.x)*cos((r.z*_Time.y) + (3.14159265359f*(1 + (r.w / 180)))) - (u.y - r.y)*sin((r.z*_Time.y) + (3.14159265359f*(1 + (r.w / 180)))))) + (p.x*_Time.y),(r.y - ((u.x - r.x)*sin((r.z*_Time.y) + (3.14159265359f*(1 + (r.w / 180)))) + (u.y - r.y)*cos((r.z*_Time.y) + (3.14159265359f*(1 + (r.w / 180)))))) + (p.y*_Time.y));
						v = float2(v.x / t.x + floor(_Time.y*t.z + t.w) / t.x,(v.y + t.y - 1) / t.y - floor(floor(_Time.y*t.z + t.w) / t.x) / t.y);
						return  v;
					}

					half4 frag(v2f i) : SV_Target
					{
						
						// calculate perturbed coordinates
						half2 bump = UnpackNormal(tex2D(_BumpMap, UV_TPR(i.uvbump, _Tile1, _Pan1, _Rot1))).rg;
						bump *= UnpackNormal(tex2D(_BumpMap2, UV_TPR(i.uvbump2, _Tile2, _Pan2, _Rot2))).rg;

						float2 offset = bump * _BumpAmt*_BumpAmt * _GrabTexture_TexelSize.xy;

						//Dissolve
						float4 dt;
						dt = tex2D(_Cube,UV_TPR(i.dslcoord3,_aTileDsl,_aPanDsl,_aRotDsl));
						dt.a = (dt.r + dt.g + dt.b) / 3;
						dt.a = _aDslGl >= dt.a ? 0 : 1;   //Calculate Dissolve
						half2 n = UnpackNormal(fixed4(0.5,0.5,1,0.5));
						offset = lerp(n,offset,dt.a);

						i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
						half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
						UNITY_APPLY_FOG(i.fogCoord, col);
						return col;
					}
				ENDCG
				}
			}
	}
}