using System;
using SharpMap.Data.Providers;
using NetTopologySuite;
using System.Linq;
using GeoAPI;
using GeoAPI.Geometries;
using System.Collections.ObjectModel;
using SharpVectors.Dom.Svg;
using System.Xml;
using System.IO;

namespace UsStateVisualizer
{
	public class Program
	{
		public static void Main (string[] args)
		{

			GeoAPI.GeometryServiceProvider.Instance = new NtsGeometryServices ();

			string DataDirectory = "/Users/jasonstewart/Dropbox/electioneering/data/boarders";
			string shpFileName = DataDirectory + "/states.shp";
			string dbfFileName = DataDirectory + "/states.dbf";

			// Size of svg in %
			//int svgHeight = ; 
			//int svgWidth = 100;

			//html Doc
			XmlDocument htmlDoc = new XmlDocument ();
			htmlDoc.AppendChild( htmlDoc.CreateDocumentType ("html","","","") );

			var htmlDocElement = htmlDoc.CreateElement ("html");

			var bodyElement = htmlDoc.CreateElement ("body");
			htmlDocElement.AppendChild (bodyElement);

			// svg document
			var svgElement = htmlDoc.CreateElement ("svg");
			bodyElement.AppendChild (svgElement);
			var svgName = htmlDoc.CreateAttribute ("name");
			svgName.Value = "Geographical Region Statistic Map";
			svgElement.Attributes.Append (svgName);

			var whiteSpace = htmlDoc.CreateWhitespace ("\n");

			// Attributes independent of data.
			var heightAttr = htmlDoc.CreateAttribute ("height");
			heightAttr.Value = "100%";
			var widthAttr = htmlDoc.CreateAttribute ("width");
			widthAttr.Value = "100%";
			svgElement.Attributes.Append(heightAttr);
			svgElement.Attributes.Append(widthAttr);

			// TODO: Use css/jquery-style formatting?
			//new Style
			XmlNode svgColorAttr = htmlDoc.CreateAttribute ("style");
			svgColorAttr.Value = "background:black";


			using (ShapeFile shapefile = new ShapeFile (shpFileName)) {
				shapefile.Open ();

				using (DbaseReader db = new DbaseReader (dbfFileName)) {
					db.Open ();
					// Set attributes for SVG.

					// Get extents, assign to svg "viewbox" attribute.
					Envelope bbox = shapefile.GetExtents ();
					Collection<IGeometry> geometries = shapefile.GetGeometriesInView (bbox);
					// 
					var viewBoxAttr = htmlDoc.CreateAttribute( "viewbox" );
					viewBoxAttr.Value = string.Format ("{0:F1} {1:F1} {2:F1} {3:F1}" , bbox.Left (), -bbox.Top (), bbox.Width, bbox.Height);
					svgElement.Attributes.Append(viewBoxAttr);


					// Create states 	
					for (uint iState = 0; iState < geometries.Count; iState++) {
						IGeometry geometry = geometries.ElementAt ((int)iState);

//						string stateName = db.GetValues (iState).Get (1); 
						//string stateCode = db.GetValues (iState).Get (5);
//						string stateName = "My State";
//						var stateElement = htmlDoc.CreateElement("State");
//						var stateNameAttr = htmlDoc.CreateAttribute("name");
//						stateNameAttr.Value = stateName;
//						stateElement.Attributes.Append (stateNameAttr);

						// state style - determined by metric
						string stateRegionStyle = "fill:#800080;stroke:#C0C0C0;stroke-width:0.1";

						// Shape
						for (int iSubRegion = 0; iSubRegion < geometry.NumGeometries; iSubRegion += 1) {
							var subRegionNode = htmlDoc.CreateElement ("polygon");
							//var subregionNameAttr = htmlDoc.CreateAttribute ("description");
							//subregionNameAttr.Value = stateElement.GetType () + " region " + iSubRegion.ToString ();

							// Define subregion border.
							var subRegion = geometry.GetGeometryN (iSubRegion);
							var coords = subRegion.Coordinates;	
							var coordStrings = coords.Select (coord => string.Format ("{0:F2},{1:F2} ", coord.X, -coord.Y));
							string coordString = coordStrings.Aggregate ((coordStringBase, coordStringNext) => coordStringBase + coordStringNext);
							var pointsAttr = htmlDoc.CreateAttribute ("points");
							pointsAttr.Value = coordString;
							subRegionNode.Attributes.Append(pointsAttr);

							// Color/Style spec.
							var styleAttr = htmlDoc.CreateAttribute ("style");
							styleAttr.Value = stateRegionStyle;
							subRegionNode.Attributes.Append(styleAttr);

							// Add state polygon, text to list 
							svgElement.AppendChild (whiteSpace);
							svgElement.AppendChild (subRegionNode);

						}
//						svgElement.AppendChild (whiteSpace);
//						svgElement.AppendChild (stateElement);
					}

//					foreach (var geometry in geometries) {
//						// title string, middle of polygon
//						var stateCoords = geometry.Coordinates;
//						double xAverage = stateCoords.Average (coord => coord.X);
//						double yAverage = stateCoords.Average (coord => coord.Y);
//						string titleStringHtml = string.Format ("<text x=\"{0}\" y=\"{1}\" "
//						                        + "style=\"font-size:2px\">" +
//						                        "State number XX</text>", xAverage, -yAverage);
//						//svgPolygonList.Add (titleStringHtml);
//					}

					bodyElement.AppendChild (whiteSpace);
					bodyElement.AppendChild (svgElement);

					htmlDocElement.AppendChild (bodyElement);

					htmlDoc.AppendChild (htmlDocElement);

					using (var stringWriter = new StringWriter ())
					using (var xmlTextWriter = XmlWriter.Create (stringWriter)) {
						htmlDoc.WriteTo(xmlTextWriter);
						xmlTextWriter.Flush();
						System.IO.File.WriteAllText (@"/Users/jasonstewart/Projects/UsStateVisualizer/output/usStates.html", 
							stringWriter.GetStringBuilder().ToString());

					}
				
				}
			}

			Console.WriteLine ("Press any key to continue...");
			Console.ReadKey ();
			Console.ReadKey ();
		}
	}
}

