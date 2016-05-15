from pheromone import Pheromone
from pheromonetype import PheromoneType

class Brick(object):
    def __init__(self, size=1, position={"x": 0, "y": 0, "z": 0}, pheromone=Pheromone(PheromoneType.neutral)):
        self.size = size
        self.position = {"x": position["x"], "y": position["y"], "z": position["z"]}
        self.pheromone = pheromone

    def setPheromone(self, pheromone):
        self.pheromone = pheromone

    def getPheromone(self):
        return self.pheromone

    def getPosition(self):
        return self.position
