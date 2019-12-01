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
using System.Diagnostics;

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
        public Solver solver = new Solver();
        public int[,] array;
        public int size;
        public string path = Environment.CurrentDirectory + "\\results.txt";
        public Image[] imgarray;
        public List<System.Windows.Controls.Image> images;

        //Main window init
        public MainWindow()
        {
            InitializeComponent();

            SelectButton.Click += Select_Click;
            ShuffleButton.Click += Shuffle_Click;
            SizeTextBox.GotFocus += SizeTextBox_Click;
            SolveButton.Click += Solve_Click;
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
                //size = 3;

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

                SizeTextBox.IsEnabled = false;

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

            int index = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    images[index].Source = ToImageSource(imgarray[array[i, j]], ImageFormat.Jpeg);
                    index++;
                }
            }
        }
        #endregion

        #region Solve on button click
        void Solve_Click(object sender, RoutedEventArgs e)
        {
            //Wyczyść plik z wynikami
            File.WriteAllText(path, String.Empty);

            //Przykład dla brute force BFS
            Stopwatch stopwatch = new Stopwatch();

            //Dodaj tablicę do rozwiązania
            solver.SetArrayToSolve(array, size);
            solver.SetResultArray();

            stopwatch.Start();

            //Rozwiąż
            solver.BruteForceBFS();

            stopwatch.Stop();

            //Odczytaj wyniki
            var time = stopwatch.Elapsed.TotalMilliseconds;
            var result = solver.GetResultArray();
            var resultPath = solver.GetResultPath();
            //var numOfMoves = resultPath.Count;

            //Wyświetl ułożony obraz na ekranie
            Print();
            
            //Zapisz i wyświetl wyniki
            File.AppendAllText(path, time.ToString());
            //Console.WriteLine("Time elapsed (milliseconds): " + time);
            //Console.WriteLine("Number of moves: " + numOfMoves);
        }
        #endregion

        #region Image Load
            //Load and split image from file to grid
            void LoadImg(int gridSize)
            {
            var img = Image.FromFile(Environment.CurrentDirectory + "\\media\\testNum.jpg");
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    var index = i * gridSize + j;
                    imgarray[index] = new Bitmap(312/gridSize, 312/gridSize);
                    var graphics = Graphics.FromImage(imgarray[index]);
                    graphics.DrawImage(img, new Rectangle(0, 0, 312/gridSize, 312/gridSize), new Rectangle(j * 312/gridSize, i * 312/gridSize, 312/gridSize, 312/gridSize), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }

            //Add empty tile
            var imgZero = Image.FromFile(Environment.CurrentDirectory + "\\media\\zero.jpg");
            imgarray[(gridSize*gridSize)-1] = imgZero;

            //Load images to grid in order/Create rows and columns
            images = new List<System.Windows.Controls.Image>();

            int ind = 0;
            for (int i = 0; i < gridSize; i++)
            {
                PuzzleGrid.RowDefinitions.Add(new RowDefinition());
                PuzzleGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < gridSize; j++)
                {
                    images.Add(new System.Windows.Controls.Image());
                    PuzzleGrid.Children.Add(images[ind]);
                    Grid.SetColumn(images[ind], j);
                    Grid.SetColumnSpan(images[ind], 1);
                    Grid.SetRow(images[ind], i);
                    Grid.SetRowSpan(images[ind], 1);
                    ind++;
                }
            }

            for (int i = 0; i < gridSize * gridSize; i++)
            {
                images[i].Source = ToImageSource(imgarray[i], ImageFormat.Jpeg);
            }

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

        #region Temporary key movement
        public void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if(e.Key == Key.Up)
            {
                solver.Move(8, Solver.dir.UP);
            }
            if (e.Key == Key.Down)
            {
                solver.Move(8, Solver.dir.DOWN);

            }
            if (e.Key == Key.Left)
            {
                solver.Move(8, Solver.dir.LEFT);

            }
            if (e.Key == Key.Right)
            {
                solver.Move(8, Solver.dir.RIGHT);
            }
            Print();
        }
        #endregion

        #region Result print function
        public void Print()
        {
            var result = solver.GetResultArray();
            int index = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    images[index].Source = ToImageSource(imgarray[result[i, j]], ImageFormat.Jpeg);
                    index++;
                }
            }
        }
        #endregion
    }
}
