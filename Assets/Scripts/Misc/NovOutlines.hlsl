struct ScharrOperators {
    float3x3 x;
    float3x3 y;
};

ScharrOperators GetEdgeDetectionKernels() {
    ScharrOperators kernels;
    kernels.x = float3x3(-3, -10, -3, 0, 0, 0, 3, 10, 3);
    kernels.y = float3x3(-3, 0, 3, -10, 0, 10, -3, 0, 3);
    return kernels;
}