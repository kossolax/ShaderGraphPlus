
HEADER
{
	Description = "";
}

MODES
{
    Default();
    VrForward();
}

FEATURES
{
}

COMMON
{
    #include "postprocess/shared.hlsl"
    #include "common/gradient.hlsl"
	#include "procedural.hlsl"
}

struct VertexInput
{
    float3 vPositionOs : POSITION < Semantic( PosXyz ); >;
    float2 vTexCoord : TEXCOORD0 < Semantic( LowPrecisionUv ); >;
};

struct PixelInput
{
    float2 vTexCoord : TEXCOORD0;

	// VS only
	#if ( PROGRAM == VFX_PROGRAM_VS )
		float4 vPositionPs		: SV_Position;
	#endif

	// PS only
	#if ( ( PROGRAM == VFX_PROGRAM_PS ) )
		float4 vPositionSs		: SV_Position;
	#endif
};

VS
{
    PixelInput MainVs( VertexInput i )
    {
        PixelInput o;
        o.vPositionPs = float4(i.vPositionOs.xyz, 1.0f);
        o.vTexCoord = i.vTexCoord;
        return o;
    }
}

PS
{
	#include "common/classes/Depth.hlsl"
    #include "postprocess/common.hlsl"
	#include "postprocess/functions.hlsl"
	#include "postprocess/PostProcessingUtils.hlsl"
	
	CreateTexture2D( g_tColorBuffer ) < Attribute( "ColorBuffer" ); SrgbRead( true ); Filter( MIN_MAG_LINEAR_MIP_POINT ); AddressU( MIRROR ); AddressV( MIRROR ); >;
	
	
    float4 MainPs( PixelInput i ) : SV_Target0
    {
		float3 FinalColor = float3( 1, 1, 1 );
		
		float2 l_0 = CalculateViewportUv( i.vPositionSs.xy );
		float l_1 = Depth::GetLinear( l_0 );
		float l_2 = i.vPositionSs.w;
		float l_3 = l_2 - -3;
		float l_4 = l_1 - l_3;
		
		FinalColor = float3( l_4, l_4, l_4 );
		
        return float4(FinalColor,1.0f);
    }
}
