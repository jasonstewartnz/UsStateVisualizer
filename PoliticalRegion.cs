using System;
using System.Xml;
using System.Linq;
using GeoAPI.Geometries;


namespace UsStateVisualizer
{
	public class PoliticalRegion
	{
		public readonly IGeometry Geometry;
		public readonly string StyleSpec;
		public readonly string Name;
		public readonly string Code;

		public PoliticalRegion (IGeometry geometry, string style, string name, string code)
		{
			Geometry = geometry;
			StyleSpec = style;
			Name = name;
			Code = code;
		}

		public void AddPolygonToSvg( XmlElement svgElement, XmlDocument htmlDoc ){

			XmlElement regionElement = htmlDoc.CreateElement ("g");

			var regionNameAttr = htmlDoc.CreateAttribute("name");
			regionNameAttr.Value = Name;
			regionElement.Attributes.Append (regionNameAttr);
			var regionClassAttr = htmlDoc.CreateAttribute ("class");
			regionClassAttr.Value = "PoliticalRegion";
			regionElement.Attributes.Append (regionClassAttr);

			// Shape
			for (int iSubRegion = 0; iSubRegion < Geometry.NumGeometries; iSubRegion += 1) {
				var subRegionNode = htmlDoc.CreateElement ("polygon");
				//var subregionNameAttr = htmlDoc.CreateAttribute ("description");
				//subregionNameAttr.Value = stateElement.GetType () + " region " + iSubRegion.ToString ();

				// Define subregion border.
				var subRegion = Geometry.GetGeometryN (iSubRegion);
				var coords = subRegion.Coordinates;	
				var coordStrings = coords.Select (coord => string.Format ("{0:F2},{1:F2} ", coord.X, -coord.Y));
				string coordString = coordStrings.Aggregate ((coordStringBase, coordStringNext) => coordStringBase + coordStringNext);
				var pointsAttr = htmlDoc.CreateAttribute ("points");
				pointsAttr.Value = coordString;
				subRegionNode.Attributes.Append(pointsAttr);

				// Color/Style spec.
				var styleAttr = htmlDoc.CreateAttribute ("style");
				styleAttr.Value = StyleSpec;
				subRegionNode.Attributes.Append(styleAttr);

				// Add state polygon, text to list 
				regionElement.AppendChild (htmlDoc.CreateWhitespace ("\n"));
				regionElement.AppendChild (subRegionNode);

			}
			// Add state polygon, text to list 
			svgElement.AppendChild (htmlDoc.CreateWhitespace ("\n"));
			svgElement.AppendChild (regionElement);

		}
	
	}
}

