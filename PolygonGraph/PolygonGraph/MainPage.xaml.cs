using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace PolygonGraph
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            Models.Services.PolygonGraph graph = new Models.Services.PolygonGraph(args, new Models.Services.PolygonGraphOptions());

            graph.setCategories(new List<string> { "cat 1", "cat 2", "cat 3", "cat 4", "cat 5", "cat 6", })
                 .setData(new List<float> { 80, 100, 90, 80, 70, 95 })
                 .drawData();
        }

        void OnCanvasViewPaintSurface2(object sender, SKPaintSurfaceEventArgs args)
        {
            Models.Services.PolygonGraph graph = new Models.Services.PolygonGraph(args, new Models.Services.PolygonGraphOptions
            {
                AngleStart = Models.Services.PolygonGraphOptions.ANGLE_DEFAULT,
                CornerRadius = 4,
                CountGraduations = 4,
                CountVertices = 4
            });

            graph.setCategories(new List<string> { "cat 1", "cat 2", "cat 3", "cat 4" })
                 .setData(new List<float> { 65, 50, 75, 100 })
                 .drawData();
        }

        void OnCanvasViewPaintSurface3(object sender, SKPaintSurfaceEventArgs args)
        {
            Models.Services.PolygonGraph graph = new Models.Services.PolygonGraph(args, new Models.Services.PolygonGraphOptions
            {
                AngleStart = Models.Services.PolygonGraphOptions.ANGLE_STRAIGHT,
                CornerRadius = 4,
                CountGraduations = 5,
                CountVertices = 3
            });

            graph.setCategories(new List<string> { "cat 1", "cat 2", "cat 3" })
                .setData(new List<float> { 70, 45, 95 })
                .drawData();
        }


    }
}
