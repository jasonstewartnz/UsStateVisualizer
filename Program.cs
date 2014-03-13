using System;
using SharpMap.Data.Providers;
using NetTopologySuite;
using System.Linq;
using GeoAPI;
using GeoAPI.Geometries;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using SharpVectors.Dom.Svg;

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
			var builder = new SvgXmlBuilder ();

			builder.HtmlDoc.AppendChild (builder.HtmlDoc.CreateDocumentType ("html", "", "", ""));


			Collection<IGeometry> stateBorders;
			Envelope bbox;
			using (ShapeFile shapefile = new ShapeFile (shpFileName)) {
				shapefile.Open ();

				// Get extents, assign to svg "viewbox" attribute.
				bbox = shapefile.GetExtents ();
				stateBorders = shapefile.GetGeometriesInView (bbox);

				shapefile.Close ();
			}


			// Generate Tuple (ideally struct), containing names and codes.
			List<Tuple<string,string>> nameAndCode;
			using (DbaseReader db = new DbaseReader (dbfFileName)) {
				db.Open ();

				int start = 0;
				int count = stateBorders.Count ();
				var regionIdx = Enumerable.Range (start,count);

				// struct would look nicer here.
				nameAndCode = regionIdx.Select (iRegion => Tuple.Create (
					(string)((object[])db.GetValues ((uint)iRegion)) [1],
					(string)((object[])db.GetValues ((uint)iRegion)) [5]
				)).ToList();
				db.Close ();
			}
			
			// state style - determined by metric
			string stateRegionStyle = "fill:#800080;stroke:#C0C0C0;stroke-width:0.1";

			var states = stateBorders.Zip (nameAndCode,
				             (geometry, nameAndCodeTuple) => new PoliticalRegion (
					             geometry,
					             stateRegionStyle,
					             nameAndCodeTuple.Item1,
					             nameAndCodeTuple.Item2)
			             );

			// 
			var viewBoxAttr = builder.HtmlDoc.CreateAttribute ("viewbox");
			viewBoxAttr.Value = string.Format ("{0:F1} {1:F1} {2:F1} {3:F1}", bbox.Left (), -bbox.Top (), bbox.Width, bbox.Height);
			builder.SvgElement.Attributes.Append (viewBoxAttr);

 

			// Create states 	
			foreach (PoliticalRegion state in states) {
				// This syntax would be preferable: states.Select( state => state.AddPolygonToSvg(svgElement, builder.HtmlDoc ) );
				state.AddPolygonToSvg (builder.SvgElement, builder.HtmlDoc);

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

			builder.FinishDocument ();

			using (var stringWriter = new StringWriter ())
			using (var xmlTextWriter = XmlWriter.Create (stringWriter)) {
				builder.HtmlDoc.WriteTo (xmlTextWriter);
				xmlTextWriter.Flush ();
				System.IO.File.WriteAllText (@"/Users/jasonstewart/Projects/UsStateVisualizer/output/usStates.html", 
					stringWriter.GetStringBuilder ().ToString ());


				

			}

		}
	}
}

