using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.IO;
using System;
using QRCodeDecoderLibrary;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Graphics = System.Drawing.Graphics;

using UniRx;

namespace HocaInk.InteractiveWall
{
    class ImageConverter
    {
        private const int MAX_IMAGE_SIZE_X = 2048;
        private const int MAX_IMAGE_SIZE_Y = 1448;

        private static int _distanceBetweenMarksX;
        private static int _distanceBetweenMarksY;
        private static QRDecoder _qrDecoder;
        private static int _curretnImageWigth;
        private static int _curretnImageHeight;
        private static string _vehicleName;
        private static QRCodeFinder[] _finders;

        private MaterialGenerator _materialGenerator;

        public ImageConverter(MaterialGenerator materialGenerator)
        {
            _materialGenerator = materialGenerator;
        }

        public void StartConversion()
        {
            
            try
            {

#if DEBUG

                // debug mode
                // change current directory to work directory if exist
                string CurDir = Environment.CurrentDirectory;
                GameManager._curDir = CurDir;
                int Index = CurDir.IndexOf("bin\\Debug");
                if (Index > 0)
                {
                    string WorkDir = string.Concat(CurDir.AsSpan(0, Index).ToString(), "Work");
                    if (Directory.Exists(WorkDir)) Environment.CurrentDirectory = WorkDir;
                }
                //Environment.CurrentDirectory = "C:/InteractiveSoftware/";
                // open trace file
                QRCodeTrace.Open("QRCodeDecoderTrace.txt");
                QRCodeTrace.Write("QRCodeDecoder");
#endif

                _qrDecoder = new QRDecoder();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void GenerateTexture(string imagePath)
        {
            Debug.Log("GetTexture " + imagePath);
            if (File.Exists(imagePath))
            {
                var normalizeImage = Observable.Start(() =>
                {
                    try
                    {
                        Bitmap image = LoadImage(imagePath);
                        File.Delete(imagePath);
                        image = RawTransform(image);
                        image = TransformImage(image);
                        var newBitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                        int x, y;
                        for (x = 0; x < newBitmap.Width; x++)
                        {
                            for (y = 0; y < newBitmap.Height; y++)
                            {
                                System.Drawing.Color pixelColor = image.GetPixel(x, y);
                                System.Drawing.Color newColor = System.Drawing.Color.FromArgb(pixelColor.G, pixelColor.R, 0, pixelColor.B);
                                newBitmap.SetPixel(x, y, newColor);
                            }
                        }
                        newBitmap.Save("c:/InteractiveSoftware/Converted/" + _vehicleName + ".jpg", ImageFormat.Jpeg);
                        var imageByte = ImageToByteArray(newBitmap);;
                        return imageByte;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                        return null;
                    }
                });
                Observable.WhenAll(normalizeImage)
                    .ObserveOnMainThread()
                    .Subscribe(normImage =>
                    {
                        if (normImage[0] != null)
                        {
                            var texture = new Texture2D(MAX_IMAGE_SIZE_X, MAX_IMAGE_SIZE_Y, TextureFormat.RGBA32, false);
                            texture.LoadRawTextureData(normImage[0]);
                            texture.Apply();
                            texture.name = _vehicleName;
                            _materialGenerator.CreateMaterial(texture);
                        }
                        else
                        {
                            Debug.Log("Conversion error");
                        }
                    });
            }
            else
            {
                Debug.Log("No file");
            }
            return;
        }

        

        private static Bitmap LoadImage(string path)
        {
            Bitmap currentImage = null;
            int counter = 0;
            while (counter < 10 && currentImage == null)
            {
                try
                {
                    using (var bmpTemp = new Bitmap(path))
                    {

                        if (bmpTemp.Width > MAX_IMAGE_SIZE_X)
                            currentImage = new Bitmap(bmpTemp, MAX_IMAGE_SIZE_X, MAX_IMAGE_SIZE_Y);
                        else
                            currentImage = new Bitmap(bmpTemp);
                    }
                }
                catch (ArgumentException)
                {
                    counter++;
                    Debug.Log("Acces denied. Try again " + counter + "/10");
                    Thread.Sleep(100);
                }
            }
            if (currentImage == null)
            {
                return null;
            }
            _curretnImageWigth = currentImage.Width;
            _curretnImageHeight = currentImage.Height;
            return currentImage;
        }

        private static Bitmap RawTransform(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            QRCodeResult[] qrResult = _qrDecoder.ImageDecoder(bitmap);
            _vehicleName = QRDecoder.ByteArrayToStr(qrResult[0].DataArray);
            _finders = qrResult[0].FindersArray;
            ChangeAlingment(ref bitmap, GetRotateType(qrResult[0].CornerPosition));
            return bitmap;
        }

        private static Bitmap ChangeAlingment(ref Bitmap bitmap, RotateFlipType rotateType)
        {
            if (rotateType != RotateFlipType.RotateNoneFlipNone)
            {
                bitmap.RotateFlip(rotateType);
                if (rotateType != RotateFlipType.Rotate180FlipNone)
                {
                    _curretnImageWigth = bitmap.Width;
                    _curretnImageHeight = bitmap.Height;
                }
                TransformFinders(rotateType);
            }
            return bitmap;
        }

        private static void TransformFinders(RotateFlipType rotateType)
        {
            var borderPoint = new Point(_curretnImageWigth, _curretnImageHeight);
            byte flipX = 0, flipY = 0;
            if  (rotateType == RotateFlipType.Rotate180FlipNone || rotateType == RotateFlipType.Rotate90FlipNone)
            {
                flipX = 1;
            }
            if (rotateType == RotateFlipType.Rotate180FlipNone || rotateType == RotateFlipType.Rotate270FlipNone)
            {
                flipY = 1;
            }
            foreach (var finder in _finders)
            {
                Debug.Log($"Position before {finder.Position}");
                Point point = new Point();
                if (rotateType == RotateFlipType.Rotate180FlipNone)
                {
                    point = finder.Position;
                }
                else
                {
                    point.X = finder.Position.Y;
                    point.Y = finder.Position.X;
                }
                point.X = borderPoint.X * flipX + point.X * (1 - 2 * flipX);
                point.Y = borderPoint.Y * flipY + point.Y * (1 - 2 * flipY);
                Debug.Log($"{borderPoint.X} * {flipX} + {point.X} * {(1 - 2 * flipX)}");
                Debug.Log($"{borderPoint.Y} * {flipY} + {point.Y} * {(1 - 2 * flipY)}");
                finder.Position = point;
                Debug.Log($"Position rotated {finder.Position}");
            }
        }

        private static RotateFlipType GetRotateType(Point topLeftMarkPosition)
        {
            if (topLeftMarkPosition.Y < _curretnImageHeight / 2)
                if (topLeftMarkPosition.X < _curretnImageWigth / 2)
                    return RotateFlipType.RotateNoneFlipNone;
                else
                    return RotateFlipType.Rotate270FlipNone;
            else
                if (topLeftMarkPosition.X < _curretnImageWigth / 2)
                return RotateFlipType.Rotate90FlipNone;
            else
                return RotateFlipType.Rotate180FlipNone;
        }

        private static Bitmap TransformImage(Bitmap image)
        {
            if (_finders == null || _finders.Length < 7)
                return null;
            //foreach (var finder in finders)
            //{
            //    Debug.Log(finder.ToString());
            //}
            var markPositions = GetMarks(_finders);
            var rotateAngle = GetRotateAngle(markPositions);
            var currentCenter = GetCurrentCenter(markPositions);
            image = RotateImage(image, -rotateAngle, currentCenter);
            image = ResizeImage(image, markPositions);
            Graphics.FromImage(image).FillRectangle(Brushes.Brown, new Rectangle(0, 0, _curretnImageWigth / 5, _curretnImageWigth / 5));
            return image;
        }

        private static int Round(float number)
        {
            return (int)Math.Round(number);
        }

        private static float GetRotateAngle(Point[] markPoints)
        {
            var anglesSum = 0f;
            for (int i = 0; i < markPoints.Length; i++)
            {
                int nextPointIndex = i < markPoints.Length - 1 ? i + 1 : 0;
                anglesSum += GetAngle(markPoints[i], markPoints[nextPointIndex]);
            }
            var averageAngle = anglesSum / markPoints.Length;
            return averageAngle;
        }

        private static float GetAngle(Point point1, Point point2)
        {
            float dx = point1.X - point2.X;
            float dy = point1.Y - point2.Y;
            var rezAngle = 0f;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                rezAngle = (float)Math.Atan(dy / dx);
            }
            else
            {
                rezAngle = (float)Math.Atan(-dx / dy);
            }
            //Debug.Log($"dx_{dx}, dy_{dy}, angle_{rezAngle}");
            return rezAngle * 180 / (float)Math.PI;
        }

        private static Bitmap RotateImage(Bitmap bmp, float angle, Point center)
        {
            var rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (var g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(angle);
                g.TranslateTransform(-center.X, -center.Y);
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }

        private static Bitmap ResizeImage(Bitmap source, Point[] markPoints)
        {
            _distanceBetweenMarksX = (int)Math.Round(1848f / 2048f * source.Width);
            _distanceBetweenMarksY = (int)Math.Round(1248f / 1448f * source.Height);
            var averageDistances = new float[2];
            for (int i = 0; i < averageDistances.Length * 2; i++)
            {
                int iNext = i < 3 ? i + 1 : 0;
                averageDistances[i % 2] += (float)Math.Sqrt(
                    Math.Pow(markPoints[i].X - markPoints[iNext].X, 2) +
                    Math.Pow(markPoints[i].Y - markPoints[iNext].Y, 2));
            }
            for (int i = 0; i < averageDistances.Length; i++)
            {
                averageDistances[i] /= 2;
            }
            var koefX = averageDistances[0] / _distanceBetweenMarksX;
            var koefY = averageDistances[1] / _distanceBetweenMarksY;
            int sizeX = (int)(source.Width * koefX);
            int sizeY = (int)(source.Height * koefY);
            var center = GetCurrentCenter(markPoints);
            int rectPointX = center.X - sizeX / 2;
            int rectPointY = center.Y - sizeY / 2;
            var section = new Rectangle(rectPointX, rectPointY, sizeX, sizeY);
            return new Bitmap(CropImage(source, section), source.Size);
        }

        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, new Rectangle(Point.Empty, section.Size), section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private static Point[] GetMarks(QRCodeFinder[] finders)
        {
            var rezPoints = new Point[4];
            var point = new Point();
            foreach (var finder in finders)
            {
                point = finder.Position;
                if (point.Y < _curretnImageHeight / 2)
                {
                    if (point.X < _curretnImageWigth / 2)
                    {
                        if (rezPoints[0] == Point.Empty)
                            rezPoints[0] = point;
                        else if (rezPoints[0].X > point.X || rezPoints[0].Y > point.Y)
                            rezPoints[0] = point;
                    }
                    else if (point.X > _curretnImageWigth / 2)
                    {
                        if (rezPoints[1] == Point.Empty)
                            rezPoints[1] = point;
                        else if (rezPoints[1].X < point.X || rezPoints[1].Y > point.Y)
                            rezPoints[1] = point;
                    }
                }
                else if (point.Y > _curretnImageHeight / 2)
                {
                    if (point.X < _curretnImageWigth / 2)
                    {
                        if (rezPoints[3] == Point.Empty)
                            rezPoints[3] = point;
                        else if (rezPoints[3].X > point.X || rezPoints[3].Y < point.Y)
                            rezPoints[3] = point;
                    }
                    else if (point.X > _curretnImageWigth / 2)
                    {
                        if (rezPoints[2] == Point.Empty)
                            rezPoints[2] = point;
                        else if (rezPoints[2].X < point.X || rezPoints[2].Y > point.Y)
                            rezPoints[2] = point;
                    }
                }
            }
            return rezPoints;
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Bmp) ;
                return ms.ToArray();
            }
        }

        private static Point GetCurrentCenter(Point[] markPositions)
        {
            int centerX = 0;
            int centerY = 0;
            foreach (var point in markPositions)
            {
                centerX += point.X;
                centerY += point.Y;
            }
            centerX /= 4;
            centerY /= 4;

            return new Point(centerX, centerY);
        }
    }
}
