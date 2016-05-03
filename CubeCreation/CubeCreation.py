#Author-Simon

import adsk.core, adsk.fusion, traceback
import random

# global set of event handlers to keep them referenced for the duration of the command
handlers = []
app = adsk.core.Application.get()
if app:
    ui = app.userInterface

newComp = None

def createNewComponent():
    # Get the active design.
    product = app.activeProduct
    design = adsk.fusion.Design.cast(product)
    rootComp = design.rootComponent
    allOccs = rootComp.occurrences
    newOcc = allOccs.addNewComponent(adsk.core.Matrix3D.create())
    return newOcc.component


class CubeCommandCreatedHandler(adsk.core.CommandCreatedEventHandler):
    def __init__(self):
        super().__init__()        
    def notify(self, args):
        try:
            cmd = args.command
            cube = Cube()
            cube.headDiameter = 1
            cube.headHeight = 1
            for i in range(25):
                x = random.randint(0, 50)
                y = random.randint(0, 50)
                center = adsk.core.Point3D.create(x, y, 0)
                onExecute = cube.buildCube(center)
            cmd.execute.add(onExecute)
            # keep the handler referenced beyond this function
            handlers.append(onExecute)

        except:
            if ui:
                ui.messageBox('Failed:\n{}'.format(traceback.format_exc()))

class Cube:
    def __init__(self):
        pass
    def buildCube(self, center):
        global newComp
        newComp = createNewComponent()
        if newComp is None:
            ui.messageBox('New component failed to create', 'New Component Failed')
            return

        # Create a new sketch.
        sketches = newComp.sketches
        xyPlane = newComp.xYConstructionPlane
        sketch = sketches.add(xyPlane)
        vertices = []

        vertex = adsk.core.Point3D.create(center.x, center.y, 0)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x + self.headDiameter, center.y, 0)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x + self.headDiameter, center.y + (self.headDiameter), 0)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x, center.y + (self.headDiameter), 0)
        vertices.append(vertex)


        for i in range(0, 4):
            sketch.sketchCurves.sketchLines.addByTwoPoints(vertices[(i+1) % 4], vertices[i])

        extrudes = newComp.features.extrudeFeatures
        prof = sketch.profiles[0]
        extInput = extrudes.createInput(prof, adsk.fusion.FeatureOperations.NewBodyFeatureOperation)

        distance = adsk.core.ValueInput.createByReal(self.headHeight)
        extInput.setDistanceExtent(False, distance)
        extrudes.add(extInput)

def run(context):
    try:
        commandDefinitions = ui.commandDefinitions
        #check the command exists or not
        cmdDef = commandDefinitions.itemById('Cube')
        if not cmdDef:
            cmdDef = commandDefinitions.addButtonDefinition('Cube',
                    'Create Cube',
                    'Create a Cube.') # relative resource file path is specified

        onCommandCreated = CubeCommandCreatedHandler()
        cmdDef.commandCreated.add(onCommandCreated)
        # keep the handler referenced beyond this function
        handlers.append(onCommandCreated)
        inputs = adsk.core.NamedValues.create()
        cmdDef.execute(inputs)

        # prevent this module from being terminate when the script returns, because we are waiting for event handlers to fire
        adsk.autoTerminate(False)
    except:
        if ui:
            ui.messageBox('Failed:\n{}'.format(traceback.format_exc()))
