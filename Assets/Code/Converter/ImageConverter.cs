using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Linq;
using System;
using QRCodeDecoderLibrary;
using UnityEngine;
using UnityEngine.Networking;
//using UniRx;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;
using Graphics = System.Drawing.Graphics;


namespace HocaInk.InteractiveWall
{
    class ImageConverter
    {
        private string test = "";
        private Texture2D texture;
        private byte[,] byteImage;
        private Vector2 _center;
        private float _angleRotated;
        private float _tilling;
        private int _aligmentRotating;
        private int _textureSize;

        
        private const int MAX_IMAGE_SIZE_X = 2048;
        private const int MAX_IMAGE_SIZE_Y = 1448;

        private static int _distanceBetweenMarksX;
        private static int _distanceBetweenMarksY;
        private static QRDecoder _qrDecoder;
        private static int _currentImageWidth;
        private static int _currentImageHeight;
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
            if (true) // File.Exists(imagePath))
            {
                Debug.Log("IfEntered");
                //var normalizeImage = Observable.Start(() =>
                //{
                    Debug.Log("ObservableStart");
                    test = "ObservableStart";
                    try
                    {
                        //Bitmap image = LoadImageFromUrl(imagePath);
                        //File.Delete(imagePath);
                    //    image = RawTransform(image);
                    //    image = TransformImage(image);
                    ///*var newBitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                    //int x, y;
                    //for (x = 0; x < newBitmap.Width; x++)
                    //{
                    //    for (y = 0; y < newBitmap.Height; y++)
                    //    {
                    //        System.Drawing.Color pixelColor = image.GetPixel(x, y);
                    //        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(pixelColor.G, pixelColor.R, 0, pixelColor.B);
                    //        //newBitmap.SetPixel(x, y, newColor);
                    //        int offset = 15;
                    //        if (x >= offset)
                    //            newBitmap.SetPixel(x - offset, y, newColor);
                    //        else
                    //        {
                    //            newBitmap.SetPixel(_curretnImageWigth - 1 - (offset - x), y, newColor);
                    //        }
                    //    }
                    //}
                    ////newBitmap.Save("c:/InteractiveSoftware/Converted/" + _vehicleName + ".jpg", ImageFormat.Jpeg);
                    //var imageByte = ImageToByteArray(newBitmap);;*/
                    ////        return PrepareToEncode(image);
                    //byte[] byteImage = PrepareToEncode(image);
                    //if (byteImage != null)
                    //{
                    //    Texture2D texture = GetTextureFromByte(byteImage);
                    //    /*var texture = new Texture2D(MAX_IMAGE_SIZE_X, MAX_IMAGE_SIZE_Y, TextureFormat.RGBA32, false);
                    //    texture.LoadRawTextureData(normImage[0]);
                    //    texture.Apply();
                    //    texture.name = _vehicleName;*/
                    //    _materialGenerator.CreateMaterial(texture);
                    //}
                    //else
                    //{
                    //    Debug.Log("Conversion error");
                    //}
                }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                //        return null;
                    }
                //});
                //Observable.WhenAll(normalizeImage)
                //    .ObserveOnMainThread()
                //    .Subscribe(normImage =>
                //    {
                        //if (normImage[0] != null)
                        //if (byteImage != null)
                        //{
                        //    Texture2D texture = GetTextureFromByte(normImage[0]);
                        //    /*var texture = new Texture2D(MAX_IMAGE_SIZE_X, MAX_IMAGE_SIZE_Y, TextureFormat.RGBA32, false);
                        //    texture.LoadRawTextureData(normImage[0]);
                        //    texture.Apply();
                        //    texture.name = _vehicleName;*/
                        //    _materialGenerator.CreateMaterial(texture);
                        //}
                        //else
                        //{
                        //    Debug.Log("Conversion error");
                        //}
                //    });
                
                Debug.Log("ObservablePass:" + test);
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
                return null;

            _currentImageWidth = currentImage.Width;
            _currentImageHeight = currentImage.Height;
            return currentImage;
        }

