import utils
from pheromonetype import PheromoneType


def shouldBuild(pheromones, agentPosition):
    buildPheromoneIntesity = 0
    initalPheromoneIntensity = 0
    for pheromone in pheromones:
        distance = utils.calculateDistanceToBrick(agentPosition, pheromone.position)
        if pheromone.getPheromoneType() == PheromoneType.build:
            buildPheromoneIntesity += pheromone.getIntensity() / (
            distance * distance) if distance != 0 else pheromone.getIntensity()
        elif pheromone.getPheromoneType() == PheromoneType.initial:
            initalPheromoneIntensity += pheromone.getIntensity() / (
            distance * distance) if distance != 0 else pheromone.getIntensity()
    return True if buildPheromoneIntesity >= 1 and initalPheromoneIntensity < 1 else False


def getSortedInfluences(pheromones, agentPosition):
    mostInfluental = []
    for pheromone in pheromones:
        distance = utils.calculateDistanceToBrick(agentPosition, pheromone.position)
        pheromoneIntesity = pheromone.getIntensity() / (
            distance * distance) if distance != 0 else pheromone.getIntensity()
        mostInfluental.append((pheromone, pheromoneIntesity))

    return sorted(mostInfluental, key=lambda intensity: intensity[1], reverse=True)


def calculateAndMove(agentPosition, pheromone):
    for key in agentPosition:
        if agentPosition[key] < pheromone[key]:
            agentPosition[key] += 1
        elif agentPosition[key] > pheromone[key]:
            agentPosition[key] -= 1
    return agentPosition
