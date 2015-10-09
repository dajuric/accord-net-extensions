Provides platform independent drawing extensions which operate on native .NET arrays.

1) Drawing 2D objects:

	//create a managed image
	var image = new Bgr<byte>[480, 640];

	//draw rectangle
	image.Draw(new Rectangle(50, 50, 200, 100),                        Bgr<byte>.Red,  -1);
	//draw circle
	image.Draw(new Circle(50, 50, 25),                                 Bgr<byte>.Blue,  5);
	//draw box 2D
	image.Draw(new Box2D(new Point(250, 150), new Size(100, 100), 30), Bgr<byte>.Green, 1);
	//draw text
	image.Draw("Hello world!", CvFont.Big, new Point(10, 100),         Bgr<byte>.White);

	//save your drawing (Imaging.IO package required)
	image.Save("out.png"); 


Discover more extensions as you type :)