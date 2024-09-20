Shader "Unlit/FogOfWar"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}  // The fog texture
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 200

        Pass
        {
            // Enable blending for transparency
            Blend SrcAlpha OneMinusSrcAlpha
            // Disable depth writing to prevent rendering issues
            ZWrite Off
            // Disable backface culling
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // Declare arrays in CGPROGRAM
            float4 _UnitPositions[350];  // Adjust size as needed
            float _RevealRadiuses[350];
            int _UnitCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float alpha = 1.0;

                for (int i = 0; i < _UnitCount; i++)
                {
                    float2 unitPosXZ = _UnitPositions[i].xz;
                    float2 currentPosXZ = IN.worldPos.xz;

                    float distanceToUnit = distance(currentPosXZ, unitPosXZ);
                    float revealRadius = _RevealRadiuses[i];

                    if (distanceToUnit < revealRadius)
                    {
                        alpha = 0.0;  // Fully transparent where units can see
                        break;        // Exit loop early since this point is visible
                    }
                }

                fixed4 col = tex2D(_MainTex, IN.uv);
                col.a *= alpha;  // Multiply texture alpha by calculated alpha

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Unlit"
}
