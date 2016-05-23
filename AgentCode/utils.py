import math


def calculateDistanceToBrick(agentPosition, pheromonePosition):
    xAgent, yAgent, zAgent = agentPosition["x"], agentPosition["y"], agentPosition["z"]
    xBlock, yBlock, zBlock = pheromonePosition["x"], pheromonePosition["y"], pheromonePosition["z"]
    xDelta = xBlock - xAgent
    yDelta = yBlock - yAgent
    zDelta = zBlock - zAgent
    return math.sqrt(xDelta * xDelta + yDelta * yDelta + zDelta * zDelta)


def isPositionFree(blocks, agents, positionToCheck):
    items = list(blocks + agents)
    for item in items:
        itemPosition = item.getPosition()
        flag = 0
        for key in itemPosition:
            if itemPosition[key] == positionToCheck[0].position[key]:
                flag += 1

        if flag is 3:
            return False

    return True
