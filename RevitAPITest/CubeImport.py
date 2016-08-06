import clr
import sys

print sys.path
import AgentCodeAPI

clr.AddReference('RevitAPI')
clr.AddReference('RevitAPIUI')
from Autodesk.Revit.DB import *

doc = __revit__.ActiveUIDocument.Document
app = __revit__.Application

t = Transaction(doc, 'Create extrusion with a loop.')

t.Start()

# Call of AgentCode "run building process"
# --> returns the positions of agents and bricks
# List with positions of agents
# dummyAgents = [(0, 1, 0), (6, 4, 4), (9, 4, -3)]
# List with positions of bricks
#dummyBricks = [(0, 0, 0), (4, 4, 4), (8, 4, -3)]


def buildCubes(brickPositions, agentPositions):
    for brickPosition in brickPositions:
        createCube(brickPosition[0], brickPosition[1], brickPosition[2], False)
    for agentPosition in agentPositions:
        createCube(agentPosition[0], agentPosition[1], agentPosition[2], True)


def createCube(posX, posY, PosZ, isAgent):
    color = Color(0, 0, 0)
    if isAgent == True:
        color = Color(255, 0, 0)

    # Create a sketch plane
    origin = XYZ(posX, posY, PosZ)
    normal = XYZ.BasisZ

    plane = Plane(normal, origin)
    skplane = SketchPlane.Create(doc, plane)

    # Create array and append vertices
    pts = []
    pts.append(XYZ(posX, posY, PosZ))
    pts.append(XYZ(posX + 1, posY, PosZ))
    pts.append(XYZ(posX + 1, posY + 1, PosZ))
    pts.append(XYZ(posX, posY + 1, PosZ))
    pts.append(XYZ(posX, posY, PosZ))

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
    dir = XYZ(0, 0, 1)

    # extrude the form
    extrude = doc.FamilyCreate.NewExtrusionForm(True, refarr, dir)


def quit():
    __window__.Close()


def agentCodeAPI():
    return AgentCodeAPI.api().runAgentsAndRetrive(numberOfAgents=5, numberOfSteps=50)


agentReturn = agentCodeAPI()

agentPositions = []
blockPositions = []

for agent in agentReturn:
    temp = (agent.position['x'], agent.position['y'], agent.position['z'])
    agentPositions.append(temp)
for block in agentReturn[0].blocks:
    temp = (block.position['x'], block.position['y'], block.position['z'])
    blockPositions.append(temp)

buildCubes(blockPositions, agentPositions)
t.Commit()

quit()
