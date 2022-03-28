    Shader "Custom/CrossFade"
    {
      Properties
      {
        _Blend ( "Blend", Range ( 0, 1 ) ) = 0.5
        _Color ( "Main Color", Color ) = ( 1, 1, 1, 1 )
        _MainTex ( "Texture 1", 2D ) = "white" {}
        _Texture2 ( "Texture 2", 2D ) = ""
      }

      SubShader
      {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300
        Pass
        {
          SetTexture[_MainTex]
          SetTexture[_Texture2]
          {
            ConstantColor ( 0, 0, 0, [_Blend] )
            Combine texture Lerp( constant ) previous
          }    
        }
      
        CGPROGRAM
        #pragma surface surf Lambert alpha
        
        sampler2D _MainTex;
        sampler2D _Texture2;
        fixed4 _Color;
        float _Blend;

        struct Input
        {
          float2 uv_MainTex;
          float2 uv_Texture2;
        };
        
        void surf ( Input IN, inout SurfaceOutput o )
        {
          fixed4 t1  = tex2D( _MainTex, IN.uv_MainTex ) * _Color;
          fixed4 t2  = tex2D ( _Texture2, IN.uv_Texture2 ) * _Color;
          o.Albedo  = lerp( t1, t2, _Blend );
          o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a; 
        }
        ENDCG
      }
      FallBack "Diffuse"
    }