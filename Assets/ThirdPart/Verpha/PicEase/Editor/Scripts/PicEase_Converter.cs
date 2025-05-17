#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Verpha.PicEase
{
    internal static class PicEase_Converter
    {
        #region Properties
        public enum ExportImageFormat { OriginalImage, Settings}
        public enum ImageFormat { PNG, JPEG, JPG, TGA, TIFF }
        #endregion

        #region Methods
        public static string GetImageFormatString()
        {
            Array imageFormats = Enum.GetValues(typeof(ImageFormat));
            string[] formatStrings = new string[imageFormats.Length];

            for (int i = 0; i < imageFormats.Length; i++)
            {
                formatStrings[i] = GetExtension((ImageFormat)imageFormats.GetValue(i));
            }

            return string.Join(",", formatStrings);
        }

        public static string GetExtension(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => "png",
                ImageFormat.JPG or ImageFormat.JPEG => "jpg",
                ImageFormat.TGA => "tga",
                ImageFormat.TIFF => "tif",
                _ => "png",
            };
        }

        public static ImageFormat GetImageFormatFromExtensionImage(string extension)
        {
            return extension switch
            {
                ".png" => ImageFormat.PNG,
                ".jpg" or "jpeg" => ImageFormat.JPG,
                ".tga" => ImageFormat.TGA,
                ".tif" or ".tiff" => ImageFormat.TIFF,
                _ => PicEase_Settings.ImageEditorDefaultExportImageFormat,
            };
        }

        public static ImageFormat GetImageFormatFromExtensionMap(string extension)
        {
            return extension switch
            {
                ".png" => ImageFormat.PNG,
                ".jpg" or "jpeg" => ImageFormat.JPG,
                ".tga" => ImageFormat.TGA,
                ".tif" or ".tiff" => ImageFormat.TIFF,
                _ => PicEase_Settings.MapGeneratorDefaultExportImageFormat,
            };
        }

        public static byte[] EncodeImage(Texture2D image, ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => image.EncodeToPNG(),
                ImageFormat.JPG or ImageFormat.JPEG => image.EncodeToJPG(),
                ImageFormat.TGA => image.EncodeToTGA(),
                ImageFormat.TIFF => EncodeToTIFF(image),
                _ => image.EncodeToPNG(),
            };
        }

        #region TGA
        public static Texture2D LoadImageTGA(byte[] fileData)
        {
            using BinaryReader reader = new(new MemoryStream(fileData));

            byte idLength = reader.ReadByte();
            byte colorMapType = reader.ReadByte();
            byte imageType = reader.ReadByte();
            short colorMapFirstEntryIndex = reader.ReadInt16();
            short colorMapLength = reader.ReadInt16();
            byte colorMapEntrySize = reader.ReadByte();
            short xOrigin = reader.ReadInt16();
            short yOrigin = reader.ReadInt16();
            short width = reader.ReadInt16();
            short height = reader.ReadInt16();
            byte pixelDepth = reader.ReadByte();
            byte imageDescriptor = reader.ReadByte();

            if (idLength > 0)
            {
                reader.ReadBytes(idLength);
            }

            byte[] colorMapData = null;
            if (colorMapType == 1)
            {
                int colorMapEntryByteSize = colorMapEntrySize / 8;
                int colorMapSize = colorMapLength * colorMapEntryByteSize;
                colorMapData = reader.ReadBytes(colorMapSize);
            }

            switch (imageType)
            {
                case 0:
                    Debug.LogError("No image data included in TGA file.");
                    return null;
                case 1:
                case 2:
                case 3:
                    return LoadUncompressedTGA(reader, imageType, width, height, pixelDepth, imageDescriptor, colorMapData, colorMapEntrySize, colorMapLength);
                case 9:
                case 10:
                case 11:
                    return LoadCompressedTGA(reader, imageType, width, height, pixelDepth, imageDescriptor, colorMapData, colorMapEntrySize, colorMapLength);
                case 32:
                case 33:
                    Debug.LogError($"Unsupported TGA data type (Huffman compressed): {imageType}");
                    return null;
                default:
                    Debug.LogError($"Unsupported TGA data type: {imageType}");
                    return null;
            }
        }

        private static Texture2D LoadUncompressedTGA(BinaryReader reader, byte imageType, int width, int height, byte pixelDepth, byte imageDescriptor, byte[] colorMapData, byte colorMapEntrySize, int colorMapLength)
        {
            int bytesPerPixel = pixelDepth / 8;
            int imageSize = width * height;

            bool flipVertically = (imageDescriptor & 0x20) != 0;
            bool flipHorizontally = (imageDescriptor & 0x10) != 0;

            byte[] imageData;
            if (imageType == 2)
            {
                imageData = reader.ReadBytes(width * height * bytesPerPixel);
                for (int i = 0; i < imageData.Length; i += bytesPerPixel)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
            }
            else if (imageType == 3)
            {
                if (pixelDepth != 8)
                {
                    Debug.LogError("Unsupported pixel depth for grayscale image.");
                    return null;
                }

                byte[] grayscaleData = reader.ReadBytes(imageSize);
                imageData = new byte[imageSize * 3];
                for (int i = 0, j = 0; i < grayscaleData.Length; i++, j += 3)
                {
                    byte gray = grayscaleData[i];
                    imageData[j] = gray;
                    imageData[j + 1] = gray;
                    imageData[j + 2] = gray;
                }
                bytesPerPixel = 3;
            }
            else if (imageType == 1)
            {
                int indexBytes = pixelDepth / 8;
                byte[] imageIndices = reader.ReadBytes(imageSize * indexBytes);

                int colorMapEntryBytes = colorMapEntrySize / 8;
                if (colorMapData == null)
                {
                    Debug.LogError("Color map data is missing for color-mapped image.");
                    return null;
                }

                imageData = new byte[imageSize * colorMapEntryBytes];
                for (int i = 0; i < imageSize; i++)
                {
                    int index = 0;
                    if (indexBytes == 1)
                    {
                        index = imageIndices[i];
                    }
                    else if (indexBytes == 2)
                    {
                        index = BitConverter.ToUInt16(imageIndices, i * 2);
                    }

                    if (index >= colorMapLength)
                    {
                        Debug.LogError("Color map index out of range.");
                        return null;
                    }
                    Array.Copy(colorMapData, index * colorMapEntryBytes, imageData, i * colorMapEntryBytes, colorMapEntryBytes);
                }

                for (int i = 0; i < imageData.Length; i += colorMapEntryBytes)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
                bytesPerPixel = colorMapEntryBytes;
            }
            else
            {
                Debug.LogError($"Unsupported uncompressed image type: {imageType}");
                return null;
            }

            if (flipVertically)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    Array.Copy(imageData, row * rowSize, flippedImageData, (height - row - 1) * rowSize, rowSize);
                }
                imageData = flippedImageData;
            }
            if (flipHorizontally)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int srcIndex = row * rowSize + col * bytesPerPixel;
                        int destIndex = row * rowSize + (width - col - 1) * bytesPerPixel;
                        Array.Copy(imageData, srcIndex, flippedImageData, destIndex, bytesPerPixel);
                    }
                }
                imageData = flippedImageData;
            }

            TextureFormat format;
            if (bytesPerPixel == 4)
            {
                format = TextureFormat.RGBA32;
            }
            else if (bytesPerPixel == 3)
            {
                format = TextureFormat.RGB24;
            }
            else
            {
                Debug.LogError("Unsupported bytes per pixel.");
                return null;
            }

            Texture2D texture = new(width, height, format, false);
            texture.LoadRawTextureData(imageData);
            texture.Apply();
            return texture;
        }

        private static Texture2D LoadCompressedTGA(BinaryReader reader, byte imageType, int width, int height, byte pixelDepth, byte imageDescriptor, byte[] colorMapData, byte colorMapEntrySize, int colorMapLength)
        {
            int bytesPerPixel = pixelDepth / 8;
            int pixelCount = width * height;
            int currentPixel = 0;
            int currentByte = 0;
            byte[] imageData = new byte[pixelCount * bytesPerPixel];

            bool flipVertically = (imageDescriptor & 0x20) != 0;
            bool flipHorizontally = (imageDescriptor & 0x10) != 0;

            try
            {
                while (currentPixel < pixelCount)
                {
                    byte packetHeader = reader.ReadByte();
                    int packetType = packetHeader & 0x80;
                    int packetSize = (packetHeader & 0x7F) + 1;

                    if (packetType != 0)
                    {
                        byte[] pixelData = reader.ReadBytes(bytesPerPixel);
                        for (int i = 0; i < packetSize; i++)
                        {
                            Array.Copy(pixelData, 0, imageData, currentByte, bytesPerPixel);
                            currentByte += bytesPerPixel;
                            currentPixel++;
                        }
                    }
                    else
                    {
                        int bytesToRead = packetSize * bytesPerPixel;
                        byte[] rawData = reader.ReadBytes(bytesToRead);
                        Array.Copy(rawData, 0, imageData, currentByte, bytesToRead);
                        currentByte += bytesToRead;
                        currentPixel += packetSize;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error decoding RLE compressed TGA: " + ex.Message);
                return null;
            }

            if (imageType == 10)
            {
                for (int i = 0; i < imageData.Length; i += bytesPerPixel)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
            }
            else if (imageType == 11)
            {
                if (pixelDepth != 8)
                {
                    Debug.LogError("Unsupported pixel depth for grayscale image.");
                    return null;
                }

                byte[] grayscaleData = imageData;
                imageData = new byte[pixelCount * 3];
                for (int i = 0, j = 0; i < grayscaleData.Length; i++, j += 3)
                {
                    byte gray = grayscaleData[i];
                    imageData[j] = gray;
                    imageData[j + 1] = gray;
                    imageData[j + 2] = gray;
                }
                bytesPerPixel = 3;
            }
            else if (imageType == 9)
            {
                int colorMapEntryBytes = colorMapEntrySize / 8;
                if (colorMapData == null)
                {
                    Debug.LogError("Color map data is missing for color-mapped image.");
                    return null;
                }
                byte[] indicesData = imageData;
                imageData = new byte[pixelCount * colorMapEntryBytes];
                for (int i = 0; i < pixelCount; i++)
                {
                    int index = indicesData[i];
                    if (index >= colorMapLength)
                    {
                        Debug.LogError("Color map index out of range.");
                        return null;
                    }
                    Array.Copy(colorMapData, index * colorMapEntryBytes, imageData, i * colorMapEntryBytes, colorMapEntryBytes);
                }
                for (int i = 0; i < imageData.Length; i += colorMapEntryBytes)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
                bytesPerPixel = colorMapEntryBytes;
            }

            if (flipVertically)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    Array.Copy(imageData, row * rowSize, flippedImageData, (height - row - 1) * rowSize, rowSize);
                }
                imageData = flippedImageData;
            }
            if (flipHorizontally)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int srcIndex = row * rowSize + col * bytesPerPixel;
                        int destIndex = row * rowSize + (width - col - 1) * bytesPerPixel;
                        Array.Copy(imageData, srcIndex, flippedImageData, destIndex, bytesPerPixel);
                    }
                }
                imageData = flippedImageData;
            }

            TextureFormat format;
            if (bytesPerPixel == 4)
            {
                format = TextureFormat.RGBA32;
            }
            else if (bytesPerPixel == 3)
            {
                format = TextureFormat.RGB24;
            }
            else
            {
                Debug.LogError("Unsupported bytes per pixel.");
                return null;
            }

            Texture2D texture = new(width, height, format, false);
            texture.LoadRawTextureData(imageData);
            texture.Apply();
            return texture;
        }
        #endregion

        #region TIFF
        public class BitReader
        {
            private readonly byte[] data;
            private int bytePos;
            private int bitPos;

            public BitReader(byte[] data)
            {
                this.data = data;
                this.bytePos = 0;
                this.bitPos = 0;
            }

            public int ReadBits(int count)
            {
                int value = 0;

                while (count > 0)
                {
                    if (bytePos >= data.Length) return -1;

                    int bitsAvailable = 8 - bitPos;
                    int bitsToRead = Math.Min(bitsAvailable, count);
                    int shift = bitsAvailable - bitsToRead;
                    int mask = (1 << bitsToRead) - 1;
                    int bits = (data[bytePos] >> shift) & mask;

                    value = (value << bitsToRead) | bits;

                    bitPos += bitsToRead;
                    if (bitPos >= 8)
                    {
                        bitPos = 0;
                        bytePos++;
                    }
                    count -= bitsToRead;
                }
                return value;
            }
        }

        public static Texture2D LoadImageTIFF(byte[] fileData)
        {
            using BinaryReader reader = new(new MemoryStream(fileData));

            bool isLittleEndian = true;
            byte[] byteOrder = reader.ReadBytes(2);
            if (byteOrder[0] == 'I' && byteOrder[1] == 'I')
            {
                isLittleEndian = true;
            }
            else if (byteOrder[0] == 'M' && byteOrder[1] == 'M')
            {
                isLittleEndian = false;
            }
            else
            {
                Debug.LogError("Invalid TIFF file: Unknown byte order.");
                return null;
            }

            ushort ReadUInt16() => ReadUInt16Endian(reader, isLittleEndian);
            uint ReadUInt32() => ReadUInt32Endian(reader, isLittleEndian);

            ushort magicNumber = ReadUInt16();
            if (magicNumber != 42)
            {
                Debug.LogError("Invalid TIFF file: Incorrect magic number.");
                return null;
            }

            uint ifdOffset = ReadUInt32();
            if (ifdOffset == 0)
            {
                Debug.LogError("Invalid TIFF file: No Image File Directory found.");
                return null;
            }

            reader.BaseStream.Seek(ifdOffset, SeekOrigin.Begin);
            ushort numEntries = ReadUInt16();

            int width = 0;
            int height = 0;
            ushort bitsPerSample = 0;
            ushort samplesPerPixel = 1;
            ushort compression = 1;
            ushort photometricInterpretation = 2;
            uint[] stripOffsets = null;
            uint[] stripByteCounts = null;
            ushort planarConfiguration = 1;
            uint[] bitsPerSampleArray = null;
            ushort predictor = 1;
            ushort rowsPerStrip = 0;

            for (int i = 0; i < numEntries; i++)
            {
                ushort tag = ReadUInt16();
                ushort type = ReadUInt16();
                uint count = ReadUInt32();
                uint valueOffset = ReadUInt32();

                long currentPosition = reader.BaseStream.Position;
                switch (tag)
                {
                    case 256:
                        width = (int)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 257:
                        height = (int)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 258:
                        if (count == 1)
                        {
                            bitsPerSample = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        }
                        else
                        {
                            bitsPerSampleArray = new uint[count];
                            reader.BaseStream.Seek(valueOffset, SeekOrigin.Begin);
                            for (int j = 0; j < count; j++)
                            {
                                bitsPerSampleArray[j] = ReadUInt16Endian(reader, isLittleEndian);
                            }
                        }
                        break;
                    case 259:
                        compression = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 262:
                        photometricInterpretation = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 273:
                        stripOffsets = ReadUIntArray(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 277:
                        samplesPerPixel = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 278:
                        rowsPerStrip = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 279:
                        stripByteCounts = ReadUIntArray(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 284:
                        planarConfiguration = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    case 317:
                        predictor = (ushort)GetValue(reader, type, count, valueOffset, isLittleEndian);
                        break;
                    default:
                        break;
                }
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            }

            if (width == 0 || height == 0)
            {
                Debug.LogError("Invalid TIFF file: Width or Height not found.");
                return null;
            }

            if (bitsPerSample == 0 && bitsPerSampleArray != null)
            {
                bitsPerSample = (ushort)bitsPerSampleArray[0];
            }
            if (bitsPerSample != 8 && bitsPerSample != 16)
            {
                Debug.LogError("Unsupported TIFF format: Only 8 or 16 bits per sample supported.");
                return null;
            }
            if (planarConfiguration != 1)
            {
                Debug.LogError("Unsupported PlanarConfiguration: Only chunky format is supported.");
                return null;
            }
            if (stripOffsets == null || stripByteCounts == null)
            {
                Debug.LogError("Invalid TIFF file: StripOffsets or StripByteCounts not found.");
                return null;
            }
            if (rowsPerStrip == 0)
            {
                rowsPerStrip = (ushort)height;
            }

            int bytesPerSample = bitsPerSample / 8;
            int bytesPerPixel = bytesPerSample * samplesPerPixel;
            int imageSize = width * height * bytesPerPixel;
            byte[] imageData = new byte[imageSize];
            int offset = 0;

            int numberOfStrips = stripOffsets.Length;
            for (int i = 0; i < numberOfStrips; i++)
            {
                reader.BaseStream.Seek(stripOffsets[i], SeekOrigin.Begin);
                int bytesToRead = (int)stripByteCounts[i];
                byte[] stripData = reader.ReadBytes(bytesToRead);

                int rowsInStrip = rowsPerStrip;
                if (i == numberOfStrips - 1)
                {
                    rowsInStrip = height - rowsPerStrip * i;
                    if (rowsInStrip <= 0)
                    {
                        rowsInStrip = rowsPerStrip;
                    }
                }

                int expectedStripDataSize = rowsInStrip * width * samplesPerPixel * bytesPerSample;
                byte[] decodedStripData = null;

                if (compression == 1)
                {
                    decodedStripData = stripData;
                }
                else if (compression == 32773)
                {
                    decodedStripData = new byte[expectedStripDataSize];
                    int decodedBytes = DecodePackBits(stripData, decodedStripData, 0);
                    if (decodedBytes != expectedStripDataSize)
                    {
                        Debug.LogError("PackBits decompression error: Unexpected decoded data size.");
                        return null;
                    }
                }
                else if (compression == 5)
                {
                    decodedStripData = DecodeLZW(stripData, predictor, width, samplesPerPixel, expectedStripDataSize);
                    if (decodedStripData == null)
                    {
                        Debug.LogError("Failed to decode LZW compressed data.");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError("Unsupported TIFF compression: Only uncompressed, PackBits, and LZW compressed images are supported.");
                    return null;
                }

                Array.Copy(decodedStripData, 0, imageData, offset, decodedStripData.Length);
                offset += decodedStripData.Length;
            }

            byte[] finalImageData;
            if (bitsPerSample == 16)
            {
                int pixelCount = width * height;
                finalImageData = new byte[pixelCount * samplesPerPixel];
                int sourceIndex = 0;
                int destIndex = 0;

                while (sourceIndex < imageData.Length)
                {
                    for (int s = 0; s < samplesPerPixel; s++)
                    {
                        ushort sampleValue;
                        if (isLittleEndian)
                        {
                            sampleValue = (ushort)((imageData[sourceIndex + 1] << 8) | imageData[sourceIndex]);
                        }
                        else
                        {
                            sampleValue = (ushort)((imageData[sourceIndex] << 8) | imageData[sourceIndex + 1]);
                        }

                        byte sample8Bit = (byte)(sampleValue >> 8);
                        finalImageData[destIndex++] = sample8Bit;

                        sourceIndex += bytesPerSample;
                    }
                }
            }
            else
            {
                finalImageData = imageData;
            }

            FlipImageVertically(finalImageData, width, height, samplesPerPixel);

            TextureFormat format;
            if (samplesPerPixel == 3)
            {
                if (photometricInterpretation == 2)
                {
                    format = TextureFormat.RGB24;
                }
                else
                {
                    Debug.LogError("Unsupported PhotometricInterpretation.");
                    return null;
                }
            }
            else if (samplesPerPixel == 4)
            {
                if (photometricInterpretation == 2)
                {
                    format = TextureFormat.RGBA32;
                }
                else
                {
                    Debug.LogError("Unsupported PhotometricInterpretation.");
                    return null;
                }
            }
            else if (samplesPerPixel == 1)
            {
                if (photometricInterpretation == 0 || photometricInterpretation == 1)
                {
                    format = TextureFormat.R8;
                }
                else
                {
                    Debug.LogError("Unsupported PhotometricInterpretation.");
                    return null;
                }
            }
            else
            {
                Debug.LogError("Unsupported SamplesPerPixel.");
                return null;
            }

            Texture2D texture = new(width, height, format, false);
            texture.LoadRawTextureData(finalImageData);
            texture.Apply();
            return texture;
        }

        private static void FlipImageVertically(byte[] data, int width, int height, int samplesPerPixel)
        {
            int stride = width * samplesPerPixel;
            byte[] rowBuffer = new byte[stride];

            for (int y = 0; y < height / 2; y++)
            {
                int topRow = y * stride;
                int bottomRow = (height - y - 1) * stride;
                Array.Copy(data, topRow, rowBuffer, 0, stride);
                Array.Copy(data, bottomRow, data, topRow, stride);
                Array.Copy(rowBuffer, 0, data, bottomRow, stride);
            }
        }

        private static int DecodePackBits(byte[] input, byte[] output, int outputOffset)
        {
            int inputOffset = 0;
            int outputIndex = outputOffset;
            int outputEnd = output.Length;

            while (outputIndex < outputEnd && inputOffset < input.Length)
            {
                sbyte n = (sbyte)input[inputOffset++];
                if (n >= 0 && n <= 127)
                {
                    int count = n + 1;
                    if (inputOffset + count > input.Length || outputIndex + count > outputEnd)
                    {
                        Debug.LogError("PackBits decompression error: Invalid counts.");
                        return -1;
                    }
                    Array.Copy(input, inputOffset, output, outputIndex, count);
                    inputOffset += count;
                    outputIndex += count;
                }
                else if (n >= -127 && n <= -1)
                {
                    int count = -n + 1;
                    if (outputIndex + count > outputEnd)
                    {
                        Debug.LogError("PackBits decompression error: Output buffer overflow.");
                        return -1;
                    }
                    byte value = input[inputOffset++];
                    for (int i = 0; i < count; i++)
                    {
                        output[outputIndex++] = value;
                    }
                }
            }
            return outputIndex - outputOffset;
        }

        private static byte[] DecodeLZW(byte[] inputData, int predictor, int width, int samplesPerPixel, int expectedDataSize)
        {
            const int ClearCode = 256;
            const int EoiCode = 257;
            const int FirstAvailableCode = 258;
            const int MaxCodeSize = 12;

            int codeSize = 9;
            int nextCode = FirstAvailableCode;
            int maxCode = (1 << codeSize) - 1;

            Dictionary<int, byte[]> dictionary = new();
            for (int i = 0; i < 256; i++)
            {
                dictionary[i] = new byte[] { (byte)i };
            }

            BitReader bitReader = new(inputData);
            byte[] output = new byte[expectedDataSize];
            int outputPosition = 0;
            byte[] previousEntry = null;

            while (outputPosition < expectedDataSize)
            {
                int code = bitReader.ReadBits(codeSize);
                if (code == -1) break;

                if (code == ClearCode)
                {
                    dictionary.Clear();
                    for (int i = 0; i < 256; i++)
                    {
                        dictionary[i] = new byte[] { (byte)i };
                    }
                    codeSize = 9;
                    nextCode = FirstAvailableCode;
                    maxCode = (1 << codeSize) - 1;
                    previousEntry = null;
                    continue;
                }
                else if (code == EoiCode)
                {
                    break;
                }

                byte[] entry;
                if (dictionary.ContainsKey(code))
                {
                    entry = dictionary[code];
                }
                else if (code == nextCode && previousEntry != null)
                {
                    entry = new byte[previousEntry.Length + 1];
                    Array.Copy(previousEntry, entry, previousEntry.Length);
                    entry[^1] = previousEntry[0];
                }
                else
                {
                    Debug.LogError($"Invalid LZW code encountered: {code}");
                    return null;
                }

                int bytesToCopy = Math.Min(entry.Length, expectedDataSize - outputPosition);
                Array.Copy(entry, 0, output, outputPosition, bytesToCopy);
                outputPosition += bytesToCopy;
                if (outputPosition >= expectedDataSize)
                {
                    break;
                }

                if (previousEntry != null)
                {
                    byte[] newEntry = new byte[previousEntry.Length + 1];
                    Array.Copy(previousEntry, newEntry, previousEntry.Length);
                    newEntry[^1] = entry[0];

                    if (nextCode < (1 << MaxCodeSize))
                    {
                        dictionary[nextCode++] = newEntry;
                        if (nextCode == maxCode && codeSize < MaxCodeSize)
                        {
                            codeSize++;
                            maxCode = (1 << codeSize) - 1;
                        }
                    }
                }
                previousEntry = entry;
            }

            if (predictor == 2)
            {
                ApplyHorizontalPredictor(output, width, samplesPerPixel);
            }

            return output;
        }

        private static void ApplyHorizontalPredictor(byte[] data, int width, int samplesPerPixel)
        {
            int stride = width * samplesPerPixel;
            for (int i = 0; i < data.Length; i += stride)
            {
                for (int j = samplesPerPixel; j < stride; j++)
                {
                    data[i + j] = (byte)((data[i + j] + data[i + j - samplesPerPixel]) & 0xFF);
                }
            }
        }

        private static uint GetValue(BinaryReader reader, ushort type, uint count, uint valueOffset, bool isLittleEndian)
        {
            if ((type == 3 && count == 1) || (type == 8 && count == 1))
            {
                return valueOffset & 0xFFFF;
            }
            else if ((type == 4 && count == 1) || (type == 9 && count == 1))
            {
                return valueOffset;
            }
            else
            {
                long currentPosition = reader.BaseStream.Position;
                reader.BaseStream.Seek(valueOffset, SeekOrigin.Begin);

                uint value = 0;
                if (type == 3)
                {
                    value = ReadUInt16Endian(reader, isLittleEndian);
                }
                else if (type == 4)
                {
                    value = ReadUInt32Endian(reader, isLittleEndian);
                }
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
                return value;
            }
        }

        private static uint[] ReadUIntArray(BinaryReader reader, ushort type, uint count, uint valueOffset, bool isLittleEndian)
        {
            uint[] array = new uint[count];
            long currentPosition = reader.BaseStream.Position;

            if (type == 3)
            {
                if (count <= 2)
                {
                    array[0] = valueOffset & 0xFFFF;
                    if (count == 2)
                    {
                        array[1] = (valueOffset >> 16) & 0xFFFF;
                    }
                }
                else
                {
                    reader.BaseStream.Seek(valueOffset, SeekOrigin.Begin);
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = ReadUInt16Endian(reader, isLittleEndian);
                    }
                }
            }
            else if (type == 4)
            {
                if (count <= 1)
                {
                    array[0] = valueOffset;
                }
                else
                {
                    reader.BaseStream.Seek(valueOffset, SeekOrigin.Begin);
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = ReadUInt32Endian(reader, isLittleEndian);
                    }
                }
            }

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            return array;
        }

        private static ushort ReadUInt16Endian(BinaryReader reader, bool isLittleEndian)
        {
            byte[] bytes = reader.ReadBytes(2);
            if (isLittleEndian == BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt16(bytes, 0);
            }
            else
            {
                Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes, 0);
            }
        }

        private static uint ReadUInt32Endian(BinaryReader reader, bool isLittleEndian)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (isLittleEndian == BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(bytes, 0);
            }
            else
            {
                Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
        }

        public static byte[] EncodeToTIFF(Texture2D image)
        {
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            writer.Write((byte)'I');
            writer.Write((byte)'I');
            writer.Write((ushort)42);
            writer.Write((uint)8);

            const ushort entryCount = 10;
            long ifdStartPosition = writer.BaseStream.Position;
            writer.Write(entryCount);

            for (int i = 0; i < entryCount; i++)
            {
                writer.Write((ushort)0);
                writer.Write((ushort)0);
                writer.Write((uint)0);
                writer.Write((uint)0);
            }

            byte[] imageData = image.GetRawTextureData();
            int width = image.width;
            int height = image.height;
            int bytesPerPixel;

            TextureFormat format = image.format;
            switch (format)
            {
                case TextureFormat.RGB24:
                    bytesPerPixel = 3;
                    break;
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                    bytesPerPixel = 4;
                    break;
                case TextureFormat.R8:
                    bytesPerPixel = 1;
                    break;
                default:
                    Debug.LogError("Unsupported texture format for TIFF encoding.");
                    return null;
            }

            FlipImageVertically(imageData, width, height, bytesPerPixel);

            long imageDataOffset = writer.BaseStream.Position;
            writer.Write(imageData);

            int imageDataSize = imageData.Length;
            writer.BaseStream.Seek(ifdStartPosition + 2, SeekOrigin.Begin);

            long bitsPerSampleOffset = writer.BaseStream.Position + (12 * (entryCount - 2)) + 4;

            WriteIFDEntry(writer, 256, 4, 1, (uint)width);
            WriteIFDEntry(writer, 257, 4, 1, (uint)height);
            if (bytesPerPixel == 1)
            {
                WriteIFDEntry(writer, 258, 3, 1, 8);
            }
            else
            {
                WriteIFDEntry(writer, 258, 3, (uint)bytesPerPixel, (uint)bitsPerSampleOffset);
            }
            WriteIFDEntry(writer, 259, 3, 1, 1);

            ushort photometricInterpretation = bytesPerPixel == 1 ? (ushort)1 : (ushort)2;
            WriteIFDEntry(writer, 262, 3, 1, photometricInterpretation);
            WriteIFDEntry(writer, 273, 4, 1, (uint)imageDataOffset);
            WriteIFDEntry(writer, 277, 3, 1, (uint)bytesPerPixel);
            WriteIFDEntry(writer, 278, 4, 1, (uint)height);
            WriteIFDEntry(writer, 279, 4, 1, (uint)imageDataSize);
            WriteIFDEntry(writer, 284, 3, 1, 1);
            writer.Write((uint)0);

            if (bytesPerPixel > 1)
            {
                long currentPosition = writer.BaseStream.Position;
                writer.BaseStream.Seek(bitsPerSampleOffset, SeekOrigin.Begin);

                for (int i = 0; i < bytesPerPixel; i++)
                {
                    writer.Write((ushort)8);
                }

                writer.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            }
            return ms.ToArray();
        }

        private static void WriteIFDEntry(BinaryWriter writer, ushort tag, ushort type, uint count, uint value)
        {
            writer.Write(tag);
            writer.Write(type);
            writer.Write(count);
            writer.Write(value);
        }
        #endregion
        #endregion
    }
}
#endif