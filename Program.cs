using System;
using SharpMap.Data.Providers;
using NetTopologySuite;
using GeoAPI;

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

				Console.WriteLine (shapefile.ToString ());
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}

