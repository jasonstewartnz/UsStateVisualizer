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

			string DataDirectory = "/Users/jasonstewart/Dropbox/electioneering/data/boarders";
			string shpFileName = DataDirectory + "/states.shp";

			// Size of svg in %
			int svgHeight = 100; 
			int svgWidth = 100;


			using (ShapeFile shapefile = new ShapeFile (shpFileName)) {
				shapefile.Open ();

				Envelope bbox = shapefile.GetExtents ();

				Collection<IGeometry> geometries = shapefile.GetGeometriesInView (bbox);

				//var geometries = geometries_.Take (1);

				Console.WriteLine ("Shapefile:");
				//Console.WriteLine (ge

				string polyStyle = "style=\"fill:#800080;stroke:#C0C0C0;stroke-width:0.1\" ";

				Collection<string> svgPolygonList = new Collection<string> ();
				// Create polygons 	
				foreach (IGeometry geometry in geometries) {
					// Shape
					for (int iSubRegion = 0; iSubRegion < geometry.NumGeometries; iSubRegion += 1) {
						//
						var subRegion = geometry.GetGeometryN (iSubRegion);
						var coords = subRegion.Coordinates;	

						var coordStrings = coords.Select (coord => string.Format ("{0:F2},{1:F2} ", coord.X, -coord.Y));

						string coordString = coordStrings.Aggregate ((coordStringBase, coordStringNext) => coordStringBase + coordStringNext);

						string svgPolygonHtml = string.Format ("<polygon points=\"{0}\" {1}/>", coordString, polyStyle);

						string stateHtml = svgPolygonHtml + "\n";// + titleStringHtml + "\n";

						// Add state polygon, text to list 
						svgPolygonList.Add (stateHtml);
					}
				}

				foreach (var geometry in geometries){
					// title string, middle of polygon
					var stateCoords = geometry.Coordinates;
					double xAverage = stateCoords.Average (coord => coord.X);
					double yAverage = stateCoords.Average (coord => coord.Y);
					string titleStringHtml = string.Format ("<text x=\"{0}\" y=\"{1}\" "
						+ "style=\"font-size:2px\">" + 
						"State number XX</text>", xAverage, -yAverage);
					//svgPolygonList.Add (titleStringHtml);
				}
				// svg properties
				string statePolygonHtml = svgPolygonList.Aggregate ((str1, str2) => str1 + str2);
				string hgtWdtStr = string.Format ("height=\"{0}%\" width=\"{1}%\" ", svgHeight, svgWidth);
				string viewBox = string.Format ("viewbox=\"{0:F1} {1:F1} {2:F1} {3:F1}\" ", bbox.Left (), -bbox.Top (), bbox.Width, bbox.Height);
				string svgColorSpec = "style=\"background:black\" ";
				string svgOpenTagHtml = "<svg " + hgtWdtStr + viewBox + svgColorSpec + ">";

				string svgHtml = svgOpenTagHtml + statePolygonHtml + "</svg>";

				string bodyHtml = "<body>" + svgHtml + "</body>";

				string docHtml = "<!DOCTYPE html><html>" + bodyHtml + "</html>";
				System.IO.File.WriteAllText (@"/Users/jasonstewart/Projects/UsStateVisualizer/output/usStates.html", docHtml);
				//Console.WriteLine (svgHtml);
			}

			Console.WriteLine ("Press any key to continue...");
			Console.ReadKey ();
			Console.ReadKey ();
		}
	}
}

