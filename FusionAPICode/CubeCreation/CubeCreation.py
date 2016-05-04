#Author-Simon

import adsk.core, adsk.fusion, traceback
import random, threading, time

# global set of event handlers to keep them referenced for the duration of the command
handlers = []
app = adsk.core.Application.get()
if app:
    ui = app.userInterface

newComp = None

def createNewComponent():
    # Initial component setup.
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
            # Create cubes
            threads = []
            for count in range(50):
                t = threading.Thread(target=createCube, name ='Cube')
                threads += [t]
                t.start()
            time.sleep(0)
            for t in threads:
                t.join(1)
        except:
            if ui:
                ui.messageBox('Failed:\n{}'.format(traceback.format_exc()))

def createCube():
    # Setup cube parameters
    cube = Cube()
    cube.headDiameter = 1
    cube.headHeight = 1
    x = random.randint(0, 50)
    y = random.randint(0, 50)
    center = adsk.core.Point3D.create(x, y, 0)
    cube.buildCube(center)

class Cube:
    def __init__(self):
        pass
    def buildCube(self, center):
        newComp = createNewComponent()
        if newComp is None:
            ui.messageBox('New component failed to create', 'New Component Failed')
            return

        # Create a new sketch.
        sketches = newComp.sketches
        xyPlane = newComp.xYConstructionPlane
        sketch = sketches.add(xyPlane)
        vertices = []
        
        # Create vertices of the square
        vertex = adsk.core.Point3D.create(center.x, center.y, 0)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x + self.headDiameter, center.y, 0)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x + self.headDiameter, center.y + (self.headDiameter), 0)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x, center.y + (self.headDiameter), 0)
        vertices.append(vertex)

        # Create square -> by creating four lines
        for i in range(0, 4):
            sketch.sketchCurves.sketchLines.addByTwoPoints(vertices[(i+1) % 4], vertices[i])

        # Create extrudes -> make cube out of square
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
