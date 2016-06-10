# IronPython Pad. Write code snippets here and F5 to run.
# IronPython Pad. Write code snippets here and F5 to run.
import clr

clr.AddReference('RevitAPI')
clr.AddReference('RevitAPIUI')
from Autodesk.Revit.DB import *

doc = __revit__.ActiveUIDocument.Document
app = __revit__.Application

t = Transaction(doc, 'Create extrusion with a loop.')

t.Start()

# Create a sketch plane
origin = XYZ.Zero
normal = XYZ.BasisZ

plane = Plane(normal, origin)
skplane = SketchPlane.Create(doc, plane)

# Create array and append vertices
pts = []
pts.append(XYZ(0, 0, 0))
pts.append(XYZ(10, 0, 0))
pts.append(XYZ(10, 10, 0))
pts.append(XYZ(0, 10, 0))
pts.append(XYZ(0, 0, 0))

# create Reference Array for curves
refarr = ReferenceArray()

# use a loop to fill reference array with model curves.
for i in range(0, len(pts) - 1):
    ptA = pts[i]
    ptB = pts[i + 1]

    line = Line.CreateBound(ptA, ptB)
    crv = doc.FamilyCreate.NewModelCurve(line, skplane)
    refarr.Append(crv.GeometryCurve.Reference)

# establish extrusion vector
dir = XYZ(0, 0, 10)

# extrude the form
extrude = doc.FamilyCreate.NewExtrusionForm(True, refarr, dir)

t.Commit()

__window__.Close()
