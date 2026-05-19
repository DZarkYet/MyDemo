Shader "Unlit/BumpedDiffuse"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("MainTex", 2D) = ""{}
        _BumpMap("BumpMap", 2D) = ""{}
        _BumpScale("BumpScale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Tags{ "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;

                float4 T2W0 :TEXCOORD1;
                float4 T2W1 :TEXCOORD2;
                float4 T2W2 :TEXCOORD3;

                //SHADOW_COORDS(4)
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float _BumpScale;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = normalize(UnityObjectToWorldDir(v.tangent));
                float3 binormal = normalize(cross(worldNormal, worldTangent) * v.tangent.w);

                o.T2W0 = float4(worldNormal.x, worldTangent.x, binormal.x, worldPos.x);
                o.T2W1 = float4(worldNormal.y, worldTangent.y, binormal.y, worldPos.y);
                o.T2W2 = float4(worldNormal.z, worldTangent.z, binormal.z, worldPos.z);

                //TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                float3 tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

                float3 worldPos = float3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                fixed3 worldNormal = float3(dot(i.T2W0.xyz, tangentNormal), dot(i.T2W1.xyz, tangentNormal), dot(i.T2W2.xyz, tangentNormal));
                
                fixed3 albedo = tex2D(_MainTex, i.uv.xy) * _Color;
                fixed3 lambertColor = _LightColor0.rgb * albedo.rgb * max(0, dot(worldNormal, lightDir));

                //UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + lambertColor; //* atten;

                return fixed4(color, 1);
            }
            ENDCG
        }
        Pass
        {
            Tags{ "LightMode"="ForwardAdd" }

            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;

                float4 T2W0 :TEXCOORD1;
                float4 T2W1 :TEXCOORD2;
                float4 T2W2 :TEXCOORD3;

                //SHADOW_COORDS(4)
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float _BumpScale;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = normalize(UnityObjectToWorldDir(v.tangent));
                float3 binormal = normalize(cross(worldNormal, worldTangent) * v.tangent.w);

                o.T2W0 = float4(worldNormal.x, worldTangent.x, binormal.x, worldPos.x);
                o.T2W1 = float4(worldNormal.y, worldTangent.y, binormal.y, worldPos.y);
                o.T2W2 = float4(worldNormal.z, worldTangent.z, binormal.z, worldPos.z);

                //TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                float3 tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

                float3 worldPos = float3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                fixed3 worldNormal = float3(dot(i.T2W0.xyz, tangentNormal), dot(i.T2W1.xyz, tangentNormal), dot(i.T2W2.xyz, tangentNormal));
                
                fixed3 albedo = tex2D(_MainTex, i.uv.xy) * _Color;
                fixed3 lambertColor = _LightColor0.rgb * albedo.rgb * max(0, dot(worldNormal, lightDir));

                //UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + lambertColor; //* atten;

                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