        public async UniTask LoadImageFromUrlTest(string url)
        {
            using (var www = new UnityWebRequest(url))
            {
                www.downloadHandler = new DownloadHandlerTexture();
                await www.SendWebRequest();

                texture = ((DownloadHandlerTexture)(www.downloadHandler)).texture;
            }
            await WaitToNextFrame();
            _currentImageHeight = texture.height;
            _currentImageWidth = texture.width;
            await GetByteArrayFromTexture(texture);

            QRCodeResult[] qrResult = _qrDecoder.ImageDecoder(byteImage);
            await WaitToNextFrame();
            Debug.Log($"Decode complete! height: {_currentImageHeight}, width: {_currentImageWidth}");
            Debug.Log($"Decode error type {qrResult[0].CornerPosition.X}");
            Debug.Log("Vehicle name" + QRDecoder.ByteArrayToStr(qrResult[0].DataArray));
            _finders = qrResult[0].FindersArray;
            foreach (var finder in _finders)
            {
                Debug.Log($"Point ({finder.Position.X}, {finder.Position.Y})");
            }
            _aligmentRotating = GetAligmentRotation(qrResult[0].CornerPosition);
            //TransformFinders(_aligmentRotating);
            GetTransformData();
            await WaitToNextFrame();
            await CropTexture();
            texture.name = QRDecoder.ByteArrayToStr(qrResult[0].DataArray);
            _materialGenerator.CreateMaterial(texture, _center, -_aligmentRotating, -_angleRotated, _tilling);
        }

        private async UniTask WaitToNextFrame()
        {
            await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
        }

        public System.Collections.IEnumerator LoadImageFromUrl(string url)
        {
            using (var www = new UnityWebRequest(url))
            {
                www.downloadHandler = new DownloadHandlerTexture();
                yield return www.SendWebRequest();

                texture = ((DownloadHandlerTexture)(www.downloadHandler)).texture;
            }
            /*
            _currentImageHeight = texture.height;
            _currentImageWidth = texture.width;
            GetByteArrayFromTexture(texture);
            
            QRCodeResult[] qrResult = _qrDecoder.ImageDecoder(byteImage);
            Debug.Log($"Decode complete! height: {_currentImageHeight}, width: {_currentImageWidth}");
                Debug.Log($"Decode error type {qrResult[0].CornerPosition.X}");
            Debug.Log("Vehicle name" + QRDecoder.ByteArrayToStr(qrResult[0].DataArray));
            _finders = qrResult[0].FindersArray;
            foreach (var finder in _finders)
            {
                Debug.Log($"Point ({finder.Position.X}, {finder.Position.Y})");
            }
            _aligmentRotating = GetAligmentRotation(qrResult[0].CornerPosition);
            //TransformFinders(_aligmentRotating);
            GetTransformData();
            CropTexture();
            texture.name = QRDecoder.ByteArrayToStr(qrResult[0].DataArray);
            _materialGenerator.CreateMaterial(texture, _center, -_aligmentRotating, -_angleRotated, _tilling);

            //return currentImage;*/
        }

        private async UniTask GetByteArrayFromTexture(Texture2D texture)
        {
            float time = Time.realtimeSinceStartup;
            byteImage = new byte[texture.width * 3, texture.height];
			var byteX = 0;
			for (int y = 0; y < texture.height; y++)
			{
				for (int x = 0; x < texture.width; x++)
				{
					byteImage[byteX, y] = (byte)(texture.GetPixel(x, _currentImageHeight - 1 - y).r * 255);
					byteImage[byteX + 1, y] = (byte)(texture.GetPixel(x, _currentImageHeight - 1 - y).g * 255);
					byteImage[byteX + 2, y] = (byte)(texture.GetPixel(x, _currentImageHeight - 1 - y).b * 255);
					if (x< 190 && x == y && x > 180)
					{
						Debug.Log($"BfromTex. x:{x}, y:{y}, byteX:{byteX}, Color:{texture.GetPixel(x, _currentImageHeight - 1 - y)}, br:{byteImage[byteX, y]}, bg:{byteImage[byteX + 1, y]}, bb:{byteImage[byteX + 2, y]}");
					}
					byteX += 3;
				}
				byteX = 0;
                if (Time.realtimeSinceStartup - time > 0.01f)
                {
                    //Debug.Log($"Time {time}, RealTime {Time.realtimeSinceStartup}, delta {Time.realtimeSinceStartup - time}");
                    await WaitToNextFrame();
                    time = Time.realtimeSinceStartup;
                }
            }
        }

