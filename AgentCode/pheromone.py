from pheromonetype import PheromoneType

class Pheromone(object):
    def __init__(self, intensity=50, vaporationRate=0.1, pheromoneType=PheromoneType.neutral, position=None):
        self.intensity = intensity
        self.vaporationRate = vaporationRate
        self.pheromoneType = pheromoneType
        self.position = {"x": position["x"],
                         "y": position["y"],
                         "z": position["z"]}

    def getPheromoneType(self):
        return self.pheromoneType

    def getIntensity(self):
        return self.intensity
