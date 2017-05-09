#ifndef VoxelStandardShadow_Included
#define VoxelStandardShadow_Included

#include "VoxelVisibilityScale.cginc"

float _DoNotShow;
float _TestGlobalConnectivity;
float _DiscardInterior;
float _DiscardConnectedFaces;

float _ElementScale;
float _ElementSize;

struct SHADOW_VERTEX
{
	float4 vertex : POSITION;
};
struct VertexShaderInput {
	float4 vertex : POSITION;
    half3 normal    : NORMAL;
};
struct Vertex2Geometry {
	float4 vertex : POSITION;
    half3 normal    : NORMAL;
};
struct Geometry2FragmentShadow
{
	V2F_SHADOW_CASTER;
};

Vertex2Geometry VS_Main(VertexShaderInput v)
{
	return v;
}

float4 FS_Main(Geometry2FragmentShadow i) : COLOR
{
	SHADOW_CASTER_FRAGMENT(i)
}

[maxvertexcount(24)]
void GS_Main(point GeometryShaderInput gin[1], inout TriangleStream<GeometryShaderOutput> triStream)
{
	SHADOW_VERTEX v;

	VoxelVisibilityInitializer
	VoxelGeometryInitializer

    if TestFace_Forward
    {
    	Geometry2FragmentShadow vt;
	    v.vertex = float4(xM, yM, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, yM, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, ym, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, ym, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
        triStream.RestartStrip();
	    UnsetFace_Forward
    }
    if TestFace_Back
    {
    	Geometry2FragmentShadow vt;
	    v.vertex = float4(xm, yM, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, yM, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, ym, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, ym, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
        triStream.RestartStrip();
	    UnsetFace_Back
    }
    if TestFace_Right
    {
    	Geometry2FragmentShadow vt;
	    v.vertex = float4(xM, yM, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, yM, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, ym, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, ym, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
        triStream.RestartStrip();
	    UnsetFace_Right
    }
    if TestFace_Left
    {
    	Geometry2FragmentShadow vt;
	    v.vertex = float4(xm, yM, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, yM, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, ym, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, ym, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
        triStream.RestartStrip();
        UnsetFace_Left
    }
    if TestFace_Up
    {
    	Geometry2FragmentShadow vt;
	    v.vertex = float4(xm, yM, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, yM, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, yM, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, yM, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
        triStream.RestartStrip();
	    UnsetFace_Up
    }
    if TestFace_Down
    {
    	Geometry2FragmentShadow vt;
	    v.vertex = float4(xm, ym, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, ym, zm, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xm, ym, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
	    v.vertex = float4(xM, ym, zM, 1.0f);
		TRANSFER_SHADOW_CASTER(vt)
        triStream.Append(vt);
        triStream.RestartStrip();
        UnsetFace_Down
    }
}

#endif
