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


RANDOM_SPAWN_RANGE = 30


class Agent(object):
    blocks = []
    pheromones = []
    pheromones.append(
        Pheromone(position={"x": 0, "y": 0, "z": 0}, intensity=10000, pheromoneType=PheromoneType.initial))
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

    def __build__(self):
        if Calculations.calculatePheromoneIntensity(self.pheromones, self.position):
            self.blocks.append(Brick(position=dict(self.position)))
            self.pheromones.append(
                Pheromone(position=dict(self.position), intensity=100, pheromoneType=PheromoneType.initial))

    def __remove__(self):
        pass

    def __move_pheromoneoriented__(self):
        mostInfluentalPheromones = Calculations.getSortedInfluences(pheromones=self.pheromones,
                                                                    agentPosition=self.position)

        tempPheromones = list(mostInfluentalPheromones)
        tempPheromonePositions = []
        for i in range(0, tempPheromones.count() - 1):
            if utils.isPositionFree(agents=self.agents, blocks=self.blocks, positionToCheck=tempPheromones[i]):
                Calculations.calculateAndMove(self.position, tempPheromones[i])
                return

    def __moveRandom__(self):
        # Take random position of V26
        v26Cells = __getSurroundingCells__(self.position)
        for i in range(0, 25):
            randIndex = random.randint(0, len(v26Cells) - 1)
            # Evaluate if position is free
            if utils.isPositionFree(agents=self.agents, blocks=self.blocks, positionToCheck=v26Cells[randIndex]):
                Calculations.calculateAndMove(self.position, v26Cells[randIndex])
                return
            else:
                v26Cells.remove(v26Cells[randIndex])
        # If all neighboured cells are occupied restart at random position
        self.position = {"x": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE),
                         "y": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE),
                         "z": random.randrange(start=-RANDOM_SPAWN_RANGE, stop=RANDOM_SPAWN_RANGE)}

    def addPayload(self, pheromone):
        self.payload = pheromone

    def getPosition(self):
        return self.position
