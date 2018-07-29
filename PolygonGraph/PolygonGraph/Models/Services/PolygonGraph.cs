using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace PolygonGraph.Models.Services
{
    public class PolygonGraphOptions
    {
        public const float ANGLE_STRAIGHT = -0.5f;
        public const float ANGLE_DEFAULT = 0f;

        private float angleStart = ANGLE_DEFAULT;
        public float AngleStart
        {
            get { return angleStart; }
            set { angleStart = value; }
        }

        private int countVertices = 6;
        public int CountVertices
        {
            get { return countVertices; }
            set { countVertices = value; }
        }

        private int countGraduations = 5;
        public int CountGraduations
        {
            get { return countGraduations; }
            set { countGraduations = value; }
        }

        private float cornerRadius = 4;
        public float CornerRadius
        {
            get { return cornerRadius; }
            set { cornerRadius = value; }
        }

        private float graduationStrokeWidth = 7;
        public float GraduationStrokeWidth
        {
            get { return graduationStrokeWidth; }
            set { graduationStrokeWidth = value; }
        }

        private float scaleFrom = 0;
        public float ScaleFrom
        {
            get { return scaleFrom; }
            set { scaleFrom = value; }
        }

        private float scaleTo = 100;
        public float ScaleTo
        {
            get { return scaleTo; }
            set { scaleTo = value; }
        }
    }

    public class PolygonGraph
    {
        private PolygonGraphOptions options;
        private SKCanvas canvas;
        private SKSurface surface;
        private SKImageInfo info;

        private List<string> categories;
        private List<double> data;


        public PolygonGraph(PolygonGraphOptions options)
        {
            this.options = options;

            
        }

        public PolygonGraph DrawCanvas(SKPaintSurfaceEventArgs args)
        {
            this.info = args.Info;
            this.surface = args.Surface;
            this.canvas = surface.Canvas;

            canvas.Clear();
            canvas.Translate(info.Width / 2, info.Height / 2);

            drawPolygons();
            drawAxes();

            return this;
        }

        public PolygonGraph setCategories(List<string> categories)
        {
            this.categories = categories;
            return this;
        }

        public PolygonGraph setData(List<double> data)
        {
            this.data = data;
            return this;
        }

        private void drawPolygons()
        {
            for (int gIndex=0; gIndex < options.CountGraduations; gIndex++)
            {
                addPolygonToCanvas(gIndex);
            }
        }

        private void drawAxes()
        {
            for (int vIndex = 0; vIndex < options.CountVertices; vIndex++)
            {
                addAxeToCanvas(vIndex);
            }
        }

        /// <summary>
        /// Return the angle of a vertex. 
        /// </summary>
        /// <param name="vertexIndex"></param>
        /// <returns></returns>
        private double getVertexAngle(int vertexIndex)
        {
            double vertexAngle = this.options.AngleStart; // straight up = -0.5f
            return vertexAngle + (2 * Math.PI / this.options.CountVertices) * (vertexIndex+1);
        }

        /// <summary>
        ///  Return the radius of the circle from a graduation. 0.5 make a far away point. 0 make a point in center;
        /// </summary>
        /// <param name="graduationIndex"></param>
        /// <returns></returns>
        private float getRadiusByGraduation(int graduationIndex)
        {
            float denMax = 0.45f;
            float denMin = 0f;
            float denDiff = denMax - denMin;
            float denScale = denDiff / options.CountGraduations;
            float den = denMax - denScale * graduationIndex;

            return den * Math.Min(info.Width, info.Height);
        }

        private float getRadiusByValue(double value)
        {
            float denMax = 0.45f;
            float denMin = 0f;
            float denDiff = denMax - denMin;

            float dataDiff = options.ScaleTo - options.ScaleFrom;
            float dataPercent = (float)value / dataDiff * 100;
            float den = denMin + denDiff * dataPercent / 100;

            return den * Math.Min(info.Width, info.Height);
        }

        /// <summary>
        /// Get the exact X, Y of a vertext from the vertex index and graduation index
        /// </summary>
        /// <param name="vertexIndex"></param>
        /// <param name="graduationIndex"></param>
        /// <returns></returns>
        private SKPoint getSKPointByCoordinates(int vertexIndex, int graduationIndex)
        {
            float radius = getRadiusByGraduation(graduationIndex);
            double vertexAngle = getVertexAngle(vertexIndex);
            return new SKPoint(radius * (float)Math.Cos(vertexAngle), radius * (float)Math.Sin(vertexAngle));
        }

        private SKPoint getSKPointByValue(int vertexIndex)
        {
            float radius = getRadiusByValue(data[vertexIndex]);
            double vertexAngle = getVertexAngle(vertexIndex);
            return new SKPoint(radius * (float)Math.Cos(vertexAngle), radius * (float)Math.Sin(vertexAngle));
        }

        void addAxeToCanvas(int vertexIndex)
        {
            SKPoint vertexPoint = getSKPointByCoordinates(vertexIndex, 0);
            SKPoint center = new SKPoint(0, 0);

            // Create the path
            using (SKPath path = new SKPath())
            {
                // Begin at the first midpoint
                path.MoveTo(vertexPoint);
                path.LineTo(center);
                path.Close();

                // Render the path from vertex to center of graph
                using (SKPaint paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = Xamarin.Forms.Color.FromHex("#1F0D0F").ToSKColor();
                    paint.StrokeWidth = options.GraduationStrokeWidth / 2;

                    canvas.DrawPath(path, paint);
                }
            }
        }

        void addPolygonToCanvas(int graduationIndex)
        {
            SKPoint[] vertices = new SKPoint[this.options.CountVertices];
            SKPoint[] midPoints = new SKPoint[this.options.CountVertices];

            double vertexAngle = 0f * Math.PI;       // straight up = -0.5f

            // Coordinates of the vertices of the polygon
            for (int vertex = 0; vertex < this.options.CountVertices; vertex++)
            {
                vertices[vertex] = getSKPointByCoordinates(vertex, graduationIndex);
                vertexAngle = getVertexAngle(vertex);
            }

            // Coordinates of the midpoints of the sides connecting the vertices
            for (int vertex = 0; vertex < this.options.CountVertices; vertex++)
            {
                int prevVertex = (vertex + this.options.CountVertices - 1) % this.options.CountVertices;
                midPoints[vertex] = new SKPoint((vertices[prevVertex].X + vertices[vertex].X) / 2, (vertices[prevVertex].Y + vertices[vertex].Y) / 2);
            }

            // Create the path
            using (SKPath path = new SKPath())
            {
                // Begin at the first midpoint
                path.MoveTo(midPoints[0]);

                for (int vertex = 0; vertex < this.options.CountVertices; vertex++)
                {
                    SKPoint nextMidPoint = midPoints[(vertex + 1) % this.options.CountVertices];

                    // Draws a line from the current point, and then the arc
                    path.ArcTo(vertices[vertex], nextMidPoint, this.options.CornerRadius);

                    // Connect the arc with the next midpoint
                    path.LineTo(nextMidPoint);
                }
                path.Close();

                // Render the path in the center of the screen
                using (SKPaint paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = Xamarin.Forms.Color.FromHex("#1F0D0F").ToSKColor();
                    paint.StrokeWidth = options.GraduationStrokeWidth;

                    canvas.DrawPath(path, paint);
                }
            }
        }

        public void drawData()
        {
            SKPoint[] vertices = new SKPoint[this.options.CountVertices];
            SKPoint[] midPoints = new SKPoint[this.options.CountVertices];

            double vertexAngle = 0f * Math.PI;       // straight up = -0.5f

            // Coordinates of the vertices of the polygon
            for (int vertex = 0; vertex < this.options.CountVertices; vertex++)
            {
                vertices[vertex] = getSKPointByValue(vertex);
                vertexAngle = getVertexAngle(vertex);
            }


            // Create the path
            using (SKPath path = new SKPath())
            {
                // Begin at the first midpoint
                
                path.MoveTo(vertices[0]);

                for (int vertex = 0; vertex < this.options.CountVertices; vertex++)
                {
                    path.LineTo(vertices[vertex]);
                }
                path.Close();

                // Render the path in the center of the screen
                using (SKPaint paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.StrokeAndFill;
                    paint.Color = Xamarin.Forms.Color.FromHex("#562832").ToSKColor().WithAlpha(215);
                    paint.StrokeWidth = options.GraduationStrokeWidth;
                    canvas.DrawPath(path, paint);
                }
            }
        }

        public void refreshData()
        {
            
            drawData();
        }
    }
}
