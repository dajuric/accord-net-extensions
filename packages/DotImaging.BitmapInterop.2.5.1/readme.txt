Provides extensions for interoperability with System.Drawing.Bitmap, Point, PointF, Color and drawing extensions.

1) Array <-> Bitmap conversion:

	var image = new Gray<byte>[240, 320];
	var bmp = image.ToBitmap();

	var imageFromBmp = bmp.ToArray() as Bgr<byte>[,]; 


2) System.Drawing.Point <-> DotImaging.DrawingPrimitives2D.Point conversion: //also for Rectangle and Size 

	var drawingPt = new System.Drawing.Point(20, 30);
	var point = drawingPt.ToPt();


3) System.Drawing.Color <-> DotImaging color conversion:

	var aquaColor = System.Drawing.Color.Aqua.ToBgr();


4) Save bitmap with quality selection
	
	var bmp = Bitmap.FromFile("sample.bmp");
	bmp.Save("compressed.jpg", 85);


Discover more extensions as you type :)