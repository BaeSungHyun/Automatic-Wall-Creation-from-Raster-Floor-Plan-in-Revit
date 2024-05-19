using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.WebSockets;

namespace AutomaticWallCreation
{
    [Transaction(TransactionMode.Manual)]
    internal class WallCreation : IExternalCommand
    {
        // ******************* User Variable *********************

        // Unit Conversion
        public double ft_to_mm = 304.8;
        public double scale = 100;

        // Level Name
        public string lowLevelName = "1F";
        public string highLevelName = "2F";

        // CSV File Path - 실제 경로로 바꾸세요.
        public string filePath = @"C:\Automatic-Wall-Creation-from-Raster-Floor-Plan-in-Revit\data\revit_wall.csv"; 


        // ******************* User Variable *********************


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            Level? lower_constraint_level = FilterLevels(document, lowLevelName);
            Level? upper_constraint_level = FilterLevels(document, highLevelName);

            if (lower_constraint_level != null && upper_constraint_level != null)
            {
                double height_in_ft = (upper_constraint_level.Elevation - lower_constraint_level.Elevation);

                ElementId level_id = lower_constraint_level.Id;

                List<Element> wallElements = FilterAllWalls(document) as List<Element>;

                Element? wallElement = null;
                ElementId? wallTypeId = null;

                // Find wall named 'Generic'
                foreach (var we in wallElements)
                {
                    if (we.Name.Contains("일반"))
                    {
                        wallElement = we;
                        wallTypeId = wallElement.Id;
                        break;
                    }
                }

                if (wallElement == null)
                {
                    message = "'일반' 이라는 이름을 가진 벽이 없습니다.";
                    return Result.Failed;
                }

                // coordinate for lines
                List<XYZ> walls = new List<XYZ>();

                if (ReadCsvWalls(walls))
                {
                    // Final line of wall
                    List<Curve> lines = new List<Curve>();

                    for (int range = 0; range < walls.Count; range += 2)
                    {
                        lines.Add(Line.CreateBound(walls[range], walls[range + 1]) as Curve);
                    }

                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("create walls");

                        // this command below creates a wall, with default type 
                        foreach (var line in lines)
                        {
                            Wall.Create(document, line, wallTypeId, level_id, height_in_ft, 0, false, false);
                        }

                        transaction.Commit();
                    }

                    return Result.Succeeded;
                } else
                {
                    message = "CSV 파일을 읽는 도중 에러가 발생하였습니다.";
                    return Result.Failed;
                }
            }
            else
            {
                if (lower_constraint_level == null && upper_constraint_level == null)
                {
                    message = $"이름이 {lowLevelName} 그리고 {highLevelName} 인 Level 이 없습니다.";
                }
                else if (lower_constraint_level == null)
                    message = $"이름이 {lowLevelName} 인 Level 이 없습니다.";
                else if (upper_constraint_level == null)
                    message = $"이름이 {highLevelName} 인 Level 이 없습니다.";

                return Result.Failed;
            }
        }

        public Level? FilterLevels(Document document, string levelName)
        {
            // Create a category filter for Levels
            //ElementCategoryFilter levelsCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
            // Apply the filter to the elements in the active document
            FilteredElementCollector collector = new FilteredElementCollector(document);
            IList<Element> levels = collector.OfClass(typeof(Level)).ToElements();
            foreach (Level lv in levels)
            {
                if (lv.Name == levelName)
                {
                    return lv;
                }
            }

            return null;
        }

        public IList<Element> FilterAllWalls(Document document)
        {
            // Gather all wall types
            FilteredElementCollector wallCollector = new FilteredElementCollector(document).OfClass(typeof(WallType));
            IList<Element> walls = wallCollector.ToElements();

            return walls;
        }

        public bool ReadCsvWalls(List<XYZ> walls)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        double Xmax = 0, Xmin = double.MaxValue, Ymax = 0, Ymin = double.MaxValue;
                        int count = 0;
                        // Regular expression to match numbers
                        foreach (Match match in Regex.Matches(line, @"\d+"))
                        {
                            double number = double.Parse(match.Value);

                            if (count % 2 == 0)
                            {
                                if (Xmax < number)
                                {
                                    Xmax = number;
                                }
                                else if (Xmin > number)
                                {
                                    Xmin = number;
                                }
                            }
                            else
                            {
                                if (Ymax < number)
                                {
                                    Ymax = number;
                                }
                                else if (Ymin > number)
                                {
                                    Ymin = number;
                                }
                            }
                            count++;

                        }

                        double endX_min, endX_max, endY_min, endY_max;

                        // Extract end coordinates of the wall
                        if ((Xmax - Xmin) > (Ymax - Ymin))
                        {
                            endY_min = (Ymax + Ymin) / 2;
                            endY_max = (Ymax + Ymin) / 2;
                            endX_min = Xmin;
                            endX_max = Xmax;
                        }
                        else
                        {
                            endX_min = (Xmax + Xmin) / 2;
                            endX_max = (Xmax + Xmin) / 2;
                            endY_min = Ymin;
                            endY_max = Ymax;
                        }

                        walls.Add(new XYZ(endX_min / ft_to_mm * scale, endY_min / ft_to_mm * scale, 0));
                        walls.Add(new XYZ(endX_max / ft_to_mm * scale, endY_max / ft_to_mm * scale, 0));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

