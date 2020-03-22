using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Architecture;

namespace creatspacewithoutparameter
{
    class RoomParameters
    {
        /// <summary>
        /// 获取房间的面积
        /// </summary>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public double GetAear(Room room)
        {
            return room.Area;
        }

        /// <summary>
        /// 获取房间的高度
        /// </summary>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public double GetHeight(Room room)
        {
            return room.Volume / room.Area;
        }

        /// <summary>
        /// 获取组成房间边界墙段的列表
        /// </summary>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public List<BoundarySegment> GetRoomBoundarySegments(Room room)
        {
            List<BoundarySegment> boundarySegments = new List<BoundarySegment>();
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions()
            {
                //枚举类型，用于几何计算的空间元素的边界。指定边界的计算方式
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
                //是否计算自由边界面
                StoreFreeBoundaryFaces = true
            };
            foreach (var subsegments in room.GetBoundarySegments(options))
            {
                foreach (var segment in subsegments)
                {
                    boundarySegments.Add(segment);
                }
            }
            return boundarySegments;
        }

        /// <summary>
        /// 根据BoundarySegment获取与房间交界的墙段的长度
        /// </summary>
        /// <param name="boundarySegment">墙段</param>
        /// <returns></returns>
        public double GetWallLength(BoundarySegment boundarySegment)
        {
            return boundarySegment.GetCurve().Length;
        }

        /// <summary>
        /// 根据BoundarySegment获取与房间交界的墙
        /// </summary>
        /// <param name="boundarySegment">墙段</param>
        /// <param name="document">文档</param>
        /// <returns></returns>
        public Wall GetWall(BoundarySegment boundarySegment,Document document)
        {
            return document.GetElement(boundarySegment.ElementId) as Wall;
        }

        /// <summary>
        /// 获取墙段对应墙的材质
        /// </summary>
        /// <param name="boundarySegment">墙段</param>
        /// <param name="document">文档</param>
        /// <returns></returns>
        public Material GetWallMaterial(BoundarySegment boundarySegment,Document document)
        {
            Material material = null;
            Wall wall = document.GetElement(boundarySegment.ElementId) as Wall;
            if (wall != null)
            {
                material = wall.Category.Material;
            }
            return material;
        }

        /// <summary>
        /// 获取与房间交界门窗
        /// </summary>
        /// <param name="document"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public List<FamilyInstance> GetFamilyInstances(Document document,Room room)
        {
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
            foreach (var boundarySegment in GetRoomBoundarySegments(room))
            {
                Curve curve = boundarySegment.GetCurve();
                Wall wall = document.GetElement(boundarySegment.ElementId) as Wall;
                
                if (wall != null)
                {
                    foreach (var door in new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Doors).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList())
                    {
                        if (door.Host.Id.IntegerValue == wall.Id.IntegerValue)
                        {
                            XYZ point = (door.Location as LocationPoint).Point;
                            if (Inline(curve,point))
                            {
                                familyInstances.Add(door);
                            }
                            
                        }

                    }
                    foreach (var window in new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Windows).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList())
                    {
                        if (window.Host.Id.IntegerValue == wall.Id.IntegerValue)
                        {
                            XYZ point = (window.Location as LocationPoint).Point;
                            if (Inline(curve, point))
                            {
                                familyInstances.Add(window);
                            }
                            
                        }                        
                    }
                }
            }
            return familyInstances.Distinct().ToList();
        }

        public bool Inline(Curve curve,XYZ point)
        {




            return true;
        }


        /// <summary>
        /// 获取门窗的面积
        /// </summary>
        /// <param name="document"></param>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public double GetElementAear(Document document,FamilyInstance familyInstance)
        {
            double a = 0;
            FamilySymbol familySymbol = familyInstance.Symbol;
            if (familySymbol.Category == Category.GetCategory(document,BuiltInCategory.OST_Doors))
            {
                double height = familyInstance.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM).AsDouble();
                double width = familyInstance.get_Parameter(BuiltInParameter.FURNITURE_WIDTH).AsDouble();
                a = height * width;
            }
            if (familySymbol.Category == Category.GetCategory(document,BuiltInCategory.OST_Windows))
            {
                double height = familyInstance.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM).AsDouble();
                double width = familyInstance.get_Parameter(BuiltInParameter.FURNITURE_WIDTH).AsDouble();
                a = height * width;
            }
            return a;
        }
    }
}
