using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rectangle
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TEXT_BOX_PLACEHOLDER = "...";
        const string MESSAGE_BOX_ERROR_NEED_NUMBER = "Координата должна быть числом!";
        const string MESSAGE_BOX_ERROR_TITLE = "Ошибка!";
        const double t = 0.0001;

        public MainWindow()
        {
            InitializeComponent();
            TitleBlock.Text = this.Title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var stackPanel in MainGrid.Children)
            {
                if (stackPanel is StackPanel)
                {
                    foreach (var grid in (stackPanel as StackPanel).Children)
                    {
                        if (grid is Grid)
                        {
                            foreach (var textBox in (grid as Grid).Children)
                            {

                                if (textBox is TextBox)
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
            }

            ResultButton.Click += getResult;
        }

        private void getResult(object sender, RoutedEventArgs e)
        {
            double x1 = getNumberOrError(x_1.Text);
            if (x1 == -1) return;
            double x2 = getNumberOrError(x_2.Text);
            if (x2 == -1) return;
            double x3 = getNumberOrError(x_3.Text);
            if (x3 == -1) return;
            double x4 = getNumberOrError(x_4.Text);
            if (x4 == -1) return;

            double y1 = getNumberOrError(y_1.Text);
            if (y1 == -1) return;
            double y2 = getNumberOrError(y_2.Text);
            if (y2 == -1) return;
            double y3 = getNumberOrError(y_3.Text);
            if (y3 == -1) return;
            double y4 = getNumberOrError(y_4.Text);
            if (y4 == -1) return;

            double[] point1 = new double[2] { x1, y1 };
            double[] point2 = new double[2] { x2, y2 };
            double[] point3 = new double[2] { x3, y3 };
            double[] point4 = new double[2] { x4, y4 };

            ResultBlock.Text = calcResult(point1, point2, point3, point4);
        }

        private void clearPlaceholderByFocus(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            if (currentTextBox.Text == TEXT_BOX_PLACEHOLDER)
            {
                currentTextBox.Text = "";
                return;
            }
            currentTextBox.SelectAll();
            return;
        }

        private void returnPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(currentTextBox.Text))
            {
                currentTextBox.Text = TEXT_BOX_PLACEHOLDER;
                return;
            }
        }

        private void changeHandler(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            string text = currentTextBox.Text;

            if (string.IsNullOrEmpty(text) || text == TEXT_BOX_PLACEHOLDER) return;
            if (!Char.IsDigit(text[0]) && text[0] != '-')
            {
                currentTextBox.Text = "";
                return;
            }

            int lengthText = text.Length - 1;
            text = text.Replace(".", ",");
            currentTextBox.Text = text;
            char lastSymb = text[lengthText];

            int commaCount = text.Count(s => s == ',');
            int minusCount = text.Count(s => s == '-');
            if (commaCount > 1 
                || minusCount > 1 
                || (!Char.IsDigit(lastSymb) && lastSymb != ',' && lastSymb != '-')
                || (lastSymb == '-' && text.Length > 1))
            {
                string curText = text.Remove(lengthText);
                currentTextBox.Text = curText;
                return;
            };
            currentTextBox.CaretIndex = text.Length;
        }

        private string calcResult(
            double[] point1,
            double[] point2,
            double[] point3,
            double[] point4
            )
        {
            double ab = getLength(point1, point2);
            double bc = getLength(point2, point3);
            double cd = getLength(point3, point4);
            double da = getLength(point4, point1);
            double ac = getLength(point1, point3);
            double bd = getLength(point2, point4);

            if(ab == 0 || bc == 0 || cd == 0 || da == 0)
            {
                return "Четырёхугольника не существует!";
            }
            if (!isParal(point1, point2, point3, point4) 
                && !isParal(point2, point3, point1, point4)) return "Четырёхугольник общего вида";
            if ((isParal(point1, point2, point3, point4) 
                && !isParal(point2, point3, point1, point4))
                || (!isParal(point1, point2, point3, point4) 
                && isParal(point2, point3, point1, point4)))
            {
                if (ab == cd)
                    return "Равнобедренная трапеция";
                if (ac == Math.Sqrt(Math.Pow(ab, 2) + Math.Pow(bc, 2)))
                    return "Прямоугольная трапеция";
                return "Трапеция общего вида";
            }
            if (isParal(point1, point2, point3, point4) && isParal(point2, point3, point1, point4))
            {
                if ((Math.Abs(ab - bc) > t) && (Math.Abs(ac - bd) > t)) return "Параллелограмм";
                if ((Math.Abs(ab - bc) > t) && (Math.Abs(ac - bd) < t)) return "Прямоугольник";
                if ((Math.Abs(ab - bc) < t) && (Math.Abs(ac - bd) > t)) return "Ромб";
                if ((Math.Abs(ab - bc) < t) && (Math.Abs(ac - bd) < t)) return "Квадрат";
            }
            return "Четырёхугольника не существует!";
        }

        private double getLength(double[] point1, double[] point2)
        {
            return Math.Sqrt(Math.Pow(point1[0] - point2[0], 2) + Math.Pow(point1[1] - point2[1], 2));
        }

        private bool isParal(double[] point1, double[] point2, double[] point3, double[] point4)
        {
            return (point2[1] - point1[1]) * (point4[0] - point3[0]) == (point4[1] - point3[1]) * (point2[0] - point1[0]);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ResultButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private double getNumberOrError(string result)
        {
            try
            {
                double num = double.Parse(result);
                return num;
            }
            catch
            {
                MessageBox.Show(MESSAGE_BOX_ERROR_NEED_NUMBER, MESSAGE_BOX_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }
        }
    }
}
