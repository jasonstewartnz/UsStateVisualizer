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
		// Borders 
		static public string DataDirectory = "/Users/jasonstewart/Dropbox/electioneering/data/boarders";

		//Election results
		static public string electionResultsFile = "/Users/jasonstewart/Datasets/election results/2012congresults/Table 7. House by Party-Table 1.csv";

		public static void Main (string[] args)
		{
			string shpFileName = DataDirectory + "/states.shp";
			string dbfFileName = DataDirectory + "/states.dbf";

			//html/Xml handler
			var builder = new SvgXmlBuilder ();

			// Excel shit
			var excelFileReaderFactor = new CsvRegionInfoReader<long> (electionResultsFile);
			excelFileReaderFactor.ReadInfo ();

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

			// Get state

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

