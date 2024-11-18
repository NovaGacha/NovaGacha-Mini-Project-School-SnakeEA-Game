using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeEA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Derection, int> dirToRotation = new()
        {
            { Derection.Up, 0 },
            { Derection.right, 90 },
            { Derection.Down, 180 },
            { Derection.left, 270 },
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning; 
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }



        private async Task RunGame()
        {
            Draw();
            await ShowCoutDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);        
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridval = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridval];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private async Task DrawDeadSnake() 
        {
            List<Positions> positions = new List<Positions>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++) 
            {
                Positions pos = positions[i];
                ImageSource source = (i == 0) ? Images.Deadhead : Images.Deadbody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private void DrawSnakeHead()
        {
            Positions headPos = gameState.HeadPositions();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int ratotion = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(ratotion);
        }

        private async Task ShowCoutDown()
        {
            for (int i = 3; i >= 1; i--)
            { 
                OverlayText.Text = i.ToString();
                await Task.Delay(500);

            }
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if(gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDerection(Derection.left);
                    break;
                case Key.Right:
                    gameState.ChangeDerection(Derection.right);
                    break;
                case Key.Up:
                    gameState.ChangeDerection(Derection.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDerection(Derection.Down);
                    break;
            }
        }

        private async Task ShowGameOver()
        { 
            await DrawDeadSnake();
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}