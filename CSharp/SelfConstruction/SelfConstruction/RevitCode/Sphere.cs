using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using SelfConstruction.AgentCode;

namespace SelfConstruction.RevitCode
{
    public class Sphere
    {
        public ElementId CreateSphere(Document doc, XYZ spherePosition, double radius, Pheromonetype pheromonetype)
        {
            List<Curve> profile = new List<Curve>();

            // first create sphere with 2' radius
            XYZ sphereCenter = spherePosition;
            double _radius = radius;
            XYZ profile00 = sphereCenter;
            XYZ profilePlus = sphereCenter + new XYZ(0, _radius, 0);
            XYZ profileMinus = sphereCenter - new XYZ(0, _radius, 0);

            profile.Add(Line.CreateBound(profilePlus, profileMinus));
            profile.Add(Arc.Create(profileMinus, profilePlus, sphereCenter + new XYZ(_radius, 0, 0)));

            CurveLoop curveLoop = CurveLoop.Create(profile);
            SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

            Frame frame = new Frame(sphereCenter, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
            Solid sphere = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options);
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel),
                                                           "Application id",
                                                           "Geometry object id");
            ds.SetShape(new GeometryObject[] { sphere });

            // Set color of DirectShape
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            // BuildingMaterial -> Brown
            Color cubeColor = new Color(255, 0, 0);
            if (pheromonetype == Pheromonetype.Space)
            {
                cubeColor = new Color(0, 255, 0);
            }
            else if (pheromonetype == Pheromonetype.Build)
            {
                cubeColor = new Color(0, 0, 255);
            }
            ogs.SetProjectionFillColor(cubeColor);
            Dictionary<string, FillPatternElement> fillPatterns = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .ToDictionary<FillPatternElement, string>(e => e.Name);
            ElementId id = fillPatterns.ContainsKey("Flächenfüllung") ? fillPatterns["Flächenfüllung"].Id : fillPatterns["Solid fill"].Id;
            ogs.SetProjectionFillPatternId(id);
            ogs.SetProjectionFillPatternVisible(true);
            doc.ActiveView.SetElementOverrides(ds.Id, ogs);
            return ds.Id;
        }
    }
}
