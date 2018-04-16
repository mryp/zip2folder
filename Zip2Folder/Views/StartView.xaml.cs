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
using Zip2Folder.ViewModels;

namespace Zip2Folder.Views
{
    /// <summary>
    /// StartView.xaml の相互作用ロジック
    /// </summary>
    public partial class StartView : Page
    {
        private StartViewModel ViewModel
        {
            get
            {
                return DataContext as StartViewModel;
            }
        }

        public StartView()
        {
            InitializeComponent();
        }
    }
}
