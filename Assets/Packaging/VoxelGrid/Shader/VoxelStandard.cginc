#ifndef VoxelStandard_Included
#define VoxelStandard_Included

#include "VoxelVisibilityScale.cginc"

float _DoNotShow;
float _TestGlobalConnectivity;
float _DiscardInterior;
float _DiscardConnectedFaces;

float _ElementScale;
float _ElementSize;

half4 fragBase (VertexOutputForwardBase i) : SV_Target { return fragForwardBaseInternal(i); }
half4 fragAdd (VertexOutputForwardAdd i) : SV_Target { return fragForwardAddInternal(i); }

GeometryShaderInput vertexShaderCopyOnly (GeometryShaderInput v) {
	return v;
}

[maxvertexcount(24)]
void geometryShaderCubeVoxel(point GeometryShaderInput gin[1], inout TriangleStream<GeometryShaderOutput> triStream)
{
    GeometryShaderInput vt = gin[0];

    VoxelVisibilityInitializer
	VoxelGeometryInitializer

    if TestFace_Forward
    {
	    vt.normal = float3(0.0f, 0.0f,  1.0f);
	    vt.vertex = float4(xM, yM, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 0.0f,  1.0f);
	    vt.vertex = float4(xm, yM, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 0.0f,  1.0f);
	    vt.vertex = float4(xM, ym, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 0.0f,  1.0f);
	    vt.vertex = float4(xm, ym, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
        triStream.RestartStrip();
	    UnsetFace_Forward
    }
    if TestFace_Back
    {
	    vt.normal = float3(0.0f, 0.0f, -1.0f);
	    vt.vertex = float4(xm, yM, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 0.0f, -1.0f);
	    vt.vertex = float4(xM, yM, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 0.0f, -1.0f);
	    vt.vertex = float4(xm, ym, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 0.0f, -1.0f);
	    vt.vertex = float4(xM, ym, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
        triStream.RestartStrip();
        UnsetFace_Back
    }
    if TestFace_Right
    {
	    vt.normal = float3(1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xM, yM, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xM, yM, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xM, ym, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xM, ym, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
        triStream.RestartStrip();
	    UnsetFace_Right
    }
    if TestFace_Left
    {
	    vt.normal = float3(-1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xm, yM, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(-1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xm, yM, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(-1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xm, ym, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(-1.0f, 0.0f,  0.0f);
	    vt.vertex = float4(xm, ym, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
        triStream.RestartStrip();
        UnsetFace_Left
    }
    if TestFace_Up
    {
	    vt.normal = float3(0.0f, 1.0f,  0.0f);
	    vt.vertex = float4(xm, yM, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 1.0f,  0.0f);
	    vt.vertex = float4(xM, yM, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 1.0f,  0.0f);
	    vt.vertex = float4(xm, yM, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, 1.0f,  0.0f);
	    vt.vertex = float4(xM, yM, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
        triStream.RestartStrip();
	    UnsetFace_Up
    }
    if TestFace_Down
    {
	    vt.normal = float3(0.0f, -1.0f,  0.0f);
	    vt.vertex = float4(xm, ym, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, -1.0f,  0.0f);
	    vt.vertex = float4(xM, ym, zm, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, -1.0f,  0.0f);
	    vt.vertex = float4(xm, ym, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
	    vt.normal = float3(0.0f, -1.0f,  0.0f);
	    vt.vertex = float4(xM, ym, zM, 1.0f);
        triStream.Append(GeometryShaderFunction ( vt ));
        triStream.RestartStrip();
        UnsetFace_Down
    }
}

#endif
