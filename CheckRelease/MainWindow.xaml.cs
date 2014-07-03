using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace CheckRelease
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DropTarget(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(file);
                        System.Diagnostics.Debug.WriteLine(assembly.FullName);

                        if (IsRelease(assembly))
                        {
                            System.Diagnostics.Debug.WriteLine("OK");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("DEBUG");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error to load " + System.IO.Path.GetFileName(file));
                    }
                }
            }
        }

        private bool IsRelease(Assembly assembly)
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);

            if (attributes.Length == 0)
            {
                //Console.WriteLine(String.Format("{0} is a RELEASE Build....", assembly.FullName));
                return true;
            }
            foreach (Attribute attr in attributes)
            {
                if (attr is DebuggableAttribute)
                {
                    DebuggableAttribute d = attr as DebuggableAttribute;
                    //Console.WriteLine(String.Format("Run time Optimizer is enabled : {0}", !d.IsJITOptimizerDisabled));
                    //Console.WriteLine(String.Format("Run time Tracking is enabled : {0}", d.IsJITTrackingEnabled));

                    if (d.IsJITOptimizerDisabled == true)
                    {
                        //Console.WriteLine(String.Format("{0} is a DEBUG Build....", assembly.FullName));
                        return false;
                    }
                    else
                    {
                        //Console.WriteLine(String.Format("{0} is a RELEASE Build....", assembly.FullName));
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
