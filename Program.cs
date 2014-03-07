using System;
using SharpMap.Data.Providers;
using NetTopologySuite;
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

			using (ShapeFile shapefile = new ShapeFile (shpFileName)) {
				shapefile.Open ();

				Envelope bbox = shapefile.GetExtents ();

				Collection<IGeometry> geometries = shapefile.GetGeometriesInView (bbox);

				Console.WriteLine ("Shapefile:");


				foreach (IGeometry geometry in geometries) {
					// Shape
					var coords = geometry.Coordinates;
					Console.WriteLine ("Next coordinate set:");

					foreach (Coordinate coord in coords){

						Console.WriteLine (coord.ToString());
					}
				}

				Console.WriteLine ("");
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}

