using System;
using SharpMap.Data.Providers;
using NetTopologySuite;
using System.Linq;
using GeoAPI;
using GeoAPI.Geometries;
using System.Collections.ObjectModel;

namespace UsStateVisualizer
{
	public class Program
	{ 
		public static void Main (string[] args)
		{

			GeoAPI.GeometryServiceProvider.Instance = new NtsGeometryServices ();

			string DataDirectory = "/Users/jasonstewart/datasets/us states shapefile";
			string shpFileName = DataDirectory + "/usa_state_shapefile.shp";


			int svgHeight = 300;
			int svgWidth = 500;


			using (ShapeFile shapefile = new ShapeFile (shpFileName)) {
				shapefile.Open ();

				Envelope bbox = shapefile.GetExtents ();

				Collection<IGeometry> geometries = shapefile.GetGeometriesInView (bbox);

				Console.WriteLine ("Shapefile:");

				Collection<string> svgPolygonList = new Collection<string>();
					// Create polygons 	
					foreach (IGeometry geometry in geometries) {
						// Shape
						var coords = geometry.Coordinates;

						Console.WriteLine ("Next coordinate set:");

						var coordStrings = coords.Select( coord => string.Format ("{0},{1} ", coord.X, coord.Y) );

						double xAverage = coords.Average ( coord => coord.X );
						double yAverage = coords.Average( coord => coord.Y );

						string coordString = coordStrings.Aggregate ( (coordStringBase,coordStringNext) => coordStringBase + coordStringNext );

						string svgPolygonHtml = string.Format("<polygon points={0} fill=\"red\"/>", coordString );

						// title string, middle of polygon
						string titleStringHtml = string.Format( "<text x=\"{0}\" y=\"{1}\" fill=\"red\">State number XX</text>", xAverage, yAverage );

						string stateHtml = svgPolygonHtml + "\n" + titleStringHtml + "\n";

						// Add state polygon, text to list 
						svgPolygonList.Add (stateHtml);
					}
				
					string statePolygonHtml = svgPolygonList.Aggregate((str1,str2) => str1 + str2);
					string hgtWdtStr = string.Format ("height=\"{0}\" width=\"{1}\">", svgHeight, svgWidth);
					
					string svgHtml = "<svg " + hgtWdtStr + statePolygonHtml + "</svg>";

				Console.WriteLine (svgHtml);
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
			Console.ReadKey();
		}
	}
}

