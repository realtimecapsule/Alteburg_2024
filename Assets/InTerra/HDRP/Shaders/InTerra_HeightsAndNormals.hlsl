#define INTERRA_OBJECT

#include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
#include "InTerra_Functions.hlsl"

void NormalAndHeight_float(float4 uv, float3 worldPos, float3 wNormal, float3 wTangent, float3 wBitangent, float3 wDir, out float4 terrainNormals, out float heightOffset, out float3 tangentViewDir, out float3 worldViewDir, out float3 worldNormal, out float3 worldTangent, out float3 worldBitangent)
{
    worldNormal = wNormal;
    float2 hmUV = float2 ((worldPos.x - _TerrainPosition.x) * (1 / _TerrainSize.x), (worldPos.z - _TerrainPosition.z) * (1 / _TerrainSize.z));
    float hm = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV, 0));
    heightOffset = worldPos.y - _TerrainPosition.y + (hm * -_TerrainHeightmapScale.y);
    float2 ts = float2(_TerrainHeightmapTexture_TexelSize.x, _TerrainHeightmapTexture_TexelSize.y);
    float hsX = _TerrainHeightmapScale.w / _TerrainHeightmapScale.x;
    float hsZ = _TerrainHeightmapScale.w / _TerrainHeightmapScale.z;

    float height[4]; 
    float3 terrainNormal;

    height[0] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(0, -1)), 0)).r * hsZ;
    height[1] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(-1, 0)), 0)).r * hsX;
    height[2] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(1, 0)), 0)).r * hsX;
    height[3] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(0, 1)), 0)).r * hsZ;

    terrainNormal.x = height[1] - height[2];
    terrainNormal.z = height[0] - height[3];
    terrainNormal.y = 1;

    float3x3 tangentTransform_World = float3x3(wTangent, wBitangent * GetOddNegativeScale(), wNormal);
    terrainNormals.xyz = TransformWorldToTangent(terrainNormal, tangentTransform_World);

    terrainNormals.xyz = IsNaN(terrainNormals.x) || IsNaN(terrainNormals.y) || IsNaN(terrainNormals.z) ? float3(0, 0, 1) : terrainNormals.xyz;

    float3 terrainWeights = pow(abs(terrainNormal), _TriplanarSharpness);
    terrainWeights = terrainWeights / (terrainWeights.x + terrainWeights.y + terrainWeights.z);
    terrainNormals.w = terrainWeights.y;

    worldViewDir = wDir;
    tangentViewDir = float3(0, 0, 0);

    worldTangent = wTangent;
    worldBitangent = wBitangent;

    #if defined (_TERRAIN_PARALLAX)
        float intersection = smoothstep(_NormIntersect.y, _NormIntersect.x, heightOffset);
        float3 mixedNormal = lerp(worldNormal, normalize(terrainNormal.xyz), intersection);

        float3 axisSign = mixedNormal < 0 ? -1 : 1;
        half3 tangentY = normalize(cross(mixedNormal.xyz, half3(1e-5f, 1e-5f, axisSign.y)));
        half3 bitangentY = normalize(cross(tangentY.xyz, mixedNormal.xyz)) * axisSign.y;
        half3x3 tbnY = half3x3(tangentY, bitangentY, mixedNormal);
        tangentViewDir = mul(tbnY, wDir);
    #endif 

    //================================================================================
    //-------------------------------------- UVs -------------------------------------
    //================================================================================
    _MainUV = uv.xy * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;
    _EmissionUV = uv.xy * _EmissiveColorMap_ST.xy + _EmissiveColorMap_ST.zw;    
    _DetailUV = uv.xy * _DetailMap_ST.xy + _DetailMap_ST.zw;

    #if defined (_TERRAIN_PARALLAX) && !defined(TESSELLATION_ON)
    if(_Object_Parallax)
    { 
        float lod = smoothstep(_MipMapFade.x, _MipMapFade.y, (distance(worldPos, _WorldSpaceCameraPos)));
        float3x3 objectToTangent = float3x3(worldTangent.xyz, worldBitangent.xyz, worldNormal.xyz);
        float2 objectParallaxOffset = ParallaxOffset(_MaskMap, sampler_MaskMap, _ParallaxSteps, _ParallaxHeight, _MainUV, mul(objectToTangent, worldViewDir), _ParallaxAffineSteps, _MipMapLevel + (lod * (log2(max(_MaskMap_TexelSize.z, _MaskMap_TexelSize.w)) + 1)), 0);

        _MainUV += objectParallaxOffset;
        _EmissionUV += objectParallaxOffset * (_EmissiveColorMap_ST.xy / _BaseColorMap_ST.xy);
        _DetailUV += objectParallaxOffset * (_DetailMap_ST.xy / _BaseColorMap_ST.xy);
    }
    #endif
}