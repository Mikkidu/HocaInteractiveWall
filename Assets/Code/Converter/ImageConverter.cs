using System.Drawing;
using System.Linq;
using System.IO;
using System;
using QRCodeDecoderLibrary;
using UnityEngine;
#if UNITY_WEBGL
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
#else
using UniRx;
using System.Drawing.Imaging;
using System.Threading;
using Graphics = System.Drawing.Graphics;
#endif
//using Debug = UnityEngine.Debug;


namespace HocaInk.InteractiveWall
{


    class ImageConverter
    {


#region OutputFields

        private Texture2D _texture;
        private Vector2 _center;
        private int _aligmentRotating;
        private float _angleRotated;
        private float _tilling;

#endregion


#region CalculationFields

        private const int MAX_IMAGE_SIZE_X = 2048;
        private const int MAX_IMAGE_SIZE_Y = 1448;

        private MaterialGenerator _materialGenerator;

        private QRDecoder _qrDecoder;
        private QRCodeFinder[] _finders;
        private int _currentImageWidth;
        private int _currentImageHeight;
        private int _distanceBetweenMarksX;
        private int _distanceBetweenMarksY;
        private string _vehicleName;
#if UNITY_WEBGL
        private float _frameStartTime;
        private byte[,] _byteImage;
        private int _textureSize;
#endif

#endregion


#region Constructor

        public ImageConverter(MaterialGenerator materialGenerator)
        {
            _materialGenerator = materialGenerator;
            ConverterInitialization();
        }

        public void ConverterInitialization()
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

#endregion


#region PC

#if !UNITY_WEBGL


        public void GenerateTexturePC(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                var normalizeImage = Observable.Start(() =>
                {
                    try
                    {
                        Bitmap image = LoadImage(imagePath);
                        File.Delete(imagePath);
                        RawTransform(image);
                        image = TransformImage(image);
                        return PrepareToEncode(image);
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
                            Texture2D texture = GetTextureFromByte(normImage[0]);
                            _materialGenerator.CreateMaterial(texture);
                        }
                        else
                        {
                            _materialGenerator.ConversionError();
                        }
                    });
            }
            else
            {
                Debug.Log("No file");
                _materialGenerator.ConversionError();
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Bitmap LoadImage(string path)
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

        /// <summary>
        /// Image recognition, get corner marks.
        /// </summary>
        /// <param name="bitmap">Image fore recognition</param>
        /// <returns></returns>
        private void RawTransform(Bitmap bitmap)
        {
            if (bitmap == null) return;

            QRCodeResult[] qrResult = _qrDecoder.ImageDecoder(bitmap);
            _vehicleName = QRDecoder.ByteArrayToStr(qrResult[0].DataArray);
            _finders = qrResult[0].FindersArray;
            ChangeAlingment(ref bitmap, GetRotateType(qrResult[0].CornerPosition));
            //return bitmap;
        }

        private Bitmap TransformImage(Bitmap image)
        {
            if (_finders == null || _finders.Length < 7)
                return null;
            var markPositions = GetMarks(_finders);
            var rotateAngle = -GetRotationAngle(markPositions);
            var currentCenter = GetCurrentCenter(markPositions);
            image = RotateImage(image, rotateAngle, currentCenter);
            image = ResizeImage(image, markPositions);
            return image;
        }

        private byte[] PrepareToEncode(Bitmap image)
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
            return ImageToByteArray(newBitmap);
        }

