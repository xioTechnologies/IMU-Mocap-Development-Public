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
            GenerateSDF("Axes");
            GenerateSDF("Dot");
            GenerateSDF("Grid");
            GenerateSDF("Label");
            GenerateSDF("Line");
            GenerateSDF("Tracking");
        }

        private static void GenerateSDF(string icon)
        {
            string inputPath = $"Assets/Viewer/Resources/UI/Icons/{icon}.png";
            string outputPath = $"Assets/Viewer/Resources/UI/Icons/{icon}.exr";

            // Load the input texture
            Texture2D inputTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(inputPath);
            if (inputTexture == null)
            {
                Debug.LogError("Input texture not found at " + inputPath);
                return;
            }

            // Ensure the input texture is readable
            string assetPath = AssetDatabase.GetAssetPath(inputTexture);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            int width = inputTexture.width;
            int height = inputTexture.height;

            // Read pixels from the input texture
            Color[] pixels = inputTexture.GetPixels();

            // Create NativeArrays for pixel data and distance field
            NativeArray<float> binaryImage = new NativeArray<float>(width * height, Allocator.TempJob);
            NativeArray<float> distanceField = new NativeArray<float>(width * height, Allocator.TempJob);

            // Convert the image to a binary format
            for (int i = 0; i < pixels.Length; i++)
            {
                binaryImage[i] = pixels[i].grayscale > 0.5f ? 0 : float.MaxValue;
            }

            float maxDistance = 64f; // Define the maximum distance for scaling

            // Create and schedule the distance field job
            DistanceFieldJob distanceFieldJob = new DistanceFieldJob
            {
                BinaryImage = binaryImage,
                DistanceField = distanceField,
                Width = width,
                Height = height,
                MaxDistance = maxDistance
            };

            JobHandle handle = distanceFieldJob.Schedule(width * height, 64); // 64 threads per batch
            handle.Complete();

            // Create a new texture for the output
            Texture2D outputTexture = new Texture2D(width, height, TextureFormat.RGFloat, false);

            // Write distances and original values to the output texture
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    float distance = 1f - Mathf.Clamp01(distanceField[index] / maxDistance); // Scale distance to [0, 1]
                    float originalValue = pixels[index].grayscale;
                    outputTexture.SetPixel(x, y, new Color(distance, originalValue, 0, 1));
                }
            }

            outputTexture.Apply();

            // Save the output texture to an EXR file
            byte[] exrData = outputTexture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
            File.WriteAllBytes(outputPath, exrData);
            AssetDatabase.Refresh();

            // Dispose of NativeArrays
            binaryImage.Dispose();
            distanceField.Dispose();

            Debug.Log("SDF with scaled distance generated and saved to " + outputPath);
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

                            if (minDistance <= 1e-5f) // Early exit for very small distances
                                break;
                        }
                    }
                }

                DistanceField[index] = Mathf.Min(minDistance, MaxDistance);
            }
        }
    }
}