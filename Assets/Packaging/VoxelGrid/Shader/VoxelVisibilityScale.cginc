#ifndef VoxelVisibilityScale_Included
#define VoxelVisibilityScale_Included

#define VoxelGeometryInitializer \
    float halfSize = 0.5f*_ElementSize*_ElementScale * ( 100.0f * gin[0].normal.z ); \
	if ( halfSize < 0.0001f ) { clip (0); return; } \
    float xm = gin[0].vertex.x - halfSize; \
    float xM = gin[0].vertex.x + halfSize; \
    float ym = gin[0].vertex.y - halfSize; \
    float yM = gin[0].vertex.y + halfSize; \
    float zm = gin[0].vertex.z - halfSize; \
    float zM = gin[0].vertex.z + halfSize; \

#define VoxelVisibilityInitializer \
    float t0 = gin[0].normal.x; \
	if ( _TestGlobalConnectivity > 0 ) { \
	    t0 = gin[0].normal.y; \
	} \
	if ( _DoNotShow > 0 ) { clip (0); return; } \
	if ( ( _DiscardInterior > 0 ) && ( t0 < 0.01f ) ) { clip (0); return; } \
    float x = floor ( 100.0f * t0 + 0.5f );

#define TestFace_Forward	( ( _DiscardConnectedFaces == 0 ) || ( x >= 32 ) )
#define UnsetFace_Forward	x -= 32;

#define TestFace_Back		( ( _DiscardConnectedFaces == 0 ) || ( x >= 16 ) )
#define UnsetFace_Back		x -= 16;

#define TestFace_Right		( ( _DiscardConnectedFaces == 0 ) || ( x >= 8 ) )
#define UnsetFace_Right		x -= 8;

#define TestFace_Left		( ( _DiscardConnectedFaces == 0 ) || ( x >= 4 ) )
#define UnsetFace_Left		x -= 4;

#define TestFace_Up			( ( _DiscardConnectedFaces == 0 ) || ( x >= 2 ) )
#define UnsetFace_Up		x -= 2;

#define TestFace_Down		( ( _DiscardConnectedFaces == 0 ) || ( x >= 1 ) )
#define UnsetFace_Down		x -= 1;

#endif
