using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Triangle
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TEXT_BOX_PLACEHOLDER = "Введите число...";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var stackPanel in MainGrid.Children)
            {
                if(stackPanel is StackPanel)
                {
                    foreach(var textBox in (stackPanel as StackPanel).Children)
                    {
                        if(textBox is TextBox)
                        {
                            TextBox currentTextBox = textBox as TextBox;
                            currentTextBox.Text = TEXT_BOX_PLACEHOLDER;
                            currentTextBox.GotFocus += clearPlaceholderByFocus;
                            currentTextBox.LostFocus += returnPlaceholder;
                            currentTextBox.TextChanged += changeHandler;
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double a = getNumberOrError(sideA.Text);
            if (a == -1)
            {
                sideA.Focus();
                sideA.SelectAll();
                return;
            }
            double b = getNumberOrError(sideB.Text);
            if (b == -1) return;
            double c = getNumberOrError(sideC.Text);
            if (c == -1) return;

            ResultBlock.Text = calcResult(a, b, c);
        }

        private bool isRightTriangle(double a, double b, double c)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)) == Math.Sqrt(Math.Pow(c, 2))
               || Math.Sqrt(Math.Pow(b, 2) + Math.Pow(c, 2)) == Math.Sqrt(Math.Pow(a, 2))
               || Math.Sqrt(Math.Pow(a, 2) + Math.Pow(c, 2)) == Math.Sqrt(Math.Pow(b, 2));
        }

        private void clearPlaceholderByFocus(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            currentTextBox.SelectAll();
            if (currentTextBox.Text == TEXT_BOX_PLACEHOLDER) currentTextBox.Text = "";
        }

        private void returnPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(currentTextBox.Text)) currentTextBox.Text = TEXT_BOX_PLACEHOLDER;
        }

        private double getNumberOrError(string result)
        {
            try
            {
                double num = double.Parse(result);

                if(num < 0)
                {
                    MessageBox.Show("Сторона должна быть положительным числом!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return -1;
                }
                 return num;
            }
            catch
            {
                MessageBox.Show("Сторона должна быть числом!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }
        }

        private void changeHandler(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            string text = currentTextBox.Text;

            if (string.IsNullOrEmpty(text)) return;
            if (!Char.IsDigit(text[0]))
            {
                currentTextBox.Text = "";
                return;
            }

            int lengthText = text.Length - 1;
            text = text.Replace(".", ",");
            currentTextBox.Text = text;
            char lastSymb = text[lengthText];

            int commaCount = text.Count(s => s == ',');
            if (commaCount > 1 || (!Char.IsDigit(lastSymb) && lastSymb != ','))
            {
                string curText = text.Remove(lengthText);
                currentTextBox.Text = curText;
                return;
            };
            currentTextBox.CaretIndex = text.Length;
        }

        private string calcResult(double a, double b, double c)
        {
            if(a + b <= c || a + c <= b || b + c <= a)
            {
                return "Треугольника не существует!";
            }
            if (a == b && b == c && c == a)
            {
                return "Треугольник равносторонний";
            }
            if (isRightTriangle(a, b, c))
            {
                return "Треугольник прямоугольный";
            }
            if ((a == b && b != c) || (b == c && c != a) || (c == a && a != b))
            {
                return "Треугольник равнобедренный";
            }
            return "Треугольник разносторонний";
        }
    }
}
