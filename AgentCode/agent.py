import random

import antBuildCalculations as Calculations
import utils
from buildingshape import Brick
from pheromone import Pheromone
from pheromonetype import PheromoneType


def __getSurroundingCells__(agentPosition):
    surroundingCartesianCoordinates = []

    for i in range(0, 28):
        surroundingCartesianCoordinates.append(calculateNextPosition(agentPosition, i))
    return surroundingCartesianCoordinates


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


RANDOM_SPAWN_RANGE = 1


class Agent(object):
    blocks = []
    pheromones = [Pheromone(position={"x": 0, "y": 0, "z": 0}, intensity=5, vaporationRate=0,
                            pheromoneType=PheromoneType.initial)]
    agents = []

    def __init__(self):
        self.payload = None
        self.agents.append(self)
        self.position = {"x": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE),
                         "y": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE),
                         "z": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE)}

    def doStep(self):
        self.__build__()
        self.__moveRandom__()
        # Evaporation
        for pheromone in self.pheromones:
            pheromone.intensity -= pheromone.intensity * pheromone.vaporationRate

    def __build__(self):
        if Calculations.shouldBuild(self.pheromones, self.position):
            self.blocks.append(Brick(position=dict(self.position)))
            self.pheromones.append(
                Pheromone(position=dict(self.position), intensity=10, pheromoneType=PheromoneType.build,
                          vaporationRate=0.00001))

    def __remove__(self):
        pass

    def __moveRandom__(self):
        # Take random position of V26
        surroundingCells = __getSurroundingCells__(self.position)
        for i in range(0, 25):
            randIndex = random.randint(0, len(surroundingCells) - 1)
            # Evaluate if position is free
            if utils.isPositionFree(agents=self.agents, blocks=self.blocks,
                                    positionToCheck=surroundingCells[randIndex]):
                Calculations.calculateAndMove(self.position, surroundingCells[randIndex])
                return
            else:
                surroundingCells.remove(surroundingCells[randIndex])
        # If all neighboured cells are occupied restart at random position
        self.position = {"x": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE),
                         "y": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE),
                         "z": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE)}

    def addPayload(self, pheromone):
        self.payload = pheromone

    def getPosition(self):
        return self.position
