using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private string direction = "right";
        private bool gameOver = false;
        private int score = 0;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; 

            new Settings();
            gameTimer.Interval = 100;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();
            StartGame();
        }

        private void StartGame()
        {
            gameOver = false;
            direction = "right";
            score = 0;
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);
            GenerateFood();
        }

        private void GenerateFood()
        {
            Random rand = new Random();
            food = new Circle
            {
                X = rand.Next(0, this.ClientSize.Width / Settings.Width),
                Y = rand.Next(0, this.ClientSize.Height / Settings.Height)
            };
        }

        private void UpdateScreen(object? sender, EventArgs e)
        {
            if (gameOver)
            {
                if (Keyboard.IsKeyPressed(Keys.Enter))
                {
                    StartGame();
                }
                return;
            }

            MovePlayer();
            Invalidate(); 
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i > 0; i--)
            {
                Snake[i].X = Snake[i - 1].X;
                Snake[i].Y = Snake[i - 1].Y;
            }

            switch (direction)
            {
                case "left":
                    Snake[0].X--;
                    break;
                case "right":
                    Snake[0].X++;
                    break;
                case "up":
                    Snake[0].Y--;
                    break;
                case "down":
                    Snake[0].Y++;
                    break;
            }

            
            if (Snake[0].X < 0 || Snake[0].Y < 0 ||
                Snake[0].X >= this.ClientSize.Width / Settings.Width ||
                Snake[0].Y >= this.ClientSize.Height / Settings.Height)
            {
                gameOver = true;
            }

            
            for (int i = 1; i < Snake.Count; i++)
            {
                if (Snake[0].X == Snake[i].X && Snake[0].Y == Snake[i].Y)
                {
                    gameOver = true;
                }
            }

            
            if (Snake[0].X == food.X && Snake[0].Y == food.Y)
            {
                Snake.Add(new Circle { X = Snake[Snake.Count - 1].X, Y = Snake[Snake.Count - 1].Y });
                score += 10;
                GenerateFood();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!gameOver)
            {
               
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColor = i == 0 ? Brushes.Black : Brushes.Green;
                    canvas.FillRectangle(snakeColor,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));
                }

                
                canvas.FillRectangle(Brushes.Red,
                    new Rectangle(food.X * Settings.Width,
                                  food.Y * Settings.Height,
                                  Settings.Width, Settings.Height));

                
                canvas.DrawString($"Wynik: {score}", new Font("Arial", 14), Brushes.Black, new PointF(10, 10));
            }
            else
            {
                string gameOverText = $"Game Over!\nWynik: {score}\nNaciœnij ENTER, aby zagraæ ponownie.";
                canvas.DrawString(gameOverText, new Font("Arial", 16), Brushes.Black, new PointF(50, 50));
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!gameOver)
            {
                switch (keyData)
                {
                    case Keys.Left:
                        if (direction != "right")
                            direction = "left";
                        break;
                    case Keys.Right:
                        if (direction != "left")
                            direction = "right";
                        break;
                    case Keys.Up:
                        if (direction != "down")
                            direction = "up";
                        break;
                    case Keys.Down:
                        if (direction != "up")
                            direction = "down";
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public class Circle
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Settings
    {
        public static int Width { get; set; }
        public static int Height { get; set; }

        public Settings()
        {
            Width = 20;
            Height = 20;
        }
    }

    public static class Keyboard
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        public static bool IsKeyPressed(Keys key)
        {
            return GetAsyncKeyState(key) < 0;
        }
    }
}