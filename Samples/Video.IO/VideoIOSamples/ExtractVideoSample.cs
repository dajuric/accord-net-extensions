using System;
using System.IO;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;

namespace VideoCapture
{
    public class ExtractVideoSample
	{
        public static void Test()
        {
            var reader = new FileCapture(Path.Combine(getResourceDir(), "Welcome Accord.NET Extensions.mp4"));
            reader.Open();

            Console.WriteLine("Extracting video frames...");

            var videoExtractor = new VideoExtractor(reader);
            videoExtractor.Start((percentage) => 
            {
                Console.Write("\r Completed: {0} %", (int)(percentage * 100));
            });

            Console.WriteLine();
        }

        private static string getResourceDir()
        {
            return Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName, "Resources");
        }
	}

    public class VideoExtractor
    {
        static string OutputDir = "OutputSamples";
        StreamableSource imageSource;

        public VideoExtractor(StreamableSource imageSource)
        {
            this.imageSource = imageSource;

            var outputPath = Path.Combine(Environment.CurrentDirectory, OutputDir);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
        }

        public void Start(Action<float> onFrameCompletition)
        {
            if (imageSource.CanSeek)
                imageSource.Seek(0, SeekOrigin.Begin);

            var idx = 0;
            foreach (var frame in imageSource) //use video stream as IEnumerable<IImage> (must support seek operation)
            {
                var path = Path.Combine(Environment.CurrentDirectory, OutputDir, String.Format("img-{0:000}.png", idx));

                frame.Convert<Bgr, byte>().Save(path);

                onFrameCompletition((float)(idx + 1) / imageSource.Length);
                idx++;
            }
        }
    }
}
