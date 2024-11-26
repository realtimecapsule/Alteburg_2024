//NOTE! This file is based on Unity file "TerrainLit.shader" which was used as a template for adding all the InTerra features.
Shader "InTerra/HDRP Tessellation/Terrain (Lit with Features)"
{
    Properties
    {
        [HideInInspector] [ToggleUI] _EnableHeightBlend("EnableHeightBlend", Float) = 0.0
        _HeightTransition("Height Transition", Range(0, 60)) = 0.0
        [HideInInspector] [Enum(Off, 0, From Ambient Occlusion, 1)]  _SpecularOcclusionMode("Specular Occlusion Mode", Int) = 1
        [HideInInspector][PerRendererData] _NumLayersCount("Total Layer Count", Float) = 1.0
        // Following are builtin properties
        // Stencil state
        // Forward
        [HideInInspector] _StencilRef("_StencilRef", Int) = 0  // StencilUsage.Clear
        [HideInInspector] _StencilWriteMask("_StencilWriteMask", Int) = 3 // StencilUsage.RequiresDeferredLighting | StencilUsage.SubsurfaceScattering
        // GBuffer
        [HideInInspector] _StencilRefGBuffer("_StencilRefGBuffer", Int) = 2 // StencilUsage.RequiresDeferredLighting
        [HideInInspector] _StencilWriteMaskGBuffer("_StencilWriteMaskGBuffer", Int) = 3 // StencilUsage.RequiresDeferredLighting | StencilUsage.SubsurfaceScattering
        // Depth prepass
        [HideInInspector] _StencilRefDepth("_StencilRefDepth", Int) = 0 // Nothing
        [HideInInspector] _StencilWriteMaskDepth("_StencilWriteMaskDepth", Int) = 8 // StencilUsage.TraceReflectionRay

        // Blending state
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector][ToggleUI] _TransparentZWrite("_TransparentZWrite", Float) = 0.0
        [HideInInspector] _CullMode("__cullmode", Float) = 2.0
        [HideInInspector] _ZTestDepthEqualForOpaque("_ZTestDepthEqualForOpaque", Int) = 4 // Less equal
        [HideInInspector] _ZTestGBuffer("_ZTestGBuffer", Int) = 4

        [ToggleUI] _EnableInstancedPerPixelNormal("Instanced per pixel normal", Float) = 1.0

        [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}

        // Caution: C# code in BaseLitUI.cs call LightmapEmissionFlagsProperty() which assume that there is an existing "_EmissionColor"
        // value that exist to identify if the GI emission need to be enabled.
        // In our case we don't use such a mechanism but need to keep the code quiet. We declare the value and always enable it.
        // TODO: Fix the code in legacy unity so we can customize the behavior for GI
        [HideInInspector] _EmissionColor("Color", Color) = (1, 1, 1)

        // HACK: GI Baking system relies on some properties existing in the shader ("_MainTex", "_Cutoff" and "_Color") for opacity handling, so we need to store our version of those parameters in the hard-coded name the GI baking system recognizes.
        [HideInInspector] _MainTex("Albedo", 2D) = "white" {}
        [HideInInspector] _Color("Color", Color) = (1,1,1,1)

        [HideInInspector] [ToggleUI] _SupportDecals("Support Decals", Float) = 1.0
        [HideInInspector] [ToggleUI] _ReceivesSSR("Receives SSR", Float) = 1.0
        [HideInInspector] [ToggleUI] _AddPrecomputedVelocity("AddPrecomputedVelocity", Float) = 0.0

        //inTerra
        _HT_distance("Distance",  vector) = (3,10,0,25)
        _HT_distance_scale("Scale",   Range(0,0.5)) = 0.25
        _HT_cover("Cover strenght",   Range(0,1)) = 0.6
        _Distance_HeightTransition("Distance Height blending Sharpness ", Range(0,60)) = 10
        _TriplanarSharpness("Triplanar Sharpness",   Range(4,10)) = 9
        _ParallaxAffineStepsTerrain("", Float) = 3        
        _TriplanarOneToAllSteep("", Float) = 0
        _TerrainColorTintTexture("Color Tint Texture", 2D) = "white" {}
        _TerrainColorTintStrenght("Color Tint Strenght", Range(1, 0)) = 0
        _TerrainNormalTintTexture("Additional Normal Texture", 2D) = "bump" {}
        _TerrainNormalTintStrenght("Additional Normal Strenght", Range(0, 1)) = 0.0
        _TerrainNormalTintDistance("Additional Normal Distance",  vector) = (3,10,0,25)
        [HideInInspector] _TerrainSizeXZPosY("",  Vector) = (0,0,0)

        //Tessellation       
        _TessellationFactor("Tessellation Factor", Range(0.0, 64.0)) = 8.0
        _TessellationFactorMinDistance("Tessellation start fading distance", Float) = 5.0
        _TessellationFactorMaxDistance("Tessellation end fading distance", Float) = 25.0
        _TessellationFactorTriangleSize("Tessellation triangle size", Float) = 100.0
        _TessellationShapeFactor("Tessellation shape factor", Range(0.0, 1.0)) = 0.75 // Only use with Phong

        [HideInInspector]_TessellationBackFaceCullEpsilon("Tessellation back face epsilon", Range(-1, 0)) = -0.25
        [HideInInspector]_TessellationMaxDisplacement("Float", Float) = 1

        _MipMapFade("MipMap fade",  vector) = (5,20,-5,35)
        _MipMapLevel("MipMap level", Float) = 0

        _Tessellation_HeightTransition("Tessellation Height blending Sharpness ", Range(0,60)) = 15
        _TessellationShadowQuality("Quality of Tessellation Shadows", Range(0,1)) = 0.5
        _TrackTessallationHeightTransition("Track Tessellation Height blending Sharpness ", Range(0,60)) = 15
        _TrackTessallationHeightOffset("",Range(-1.0 ,1.0)) = 0

        _TrackAO("", Range(0,1)) = 0.8
        _TrackTessallation("", Range(0,1)) = 0
        _TrackEdgeNormals("Track Edge Normals", Float) = 2
        _TrackDetailTexture("Track Color Detail Texture", 2D) = "white" {}
        [Normal] _TrackDetailNormalTexture("Track Normal Detail Texture", 2D) = "bump" {}
        _TrackDetailNormalStrenght("Track Detail Normal Strenght", Float) = 1
        _TrackNormalStrenght("Track Normal Strenght", Float) = 1
        _TrackEdgeSharpness("Track Edge Normals", Range(0.001,4)) = 1
        _TrackHeightOffset("Track Heightmap Offset", Range(-1,1)) = 0
        _TrackMultiplyStrenght("Track Multiply strenght", Float) = 3
        _TrackHeightTransition("Track Normal Strenght", Range(0, 60)) = 20
        _ParallaxTrackAffineSteps("", Float) = 3
        _ParallaxTrackSteps("", Float) = 5
        _Gamma("", Float) = 0
        _WorldMapping("", Float) = 0

        _HeightmapBlending("", Float) = 1
        _Terrain_Parallax("", Float) = 0
        _Tracks("", Float) = 0
    }

    HLSLINCLUDE

    #pragma target 5.0
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    // Terrain builtin keywords
    #pragma shader_feature_local _TERRAIN_8_LAYERS
    #pragma shader_feature_local _NORMALMAP
    #pragma shader_feature_local _MASKMAP
    #pragma shader_feature_local _SPECULAR_OCCLUSION_NONE

    // Sample normal in pixel shader when doing instancing
    #pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL

    #pragma shader_feature_local_fragment _DISABLE_DECALS
    #pragma shader_feature_local _ADD_PRECOMPUTED_VELOCITY

    //enable GPU instancing support
    #pragma multi_compile_instancing
    #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

    #pragma multi_compile _ _ALPHATEST_ON


    //-----------TESSELLATION-------------------------------
    #define TESSELLATION_ON
    #define HAVE_TESSELLATION_MODIFICATION

    #define _TESSELLATION_DISPLACEMENT
    #define _HEIGHTMAP 
    #define _CONSERVATIVE_DEPTH_OFFSET //heightmap
    //--------------------------------------------------------------

    // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
    #define _DEFERRED_CAPABLE_MATERIAL
    #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
        #define _WRITE_TRANSPARENT_MOTION_VECTOR
    #endif


    // InTerra Keywords
    #pragma shader_feature_local __ _TERRAIN_TRIPLANAR_ONE _TERRAIN_TRIPLANAR
    #pragma shader_feature_local_fragment _TERRAIN_DISTANCEBLEND
    #pragma shader_feature_local_domain _TESSELLATION_PHONG
    #pragma shader_feature_local _LAYERS_TWO

    #define INTERRA_TERRAIN

    #include "InTerra_TerrainLit_Splatmap_Includes.hlsl"

    ENDHLSL

    SubShader
    {
        PackageRequirements { "com.unity.render-pipelines.high-definition":"[12.0,13.99.9]" }

        // This tags allow to use the shader replacement features
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
            "RenderType" = "Opaque"
            "SplatCount" = "8"
            "MaskMapR" = "Metallic"
            "MaskMapG" = "AO"
            "MaskMapB" = "Height"
            "MaskMapA" = "Smoothness"
            "DiffuseA" = "Smoothness (becomes Density when Mask map is assigned)"   // when MaskMap is disabled
            "DiffuseA_MaskMapUsed" = "Density"                                      // when MaskMap is enabled
            "TerrainCompatible" = "True"
        }

        // Caution: The outline selection in the editor use the vertex shader/hull/domain shader of the first pass declare. So it should not bethe  meta pass.
        Pass
        {
            Name "GBuffer"
            Tags { "LightMode" = "GBuffer" } // This will be only for opaque object based on the RenderQueue index

            Cull [_CullMode]
            ZTest [_ZTestGBuffer]

            Stencil
            {
                WriteMask [_StencilWriteMaskGBuffer]
                Ref [_StencilRefGBuffer]
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM

            #pragma require tessellation tessHW

            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
            // Setup DECALS_OFF so the shader stripper can remove variants
            #pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT
            #pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT
            #pragma multi_compile_fragment _ LIGHT_LAYERS            

            #define SHADERPASS SHADERPASS_GBUFFER
            #include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
            #include "InTerra_TerrainLitTemplate.hlsl"
            #include "InTerra_TerrainLit_Splatmap.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma hull Hull
            #pragma domain Domain

            ENDHLSL
        }

        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags{ "LightMode" = "META" }

            Cull Off

            HLSLPROGRAM

            // Lightmap memo
            // DYNAMICLIGHTMAP_ON is used when we have an "enlighten lightmap" ie a lightmap updated at runtime by enlighten.This lightmap contain indirect lighting from realtime lights and realtime emissive material.Offline baked lighting(from baked material / light,
            // both direct and indirect lighting) will hand up in the "regular" lightmap->LIGHTMAP_ON.

            // No tessellation for Meta pass
            #undef TESSELLATION_ON

            #define SHADERPASS SHADERPASS_LIGHT_TRANSPORT
            #pragma shader_feature EDITOR_VISUALIZATION
            #include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
            #include "InTerra_TerrainLitTemplate.hlsl"
            #include "InTerra_TerrainLit_Splatmap.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }

            Cull[_CullMode]

            ZClip [_ZClip]
            ZWrite On
            ZTest LEqual

            ColorMask 0

            HLSLPROGRAM

            #pragma require tessellation tessHW

            #define SHADERPASS SHADERPASS_SHADOWS
            #include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
            #include "InTerra_TerrainLitTemplate.hlsl"
            #include "InTerra_TerrainLit_Splatmap.hlsl"          

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma hull Hull
            #pragma domain Domain

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{ "LightMode" = "DepthOnly" }

            Cull[_CullMode]

            // To be able to tag stencil with disableSSR information for forward
            Stencil
            {
                WriteMask [_StencilWriteMaskDepth]
                Ref [_StencilRefDepth]
                Comp Always
                Pass Replace
            }

            ZWrite On

            HLSLPROGRAM

            #pragma require tessellation tessHW

            // In deferred, depth only pass don't output anything.
            // In forward it output the normal buffer
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile_fragment _ WRITE_DECAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_DEPTH_ONLY

            #include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
            #include "InTerra_TerrainLitTemplate.hlsl"
            #ifdef WRITE_NORMAL_BUFFER
                #if defined(_NORMALMAP)
                    #define OVERRIDE_SPLAT_SAMPLER_NAME sampler_Normal0
                #elif defined(_MASKMAP)
                    #define OVERRIDE_SPLAT_SAMPLER_NAME sampler_Mask0
                #endif
            #endif
            
            #include "InTerra_TerrainLit_Splatmap.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma hull Hull
            #pragma domain Domain

            ENDHLSL
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "Forward" } // This will be only for transparent object based on the RenderQueue index

            Stencil
            {
                WriteMask [_StencilWriteMask]
                Ref [_StencilRef]
                Comp Always
                Pass Replace
            }

            // In case of forward we want to have depth equal for opaque mesh
            ZTest [_ZTestDepthEqualForOpaque]
            ZWrite [_ZWrite]
            Cull [_CullMode]

            HLSLPROGRAM

            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
            #pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON
            // Setup DECALS_OFF so the shader stripper can remove variants
            #pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT
            #pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT

            // Supported shadow modes per light type
            #pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH

            #pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST

            #pragma require tessellation tessHW

            #define SHADERPASS SHADERPASS_FORWARD
            #include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
            #include "InTerra_TerrainLitTemplate.hlsl"
            #include "InTerra_TerrainLit_Splatmap.hlsl"

            

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma hull Hull
            #pragma domain Domain

            ENDHLSL
        }

        Pass
        {
            Name "SceneSelectionPass"
            Tags { "LightMode" = "SceneSelectionPass" }

            Cull Off

            HLSLPROGRAM

            #pragma editor_sync_compilation
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
            #define SCENESELECTIONPASS
            #undef TESSELLATION_ON
            #include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
            #include "InTerra_TerrainLitTemplate.hlsl"
            #include "InTerra_TerrainLit_Splatmap.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
    }

    Dependency "BaseMapShader" = "Hidden/InTerra/HDRP/TerrainLit_Basemap"
    Dependency "BaseMapGenShader" = "Hidden/InTerra/HDRP/TerrainLit_BasemapGen"
    CustomEditor "InTerra.InTerra_TerrainShaderGUI"
}
