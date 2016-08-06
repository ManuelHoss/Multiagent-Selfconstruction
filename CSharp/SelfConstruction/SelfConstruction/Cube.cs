using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace SelfConstruction
{
    public class Cube
    {
        public void CreateCube(Document doc, XYZ cubePosition, bool isAgent)
        {
            TessellatedShapeBuilder builder = new TessellatedShapeBuilder();

            builder.OpenConnectedFaceSet(true);

            XYZ point1 = cubePosition;
            XYZ point2 = new XYZ(cubePosition.X + 1, cubePosition.Y + 0, cubePosition.Z + 0);
            XYZ point3 = new XYZ(cubePosition.X + 1, cubePosition.Y + 1, cubePosition.Z + 0);
            XYZ point4 = new XYZ(cubePosition.X + 0, cubePosition.Y + 1, cubePosition.Z + 0);
            XYZ point5 = new XYZ(cubePosition.X + 0, cubePosition.Y + 0, cubePosition.Z + 1);
            XYZ point6 = new XYZ(cubePosition.X + 1, cubePosition.Y + 0, cubePosition.Z + 1);
            XYZ point7 = new XYZ(cubePosition.X + 1, cubePosition.Y + 1, cubePosition.Z + 1);
            XYZ point8 = new XYZ(cubePosition.X + 0, cubePosition.Y + 1, cubePosition.Z + 1);
            
            builder.AddFace(new TessellatedFace(new List<XYZ>(4) { point1, point2, point3, point4 }, ElementId.InvalidElementId));
            builder.AddFace(new TessellatedFace(new List<XYZ>(4) { point5, point6, point7, point8 }, ElementId.InvalidElementId));
            builder.AddFace(new TessellatedFace(new List<XYZ>(4) { point1, point5, point8, point4 }, ElementId.InvalidElementId));
            builder.AddFace(new TessellatedFace(new List<XYZ>(4) { point1, point5, point6, point2 }, ElementId.InvalidElementId));
            builder.AddFace(new TessellatedFace(new List<XYZ>(4) { point2, point6, point7, point3 }, ElementId.InvalidElementId));
            builder.AddFace(new TessellatedFace(new List<XYZ>(4) { point3, point7, point8, point4 }, ElementId.InvalidElementId));

            builder.CloseConnectedFaceSet();

            TessellatedShapeBuilderResult result = builder.Build(TessellatedShapeBuilderTarget.Solid, TessellatedShapeBuilderFallback.Abort, new ElementId(BuiltInCategory.OST_ColorFillSchema));
            // OST_Walls -> RoomBounding = true
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Walls), "Application id", "Geometry object id");
            ds.SetShape(result.GetGeometricalObjects());

            // Set color of DirectShape
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            // BuildingMaterial -> Brown
            Color cubeColor = new Color(153,76,0);
            // Agents -> Black
            if (isAgent)
            {
                cubeColor = new Color(0, 0, 0);
            }
            ogs.SetProjectionFillColor(cubeColor);
            Dictionary<string, FillPatternElement> fillPatterns = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .ToDictionary<FillPatternElement, string>(e => e.Name);
            ogs.SetProjectionFillPatternId(fillPatterns["Flächenfüllung"].Id);
            ogs.SetProjectionFillPatternVisible(true);
            doc.ActiveView.SetElementOverrides(ds.Id, ogs);
        }
    }
}