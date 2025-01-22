using System.IO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace Viewer.Editor
{
    public class SdfGenerator
    {
        [MenuItem("Viewer/Generate Icon SDF (s)")]
        public static void GenerateAll()
        {
            const int resolution = 64;
            GenerateSDF("Axes", resolution);
            GenerateSDF("Dot", resolution);
            GenerateSDF("Grid", resolution);
            GenerateSDF("Label", resolution);
            GenerateSDF("Line", resolution);
            GenerateSDF("Tracking", resolution);
            GenerateSDF("Not allowed", resolution);
        }

        private static void GenerateSDF(string icon, int outputResolution)
        {
            string inputPath = $"Assets/Viewer/Resources/UI/Icons/{icon}.png";
            string outputPath = $"Assets/Viewer/Resources/UI/Icons/{icon}.exr";
            
            Texture2D inputTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(inputPath);
            if (inputTexture == null)
            {
                Debug.LogError("Input texture not found at " + inputPath);
                return;
            }
            
            string assetPath = AssetDatabase.GetAssetPath(inputTexture);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            int inputWidth = inputTexture.width;
            int inputHeight = inputTexture.height;
            
            Color[] inputPixels = inputTexture.GetPixels();
            NativeArray<float> binaryImage = new NativeArray<float>(inputWidth * inputHeight, Allocator.TempJob);
            NativeArray<float> fullResDistanceField = new NativeArray<float>(inputWidth * inputHeight, Allocator.TempJob);

            for (int i = 0; i < inputPixels.Length; i++)
            {
                binaryImage[i] = inputPixels[i].grayscale > 0.5f ? 0 : float.MaxValue;
            }

            float maxDistance = Mathf.Max(inputWidth, inputHeight) / 2f;

            DistanceFieldJob distanceFieldJob = new DistanceFieldJob
            {
                BinaryImage = binaryImage,
                DistanceField = fullResDistanceField,
                Width = inputWidth,
                Height = inputHeight,
                MaxDistance = maxDistance
            };

            JobHandle handle = distanceFieldJob.Schedule(inputWidth * inputHeight, 64);
            handle.Complete();
            
            Texture2D outputTexture = new Texture2D(outputResolution, outputResolution, TextureFormat.RGFloat, false);
            
            for (int y = 0; y < outputResolution; y++)
            {
                for (int x = 0; x < outputResolution; x++)
                {
                    float u = x / (float)(outputResolution - 1) * (inputWidth - 1);
                    float v = y / (float)(outputResolution - 1) * (inputHeight - 1);

                    int x1 = Mathf.FloorToInt(u);
                    int y1 = Mathf.FloorToInt(v);
                    int x2 = Mathf.Min(x1 + 1, inputWidth - 1);
                    int y2 = Mathf.Min(y1 + 1, inputHeight - 1);

                    float fx = u - x1;
                    float fy = v - y1;

                    float d11 = fullResDistanceField[y1 * inputWidth + x1];
                    float d12 = fullResDistanceField[y2 * inputWidth + x1];
                    float d21 = fullResDistanceField[y1 * inputWidth + x2];
                    float d22 = fullResDistanceField[y2 * inputWidth + x2];

                    float distance = Mathf.Lerp(
                        Mathf.Lerp(d11, d12, fy),
                        Mathf.Lerp(d21, d22, fy),
                        fx
                    );
                    outputTexture.SetPixel(x, y, new Color(distance / 10f, 0, 0, 1));
                }
            }

            outputTexture.Apply();
            
            byte[] exrData = outputTexture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
            File.WriteAllBytes(outputPath, exrData);
            AssetDatabase.Refresh();
            
            binaryImage.Dispose();
            fullResDistanceField.Dispose();

            Debug.Log($"SDF generated at {outputResolution}x{outputResolution} (from {inputWidth}x{inputHeight}) and saved to {outputPath}");
        }

        [BurstCompile]
        private struct DistanceFieldJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float> BinaryImage;
            public NativeArray<float> DistanceField;
            public int Width;
            public int Height;
            public float MaxDistance;

            public void Execute(int index)
            {
                int x = index % Width;
                int y = index / Width;

                float minDistance = MaxDistance;
                bool isInside = BinaryImage[index] == 0;

                int startX = Mathf.Max(0, x - (int)MaxDistance);
                int endX = Mathf.Min(Width, x + (int)MaxDistance);
                int startY = Mathf.Max(0, y - (int)MaxDistance);
                int endY = Mathf.Min(Height, y + (int)MaxDistance);

                if (isInside)
                {
                    DistanceField[index] = 0;
                    return;
                }

                for (int j = startY; j < endY; j++)
                {
                    for (int i = startX; i < endX; i++)
                    {
                        int neighborIndex = j * Width + i;
                        bool neighborInside = BinaryImage[neighborIndex] == 0;

                        if (neighborInside)
                        {
                            float dx = x - i;
                            float dy = y - j;
                            float distance = Mathf.Sqrt(dx * dx + dy * dy);
                            minDistance = Mathf.Min(minDistance, distance);

                            if (minDistance <= 1e-5f)
                                break;
                        }
                    }
                }

                DistanceField[index] = Mathf.Min(minDistance, MaxDistance);
            }
        }
    }
}