using System;
using System.IO;
//using System.IO.File;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;



namespace UsStateVisualizer
{
	public class RegionInfo<Type>
	{
		public readonly string IdentifierName;
		public readonly string[] ValueNames;
		public readonly Dictionary<string,Type> Values;

		public RegionInfo(string idName, string[] valueNames, Dictionary<string,Type> values){
			IdentifierName = idName;
			ValueNames = valueNames;
			Values = values;
		}
	}

	public class CsvRegionInfoReader<T>
	{
		string FileName;
		readonly uint FieldNameRow = 4;
		readonly uint IdentifierColumnNumber = 1;
		readonly IEnumerable<int> ValueColIdx = Enumerable.Range(5,3);//Row numbers. (1-based)
		readonly IEnumerable<int> ValueRowIdx = Enumerable.Range(5,55);//Row numbers. (1-based)
		readonly float Fill = 0;

		public CsvRegionInfoReader (string filename)
		{
			FileName = filename;
		}

		public RegionInfo<long[]> ReadInfo(){

			string[] csvFileLines = File.ReadAllLines (FileName);

			// Remove commas that seperate thousands (e.g. 1,200);
			Regex embeddedCommaExpr = new Regex (@",(?=(\d{3},)*\d{3}"")");

			// read Headers
			string[] headers = csvFileLines[FieldNameRow-1].Split(",".ToCharArray());
		
			// data 
			string[] linesWithoutNumericalCommas = csvFileLines.Select( s => embeddedCommaExpr.Replace(s,"").Replace("\"","") ).ToArray();
			string[][] dataRows = ValueRowIdx.Select(rowNum => linesWithoutNumericalCommas[rowNum-1].Split(",".ToCharArray()) ).ToArray();

			string[] identifiers = dataRows.Select (r => r [IdentifierColumnNumber - 1]).ToArray();
			string identifierColHeader = headers [IdentifierColumnNumber - 1];

			string[] valueColHeaders = ValueColIdx.Select (r => headers[(uint)r-1]).ToArray();

			//string[] dataStringArray = new string[dataRows.Length, dataRows[0].Length];
			var values = new Dictionary<string,long[]> ();
			for (int iRow=0;iRow<dataRows.Count();iRow+=1){
				long[] rowValues = new long[ValueColIdx.Count()];
				string valueStr;
				for (int iCol=0;iCol<ValueColIdx.Count()-1;iCol+=1){
					valueStr = dataRows[iRow][ValueColIdx.ToArray()[iCol]-1];
					if (valueStr == "") {
						rowValues [iCol] = (long)Fill;
					} else {
						rowValues [iCol] = Convert.ToInt64 (valueStr);
					}
				}

				values.Add (identifiers [iRow], rowValues);
			}

//			string[] valueRows = ValueRowIdx.Select (rId => dataRows.ToArray ().GetValue (rId));
			//Func<>
			return new RegionInfo<long[]>( identifierColHeader, valueColHeaders, values );
		
		}
	}
}