        private async UniTask CropTexture()
        {

            int fieldWidth, fieldsHeight, x1, x2, y1, y2;
           
            if (_aligmentRotating == 0 || _aligmentRotating == 180)
            {
                fieldWidth = _textureSize;
                fieldsHeight = Math.Abs(_currentImageHeight - _currentImageWidth) / 2;
                x1 = x2 = 0;
                y1 = fieldsHeight;
                y2 = fieldsHeight + _currentImageHeight;
            }
            else
            {
                fieldWidth = Math.Abs(_currentImageHeight - _currentImageWidth) / 2;
                fieldsHeight = _textureSize;
                y1 = y2 = 0;
                x1 = fieldWidth;
                x2 = fieldWidth + _currentImageWidth;
            }
            Debug.Log($"Angle{_aligmentRotating}TexSize{_textureSize}fWidth{fieldWidth}fHeight{fieldsHeight}x1:{x1}y1:{y1}x2:{x2}y2:{y2}");
            Texture2D squareTexture = new Texture2D(_textureSize, _textureSize);
            UnityEngine.Color color = new UnityEngine.Color(128 / 225f, 64 / 225f, 48 / 225f);
            await WaitToNextFrame();
            UnityEngine.Color[] colorArrayField = Enumerable.Repeat(color, fieldWidth * fieldsHeight).ToArray();
            await WaitToNextFrame();
            UnityEngine.Color[] pixelsFromTexture = texture.GetPixels();
            await WaitToNextFrame();
            squareTexture.SetPixels(0, 0, fieldWidth, fieldsHeight, colorArrayField);
            await WaitToNextFrame();
            squareTexture.SetPixels(x1, y1, texture.width, texture.height, pixelsFromTexture);
            await WaitToNextFrame();
            squareTexture.SetPixels(x2, y2, fieldWidth, fieldsHeight, colorArrayField);
            await WaitToNextFrame();
            squareTexture.Apply();
            texture = squareTexture;
        }

        private static int GetAligmentRotation(Point topLeftMarkPosition)
        {
            if (topLeftMarkPosition.Y > _currentImageHeight / 2 != topLeftMarkPosition.X > _currentImageWidth / 2)
            {
                //int tempPropertie = _currentImageHeight;
                //_currentImageHeight = _currentImageWidth;
                //_currentImageWidth = tempPropertie;
                if (topLeftMarkPosition.Y < _currentImageWidth / 2)
                    return 270;
                else
                    return 90;
            }
            else
            {
                if (topLeftMarkPosition.Y < _currentImageHeight / 2)
                    return 0;
                else
                    return 180;
            }
        }

        private void ConvertMarksToSquare(Point[] marks, int aligmentAngle)
        {
            int fieldWidth = Math.Abs(_currentImageHeight - _currentImageWidth) / 2;
            if (aligmentAngle == 0 || aligmentAngle == 180)
            {
                _textureSize = _currentImageWidth;
                for (int i = 0; i < marks.Length; i++)
                {
                    marks[i].Y += fieldWidth;
                }
            }
            else
            {
                _textureSize = _currentImageHeight;
                for (int i = 0; i < marks.Length; i++)
                {
                    marks[i].X += fieldWidth;
                }
            }

            Debug.Log($"TextureSize {_textureSize}, Alight {aligmentAngle}");
        }

        /*private static void TransformFinders(int aligment)
        {
            if (aligment == 0)
                return;
            var borderPoint = new Point(_currentImageWidth, _currentImageHeight);
            Debug.Log($"Border point {borderPoint}");
            byte flipX = 0, flipY = 0;
            if (aligment == 180 || aligment == 90)
            {
                flipX = 1;
            }
            if (aligment == 180 || aligment == 270)
            {
                flipY = 1;
            }

            foreach (var finder in _finders)
            {
                Point point = new Point();
                if (aligment == 180)
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
                Debug.Log($"Old Finder {finder.Position} NewFinder ({point})");
                finder.Position = point;
            }
        }*/

        private void GetTransformData()
        {
            if (_finders == null || _finders.Length < 7)
                return;
            var markPositions = GetMarks(_finders);
            ConvertMarksToSquare(markPositions, _aligmentRotating);
            _angleRotated = -GetRotationAngle(markPositions);
            _center = GetCurrentCenterVector(markPositions);
            GetTillinFactor(markPositions);
        }

