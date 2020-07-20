using MathOperation.Common;
using MathOperation.Helpers;
using MathOperation.View;
using MathOperation.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MathOperation
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainViewModel MainViewModel { get; private set; }
        public Grid grid { get; private set; }
        public Label label { get; private set; }

        private object locker = new object();
        private DisplayInfo mainDisplayInfo;
        private float cellHeight;
        private float cellWidth;
        private float goalHeight => (float)((mainDisplayInfo.Height / mainDisplayInfo.Density) * 0.20);
        private float minorLayoutHight => (float)((mainDisplayInfo.Height / mainDisplayInfo.Density) * 0.06);
        private float whiteSpace => (float)((mainDisplayInfo.Height / mainDisplayInfo.Density) * 0.15);

        private int From = 30;
        private int To = 50;
        

        public MainPage(string from, string to)
        {
            InitializeComponent();
            ParseNumber(from, to);
            MainViewModel = new MainViewModel(From);
            mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            MainViewModel.GenerateListButton += GenerateNewButtons;
            MainViewModel.TableViewModel.RemoveButton += RemoveButton;
            CreateMainView();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void ParseNumber(string from, string to)
        {
            if(from != string.Empty && to != string.Empty)
            {
                From = Int32.Parse(from);
                To = Int32.Parse(to);
            }
        }

        private void CreateGrid()
        {
            grid = new Grid()
            {
                RowSpacing = 0,
                ColumnSpacing = 0
            };

            cellHeight = (float)((mainDisplayInfo.Height / mainDisplayInfo.Density - whiteSpace - goalHeight - minorLayoutHight) / MainViewModel.Row);
            cellWidth = (float)(mainDisplayInfo.Width / mainDisplayInfo.Density / MainViewModel.Column);

            for (int i = 0; i < MainViewModel.Row; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = cellHeight });
            }

            for (int i = 0; i < MainViewModel.Column; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = cellWidth });
            }

            FillGrid();
        }

        private void FillGrid()
        {
            var listOfCells = new List<CellViewModel>();
            for (int i = 0; i < MainViewModel.Row; i++)
            {
                for (int j = 0; j < MainViewModel.Column; j++)
                {
                    CellViewModel currentCell = MainViewModel.TableViewModel.GetNumberByIndexes(i, j);
                    currentCell.SkipRow = i;
                    CreateButton(currentCell, 0, j);

                    listOfCells.Add(currentCell);
                }
            }

            MainViewModel.TableViewModel.RaiseTransleteCells(listOfCells, cellHeight);
        }

        private void CreateMainView()
        {
            mainLayout.Children.Clear();
            CreateAddAndScoreButtons();
            CreateGoalLabel();
            CreateGrid();
            //mainLayout.Children.Add(label);
            mainLayout.Children.Add(grid);
        }

        private void CreateGoalLabel()
        {
            var goalViewModel = MainViewModel.GoalViewModel;

            label = new Label()
            {
                IsVisible = true,
                FontSize = goalViewModel.Size,
                WidthRequest = (float)(mainDisplayInfo.Width / mainDisplayInfo.Density) - 10,
                HeightRequest = goalHeight,
                BackgroundColor = StaticResources.GoalBackgroundColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = StaticResources.ColorGoalText
            };


            Binding bindingText = new Binding() { Source = goalViewModel, Path = "Number" };
            label.SetBinding(Label.TextProperty, bindingText);

            Frame frame = new Frame()
            {
                Margin = new Thickness(1,1,1,0),
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true,
                IsClippedToBounds = true,
                CornerRadius = StaticResources.RadiusGoal,
                Content = label
            };

            mainLayout.Children.Add(frame);
        }

        private void GenerateNewButtons(object twoList, EventArgs args)
        {
            List<CellViewModel> selectedCells = twoList as List<CellViewModel>;
            if(selectedCells != null)
            {
                MainViewModel.TableViewModel.RemoveCellAndAddFallLDown(selectedCells, cellHeight);
            }
            else
            {
                var listInt = twoList as List<int>;
                var newCellViewModel = MainViewModel.TableViewModel.CreaderNewCell(listInt[0], listInt[2]);
                newCellViewModel.SkipRow = listInt[1];
                newCellViewModel.Button = CreateButton(newCellViewModel, 0, listInt[2]);
                MainViewModel.TableViewModel.RaiseTransleteCells(new List<CellViewModel>() { newCellViewModel }, cellHeight);
            }
        }

        private Button CreateButton(CellViewModel currentCell,int i, int j)
        {
            Button btn = new Button
            {
                CommandParameter = currentCell,
                TextColor = Color.White,
                CornerRadius = 30,
                FontSize = currentCell.Size,
                BorderWidth = 6
            };
            currentCell.Button = btn;

            Binding bindText = new Binding() { Source = currentCell, Path = "Number" };
            Binding bindColor = new Binding() { Source = currentCell, Path = "Color" };
            Binding bindVisible = new Binding() { Source = currentCell, Path = "IsVisible", Mode = BindingMode.TwoWay };
            Binding bindMargin = new Binding() { Source = currentCell, Path = "Margin", Mode = BindingMode.TwoWay };
            Binding bindCommand = new Binding() { Source = MainViewModel.TableViewModel, Path = "SelectCell" };

            btn.SetBinding(Button.TextProperty, bindText);
            btn.SetBinding(Button.BackgroundColorProperty, bindColor);
            btn.SetBinding(Button.IsVisibleProperty, bindVisible);
            btn.SetBinding(Button.MarginProperty, bindMargin);
            btn.SetBinding(Button.CommandProperty, bindCommand);

            AddButtonToGridByIndexes(btn, i, j);

            return btn;
        }

        private void RemoveChildFromGrid(List<CellViewModel> cellVM)
        {
            while(cellVM.Count != 0)
            {
                grid.Children.Remove(cellVM[0].Button);
                cellVM.RemoveAt(0);
            }
        }

        private void AddButtonToGridByIndexes(Button btn, int i, int j)
        {
            btn.SetValue(Grid.RowProperty, i);
            btn.SetValue(Grid.ColumnProperty, j);

            grid.Children.Add(btn);
        }

        private void RemoveButton(object button, EventArgs eventArgs)
        {
            grid.Children.Remove(button as Button);

            if (grid.Children.Count == 0)
                RefrashGrid();
        }

        private void CreateAddAndScoreButtons()
        {
            AddCellViewModel addCell = MainViewModel.AddCellViewModel;
            ScoreVIewModel scoreVM = MainViewModel.ScoreVIewModel;

            StackLayout minorLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = minorLayoutHight,
                Padding = new Thickness(0)
            };

            Button addButton = new Button()
            {
                Text = addCell.Text,
                TextColor = StaticResources.ColorGoalText,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                HeightRequest = minorLayoutHight,
                Margin = new Thickness(5),
                CornerRadius = StaticResources.RadiusGoal,
                Command = addCell.ClickOnAddButton
            };

            Binding color = new Binding() { Source = addCell, Path = "ColorClickable" };
            addButton.SetBinding(Button.BackgroundColorProperty, color);

            Button scoreButton = new Button()
            {
                TextColor = StaticResources.ColorGoalText,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HeightRequest = minorLayoutHight,
                Margin = new Thickness(5),
                CornerRadius = StaticResources.RadiusGoal,
                BackgroundColor = StaticResources.GoalBackgroundColor
            };

            Button setNumbers = new Button()
            {
                TextColor = StaticResources.ColorGoalText,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HeightRequest = minorLayoutHight,
                Margin = new Thickness(5),
                CornerRadius = StaticResources.RadiusGoal,
                BackgroundColor = StaticResources.GoalBackgroundColor
            };

            setNumbers.Clicked += SetNumbersButton_Click;

            Binding textBinding = new Binding() { Source = scoreVM, Path = "Text" };
            scoreButton.SetBinding(Button.TextProperty, textBinding);

            minorLayout.Children.Add(scoreButton);
            minorLayout.Children.Add(setNumbers);
            minorLayout.Children.Add(addButton);
            mainLayout.Children.Add(minorLayout);
        }

        private void RefrashGrid()
        {
            grid.Children.Clear();
            MainViewModel.ReFillTable(From, To);
            FillGrid();
        }

        private async void SetNumbersButton_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SetNumbersPage());
        }
    }
}
