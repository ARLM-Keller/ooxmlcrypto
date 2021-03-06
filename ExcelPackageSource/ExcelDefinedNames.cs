﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace OfficeOpenXml
{
    public class ExcelDefinedNames
    {
        ExcelWorksheets _worksheets;
        XmlDocument _worsheetsXml;
        XmlNode _definedNames;

        protected internal ExcelDefinedNames(ExcelWorksheets worksheets, XmlDocument worksheetsXml)
        {
            _worksheets = worksheets;
            _worsheetsXml = worksheetsXml;

            _definedNames = worksheetsXml.SelectSingleNode("//d:definedNames", _worksheets.NsManager);
        }

        public void Add(String name, String rangeRef)
        {
            if (!IsValidName(name)) { throw new ArgumentException("name"); }
            if (!IsValidRangeRef(rangeRef)) { throw new ArgumentException("Invalid rangeRef"); }

            if (Contains(name)) { throw new ArgumentException("Already exists: " + name); }

            // Create list element if needed
            if (_definedNames == null)
            {
                XmlNode wbNode = _worsheetsXml.SelectSingleNode("//d:workbook", _worksheets.NsManager);
                if (wbNode == null) { throw new NullReferenceException("Workbook node missing."); }

                _definedNames = _worsheetsXml.CreateElement("definedNames", ExcelPackage.schemaMain);
                wbNode.AppendChild(_definedNames);
            }

            XmlElement dnElement = _worsheetsXml.CreateElement("definedName", ExcelPackage.schemaMain);
            XmlAttribute nameAttrib = _worsheetsXml.CreateAttribute("name");
            nameAttrib.Value = name;
            dnElement.Attributes.Append(nameAttrib);
            dnElement.InnerText = rangeRef;
            _definedNames.AppendChild(dnElement);
        }

        bool IsValidRangeRef(String rangeRef)
        {
            return !String.IsNullOrEmpty(rangeRef) &&
                    rangeRef.Contains("!") &&
                    rangeRef.Contains(":");
        }
        bool IsValidName(String name)
        {
            return !String.IsNullOrEmpty(name) &&
                !name.Contains(" ") &&
                char.IsLetter(name[0]);
        }

        public void Remove(String name)
        {
            if (_definedNames == null) { return; }
            
            XmlNode dnNode = GetSingleNameNode(name);
            if (dnNode == null) { return; }

            _definedNames.RemoveChild(dnNode);
        }

        public string this[String name]
        {
            get 
            {
                XmlNode dnNode = GetSingleNameNode(name);
                if (dnNode == null) { return null; }
                return dnNode.InnerText; 
            }
            set 
            {
                if (value == null)
                {
                    Remove(name);
                    return;
                }

                XmlNode dnNode = GetSingleNameNode(name);
                if (dnNode == null)
                {
                    Add(name, value);
                    return;
                }

                if (!IsValidRangeRef(value)) { throw new ArgumentException("Invalid rangeRef"); }
                dnNode.InnerText = value;
            }
        }

        public bool Contains(String name)
        {
            return GetSingleNameNode(name) != null;
        }

        XmlNode GetSingleNameNode(String name)
        {
            if (_definedNames == null) { return null; }
            foreach (XmlNode dnNode in _definedNames.ChildNodes)
            {
                if (dnNode.Attributes["name"].Value == name) { return dnNode; }
            }
            return null;
        }

        /// <summary>
        /// Get a range reference in Excel format
        /// e.g. GetRange("sheet", 1,5,10,2) => "sheet!$J$1:$K$5"
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="startRow"></param>
        /// <param name="rowCount">Number of rows to include, >=1</param>
        /// <param name="startCol"></param>
        /// <param name="colCount">Number of columns to include, >=1</param>
        /// <returns></returns>
        public static String GetRangeRef(String worksheet,
            int startRow, int rowCount, int startCol, int colCount)
        {
            // Be tolerant
            if (rowCount <= 0) { rowCount = 1; }
            if (colCount <= 0) { colCount = 1; }

            return worksheet + "!" +
                "$" + ExcelCell.GetColumnLetter(startCol) + "$" + startRow + ":" +
                "$" + ExcelCell.GetColumnLetter(startCol + colCount -1) + "$" + (startRow + rowCount - 1);
        }
    }
}
