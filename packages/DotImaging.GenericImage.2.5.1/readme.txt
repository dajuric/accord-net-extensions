Provides .NET native array imaging extensions. Color-spaces and channel depth conversion is included.
Implements slim generic image class when fast pixel manipulation is needed.
To get compatibility for other image types install appropriate extension - NuGet package (e.g. Imaging.BitmapInterop).

1) Color and depth conversion:

	Bgr<byte>[,] image = ImageIO.LoadColor("sample.jpg"); //requires DotImaging.IO package
	Gray<float> grayFloatIm = image.ToGray()
	                               .Cast<float>();


2) Splitting and merging channels:

    Bgra<byte>[,] image = new Bgra<byte>[480, 640];
	Gray<byte>[][,] channels = image.SplitChannels(0, 1, 2); //take B, G and R channel
	Bgr<byte> bgrIm = channels.MergeChannels<Bgr<byte>, byte>();


3) Unsafe operations:

     Bgr<byte>[,] image = new Bgr<byte>[240, 320];

	 using(var unmanagedImage = image.Lock()) //create unmanaged structure that shares data with the array
	 {
		Bgr8* ptr = (Bgr8*)unmanagedImage.GetData(10, 10);
		ptr->B = 111;
	 }


4) OpenCV compatibility:

    Gray<byte>[,] image = new Gray<byte>[240, 320];

	IplImage iplIm = image.AsOpenCvImage(); //to OpenCV image
	Gray<byte>[,] imFromIpl = iplIm.AsImage().Clone(); //to array


5) Misc

   Hsv<byte>[,] image = new Hsv<byte>[240, 320];
   image.SetValue(new Hsv<byte>(10, 10, 255)); //set pixels value

   Console.WriteLine(image.Size()); //write image size
   Console.WriteLine(image.ColorInfo()); //write color info
   ...


Discover more extensions as you type :)

