import utils
from pheromonetype import PheromoneType


def shouldBuild(pheromones, agentPosition):
    buildPheromoneIntesity = 0
    initalPheromoneIntensity = 0
    for pheromone in pheromones:
        distance = utils.calculateDistanceToBrick(agentPosition, pheromone.position)
        if pheromone.pheromoneType == PheromoneType.build:
            buildPheromoneIntesity += pheromone.intensity / (
                distance * distance) if distance != 0 else pheromone.intensity
        elif pheromone.pheromoneType == PheromoneType.initial:
            initalPheromoneIntensity += pheromone.intensity / (
                distance * distance) if distance != 0 else pheromone.intensity
    return True if (
                   buildPheromoneIntesity >= 0.5 or buildPheromoneIntesity == 0) and initalPheromoneIntensity < 0.05 else False


def getSortedInfluences(pheromones, agentPosition):
    mostInfluental = []
    for pheromone in pheromones:
        distance = utils.calculateDistanceToBrick(agentPosition, pheromone.position)
        pheromoneIntesity = pheromone.intensity / (
            distance * distance) if distance != 0 else pheromone.intensity
        mostInfluental.append((pheromone, pheromoneIntesity))

    return sorted(mostInfluental, key=lambda intensity: intensity[1], reverse=True)


def calculateAndMove(agentPosition, pheromone):
    for key in agentPosition:
        if agentPosition[key] < pheromone[key]:
            agentPosition[key] += 1
        elif agentPosition[key] > pheromone[key]:
            agentPosition[key] -= 1
    return agentPosition