        private Vector2 GetCurrentCenterVector(Point[] markPositions)
        {
            float centerX = 0;
            float centerY = 0;
            foreach (var point in markPositions)
            {
                centerX += point.X;
                centerY += point.Y;
            }
            Debug.Log($"Center ({centerX /4}, {centerY/4})");
            centerX /= 4 * (_textureSize - 1);
            centerY /= 4 * (_textureSize - 1);

            return new Vector2(centerX, centerY);
        }

        private void GetTillinFactor(Point[] markPoints)
        {
            if (_aligmentRotating == 0 || _aligmentRotating == 180)
            {
                _distanceBetweenMarksX = (int)Math.Round(1848f / 2048f * _currentImageWidth);
                _distanceBetweenMarksY = (int)Math.Round(1248f / 1448f * _currentImageHeight);
            }
            else
            {
                _distanceBetweenMarksY = (int)Math.Round(1848f / 2048f * _currentImageHeight);
                _distanceBetweenMarksX = (int)Math.Round(1248f / 1448f * _currentImageWidth);
            }
            
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
            float koefX = averageDistances[0] / _distanceBetweenMarksX;
            float koefY = averageDistances[1] / _distanceBetweenMarksY;
            _tilling = (koefX + koefY) / 2;
            Debug.Log($"KoefX{koefX}, KoefY{koefY}, tilling{_tilling}");
            /*int sizeX = (int)(source.Width * koefX);
            int sizeY = (int)(source.Height * koefY);
            Point center = GetCurrentCenter(markPoints);
            int rectPointX = center.X - sizeX / 2;
            int rectPointY = center.Y - sizeY / 2;
            var section = new Rectangle(rectPointX, rectPointY, sizeX, sizeY);
            return new Bitmap(CropImage(source, section), source.Size);*/
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

        private static Bitmap TransformImage(Bitmap image)
        {
            if (_finders == null || _finders.Length < 7)
                return null;
            //foreach (var finder in finders)
            //{
            //    Debug.Log(finder.ToString());
            //}
            var markPositions = GetMarks(_finders);
            var rotateAngle = -GetRotationAngle(markPositions);
            var currentCenter = GetCurrentCenter(markPositions);
            image = RotateImage(image, rotateAngle, currentCenter);
            image = ResizeImage(image, markPositions);
            Graphics.FromImage(image).FillRectangle(Brushes.Brown, new Rectangle(0, 0, _currentImageWidth / 5, _currentImageWidth / 5));
            return image;
        }

        private static byte[] PrepareToEncode(Bitmap image)
        {
            var newBitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            int x, y;
            for (x = 0; x < newBitmap.Width; x++)
            {
                for (y = 0; y < newBitmap.Height; y++)
                {
                    System.Drawing.Color pixelColor = image.GetPixel(x, y);
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(pixelColor.G, pixelColor.R, 0, pixelColor.B);
                    int offset = 15;
                    if (x >= offset)
                        newBitmap.SetPixel(x - offset, y, newColor);
                    else
                    {
                        newBitmap.SetPixel(_currentImageWidth - 1 - (offset - x), y, newColor);
                    }
                }
            }
            //newBitmap.Save("c:/InteractiveSoftware/Converted/" + _vehicleName + ".jpg", ImageFormat.Jpeg);
            return ImageToByteArray(newBitmap);
        }

        private static Texture2D GetTextureFromByte(byte[] byteImage)
        {
            var texture = new Texture2D(MAX_IMAGE_SIZE_X, MAX_IMAGE_SIZE_Y, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(byteImage);
            texture.Apply();
            texture.name = _vehicleName;
            return texture;
        }

        private static RotateFlipType GetRotateType(Point topLeftMarkPosition)
        {
            if (topLeftMarkPosition.Y < _currentImageHeight / 2)
                if (topLeftMarkPosition.X < _currentImageWidth / 2)
                    return RotateFlipType.RotateNoneFlipNone;
                else
                    return RotateFlipType.Rotate270FlipNone;
            else
                if (topLeftMarkPosition.X < _currentImageWidth / 2)
                return RotateFlipType.Rotate90FlipNone;
            else
                return RotateFlipType.Rotate180FlipNone;
        }

        private static Bitmap ChangeAlingment(ref Bitmap bitmap, RotateFlipType rotateType)
        {
            if (rotateType != RotateFlipType.RotateNoneFlipNone)
            {
                bitmap.RotateFlip(rotateType);
                if (rotateType != RotateFlipType.Rotate180FlipNone)
                {
                    _currentImageWidth = bitmap.Width;
                    _currentImageHeight = bitmap.Height;
                }
                TransformFinders(rotateType);
            }
            return bitmap;
        }

        private static Point[] GetMarks(QRCodeFinder[] finders)
        {
            var rezPoints = new Point[4];
            var point = new Point();
            foreach (var finder in finders)
            {
                point = finder.Position;
                if (point.Y < _currentImageHeight / 2)
                {
                    if (point.X < _currentImageWidth / 2)
                    {
                        if (rezPoints[0] == Point.Empty)
                            rezPoints[0] = point;
                        else if (rezPoints[0].X > point.X || rezPoints[0].Y > point.Y)
                            rezPoints[0] = point;
                    }
                    else if (point.X > _currentImageWidth / 2)
                    {
                        if (rezPoints[1] == Point.Empty)
                            rezPoints[1] = point;
                        else if (rezPoints[1].X < point.X || rezPoints[1].Y > point.Y)
                            rezPoints[1] = point;
                    }
                }
                else if (point.Y > _currentImageHeight / 2)
                {
                    if (point.X < _currentImageWidth / 2)
                    {
                        if (rezPoints[3] == Point.Empty)
                            rezPoints[3] = point;
                        else if (rezPoints[3].X > point.X || rezPoints[3].Y < point.Y)
                            rezPoints[3] = point;
                    }
                    else if (point.X > _currentImageWidth / 2)
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

        private static float GetRotationAngle(Point[] markPoints)
        {
            var anglesSum = 0f;
            for (int i = 0; i < markPoints.Length; i++)
            {
                int nextPointIndex = i < markPoints.Length - 1 ? i + 1 : 0;
                anglesSum += GetAnglePointToPoint(markPoints[i], markPoints[nextPointIndex]);
                Debug.Log($"1: {markPoints[i]}, 2: {markPoints[nextPointIndex]}, angle {GetAnglePointToPoint(markPoints[i], markPoints[nextPointIndex])}");
            }
            var averageAngle = anglesSum / markPoints.Length;
            return averageAngle;
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
            float koefX = averageDistances[0] / _distanceBetweenMarksX;
            float koefY = averageDistances[1] / _distanceBetweenMarksY;
            int sizeX = (int)(source.Width * koefX);
            int sizeY = (int)(source.Height * koefY);
            Point center = GetCurrentCenter(markPoints);
            int rectPointX = center.X - sizeX / 2;
            int rectPointY = center.Y - sizeY / 2;
            var section = new Rectangle(rectPointX, rectPointY, sizeX, sizeY);
            return new Bitmap(CropImage(source, section), source.Size);
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private static void TransformFinders(RotateFlipType rotateType)
        {
            var borderPoint = new Point(_currentImageWidth, _currentImageHeight);
            byte flipX = 0, flipY = 0;
            if (rotateType == RotateFlipType.Rotate180FlipNone || rotateType == RotateFlipType.Rotate90FlipNone)
            {
                flipX = 1;
            }
            if (rotateType == RotateFlipType.Rotate180FlipNone || rotateType == RotateFlipType.Rotate270FlipNone)
            {
                flipY = 1;
            }

            foreach (var finder in _finders)
            {
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
                finder.Position = point;
            }
        }

        /// <summary>
        /// Return angle between vector(basePoint, endPoint) and a nerless axes.
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private static float GetAnglePointToPoint(Point basePoint, Point endPoint)
        {
            float dx = basePoint.X - endPoint.X;
            float dy = basePoint.Y - endPoint.Y;
            var rezAngle = 0f;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                rezAngle = (float)Math.Atan(dy / dx);
            }
            else
            {
                rezAngle = (float)Math.Atan(-dx / dy);
            }

            return rezAngle * 180 / (float)Math.PI;
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

        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, new Rectangle(Point.Empty, section.Size), section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

       
    }
}
