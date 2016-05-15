import math
import random

from buildingshape import Brick
from pheromonetype import PheromoneType


def __calculateDistanceToBrick__(agentPosition, brickPosition):
    xAgent, yAgent, zAgent = agentPosition["x"], agentPosition["y"], agentPosition["z"]
    xBlock, yBlock, zBlock = brickPosition["x"], brickPosition["y"], brickPosition["z"]
    xDelta = xBlock - xAgent
    yDelta = yBlock - yAgent
    zDelta = zBlock - zAgent
    return math.sqrt(xDelta * xDelta + yDelta * yDelta + zDelta * zDelta)


class Agent(object):
    blocks = []
    agents = []

    def __init__(self):
        self.payload = None
        self.agents.append(self)
        self.position = {"x": random.randrange(start=0, stop=500),
                         "y": random.randrange(start=0, stop=500),
                         "z": random.randrange(start=0, stop=500)}
        self.pheromoneInfluences = {PheromoneType.initial: 0,
                                    PheromoneType.neutral: 0,
                                    PheromoneType.build: 0}

    def doStep(self):
        self.__build__()
        self.__move__()

    def __build__(self):
        #for block in self.blocks:
        #   distance = __calculateDistanceToBrick__(self.position, block.getPosition())
        #   blockPheromone = block.getPheromone()
        #   blockPheromoneType = blockPheromone.getPheromoneType()
        #   self.pheromoneInfluences[blockPheromoneType] += blockPheromone.getIntensity() / (distance * distance)
        self.blocks.append(Brick(position=self.position))

    def __remove__(self):
        pass

    def __move__(self):
        randomCubeCoordinate = random.randrange(start=0, stop=26)

        x, y, z = 0, 0, 0
        # Calculate Position X
        if randomCubeCoordinate in [0, 3, 6, 10, 13, 16, 20, 23, 26]:
            x = self.position["x"] + 1
        if randomCubeCoordinate in [2, 5, 8, 12, 15, 18, 22, 25, 28]:
            x = self.position["y"] - 1
        # Calculate Position Y
        if randomCubeCoordinate in range(8):
            y = self.position["y"] + 1
        if randomCubeCoordinate in range(20, 28):
            y = self.position["y"] - 1
        # Calculate Position Z
        if randomCubeCoordinate in [0, 1, 2, 10, 11, 12, 20, 21, 22]:
            z = self.position["z"] + 1
        if randomCubeCoordinate in [6, 7, 8, 16, 17, 18, 26, 27, 28]:
            z = self.position["z"] - 1

        # TODO: Eliminate possibility of infinite loop
        # Check if cell is empty
        """for block in self.blocks:
            blockPosition = block.getPosition()
            if [x, y, z] != [blockPosition["x"], blockPosition["y"], blockPosition["z"]]:
                self.__move__()
        for agent in self.agents:
            agentPosition = agent.getPosition()
            if [x, y, z] != [agentPosition["x"], agentPosition["y"], agentPosition["z"]]:
                self.__move__()
        """
        # If cell is empty --> move there
        self.position["x"] = x
        self.position["y"] = y
        self.position["z"] = z

    def addPayload(self, pheromone):
        self.payload = pheromone

    def getPosition(self):
        return self.position
