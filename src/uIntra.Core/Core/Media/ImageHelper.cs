﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace uIntra.Core.Media
{
    public class ImageHelper : IImageHelper
    {
        public MemoryStream NormalizeOrientation(Stream imageStream, string imageExtension, bool removeExifOrientationTag = true)
        {
            var img = Image.FromStream(imageStream);
            FixOrientation(img, removeExifOrientationTag);

            var newImageStream = ToStream(img, GetImageFormat(imageExtension));
            return newImageStream;
        }

        public bool IsFileImage(byte[] fileBytes)
        {
            /* TODO It is how umbraco determines that file is image. Maybe we can use next code? 
            if (UmbracoConfig.For.UmbracoSettings().Content.ImageFileTypes.Contains(ext))*/

            bool fileIsImage;
            try
            {
                using (var stream = new MemoryStream(fileBytes))
                {
                    Image.FromStream(stream).Dispose();
                }
                fileIsImage = true;
            }
            catch
            {
                fileIsImage = false;
            }

            return fileIsImage;
        }

        private void FixOrientation(Image img, bool removeExifOrientationTag = true)
        {
            var orientationTagId = 0x0112;
            if (img.PropertyIdList.Contains(orientationTagId))
            {
                var propertyItem = img.GetPropertyItem(orientationTagId);
                var flipType = GetRotateFlipTypeByExifOrientationData(propertyItem.Value[0]);
                if (flipType != RotateFlipType.RotateNoneFlipNone)
                {
                    img.RotateFlip(flipType);
                    if (removeExifOrientationTag) img.RemovePropertyItem(orientationTagId);
                }
            }
        }

        private RotateFlipType GetRotateFlipTypeByExifOrientationData(int orientation)
        {
            switch (orientation)
            {
                default:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
            }
        }

        private ImageFormat GetImageFormat(string imageExtension)
        {
            imageExtension = imageExtension.Replace(".", "");

            switch (imageExtension.ToLower())
            {
                case "bmp":
                    return ImageFormat.Bmp;

                case "gif":
                    return ImageFormat.Gif;

                case "ico":
                    return ImageFormat.Icon;

                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;

                case "png":
                    return ImageFormat.Png;

                case "tif":
                case "tiff":
                    return ImageFormat.Tiff;

                case "wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new FormatException();
            }
        }

        private MemoryStream ToStream(Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public string GetImageWithPreset(string source, string preset)
        {
            return GetImagePath(source, preset);
        }

        private string GetImagePath(string source, string imageGenClass)
        {
            return StartWithParam(source, "preset", imageGenClass);
        }

        private string StartWithParam(string source, string paramName, object value)
        {
            return $"{source}?{paramName}={value}";
        }
    }
}

