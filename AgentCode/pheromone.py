from pheromonetype import PheromoneType

class Pheromone(object):
    def __init__(self, intensity=50, vaporationRate=0.0000001, pheromoneType=PheromoneType.neutral, position=None):
        self.intensity = intensity
        self.vaporationRate = vaporationRate
        self.pheromoneType = pheromoneType
        self.position = {"x": position["x"],
                         "y": position["y"],
                         "z": position["z"]}