        private Texture2D GetTextureFromByte(byte[] byteImage)
        {
            var texture = new Texture2D(_currentImageWidth, _currentImageWidth, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(byteImage);
            texture.Apply();
            texture.name = _vehicleName;
            return texture;
        }

        private RotateFlipType GetRotateType(Point topLeftMarkPosition)
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

        private Bitmap ChangeAlingment(ref Bitmap bitmap, RotateFlipType rotateType)
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

        private Bitmap RotateImage(Bitmap bmp, float angle, Point center)
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

        private Bitmap ResizeImage(Bitmap source, Point[] markPoints)
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
            return CropImage(source, section);
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private void TransformFinders(RotateFlipType rotateType)
        {
            var borderPoint = new Point(_currentImageWidth, _currentImageHeight);
            int flipX = 0, flipY = 0;
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

        private Point GetCurrentCenter(Point[] markPositions)
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

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(source.Width, source.Width);
            int fieldHeight = (source.Width - source.Height) / 2;
            var fieldSize = new Size(_currentImageWidth, fieldHeight);
            var brush = new SolidBrush(System.Drawing.Color.Brown);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(brush, new Rectangle(Point.Empty, fieldSize));
                g.DrawImage(source, new Rectangle(new Point(0, fieldHeight), source.Size), section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }


#endif

        #endregion


        #region WebGL

#if UNITY_WEBGL

        public async UniTask GenerateTextureWebGL(string url)
        {
            _frameStartTime = Time.realtimeSinceStartup;
            await LoadTexture(url);
            _byteImage = await GetByteArrayFromTexture(_texture);
            _aligmentRotating = GetAligmentRotation(_byteImage);
            await CheckFrameTime();
            GetTransformData();
            await CheckFrameTime();
            _texture = await CropTexture();
            _texture.name = _vehicleName;
            _materialGenerator.CreateMaterial(_texture, _center, -_aligmentRotating, _angleRotated, _tilling);
            _texture = null;
        }

        private async UniTask CheckFrameTime()
        {
            if (Time.realtimeSinceStartup - _frameStartTime > 0.01f)
            {
                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
                _frameStartTime = Time.realtimeSinceStartup;
            }
        }

        private async UniTask LoadTexture(string url)
        {
            using (var request = new UnityWebRequest(url))
            {
                request.downloadHandler = new DownloadHandlerTexture();
                await request.SendWebRequest();

                _texture = ((DownloadHandlerTexture)(request.downloadHandler)).texture;
            }
            _currentImageHeight = _texture.height;
            _currentImageWidth = _texture.width;
            await CheckFrameTime();
        }

        private async UniTask<byte[,]> GetByteArrayFromTexture(Texture2D texture)
        {
            var byteImage = new byte[texture.width * 3, texture.height];
            var byteX = 0;
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    byteImage[byteX, y] = (byte)(texture.GetPixel(x, _currentImageHeight - 1 - y).r * 255);
                    byteImage[byteX + 1, y] = (byte)(texture.GetPixel(x, _currentImageHeight - 1 - y).g * 255);
                    byteImage[byteX + 2, y] = (byte)(texture.GetPixel(x, _currentImageHeight - 1 - y).b * 255);
                    byteX += 3;
                }
                byteX = 0;
                await CheckFrameTime();
            }
            return byteImage;
        }

        private int GetAligmentRotation(byte[,] byteImage)
        {
            QRCodeResult[] qrResult = _qrDecoder.ImageDecoder(byteImage);
            _finders = qrResult[0].FindersArray;
            _vehicleName = QRDecoder.ByteArrayToStr(qrResult[0].DataArray);
            Point topLeftMarkPosition = qrResult[0].CornerPosition;
            if (topLeftMarkPosition.Y > _currentImageHeight / 2 != topLeftMarkPosition.X > _currentImageWidth / 2)
            {
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

        private void GetTransformData()
        {
            if (_finders == null || _finders.Length < 7)
                return;
            var markPositions = GetMarks(_finders);
            ConvertMarksToSquare(markPositions, _aligmentRotating);
            _angleRotated = GetRotationAngle(markPositions);
            _center = GetCurrentCenterVector(markPositions);
            _tilling = GetTillinFactor(markPositions);
        }

        private async UniTask<Texture2D> CropTexture()
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
            Texture2D squareTexture = new Texture2D(_textureSize, _textureSize);
            UnityEngine.Color color = new UnityEngine.Color(128 / 225f, 64 / 225f, 48 / 225f);
            await CheckFrameTime();
            UnityEngine.Color[] colorArrayField = Enumerable.Repeat(color, fieldWidth * fieldsHeight).ToArray();
            await CheckFrameTime();
            UnityEngine.Color[] pixelsFromTexture = _texture.GetPixels();
            await CheckFrameTime();
            squareTexture.SetPixels(0, 0, fieldWidth, fieldsHeight, colorArrayField);
            await CheckFrameTime();
            squareTexture.SetPixels(x1, y1, _texture.width, _texture.height, pixelsFromTexture);
            await CheckFrameTime();
            squareTexture.SetPixels(x2, y2, fieldWidth, fieldsHeight, colorArrayField);
            await CheckFrameTime();
            squareTexture.Apply();
            return squareTexture;
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
            centerX /= 4 * (_textureSize - 1);
            centerY /= 4 * (_textureSize - 1);

            return new Vector2(centerX, centerY);
        }

        private float GetTillinFactor(Point[] markPoints)
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
            return (koefX + koefY) / 2;
        }

#endif

        #endregion

        #region UniversalCode

        public void GenerateTexture(string url)
        {
#if UNITY_WEBGL
            GenerateTextureWebGL(url).Forget();
#else
            GenerateTexturePC(url);
#endif
        }

        private Point[] GetMarks(QRCodeFinder[] finders)
        {
            var rezPoints = new Point[4];
            foreach (var finder in finders)
            {
                Point point = finder.Position;
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

        private float GetRotationAngle(Point[] markPoints)
        {
            var anglesSum = 0f;
            for (int i = 0; i < markPoints.Length; i++)
            {
                int nextPointIndex = i < markPoints.Length - 1 ? i + 1 : 0;
                anglesSum += GetAnglePointToPoint(markPoints[i], markPoints[nextPointIndex]);
            }
            var averageAngle = anglesSum / markPoints.Length;
            return averageAngle;
        }

        /// <summary>
        /// Return angle between vector(basePoint, endPoint) and a nerless axes.
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private float GetAnglePointToPoint(Point basePoint, Point endPoint)
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

#endregion

    }
}
