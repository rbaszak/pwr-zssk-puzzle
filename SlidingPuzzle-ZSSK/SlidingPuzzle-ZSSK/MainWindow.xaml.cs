using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;
using System.Security.Cryptography;

namespace SlidingPuzzle_ZSSK
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Init global variables
        private static Random rng = new Random();
        public Puzzle puzzle = new Puzzle();
        public int[,] array;
        public int size;
        Image[] imgarray;

        //Main window init
        public MainWindow()
        {
            InitializeComponent();

            SelectButton.Click += Select_Click;
            ShuffleButton.Click += Shuffle_Click;
            SizeTextBox.GotFocus += SizeTextBox_Click;
        }
        #region Clear textbox on click
        void SizeTextBox_Click(object sender, EventArgs e)
        {
            SizeTextBox.Clear();
        }
        #endregion

        #region Select grid size on button click and load image
        void Select_Click(object sender, RoutedEventArgs e)
        {
            //Get size from textbox
            string sizeText = SizeTextBox.Text;

            if(sizeText.Equals("3") || sizeText.Equals("4") || sizeText.Equals("5") || sizeText.Equals("6"))
            {
                //Set grid size
                size = Convert.ToInt32(sizeText);

                //Temporary debug (size always 3)
                size = 3;

                //Set array size and fill
                puzzle.SetArraySize(size);
                puzzle.ArrayInit();
                array = puzzle.GetArray();

                //Disable select button
                SelectButton.IsEnabled = false;

                //Console info
                Console.WriteLine("Size set to: " + puzzle.size);

                //Create image array and load
                imgarray = new Image[(size * size)];
                LoadImg(size);
            }
            else
            {
                //Error if chosen wrong grid size
                string errorSize = "Wrong size.";
                System.Windows.MessageBox.Show(errorSize);
                this.Close();
            }
        }
        #endregion

        #region Shuffle on button click
        void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            Shuffle(array, size);
            image1.Source = ToImageSource(imgarray[array[0,0]], ImageFormat.Jpeg);
            image2.Source = ToImageSource(imgarray[array[0,1]], ImageFormat.Jpeg);
            image3.Source = ToImageSource(imgarray[array[0,2]], ImageFormat.Jpeg);
            image4.Source = ToImageSource(imgarray[array[1,0]], ImageFormat.Jpeg);
            image5.Source = ToImageSource(imgarray[array[1,1]], ImageFormat.Jpeg);
            image6.Source = ToImageSource(imgarray[array[1,2]], ImageFormat.Jpeg);
            image7.Source = ToImageSource(imgarray[array[2,0]], ImageFormat.Jpeg);
            image8.Source = ToImageSource(imgarray[array[2,1]], ImageFormat.Jpeg);
            image0.Source = ToImageSource(imgarray[array[2,2]], ImageFormat.Jpeg);
        }
        #endregion

        #region Image Load
        //Load and split image from file to grid
        void LoadImg(int gridSize)
        {
            var img = Image.FromFile(Environment.CurrentDirectory + "\\media\\test.jpg");
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    var index = i * 3 + j;
                    imgarray[index] = new Bitmap(104, 104);
                    var graphics = Graphics.FromImage(imgarray[index]);
                    graphics.DrawImage(img, new Rectangle(0, 0, 104, 104), new Rectangle(i * 104, j * 104, 104, 104), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }

            //Add empty tile
            var imgZero = Image.FromFile(Environment.CurrentDirectory + "\\media\\zero.jpg");
            imgarray[8] = imgZero;

            //Load images to grid in order
            image1.Source = ToImageSource(imgarray[0], ImageFormat.Jpeg);
            image2.Source = ToImageSource(imgarray[3], ImageFormat.Jpeg);
            image3.Source = ToImageSource(imgarray[6], ImageFormat.Jpeg);
            image4.Source = ToImageSource(imgarray[1], ImageFormat.Jpeg);
            image5.Source = ToImageSource(imgarray[4], ImageFormat.Jpeg);
            image6.Source = ToImageSource(imgarray[7], ImageFormat.Jpeg);
            image7.Source = ToImageSource(imgarray[2], ImageFormat.Jpeg);
            image8.Source = ToImageSource(imgarray[5], ImageFormat.Jpeg);
            image0.Source = ToImageSource(imgarray[8], ImageFormat.Jpeg);
        }
        #endregion

        #region Convert Image
        public static ImageSource ToImageSource(System.Drawing.Image image, ImageFormat imageFormat)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save to the stream
                image.Save(stream, imageFormat);

                // Rewind the stream
                stream.Seek(0, SeekOrigin.Begin);

                // Tell the WPF BitmapImage to use this stream
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }

            return bitmap;
        }
        #endregion

        #region Shuffle temporary list
        void Shuffle(IList<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion

        #region Shuffle tile array
        void Shuffle(int[,] arr, int gridSize)
        {
            //Variables
            List<int> tempList = new List<int>();
            int index = 0;

            //Convert 2-dimensional array to 1-dimensional list
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    tempList.Add(arr[i, j]);
                }
            }

            //Shuffle temporary list
            Shuffle(tempList);

            //Convert back to array
            foreach(int item in tempList)
            {
                Console.WriteLine(item);
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    arr[i, j] = tempList[index];
                    index++;
                }
            }
        }
        #endregion
    }
}
