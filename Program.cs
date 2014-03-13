using System;
using System.Linq;
using System.Collections.Generic;

using System.Xml;
using System.IO;

using GeoAPI.Geometries; // Ideally, would get rid of this dependency.


namespace UsStateVisualizer
{
	public class Program
	{
		public static void Main (string[] args)
		{
			string DataDirectory = "/Users/jasonstewart/Dropbox/electioneering/data/boarders";
			string shpFileName = DataDirectory + "/states.shp";
			string dbfFileName = DataDirectory + "/states.dbf";

			//html/Xml handler
			var builder = new SvgXmlBuilder ();

			// Region map info
			var regionBuilder = new PoliticalMapBuilder (shpFileName, dbfFileName);

			// Set svg extents
			Envelope bbox = regionBuilder.GetExtents ();
			var viewBoxAttr = builder.HtmlDoc.CreateAttribute ("viewbox");
			viewBoxAttr.Value = string.Format ("{0:F1} {1:F1} {2:F1} {3:F1}", bbox.Left (), -bbox.Top (), bbox.Width, bbox.Height);
			builder.SvgElement.Attributes.Append (viewBoxAttr);

			// Create states 	
			IEnumerable<PoliticalRegion> states = regionBuilder.CreateRegionInfo ();
			Action<PoliticalRegion> addState = state => state.AddPolygonToSvg (builder.SvgElement, builder.HtmlDoc);
			states.ToList ().ForEach (addState);

			// Close-out elements. Ideally, we wouldn't [have to?] do this.
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

