using System;
using System.Xml;

namespace UsStateVisualizer
{
	public class SvgXmlBuilder
	{
		public XmlDocument HtmlDoc;
		public XmlElement SvgElement;

		private XmlElement bodyElement;
		private XmlElement htmlDocElement;

		// Provides methods for structuring xml to produce an Svg (nested in an HTML doc).
		public SvgXmlBuilder ()
		{
			HtmlDoc = new XmlDocument ();

			// HTML-level
			HtmlDoc.AppendChild (HtmlDoc.CreateDocumentType ("html", "", "", ""));
			htmlDocElement = HtmlDoc.CreateElement ("html");

			// Body-level
			bodyElement = HtmlDoc.CreateElement ("body");
			htmlDocElement.AppendChild (bodyElement);

			// svg document
			SvgElement = HtmlDoc.CreateElement ("svg");
			bodyElement.AppendChild (SvgElement);
			var svgName = HtmlDoc.CreateAttribute ("name");
			svgName.Value = "Geographical Region Statistic Map";
			SvgElement.Attributes.Append (svgName);

			// Attributes independent of data.
			var heightAttr = HtmlDoc.CreateAttribute ("height");
			heightAttr.Value = "100%";
			var widthAttr = HtmlDoc.CreateAttribute ("width");
			widthAttr.Value = "100%";
			SvgElement.Attributes.Append (heightAttr);
			SvgElement.Attributes.Append (widthAttr);

			// TODO: Use css/jquery-style formatting?
			//new Style
			var svgColorAttr = HtmlDoc.CreateAttribute ("style");
			svgColorAttr.Value = "background:black";
			SvgElement.Attributes.Append (svgColorAttr);
		}

		public void FinishDocument () {
			bodyElement.AppendChild (HtmlDoc.CreateWhitespace ("\n"));
			bodyElement.AppendChild (SvgElement);
			bodyElement.AppendChild (HtmlDoc.CreateWhitespace ("\n"));

			htmlDocElement.AppendChild (bodyElement);
			htmlDocElement.AppendChild (HtmlDoc.CreateWhitespace ("\n"));

			HtmlDoc.AppendChild (htmlDocElement);
			HtmlDoc.AppendChild (HtmlDoc.CreateWhitespace ("\n"));
		}

	}
}

