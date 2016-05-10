import adsk.core, adsk.fusion, traceback, threading, math, time

app = adsk.core.Application.get()
ui  = app.userInterface

def run(context):
    try:
        # Get all components in the active design.
        product = app.activeProduct
        design = adsk.fusion.Design.cast(product)
        title = 'Import Fusion Export File'
        if not design:
            ui.messageBox('No active Fusion design', title)
            return
        
        dlg = ui.createFileDialog()
        dlg.title = 'Import Fusion Export File'
        dlg.filter = 'Fusion Export File (*.fusionExport)'
        if dlg.showOpen() != adsk.core.DialogResults.DialogOK :
            return
        
        filename = dlg.filename
        f = open(filename, 'r')
        line = f.readline()
        data = []
        threads = []
        
        while line:
            pntStrArr = line.split(';')
            localData = []
            localData.append(pntStrArr[0])
            for pntStr in pntStrArr[1:]:
                localData.append( float(pntStr))
            data.append(localData)
            line = f.readline()
        
        dataCounter = len(data)
        threadcount = int(math.sqrt(dataCounter))
        dataPerThread = int(dataCounter / threadcount)
        
        counter = 0
        for threadCount in range(threadcount):
            helper = counter + dataPerThread
            t = threading.Thread(target=createCube, args=(data[counter:helper],), name ='Cube')
            counter = helper
            threads += [t]
            t.start()
            
        if(dataCounter - dataPerThread * threadcount) > 0:
            t = threading.Thread(target=createCube, args=(data[counter:],), name ='Cube')
            threads += [t]
            t.start()
        
        time.sleep(0)
        for t in threads:
            t.join(1) 

        f.close()
    except:
        if ui:
            ui.messageBox('Failed:\n{}'.format(traceback.format_exc()))
            
def createCube(data):
    # Setup cube parameters
    for point in data:
        cube = Cube()
        #TODO: add cubeSize to Exportfile
        #TODO: add color for agent
        cube.headDiameter = 1
        cube.headHeight = 1
          
        center = adsk.core.Point3D.create(point[1], point[2], point[3])
        cube.buildCube(center)
        
class Cube:
    def __init__(self):
        pass
    def buildCube(self, center):
        newComp = createNewComponent()
        if newComp is None:
            return

        # Create a new sketch.
        sketches = newComp.sketches
        xyPlane = newComp.xYConstructionPlane
        sketch = sketches.add(xyPlane)
        vertices = []
        
        # Create vertices of the square
        vertex = adsk.core.Point3D.create(center.x, center.y, center.z)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x + self.headDiameter, center.y, center.z)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x + self.headDiameter, center.y + (self.headDiameter), center.z)
        vertices.append(vertex)
        vertex = adsk.core.Point3D.create(center.x, center.y + (self.headDiameter), center.z)
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

def createNewComponent():
    # Initial component setup.
    product = app.activeProduct
    design = adsk.fusion.Design.cast(product)
    rootComp = design.rootComponent
    allOccs = rootComp.occurrences
    newOcc = allOccs.addNewComponent(adsk.core.Matrix3D.create())
    return newOcc.component