import math
import random

from buildingshape import Brick


def __calculateDistanceToBrick__(agentPosition, brickPosition):
    xAgent, yAgent, zAgent = agentPosition["x"], agentPosition["y"], agentPosition["z"]
    xBlock, yBlock, zBlock = brickPosition["x"], brickPosition["y"], brickPosition["z"]
    xDelta = xBlock - xAgent
    yDelta = yBlock - yAgent
    zDelta = zBlock - zAgent
    return math.sqrt(xDelta * xDelta + yDelta * yDelta + zDelta * zDelta)


def __getSurroundingCells__(agentPosition):
    surroundingCoordinates = []

    for i in range(0, 28):
        tempCoordinate = {"x": agentPosition["x"], "y": agentPosition["y"], "z": agentPosition["z"]}
        # Calculate Position X
        if i in [0, 3, 6, 10, 13, 16, 20, 23, 26]:
            tempCoordinate["x"] = agentPosition["x"] + 1
        if i in [2, 5, 8, 12, 15, 18, 22, 25, 28]:
            tempCoordinate["x"] = agentPosition["x"] - 1
        # Calculate Position Y
        if i in range(8):
            tempCoordinate["y"] = agentPosition["y"] + 1
        if i in range(20, 28):
            tempCoordinate["y"] = agentPosition["y"] - 1
        # Calculate Position Z
        if i in [0, 1, 2, 10, 11, 12, 20, 21, 22]:
            tempCoordinate["z"] = agentPosition["z"] + 1
        if i in [6, 7, 8, 16, 17, 18, 26, 27, 28]:
            tempCoordinate["z"] = agentPosition["z"] - 1
        surroundingCoordinates.append(tempCoordinate)
    return surroundingCoordinates


class Agent(object):
    blocks = []
    agents = []

    def __init__(self):
        self.payload = None
        self.agents.append(self)
        self.position = {"x": random.randrange(start=0, stop=500),
                         "y": random.randrange(start=0, stop=500),
                         "z": random.randrange(start=0, stop=500)}

    def doStep(self):
        self.__build__()
        self.__move__()

    def __build__(self):
        # for block in self.blocks:
        #   distance = __calculateDistanceToBrick__(self.position, block.getPosition())
        #   blockPheromone = block.getPheromone()
        #   blockPheromoneType = blockPheromone.getPheromoneType()
        #   self.pheromoneInfluences[blockPheromoneType] += blockPheromone.getIntensity() / (distance * distance)
        self.blocks.append(Brick(position=self.position))

    def __remove__(self):
        pass

    def __move__(self):
        surroundingCells = __getSurroundingCells__(self.position)
        validCells = self.__filterSurroundingCells__(surroundingCells)
        randomCubeCoordinate = validCells[random.randrange(start=0, stop=len(validCells) - 1)]
        self.position = randomCubeCoordinate

    def __filterSurroundingCells__(self, surroundingCells):
        for block in self.blocks:
            blockPosition = block.getPosition()
            if blockPosition in surroundingCells:
                surroundingCells.remove(blockPosition)
        for agent in self.agents:
            agentPosition = agent.getPosition()
            if agentPosition in surroundingCells:
                surroundingCells.remove(agentPosition)
        return surroundingCells

    def addPayload(self, pheromone):
        self.payload = pheromone

    def getPosition(self):
        return self.position
