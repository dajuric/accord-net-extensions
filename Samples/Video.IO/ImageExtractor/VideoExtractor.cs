using System;
using System.IO;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;

namespace ImageExtractor
{
    public class VideoExtractor
    {
        ImageStreamReader imageSource;
        string outputDir;
        int imageQuality;
        string fileNameFormat;

        public VideoExtractor(ImageStreamReader imageSource, string outputDir, int imageQuality = 90, string fileNameFormat = "img-{0:000}.jpg")
        {
            this.imageSource = imageSource;
            this.outputDir = outputDir;
            this.imageQuality  = imageQuality;
            this.fileNameFormat = fileNameFormat;

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
        }

        public void Start(Action<float> onFrameCompletition)
        {
            if (imageSource.CanSeek)
                imageSource.Seek(0, SeekOrigin.Begin);

            var idx = 0;
            foreach (var frame in imageSource) //use video stream as IEnumerable<IImage> (must support seek operation)
            {
                if (frame != null) //some videos skip key frames (discard those frames)
                {
                    var path = Path.Combine(outputDir, String.Format(fileNameFormat, idx));
                    frame.Convert<Bgr, byte>().ToBitmap().Save(path, imageQuality);
                }

                onFrameCompletition((float)(idx + 1) / imageSource.Length);
                idx++;
            }
        }
    }
}
