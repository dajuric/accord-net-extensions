Provides unified API for IO video access: web-camera support, various video-format reading / writing, image-directory reader and functions for image reading and writing.
All operations on image streams are standard stream operations and do not depend on actual video source.
The library is made in platform-abstract fashion.

1) image loading / saving:

   Bgr<byte>[,] image = ImageIO.LoadColor("sample.jpg"); //load Bgr color image
   Gray<float>[,] hdrImage = (ImageIO.LoadUnchanged("hdrImage.png") as Image<Gray<float>>).Clone(); //load HDR grayscale (or any other) image

   image.Save("image.png");


2) media (camera, video, image-directory) capture:

   ImageStreamReader reader = new CameraCapture(); //use specific class for device-specific properties (e.g. exposure, image name, ...)
   reader.Open();

   //read single frame
   var frame = reader.ReadAs<Bgr<byte>>();

   //read the rest of images (do not do that with the CameraCapture :) )
   foreach(var image in reader)
   {
	  //do something with the image
   }

   reader.Close();
   
3) video writer:

   ImageStreamWriter videoWriter = new VideoWriter("out.avi", new Size(1280, 1024));

   var image = new Bgr<byte>[1024, 1280];
   videoWriter.Write(image.Lock()); //write a single frame

   videoWriter.Close();


Discover more types as you type :)


