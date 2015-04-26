using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System.Windows.Forms;
using CvFont = Accord.Extensions.Imaging.Font;
using Point = AForge.IntPoint;

namespace BasicImageOperations
{
    public partial class BasicImageOperationsDemo : Form
    {
        public BasicImageOperationsDemo()
        {
            InitializeComponent();

            //create a managed image
            var image = new Bgr<byte>[480, 640];

            //draw something
            image.Draw(new Rectangle(50, 50, 200, 100), Bgr<byte>.Red, -1);
            image.Draw(new Circle(50, 50, 25), Bgr<byte>.Blue, 5);
            image.Draw(new Box2D(new Point(250, 150), new Size(100, 100), 30), Bgr<byte>.Green, 1);
            image.Draw("Hello world!", CvFont.Big, new Point(10, 100), Bgr<byte>.White);

            //save and load
            image.Save("out.png");  
            pictureBox.Image = ImageIO.LoadColor("out.png").ToBitmap();
        }
    }
}
