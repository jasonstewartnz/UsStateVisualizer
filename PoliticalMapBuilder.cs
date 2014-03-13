using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using SharpMap.Data.Providers;
using NetTopologySuite;
using GeoAPI;
using GeoAPI.Geometries;

namespace UsStateVisualizer
{
	public class PoliticalMapBuilder
	{
		private string ShpFileName;
		private string DbfFileName;
		private Envelope BBox;


		// state style - determined by metric (in future version)
		public readonly string stateRegionStyle = "fill:#800080;stroke:#C0C0C0;stroke-width:0.1";

		// Reads shape info and database records to convert into PoliticalRegions
		public PoliticalMapBuilder (string shpFileName, string dbfFileName)
		{
			GeoAPI.GeometryServiceProvider.Instance = new NtsGeometryServices ();

			ShpFileName = shpFileName;
			DbfFileName = dbfFileName;
		}

		public IEnumerable<PoliticalRegion> CreateRegionInfo ()
		{
			Collection<IGeometry> stateBorders = GetBorders ();

			PoliticalMapBuilder mapBuilder = new PoliticalMapBuilder (ShpFileName, DbfFileName);
			var nameAndCode = mapBuilder.GetNameAndCode (stateBorders.Count());

			var states = stateBorders.Zip (nameAndCode,
				             (geometry, nameAndCodeTuple) => new PoliticalRegion (
					             geometry,
					             stateRegionStyle,
					             nameAndCodeTuple.Item1,
					             nameAndCodeTuple.Item2)
			             );

			return states;
		}

		private Collection<IGeometry> GetBorders ()
		{
			Collection<IGeometry> stateBorders;
			using (ShapeFile shapefile = new ShapeFile (ShpFileName)) {
				shapefile.Open ();

				// Get extents, assign to svg "viewbox" attribute.
				Envelope bbox = shapefile.GetExtents ();
				stateBorders = shapefile.GetGeometriesInView (bbox);

				shapefile.Close ();
			}
			return stateBorders;
		}

		public Envelope GetExtents ()
		{
			if (BBox == null) {

				using (ShapeFile shapefile = new ShapeFile (ShpFileName)) {
					shapefile.Open ();

					// Get extents, assign to svg "viewbox" attribute.
					BBox = shapefile.GetExtents ();
					shapefile.Close ();
				}
			}
			return BBox;
		}

		private List<Tuple<string,string>> GetNameAndCode (int count)
		{
			// Generate Tuple (ideally struct), containing names and codes.
			List<Tuple<string,string>> nameAndCode;
			using (DbaseReader db = new DbaseReader (DbfFileName)) {
				db.Open ();

				int start = 0;
				//int count = stateBorders.Count ();
				var regionIdx = Enumerable.Range (start, count);

				// struct would look nicer here.
				nameAndCode = regionIdx.Select (iRegion => Tuple.Create (
					(string)((object[])db.GetValues ((uint)iRegion)) [1],
					(string)((object[])db.GetValues ((uint)iRegion)) [5]
				)).ToList ();
				db.Close ();

			}
			return nameAndCode;
		}
	}
}

