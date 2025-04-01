Shader "Custom/Terrain"
{
    Properties
    {
        _Texture1 ("Low Altitude Texture", 2D) = "white" {}
        _Texture2 ("Mid Altitude Texture", 2D) = "white" {}
        _Texture3 ("High Altitude Texture", 2D) = "white" {}
        _BlendHeight1 ("First Blend Height", Range(0,1)) = 0.4
        _BlendHeight2 ("Second Blend Height", Range(0,1)) = 0.7
        _MaxHeight ("Max Terrain Height", Float) = 20
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float normHeight : TEXCOORD1;
            };

            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;
            float _BlendHeight1;
            float _BlendHeight2;
            float _MaxHeight;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normHeight = v.vertex.y / _MaxHeight;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col1 = tex2D(_Texture1, i.uv);
                fixed4 col2 = tex2D(_Texture2, i.uv);
                fixed4 col3 = tex2D(_Texture3, i.uv);
            
                if (i.normHeight < _BlendHeight1)
                    return col1;
                else if (i.normHeight < _BlendHeight2)
                    return lerp(col1, col2, (i.normHeight - _BlendHeight1) / (_BlendHeight2 - _BlendHeight1));
                else if (i.normHeight < 0.85)
                    return lerp(col2, col3, (i.normHeight - _BlendHeight2) / (0.85 - _BlendHeight2));
                else
                    return col3;
            }
            
            ENDCG
        }
    }
}
