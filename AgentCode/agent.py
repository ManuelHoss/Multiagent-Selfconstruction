import random

import antBuildCalculations as Calculations
import utils
from buildingshape import Brick
from pheromone import Pheromone
from pheromonetype import PheromoneType


def __getSurroundingCells__(agentPosition):
    surroundingCoordinates = []

    for i in range(0, 28):
        surroundingCoordinates.append(calculateNextPosition(agentPosition, i))
    return surroundingCoordinates


def calculateNextPosition(agentPosition, cubePosition):
    nextPosition = {"x": agentPosition["x"], "y": agentPosition["y"], "z": agentPosition["z"]}
    # Calculate Position X
    if cubePosition in [0, 3, 6, 10, 13, 16, 20, 23, 26]:
        nextPosition["x"] = agentPosition["x"] + 1
    if cubePosition in [2, 5, 8, 12, 15, 18, 22, 25, 28]:
        nextPosition["x"] = agentPosition["x"] - 1
    # Calculate Position Y
    if cubePosition in range(8):
        nextPosition["y"] = agentPosition["y"] + 1
    if cubePosition in range(20, 28):
        nextPosition["y"] = agentPosition["y"] - 1
    # Calculate Position Z
    if cubePosition in [0, 1, 2, 10, 11, 12, 20, 21, 22]:
        nextPosition["z"] = agentPosition["z"] + 1
    if cubePosition in [6, 7, 8, 16, 17, 18, 26, 27, 28]:
        nextPosition["z"] = agentPosition["z"] - 1
    return nextPosition


class Agent(object):
    blocks = []
    pheromones = []
    pheromones.append(
        Pheromone(position={"x": 0, "y": 0, "z": 0}, intensity=10000, pheromoneType=PheromoneType.initial))
    agents = []

    def __init__(self):
        self.payload = None
        self.agents.append(self)
        self.position = {"x": random.randrange(start=-10, stop=10),
                         "y": random.randrange(start=-10, stop=10),
                         "z": random.randrange(start=-10, stop=10)}

    def doStep(self):
        self.__build__()
        self.__move__()

    def __build__(self):
        if Calculations.calculatePheromoneIntensity(self.pheromones, self.position):
            self.blocks.append(Brick(position=dict(self.position)))
            self.pheromones.append(
                Pheromone(position=dict(self.position), intensity=100, pheromoneType=PheromoneType.initial))

    def __remove__(self):
        pass

    def __move__(self):
        mostInfluentalPheromones = Calculations.getSortedInfluences(pheromones=self.pheromones,
                                                                    agentPosition=self.position)

        tempPheromones = list(mostInfluentalPheromones)
        while tempPheromones:
            if utils.isPositionFree(agents=self.agents, blocks=self.blocks, positionToCheck=tempPheromones[0]):
                Calculations.calculateAndMove(self.position, tempPheromones[0])
                break
            else:
                tempPheromones.remove(tempPheromones[0])

    def addPayload(self, pheromone):
        self.payload = pheromone

    def getPosition(self):
        return self.position
