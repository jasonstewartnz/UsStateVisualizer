using System;
using System.IO;
//using System.IO.File;
using System.Data;
using System.Linq;
using System.Collections.Generic;



namespace UsStateVisualizer
{
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

		public void ReadInfo(){

			string[] csvFileLines = File.ReadAllLines (FileName);

			// read Headers
			string[] headers = csvFileLines[FieldNameRow-1].Split(",".ToCharArray());
		
			string[][] dataRows = ValueRowIdx.Select(rowNum => csvFileLines[rowNum-1].Split(",".ToCharArray()) ).ToArray();

			string[] identifiers = dataRows.Select (r => r [IdentifierColumnNumber - 1]).ToArray();
			string identifierColHeader = headers [IdentifierColumnNumber - 1];

			string[] valueColHeaders = ValueColIdx.Select (r => headers[(uint)r-1]).ToArray();

//			string[] dataStringArray = new string[dataRows.Length, dataRows[0].Length];
//			for (int iRow=0;iRow<dataRows.Count();iRow+=1){
//				for (int iCol=0;iCol<dataRows[0].Length;iCol+=1){
//					dataStringArray[iRow,iCol] = dataRows[ValueRowIdx.ToArray()[iRow]][ValueColIdx.ToArray()[iCol]];
//				}
//			}

//			string[] valueRows = ValueRowIdx.Select (rId => dataRows.ToArray ().GetValue (rId));
			//Func<>
		}
	}
}