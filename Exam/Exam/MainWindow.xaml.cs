using Exam.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Exam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Cell> cells = new List<Cell>();
        const string pathWhite = "path white.txt";
        const string pathBlack = "path black.txt";
        private int currentPlayer = 1;
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(pathWhite)) File.Create(pathWhite);
            if (!File.Exists(pathBlack)) File.Create(pathBlack);
            Init();
        }
        private void Init()
        {
            string imgWhitePath = File.ReadAllText(pathWhite);
            if (!File.Exists(imgWhitePath))
            {
                LoadImageWhite();
                imgWhitePath = File.ReadAllText(pathWhite);
            }
            FileInfo fi = new FileInfo(imgWhitePath);
            string fullImgWhitePath = fi.FullName;

            string imgBlackPath = File.ReadAllText(pathBlack);
            if (!File.Exists(imgBlackPath))
            {
                LoadImageBlack();
                imgBlackPath = File.ReadAllText(pathBlack);
            }
            FileInfo f = new FileInfo(imgBlackPath);
            string fullImgBlackPath = f.FullName;

            for (int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Cell cell = new Cell
                    {
                        Row = i,
                        Column = j,
                    };
                    if ((j % 2 == 0 && i % 2 != 0) || (i % 2 == 0 && j % 2 != 0)) cell.Background = Brushes.Brown;
                    else cell.Background = Brushes.White;
                    cell.MouseDown += Item_MouseDown;
                    cells.Add(cell);
                    grid.Children.Add(cell);
                    Grid.SetRow(cell, cell.Row);
                    Grid.SetColumn(cell, cell.Column);
                }
            }

            foreach (var item in cells)
            {
                if ((item.Row % 2 == 0 && item.Row < 3 && item.Column % 2 != 0) || (item.Row % 2 != 0 && item.Row < 3 && item.Column % 2 == 0))
                {
                    item.Content = new Image
                    {
                        Source = new BitmapImage(new Uri(fullImgWhitePath))
                    };
                    item.IsChecker = true;
                    item.Player = 1;
                }
                else if ((item.Row > 4 && item.Row % 2 == 0 && item.Column % 2 != 0) || (item.Row > 4 && item.Row % 2 != 0 && item.Column % 2 == 0))
                {
                    item.Content = new Image
                    {
                        Source = new BitmapImage(new Uri(fullImgBlackPath))
                    };
                    item.IsChecker = true;
                    item.Player = 2;
                }
            }

        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Cell c = sender as Cell;
            if (c.IsChecker)
            {
                if (!IsCellChecked() && c.Player == currentPlayer)
                {
                    c.Background = Brushes.Yellow;
                    PaintActns(ShowActions(c), c);
                }
                else if (c.Background == Brushes.Yellow) EndOfMove();
            }
            else if(c.Background==Brushes.Green)
            {
                foreach (var item in cells)
                {
                    if (item.Background == Brushes.Yellow)
                    {
                        c.Content = item.Content;
                        c.IsChecker = true;
                        c.Player = item.Player;
                        c.IsQueen = item.IsQueen;
                        item.Content = null;
                        item.IsChecker = false;
                        item.IsQueen = false;
                        item.Player = 0;
                        if (Math.Abs(item.Row - c.Row) == 2 && Math.Abs(item.Column - c.Column) == 2 && !c.IsQueen)
                        {
                            int row = (c.Row + item.Row) / 2;
                            int col = (c.Column + item.Column) / 2;
                            for(int i = 0; i < cells.Count; i++)
                            {
                                if(cells[i].Row==row && cells[i].Column == col)
                                {
                                    cells[i].Content = null;
                                    cells[i].IsChecker = false;
                                    cells[i].Player = 0;
                                    cells[i].IsQueen = false;
                                }
                            }
                        }
                        else if(c.IsQueen)
                        {
                            for(int i = 0; i < cells.Count; i++)
                            {
                                if (cells[i].IsChecker)
                                {
                                    if ((cells[i].Row < item.Row && cells[i].Row > c.Row && cells[i].Column < item.Column && cells[i].Column > c.Column) || (cells[i].Row < item.Row && cells[i].Row > c.Row && cells[i].Column > item.Column && cells[i].Column < c.Column) || (cells[i].Row > item.Row && cells[i].Row < c.Row && cells[i].Column > item.Column && cells[i].Column < c.Column) || (cells[i].Row > item.Row && cells[i].Row < c.Row && cells[i].Column < item.Column && cells[i].Column > c.Column))
                                    {
                                        cells[i].Content = null;
                                        cells[i].IsChecker = false;
                                        cells[i].Player = 0;
                                        cells[i].IsQueen = false;
                                    }
                                }
                                
                            }
                            
                        }
                    }
                }
                EndOfMove();
                EndOfGame();
                CheckQueen();
                if (currentPlayer == 1) currentPlayer++;
                else currentPlayer--;

            }
        }

        private void CheckQueen()
        {
            int counter = 0;
            foreach (var item in cells)
            {
                if((currentPlayer==1 && item.Player==1 && item.Row == 7) || (currentPlayer == 2 && item.Player == 2 && item.Row == 0))
                {
                    item.IsQueen = true;
                    counter++;
                }
            }
        }

        private void EndOfGame()
        {
            int counterWhite = 0;
            int counterBlack = 0;
            foreach (var item in cells)
            {
                if (item.Player == 1) counterWhite++;
                else if (item.Player == 2) counterBlack++;
            }
            if(counterWhite == 0)
            {
                MessageBox.Show("Black won", "Game over!!!");
                Close();
            }
            else if(counterBlack == 0)
            {
                MessageBox.Show("White won", "Game over!!!");
                Close();
            }
        }

        private bool IsCellChecked()
        {
            foreach (var item in cells)
            {
                if (item.Background == Brushes.Yellow || item.Background == Brushes.Green) return true;
            }
            return false;
        }

        private void EndOfMove()
        {
            foreach (var item in cells)
            {
                if (item.Background != Brushes.Brown && item.Background!=Brushes.White) item.Background = Brushes.Brown;
            }
        }

        private List<Cell> ShowActions(Cell head)
        {
            List<Cell> actnCells = new List<Cell>();
            for (int i = 0; i < cells.Count; i++)
            {
                Cell tmp = cells[i];
                if (!head.IsQueen)
                {
                    if ((tmp.Row == head.Row + 1 && tmp.Column == head.Column - 1) || (tmp.Row == head.Row + 1 && tmp.Column == head.Column + 1) || (tmp.Row == head.Row - 1 && tmp.Column == head.Column - 1) || (tmp.Row == head.Row - 1 && tmp.Column == head.Column + 1))
                    {
                        actnCells.Add(tmp);
                    }
                }
                else
                {
                    if (Math.Abs(tmp.Row - head.Row) == Math.Abs(tmp.Column - head.Column))
                    {
                        actnCells.Add(tmp);
                    }
                }
            }
            return actnCells;
        }

        private void PaintActns(List<Cell> actnCells, Cell head)
        {
            if (!head.IsQueen)
            {
                for (int j = 0; j < actnCells.Count; j++)
                {
                    if (!actnCells[j].IsChecker)
                    {
                        if (actnCells[j].Row > head.Row && head.Player == 1)
                        {
                            actnCells[j].Background = Brushes.Green;
                        }
                        else if (actnCells[j].Row < head.Row && head.Player == 2)
                        {
                            actnCells[j].Background = Brushes.Green;
                        }
                    }
                    else if (head.Player != actnCells[j].Player)
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            Cell tmp = cells[i];
                            if (!head.IsQueen)
                            {
                                if ((tmp.Row == actnCells[j].Row + 1 && tmp.Column == actnCells[j].Column - 1 && tmp.Row == head.Row + 2 && tmp.Column == head.Column - 2) || (tmp.Row == actnCells[j].Row + 1 && tmp.Column == actnCells[j].Column + 1 && tmp.Row == head.Row + 2 && tmp.Column == head.Column + 2) || (tmp.Row == actnCells[j].Row - 1 && tmp.Column == actnCells[j].Column - 1 && tmp.Row == head.Row - 2 && tmp.Column == head.Column - 2) || (tmp.Row == actnCells[j].Row - 1 && tmp.Column == actnCells[j].Column + 1 && tmp.Row == head.Row - 2 && tmp.Column == head.Column + 2))
                                {
                                    if (!tmp.IsChecker)
                                    {
                                        tmp.Background = Brushes.Green;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                actnCells.Remove(head);
                List<Cell> actnCellsToRemove = new List<Cell>();
                foreach (var item in actnCells)
                {
                    if (item.IsChecker)
                    {
                        if (item.Player == currentPlayer && item.Player != 0)
                        {
                            if (!actnCellsToRemove.Exists((a) =>
                            {
                                if (a == item) return true;
                                else return false;
                            }))
                            {
                                actnCellsToRemove.Add(item);
                            }
                            if (head.Row < item.Row)
                            {
                                if (head.Column > item.Column)
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column < item.Column && actnCells[i].Row > item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column > item.Column && actnCells[i].Row > item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (head.Column > item.Column)
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column < item.Column && actnCells[i].Row < item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column > item.Column && actnCells[i].Row < item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!actnCellsToRemove.Exists((a) =>
                            {
                                if (a == item) return true;
                                else return false;
                            }))
                            {
                                actnCellsToRemove.Add(item);
                            }
                        }

                    }
                    foreach (var item2 in actnCells)
                    {
                        if(item != item2 && Math.Abs(item.Row-item2.Row) == 1 && item.Player != 0 && item.Player == item2.Player)
                        {
                            if(!actnCellsToRemove.Exists((a) => {
                                if (a == item) return true;
                                else return false;
                            }))
                            {
                                actnCellsToRemove.Add(item);
                            }
                            else if (!actnCellsToRemove.Exists((a) => {
                                if (a == item2) return true;
                                else return false;
                            }))
                            {
                                actnCellsToRemove.Add(item2);
                            }
                            if (head.Row < item.Row)
                            {
                                if (head.Column > item.Column)
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column < item.Column && actnCells[i].Row > item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column > item.Column && actnCells[i].Row > item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (head.Column > item.Column)
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column < item.Column && actnCells[i].Row < item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < actnCells.Count; i++)
                                    {
                                        if (actnCells[i].Column > item.Column && actnCells[i].Row < item.Row)
                                        {
                                            if (!actnCellsToRemove.Exists((a) =>
                                            {
                                                if (a == actnCells[i]) return true;
                                                else return false;
                                            }))
                                            {
                                                actnCellsToRemove.Add(actnCells[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < actnCellsToRemove.Count; i++)
                {
                    actnCells.Remove(actnCellsToRemove[i]);
                }
                foreach (var item in actnCells)
                {
                    item.Background = Brushes.Green;
                }
            }
        }

        private void LoadImageWhite()
        {
            WebClient wc = new WebClient();
            string fileName = System.IO.Path.GetFileName("https://pngimg.com/uploads/checkers/checkers_PNG17.png");
            string extension = System.IO.Path.GetExtension(fileName);
            string savePath = System.IO.Path.Combine($"image white{extension}");
            try
            {
                wc.DownloadFile("https://pngimg.com/uploads/checkers/checkers_PNG17.png", savePath);
                File.WriteAllText(pathWhite, savePath);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadImageBlack()
        {
            WebClient wc = new WebClient();
            string fileName = System.IO.Path.GetFileName("https://habrastorage.org/files/2a1/f95/d15/2a1f95d15b9b4a1eb3b4f10db1e0c2c3.png");
            string extension = System.IO.Path.GetExtension(fileName);
            string savePath = System.IO.Path.Combine($"image black{extension}");
            try
            {
                wc.DownloadFile("https://habrastorage.org/files/2a1/f95/d15/2a1f95d15b9b4a1eb3b4f10db1e0c2c3.png", savePath);
                File.WriteAllText(pathBlack, savePath);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
