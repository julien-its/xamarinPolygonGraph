using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace PolygonGraph
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Exemple2 : ContentPage
	{
        protected Models.Services.PolygonGraph graph;

        private List<double> _data;
        private List<string> _categories;
        private List<Slider> _sliders;

        public Exemple2 ()
		{
			InitializeComponent ();

            _data = new List<double> { 80, 100, 90, 80 };
            _categories = new List<string> { "cat 1", "cat 2", "cat 3", "cat 4" };
            _sliders = new List<Slider>();

            graph = new Models.Services.PolygonGraph(new Models.Services.PolygonGraphOptions
            {
                AngleStart = Models.Services.PolygonGraphOptions.ANGLE_DEFAULT,
                CornerRadius = 4,
                CountGraduations = 4,
                CountVertices = 4
            });
            graph.setCategories(_categories).setData(_data);


            for (int index = 0; index < _data.Count(); index++)
            {
                Slider slider = new Slider(0, 100, _data[index]);
                slider.Margin = 0;

                slider.ValueChanged += OnSliderValueChanged;

                _sliders.Add(slider);
                MainLayout.Children.Add(slider);
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            graph.DrawCanvas(args);
            graph.drawData();
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            int index = 0;
            foreach (Slider slider in _sliders)
            {
                if (slider == (Slider)sender) break;
                index++;
            }
            _data[index] = args.NewValue;
            graph.setData(_data);
            GraphCanvasView.InvalidateSurface();
        }
    }
}